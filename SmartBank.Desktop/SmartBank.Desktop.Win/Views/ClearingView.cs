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

            // Kolon formatları (designer’da DataPropertyName’leri DTO ile bire bir yap)
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
                // int dönebilir: M’ye dönen satır sayısı
                var changed = await _client.PostAsync<object, int>($"api/clearing/batches/{id}/retry", new { });

                await LoadSelectedBatchRecordsAsync();
                await LoadBatchesAsync();

                MessageBox.Show($"Yeniden denendi. Matched olan: {changed}", "Bilgi");
            }
            catch (ApiException ex)
            {
                ShowApiError("Retry", ex);
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
            if (body.Length > 400) body = body[..400] + "...";

            var msg = ex.StatusCode switch
            {
                400 => body != "" ? body : "Geçersiz istek.",
                404 => "Kayıt bulunamadı.",
                409 => body != "" ? body : "Çakışma / iş kuralı ihlali.",
                500 => "Sunucu hatası.",
                _ => body != "" ? body : $"İşlem başarısız. Kod: {ex.StatusCode}"
            };

            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ====== OUT dosya indirme (octet-stream veya base64 JSON) ======
        private async Task<string> DownloadOutgoingAsync(DateTime settlementDate)
        {
            // 1) API raw dosya döndürüyorsa:
            var url = $"api/clearing/out?settlementDate={settlementDate:yyyy-MM-dd}";

            // ApiClient’ın varsa: GetBytesAsync; yoksa:
            using var http = new HttpClient { BaseAddress = _client.BaseAddress };
            foreach (var (k, v) in _client.DefaultHeaders) http.DefaultRequestHeaders.TryAddWithoutValidation(k, v);

            var resp = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
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

