using SmartBank.Desktop.Win.Core;
using SmartBank.Desktop.Win.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel; // BindingList
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartBank.Desktop.Win.Views
{
    public partial class ReversalView : UserControl
    {
        private readonly ApiClient _client;

        private readonly BindingSource _bindingSource = new();
        private List<SelectReversalDto> _rows = new();

        private bool _suppressUi;
        public ReversalView(ApiClient api)
        {
            InitializeComponent();
            _client = api;

            // grid & combos
            SetupGrid();
            InitInputs();
            WireMenu();

            _ = LoadAllAsync();

            dgvRev.SelectionChanged += (_, __) => { FillDetailsFromCurrent(); UpdateVoidButtonState(); };
            _bindingSource.CurrentChanged += (_, __) => UpdateVoidButtonState();
        }

        private void SetupGrid()
        {
            dgvRev.AutoGenerateColumns = false;
            dgvRev.ReadOnly = true;
            dgvRev.AllowUserToAddRows = false;
            dgvRev.AllowUserToDeleteRows = false;
            dgvRev.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRev.DataSource = _bindingSource;

            colId.DataPropertyName = "Id";
            colTransactionId.DataPropertyName = "TransactionId";
            colReversedAmount.DataPropertyName = "ReversedAmount";
            colReason.DataPropertyName = "Reason";
            colPerformedBy.DataPropertyName = "PerformedBy";
            colStatus.DataPropertyName = "Status";
            colReversalDate.DataPropertyName = "ReversalDate";
            colReversalSource.DataPropertyName = "ReversalSource";
            colIsCardLimitRestored.DataPropertyName = "IsCardLimitRestored";
            colVoidedBy.DataPropertyName = "VoidedBy";
            colVoidedAt.DataPropertyName = "VoidedAt";
            colVoidReason.DataPropertyName = "VoidReason";

            // Hatalı hücrede default uyarıyı bastır
            dgvRev.DataError += (s, e) => e.ThrowException = false;

            // Hata kutucuğunu bastır
            dgvRev.DataError += (s, e) => e.ThrowException = false;

            // Görsel formatlar
            colReversedAmount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colReversedAmount.DefaultCellStyle.Format = "N2";

            colReversalDate.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            colVoidedAt.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvRev.SelectionChanged += (_, __) => FillDetailsFromCurrent();
        }

        private void InitInputs()
        {
            // Para/Tutar alanı
            nudAmount.DecimalPlaces = 2;
            nudAmount.ThousandsSeparator = true;
            nudAmount.Minimum = 0;
            nudAmount.Maximum = 99999999;

            // Kaynak
            cboSource.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSource.Items.Clear();
            cboSource.Items.AddRange(new object[] { "API", "MANUEL", "BATCH" });
            cboSource.SelectedItem = "API";

            // Readonly alanlar
            txtReversalId.ReadOnly = true;
            txtStatus.ReadOnly = true;
            txtDate.ReadOnly = true;
        }

        private void WireMenu()
        {
            miList.Click += async (_, __) => await LoadAllAsync();
            miGetById.Click += async (_, __) => await FetchByIdAsync();
            miGetByTxn.Click += async (_, __) => await FetchByTxnAsync();
            miCreate.Click += async (_, __) => await CreateAsync();
            miVoid.Click += async (_, __) => await VoidAsync();
            miClear.Click += async (_, __) => await ClearAndReloadAsync();
        }

        private void UpdateVoidButtonState()
        {
            var r = _bindingSource.Current as SelectReversalDto;
            miVoid.Enabled = r != null && string.Equals(r.Status, "S", StringComparison.OrdinalIgnoreCase);
        }

        // ---------------- API ----------------

        private async Task LoadAllAsync()
        {
            try
            {
                var list = await _client.GetAsync<List<SelectReversalDto>>("api/reversal") ?? new();
                _rows = list;
                BindRows(_rows);
            }
            catch (ApiException ex) { ShowApiError("Listeleme", ex); }
        }

        private async Task FetchByIdAsync()
        {
            if (!int.TryParse(txtSearchId.Text.Trim(), out var id) || id <= 0)
            { MessageBox.Show("Geçerli bir Reversal ID gir.", "Uyarı"); return; }

            try
            {
                var dto = await _client.GetAsync<SelectReversalDto>($"api/reversal/{id}");
                _rows = dto is null ? new() : new() { dto };
                BindRows(_rows);
            }
            catch (ApiException ex) { ShowApiError("ID ile Getir", ex); }
        }

        private async Task FetchByTxnAsync()
        {
            if (!int.TryParse(txtSearchTxnId.Text.Trim(), out var txnId) || txnId <= 0)
            { MessageBox.Show("Geçerli bir İşlem (Txn) ID gir.", "Uyarı"); return; }

            try
            {
                var list = await _client.GetAsync<List<SelectReversalDto>>($"api/reversal/tx/{txnId}") ?? new();
                _rows = list;
                BindRows(_rows);
            }
            catch (ApiException ex) { ShowApiError("Txn ile Getir", ex); }
        }

        private async Task CreateAsync()
        {
            // Gerekli alanlar: TransactionId, Amount, Reason, PerformedBy, Source
            if (!int.TryParse(txtSearchTxnId.Text.Trim(), out var txnId) || txnId <= 0)
            { MessageBox.Show("Oluşturmak için İşlem (Txn) ID gir.", "Uyarı"); return; }

            if (nudAmount.Value <= 0)
            { MessageBox.Show("Reversal tutarı > 0 olmalı.", "Uyarı"); return; }

            if (string.IsNullOrWhiteSpace(txtReason.Text))
            { MessageBox.Show("İptal nedeni zorunludur.", "Uyarı"); return; }

            if (string.IsNullOrWhiteSpace(txtPerformedBy.Text))
            { MessageBox.Show("İşlemi yapan zorunludur.", "Uyarı"); return; }

            var src = (cboSource.SelectedItem as string) ?? "API";

            var dto = new CreateReversalDto
            {
                TransactionId = txnId,
                ReversedAmount = nudAmount.Value,
                Reason = txtReason.Text.Trim(),
                PerformedBy = txtPerformedBy.Text.Trim(),
                ReversalSource = src
            };

            try
            {
                await _client.PostAsync<CreateReversalDto, object?>("api/reversal", dto);

                await LoadAllAsync();
                await ClearAndReloadAsync();

                MessageBox.Show("Reversal oluşturuldu.", "Bilgi");
            }
            catch (ApiException ex) { ShowApiError("Reversal Oluşturma", ex); }
        }

        private async Task VoidAsync()
        {
            // Seçili reversal ID veya txtId
            if (!int.TryParse(txtReversalId.Text.Trim(), out var id) || id <= 0)
            {
                MessageBox.Show("İptal etmek için bir reversal seç.", "Uyarı");
                return;
            }

            // İptal eden + sebep (opsiyonel ama PerformedBy zorunlu)
            var body = new
            {
                PerformedBy = string.IsNullOrWhiteSpace(txtPerformedBy.Text) ? "user" : txtPerformedBy.Text.Trim(),
                Reason = string.IsNullOrWhiteSpace(txtReason.Text) ? null : txtReason.Text.Trim()
            };

            try
            {
                await _client.PostAsync<object, object?>($"api/reversal/{id}/void", body);
                await LoadAllAsync();
                MessageBox.Show("Reversal iptal edildi.", "Bilgi");
            }
            catch (ApiException ex) { ShowApiError("Reversal İptal", ex); }
        }

        // --------------- UI Helpers ---------------

        private void BindRows(IEnumerable<SelectReversalDto> rows)
        {
            // İstersen ID’ye göre artan/azalan burada ayarlanır
            var ordered = rows.OrderBy(r => r.Id).ToList();

            _bindingSource.DataSource = new BindingList<SelectReversalDto>(ordered);
            dgvRev.ClearSelection();

            // Detayları sıfırla
            if (!_suppressUi)
            {
                txtReversalId.Clear();
                txtStatus.Clear();
                txtDate.Clear();
            }
        }

        private async Task ClearAndReloadAsync()
        {
            _suppressUi = true;

            // aramalar
            txtSearchId.Clear();
            txtSearchTxnId.Clear();

            // giriş alanları
            txtReason.Clear();
            txtPerformedBy.Clear();
            nudAmount.Value = 0;
            if (cboSource.Items.Count > 0) cboSource.SelectedIndex = 0;

            // detay
            txtReversalId.Clear();
            txtStatus.Clear();
            txtDate.Clear();

            dgvRev.CurrentCell = null;
            dgvRev.ClearSelection();
            _bindingSource.Position = -1;

            await LoadAllAsync();

            _suppressUi = false;
        }

        private void FillDetailsFromCurrent()
        {
            if (_suppressUi) return;
            if (_bindingSource.Current is not SelectReversalDto r) return;

            txtReversalId.Text = r.Id.ToString();
            txtStatus.Text = r.Status ?? "";
            txtDate.Text = r.ReversalDate == default ? "" : r.ReversalDate.ToString("dd.MM.yyyy HH:mm");
            txtReason.Text = r.Reason ?? "";
            nudAmount.Value = r.ReversedAmount;
            txtPerformedBy.Text = r.PerformedBy ?? "";
            txtSearchId.Text = r.Id.ToString();
            txtSearchTxnId.Text = r.TransactionId.ToString();
        }

        private void ShowApiError(string title, ApiException ex)
        {
            var body = (ex.ResponseBody ?? "").Trim();
            if (body.Length > 400) body = body[..400] + "...";

            var msg = ex.StatusCode switch
            {
                400 => body != "" ? body : "Geçersiz istek.",
                404 => "Kayıt bulunamadı veya kayıt bir Void işlem.",
                409 => body != "" ? body : "İş kuralı ihlali.",
                500 => "Sunucu hatası.",
                _ => body != "" ? body : $"İşlem başarısız. Kod: {ex.StatusCode}"
            };

            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void dgvRev_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRev.Rows[e.RowIndex].DataBoundItem is SelectReversalDto r && r.Status != "V")
            {
                var name = dgvRev.Columns[e.ColumnIndex].Name;
                if (name is "colVoidedBy" or "colVoidedAt" or "colVoidReason")
                    e.Value = ""; // V değilse boş göster
            }
        }

        private void ReversalView_Load(object sender, EventArgs e)
        {
            if (this.ParentForm != null)
            {
                this.ParentForm.Text = "SmartBank Ödeme Sistemleri / Reversal";
            }
        }
    }
}
