using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartBank.Desktop.Win.Core;
using SmartBank.Desktop.Win.Core.Contracts;
using System.ComponentModel;

namespace SmartBank.Desktop.Win.Views
{
    public partial class SwitchView : UserControl
    {
        private readonly ApiClient _client;
        private readonly BindingSource _bsMsgs = new();
        private readonly BindingSource _bsLogs = new();

        public SwitchView(ApiClient client)
        {
            InitializeComponent();
            _client = client;

            InitInputs();
            SetupGrids();
            WireEvents();
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

            if (FindForm() is Form f)
                f.Text = "SmartBank Ödeme Sistemleri / Switch";
        }

        private void SetupGrids()
        {
            dgvMessages.AutoGenerateColumns = false;
            dgvMessages.DataSource = _bsMsgs;

            dgvLogs.AutoGenerateColumns = false;
            dgvLogs.DataSource = _bsLogs;

            // Seçim değişince logları yükle
            dgvMessages.SelectionChanged += async (_, __) => await LoadLogsForSelectedAsync();
        }

        private void WireEvents()
        {
            btnSend.Click += async (_, __) => await SendAsync();
            btnGetMessages.Click += async (_, __) => await LoadMessagesAsync();
            btnGetLogs.Click += async (_, __) => await LoadLogsForSelectedAsync();
        }

        // --- Actions ---

        private async Task SendAsync()
        {
            try
            {
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
                    MessageBox.Show($"Cevap: {res.Status}\nId: {res.Id}\nIssuer: {res.Issuer}", "Process");
                    await LoadMessagesAsync(); // tazele
                }
            }
            catch (ApiException ex)
            {
                ShowApiError("Switch Process", ex);
            }
        }

        private async Task LoadMessagesAsync()
        {
            try
            {
                var list = await _client.GetAsync<List<SelectSwitchMessageDto>>("api/switch/messages", withAuth: true)
                           ?? new List<SelectSwitchMessageDto>();

                list = list.OrderByDescending(x => x.Id).ToList();
                _bsMsgs.DataSource = new BindingList<SelectSwitchMessageDto>(list);
                dgvMessages.ClearSelection();

                // log grid'i temizle
                _bsLogs.DataSource = new BindingList<SwitchLogDto>();
                dgvLogs.ClearSelection();
            }
            catch (ApiException ex)
            {
                ShowApiError("Switch Messages", ex);
            }
        }

        private async Task LoadLogsForSelectedAsync()
        {
            try
            {
                if (_bsMsgs.Current is not SelectSwitchMessageDto msg || msg.Id <= 0)
                {
                    _bsLogs.DataSource = new BindingList<SwitchLogDto>();
                    return;
                }

                var logs = await _client.GetAsync<List<SwitchLogDto>>($"api/switch/logs/{msg.Id}", withAuth: true)
                           ?? new List<SwitchLogDto>();

                _bsLogs.DataSource = new BindingList<SwitchLogDto>(logs);
                dgvLogs.ClearSelection();
            }
            catch (ApiException ex)
            {
                ShowApiError("Switch Logs", ex);
            }
        }

        // --- Error helper (kısaca) ---
        private void ShowApiError(string title, ApiException ex)
        {
            var body = (ex.ResponseBody ?? "").Trim();
            if (body.Length > 1000) body = body[..1000] + "...";
            MessageBox.Show(string.IsNullOrWhiteSpace(body) ? $"{title}: HTTP {ex.StatusCode}" : body, title,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
