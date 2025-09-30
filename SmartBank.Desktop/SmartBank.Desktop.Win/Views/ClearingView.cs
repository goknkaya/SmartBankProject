using SmartBank.Desktop.Win.Core;
using SmartBank.Desktop.Win.Core.Contracts;
using System.ComponentModel; // BindingList
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace SmartBank.Desktop.Win.Views
{
    public partial class ClearingView : UserControl
    {
        private readonly ApiClient _client;
        private readonly BindingSource _bsBatches = new();
        private readonly BindingSource _bsRecords = new();

        private bool _suppressUi;

        public ClearingView(ApiClient client)
        {
            InitializeComponent();
            _client = client;

            SetupGrids();
            InitInputs();
            WireButtons();

            _ = LoadBatchesAsync();
        }

        private void SetupGrids()
        {
            // BATCHES
            dgvBatches.AutoGenerateColumns = false;
            dgvBatches.ReadOnly = true;
            dgvBatches.AllowUserToAddRows = false;
            dgvBatches.AllowUserToDeleteRows = false;
            dgvBatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBatches.DataSource = _bsBatches;
            dgvBatches.DataError += (s, e) => e.ThrowException = false;

            colB_BatchId.DataPropertyName = "Id";
            colB_Direction.DataPropertyName = "Direction";
            colB_FileName.DataPropertyName = "FileName";
            colB_FileHash.DataPropertyName = "FileHash";
            colB_SettlementDate.DataPropertyName = "SettlementDate";
            colB_Status.DataPropertyName = "Status";
            colB_TotalCount.DataPropertyName = "TotalCount";
            colB_SuccessCount.DataPropertyName = "SuccessCount";
            colB_FailCount.DataPropertyName = "FailCount";
            colB_CreatedAt.DataPropertyName = "CreatedAt";
            colB_ProcessedAt.DataPropertyName = "ProcessedAt";
            colB_Notes.DataPropertyName = "Notes";

            colB_SettlementDate.DefaultCellStyle.Format = "dd.MM.yyyy";
            colB_CreatedAt.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            colB_ProcessedAt.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            colB_BatchId.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colB_TotalCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colB_SuccessCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colB_FailCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvBatches.SelectionChanged += async (_, __) => await LoadSelectedBatchRecordsAsync();

            // RECORDS
            dgvRecords.AutoGenerateColumns = false;
            dgvRecords.ReadOnly = true;
            dgvRecords.AllowUserToAddRows = false;
            dgvRecords.AllowUserToDeleteRows = false;
            dgvRecords.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecords.DataSource = _bsRecords;
            dgvRecords.DataError += (s, e) => e.ThrowException = false;

            colR_RecordId.DataPropertyName = "Id";
            colR_LineNumber.DataPropertyName = "LineNumber";
            colR_TransactionId.DataPropertyName = "TransactionId";
            colR_CardId.DataPropertyName = "CardId";
            colR_CardLast4.DataPropertyName = "CardLast4";
            colR_Amount.DataPropertyName = "Amount";
            colR_Currency.DataPropertyName = "Currency";
            colR_TransactionDate.DataPropertyName = "TransactionDate";
            colR_MerchantName.DataPropertyName = "MerchantName";
            colR_MatchStatus.DataPropertyName = "MatchStatus";
            colR_ErrorMessage.DataPropertyName = "ErrorMessage";
            colR_CreatedAt.DataPropertyName = "CreatedAt";

            colR_Amount.DefaultCellStyle.Format = "N2";
            colR_Amount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colR_TransactionDate.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            colR_RecordId.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colR_LineNumber.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void InitInputs()
        {
            // Direction
            cboDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDirection.Items.Clear();
            cboDirection.Items.AddRange(new object[] { "IN", "OUT" });
            cboDirection.SelectedItem = "IN";

            // Tarih
            dtpSettlement.Format = DateTimePickerFormat.Custom;
            dtpSettlement.CustomFormat = "dd.MM.yyyy";
            dtpSettlement.Value = DateTime.Now;

            this.Load += (_, __) => UpdateFormTitle();
            this.VisibleChanged += (_, __) => { if (Visible) UpdateFormTitle(); };
        }

        private void WireButtons()
        {
            btnRefreshBatches.Click += async (_, __) => await LoadBatchesAsync();
            btnRefreshRecords.Click += async (_, __) => await LoadSelectedBatchRecordsAsync();
            btnRetryUnmatched.Click += async (_, __) => await RetryUnmatchedAsync();
            btnRetrySelected.Click += async (_, __) => await RetryUnmatchedAsync();

            btnUploadIn.Click += async (_, __) => await UploadInAsync();
            btnGenerateOut.Click += async (_, __) => await GenerateOutAsync();
        }

        private void UpdateFormTitle()
        {
            if (FindForm() is Form f)
                f.Text = "SmartBank Ödeme Sistemleri / Clearing";
        }

        // ================== Data ==================

        private async Task LoadBatchesAsync()
        {
            try
            {
                string? dir = cboDirection.SelectedItem?.ToString();
                var d = dtpSettlement.Value.Date;

                string url = $"api/clearing/batches?direction={dir}&settlementDate={d:yyyy-MM-dd}";
                var list = await _client.GetAsync<List<SelectClearingBatchDto>>(url) ?? new();

                // Sıralama: CreatedAt DESC
                var ordered = list.OrderByDescending(b => b.CreatedAt).ToList();
                _bsBatches.DataSource = new BindingList<SelectClearingBatchDto>(ordered);

                dgvBatches.ClearSelection();
                _bsRecords.DataSource = new BindingList<SelectClearingRecordDto>(); // altı boşalt
                dgvRecords.ClearSelection();
            }
            catch (ApiException ex)
            {
                ShowApiError("Batch Listeleme", ex);
            }
        }

        private int? CurrentBatchId()
        {
            if (_bsBatches.Current is SelectClearingBatchDto b)
                return b.Id;
            return null;
        }

        private async Task LoadSelectedBatchRecordsAsync()
        {
            if (_suppressUi) return;

            var id = CurrentBatchId();
            if (id is null) { _bsRecords.DataSource = new BindingList<SelectClearingRecordDto>(); return; }

            try
            {
                // İstersen matchStatus filtresi ekle (P/M/N/X/E)
                var rows = await _client.GetAsync<List<SelectClearingRecordDto>>(
                    $"api/clearing/batches/{id}/records") ?? new();

                var ordered = rows.OrderBy(r => r.LineNumber).ToList();
                _bsRecords.DataSource = new BindingList<SelectClearingRecordDto>(ordered);
                dgvRecords.ClearSelection();
            }
            catch (ApiException ex)
            {
                ShowApiError("Records Listeleme", ex);
            }
        }

        private async Task RetryUnmatchedAsync()
        {
            var id = CurrentBatchId();
            if (id is null) { MessageBox.Show("Önce bir batch seç.", "Uyarı"); return; }

            try
            {
                // CEVABI ÖNEMSEME: sadece tetikle
                await _client.PostAsync<object, object>($"api/clearing/batches/{id}/retry", new { }, withAuth: true);

                // Listeleri tazele
                await LoadSelectedBatchRecordsAsync();
                await LoadBatchesAsync();

                MessageBox.Show("Eşleşmeyenler yeniden denendi.", "Bilgi");
            }
            catch (ApiException ex)
            {
                ShowApiError("Eşleşmeyenleri Yeniden Dene", ex);
            }
        }

        // ========== IN Upload ==========
        private async Task UploadInAsync()
        {
            if ((cboDirection.SelectedItem?.ToString() ?? "IN") != "IN")
            {
                MessageBox.Show("IN dosyası yüklemek için Direction = IN olmalı.", "Uyarı");
                return;
            }

            using var ofd = new OpenFileDialog
            {
                Filter = "Clearing Files|*.csv;*.psv;*.txt|All Files|*.*",
                Title = "Clearing IN dosyası seç"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var d = dtpSettlement.Value.Date;

            try
            {
                // Backend’in beklediği alanlar: file + settlementDate + (opsiyonel) fileName,fileHash,notes
                var form = new Dictionary<string, string>
                {
                    ["settlementDate"] = d.ToString("yyyy-MM-dd"),
                    ["direction"] = "IN" // IncomingUploadRequest zaten IN; yine de yollayalım
                };

                // ApiClient’ına küçük bir helper ekleyeceksin (aşağı not)
                var created = await _client.PostMultipartAsync<SelectClearingBatchDto>(
                    path: "api/clearing/in",
                    fields: form,
                    fileFieldName: "file",
                    filePath: ofd.FileName
                );

                await LoadBatchesAsync();
                if (created != null)
                    MessageBox.Show($"IN yüklendi. Batch #{created.Id}", "Bilgi");
            }
            catch (ApiException ex)
            {
                ShowApiError("IN Upload", ex);
            }
        }

        // ========== OUT Generate ==========
        private async Task GenerateOutAsync()
        {
            if ((cboDirection.SelectedItem?.ToString() ?? "IN") != "OUT")
            {
                MessageBox.Show("OUT üretmek için Direction = OUT seç.", "Uyarı");
                return;
            }

            var d = dtpSettlement.Value.Date;

            try
            {
                // İKİ OLASI SENARYO:
                // A) API dosya döner (octet-stream) -> kaydet
                // B) API JSON döner (base64 + fileName) -> decode edip kaydet
                // Aşağıdaki helper, ikisini de karşılar.

                var savedPath = await DownloadOutgoingAsync(d);
                await LoadBatchesAsync();

                MessageBox.Show($"OUT üretildi ve kaydedildi:\n{savedPath}", "Bilgi");
            }
            catch (ApiException ex)
            {
                ShowApiError("OUT Generate", ex);
            }
        }

        // ================== Helpers ==================

        private void ShowApiError(string title, ApiException ex)
        {
            var body = (ex.ResponseBody ?? "").Trim();
            if (body.Length > 2000) body = body[..2000] + "...";

            // herhangi bir statüde, body varsa onu göster (özellikle 500'de)
            if (!string.IsNullOrWhiteSpace(body))
            {
                MessageBox.Show(body, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // body yoksa kısa özet
            MessageBox.Show($"{title}: HTTP {ex.StatusCode}", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        // ====== OUT dosya indirme (octet-stream veya base64 JSON) ======
        private async Task<string> DownloadOutgoingAsync(DateTime settlementDate)
        {
            // 1) API raw dosya döndürüyorsa:
            var url = $"api/clearing/outgoing?settlementDate={settlementDate:yyyy-MM-dd}";

            using var http = new HttpClient { BaseAddress = _client.BaseAddress };
            foreach (var (k, v) in _client.DefaultHeaders)
                http.DefaultRequestHeaders.TryAddWithoutValidation(k, v);

            // Authorization'ı kesin ekle
            if (!string.IsNullOrWhiteSpace(AuthContext.Token))
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AuthContext.Token);

            // GET yerine POST (boş içerik)
            var resp = await http.PostAsync(url, content: null);
            resp.EnsureSuccessStatusCode();

            // Octet-stream mi?
            if (resp.Content.Headers.ContentType?.MediaType == "application/octet-stream")
            {
                var bytes = await resp.Content.ReadAsByteArrayAsync();
                var suggested = resp.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                                ?? $"out_{settlementDate:yyyyMMdd}.csv";

                using var sfd = new SaveFileDialog { FileName = suggested, Filter = "CSV|*.csv|Tümü|*.*" };
                if (sfd.ShowDialog() != DialogResult.OK)
                    throw new OperationCanceledException("Kaydetme iptal edildi.");

                await File.WriteAllBytesAsync(sfd.FileName, bytes);
                return sfd.FileName;
            }
            else
            {
                // JSON ise (ör: { fileName: "…", fileBytesBase64: "…" })
                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var fileName = root.GetProperty("fileName").GetString() ?? $"out_{settlementDate:yyyyMMdd}.csv";
                var b64 = root.GetProperty("fileBytesBase64").GetString() ?? "";
                var bytes = Convert.FromBase64String(b64);

                using var sfd = new SaveFileDialog { FileName = fileName, Filter = "CSV|*.csv|Tümü|*.*" };
                if (sfd.ShowDialog() != DialogResult.OK)
                    throw new OperationCanceledException("Kaydetme iptal edildi.");

                await File.WriteAllBytesAsync(sfd.FileName, bytes);
                return sfd.FileName;
            }
        }
    }
}

