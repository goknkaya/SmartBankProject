using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartBank.Desktop.Win.Core;
using SmartBank.Desktop.Win.Core.Contracts;

namespace SmartBank.Desktop.Win.Views
{
    public partial class SwitchView : UserControl
    {
        private readonly ApiClient _client;
        private readonly BindingSource _bsMsgs = new();
        private readonly BindingSource _bsLogs = new();
        private bool _loadedOnce = false;
        private bool _loadingLogs = false; // re-entrancy guard

        public SwitchView(ApiClient client)
        {
            InitializeComponent();
            _client = client;

            InitInputs();
            SetupGrids();
            WireEvents();

            this.Load += async (_, __) => await LoadOnStartupAsync();
        }

        // ---------------- Startup ----------------

        private async Task LoadOnStartupAsync()
        {
            if (_loadedOnce) return;
            _loadedOnce = true;

            try
            {
                await LoadMessagesAsync();
                // İlk satırı seç -> SelectionChanged tetiklenir, loglar da yüklenir
                if (dgvMessages.Rows.Count > 0)
                {
                    dgvMessages.ClearSelection();
                    dgvMessages.Rows[0].Selected = true;
                }
            }
            catch (ApiException ex)
            {
                ShowApiError("Initial Load", ex);
            }
        }

        private void InitInputs()
        {
            cboCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCurrency.Items.Clear();
            cboCurrency.Items.AddRange(new object[] { "TRY", "USD", "EUR" });
            cboCurrency.SelectedItem = "TRY";

            if (string.IsNullOrWhiteSpace(txtAcquirer.Text))
                txtAcquirer.Text = "DemoPOS";

            dtpTxnTime.Format = DateTimePickerFormat.Custom;
            dtpTxnTime.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpTxnTime.Value = DateTime.Now;

            this.Load += (_, __) => UpdateFormTitle();
        }

        private void SetupGrids()
        {
            // Kolonlar Designer’dan geliyorsa AutoGenerateColumns=false doğru.
            dgvMessages.AutoGenerateColumns = false;
            dgvMessages.ReadOnly = true;
            dgvMessages.MultiSelect = false;
            dgvMessages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMessages.DataSource = _bsMsgs;

            dgvLogs.AutoGenerateColumns = false;
            dgvLogs.ReadOnly = true;
            dgvLogs.MultiSelect = false;
            dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLogs.DataSource = _bsLogs;

            // Seçim değişince logları yükle
            dgvMessages.SelectionChanged += async (_, __) => await LoadLogsForSelectedAsync();
        }

        private void WireEvents()
        {
            btnSend.Click += async (_, __) => await SendAsync();
            btnGetMessages.Click += async (_, __) => await LoadMessagesAsync();
            btnGetLogs.Click += async (_, __) => await LoadLogsForSelectedAsync();
            btnClear.Click += (_, __) => ClearInputs();
        }

        // ---------------- Actions ----------------

        private void ClearInputs()
        {
            txtPAN.Text = "";
            nudAmount.Value = nudAmount.Minimum < 0.01m ? 0.01m : nudAmount.Minimum;
            if (cboCurrency.Items.Contains("TRY"))
                cboCurrency.SelectedItem = "TRY";
            else if (cboCurrency.Items.Count > 0)
                cboCurrency.SelectedIndex = 0;

            txtAcquirer.Text = "DemoPOS";
            txtMerchant.Text = "";
            dtpTxnTime.Value = DateTime.Now;

            // Gridleri temizleme! Sadece seçimleri sıfırlayalım:
            dgvMessages.ClearSelection();
            dgvLogs.ClearSelection();

            txtPAN.Focus();
        }

        private async Task SendAsync()
        {
            try
            {
                SetBusy(true);
                var dto = new CreateSwitchMessageDto
                {
                    PAN = (txtPAN.Text ?? "").Trim(),
                    Amount = nudAmount.Value,
                    Currency = cboCurrency.SelectedItem?.ToString() ?? "TRY",
                    Acquirer = string.IsNullOrWhiteSpace(txtAcquirer.Text) ? "DemoPOS" : txtAcquirer.Text.Trim(),
                    MerchantName = string.IsNullOrWhiteSpace(txtMerchant.Text) ? null : txtMerchant.Text.Trim(),
                    TxnTime = dtpTxnTime.Value
                };

                var res = await _client.PostAsync<CreateSwitchMessageDto, SelectSwitchMessageDto>(
                    "api/switch/process", dto, withAuth: true);

                if (res != null)
                {
                    MessageBox.Show($"Cevap: {res.Status}\nId: {res.Id}\nIssuer: {res.Issuer}", "Process",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // mevcut seçim korunarak tazele
                    await LoadMessagesAsync(preserveSelectionId: res.Id);
                    ClearInputs();
                }
            }
            catch (ApiException ex)
            {
                ShowApiError("Switch Process", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task LoadMessagesAsync(int? preserveSelectionId = null)
        {
            try
            {
                SetBusy(true);
                // önceki seçim
                var selectedId = preserveSelectionId ?? (_bsMsgs.Current as SelectSwitchMessageDto)?.Id;

                var list = await _client.GetAsync<List<SelectSwitchMessageDto>>(
                               "api/switch/messages", withAuth: true)
                           ?? new List<SelectSwitchMessageDto>();

                list = list.OrderByDescending(x => x.Id).ToList();
                _bsMsgs.DataSource = new BindingList<SelectSwitchMessageDto>(list);

                // Seçimi geri getir
                dgvMessages.ClearSelection();
                if (selectedId.HasValue)
                {
                    var rowIndex = list.FindIndex(x => x.Id == selectedId.Value);
                    if (rowIndex >= 0 && rowIndex < dgvMessages.Rows.Count)
                        dgvMessages.Rows[rowIndex].Selected = true;
                }

                // log grid'i temizle (sadece veri, kolonları değil)
                _bsLogs.DataSource = new BindingList<SwitchLogDto>();
                dgvLogs.ClearSelection();
            }
            catch (ApiException ex)
            {
                ShowApiError("Switch Messages", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task LoadLogsForSelectedAsync()
        {
            if (_loadingLogs) return; // re-entrancy
            try
            {
                _loadingLogs = true;

                if (_bsMsgs.Current is not SelectSwitchMessageDto msg || msg.Id <= 0)
                {
                    _bsLogs.DataSource = new BindingList<SwitchLogDto>();
                    return;
                }

                var logs = await _client.GetAsync<List<SwitchLogDto>>(
                               $"api/switch/logs/{msg.Id}", withAuth: true)
                           ?? new List<SwitchLogDto>();

                _bsLogs.DataSource = new BindingList<SwitchLogDto>(logs);
                dgvLogs.ClearSelection();
            }
            catch (ApiException ex)
            {
                ShowApiError("Switch Logs", ex);
            }
            finally
            {
                _loadingLogs = false;
            }
        }

        // ---------------- Helpers ----------------

        private void SetBusy(bool busy)
        {
            UseWaitCursor = busy;
            btnSend.Enabled = !busy;
            btnGetMessages.Enabled = !busy;
            btnGetLogs.Enabled = !busy;
            btnClear.Enabled = !busy;
        }

        private void ShowApiError(string title, ApiException ex)
        {
            var body = (ex.ResponseBody ?? "").Trim();
            if (body.Length > 1000) body = body[..1000] + "...";
            MessageBox.Show(
                string.IsNullOrWhiteSpace(body) ? $"{title}: HTTP {ex.StatusCode}" : body,
                title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void UpdateFormTitle()
        {
            var f = this.FindForm();
            if (f != null)
                f.Text = "SmartBank Ödeme Sistemleri / Switch";
        }
    }
}
