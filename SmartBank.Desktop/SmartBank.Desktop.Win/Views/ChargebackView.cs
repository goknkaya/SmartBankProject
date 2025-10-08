using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartBank.Desktop.Win.Core;                 // ApiClient
using SmartBank.Desktop.Win.Core.Contracts;       // DTO'lar (Contracts/ChargebackDtos.cs)

namespace SmartBank.Desktop.Win.Views
{
    public partial class ChargebackView : UserControl
    {
        private readonly ApiClient _client;
        private readonly BindingSource _bsCases = new();
        private readonly BindingSource _bsEvents = new();

        public ChargebackView(ApiClient client)
        {
            InitializeComponent();
            _client = client;

            InitInputs();
            SetupGrids();
            WireEvents();

            // ekran açılır açılmaz listele
            _ = LoadCasesAsync();
        }

        #region Init

        private void InitInputs()
        {
            // Status filtre seçenekleri
            cboStatus.Items.Clear();
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.Items.AddRange(new object[] { "", "Open", "Won", "Lost", "Cancelled" });
            cboStatus.SelectedIndex = 0;

            // Enter ile arama
            txtTxnFilter.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    await LoadCasesAsync();
                }
            };

            this.Load += (_, __) => UpdateFormTitle();
        }

        private void SetupGrids()
        {
            // === CASES GRID ===
            dgvCases.AutoGenerateColumns = false;
            dgvCases.ReadOnly = true;
            dgvCases.MultiSelect = false;
            dgvCases.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCases.AllowUserToAddRows = false;
            dgvCases.AllowUserToDeleteRows = false;
            dgvCases.RowHeadersVisible = false;
            dgvCases.DataSource = _bsCases;

            dgvCases.Columns.Clear();
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "Id", DataPropertyName = "Id", Width = 60 });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTransactionId", HeaderText = "TxnId", DataPropertyName = "TransactionId", Width = 90 });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", DataPropertyName = "Status", Width = 90 });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colReasonCode", HeaderText = "Reason", DataPropertyName = "ReasonCode", Width = 90 });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDisputedAmount",
                HeaderText = "Disputed",
                DataPropertyName = "DisputedAmount",
                Width = 90,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N2" }
            });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCurrency", HeaderText = "Cur", DataPropertyName = "Currency", Width = 50 });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTransactionAmount",
                HeaderText = "TxnAmt",
                DataPropertyName = "TransactionAmount",
                Width = 90,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N2" }
            });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMerchantName", HeaderText = "Merchant", DataPropertyName = "MerchantName", Width = 160 });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOpenedAt", HeaderText = "Opened", DataPropertyName = "OpenedAt", Width = 130, DefaultCellStyle = { Format = "yyyy-MM-dd HH:mm" } });
            dgvCases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colReplyBy", HeaderText = "ReplyBy", DataPropertyName = "ReplyBy", Width = 110, DefaultCellStyle = { Format = "yyyy-MM-dd" } });

            // Çift tık ile detay dialogu
            dgvCases.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgvCases.Rows[e.RowIndex].DataBoundItem is SelectChargebackCaseDto d)
                {
                    using var dlg = new CaseDetailDialog(d);
                    dlg.ShowDialog(this);
                }
            };

            // === EVENTS GRID ===
            dgvEvents.AutoGenerateColumns = false;
            dgvEvents.ReadOnly = true;
            dgvEvents.MultiSelect = false;
            dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEvents.AllowUserToAddRows = false;
            dgvEvents.AllowUserToDeleteRows = false;
            dgvEvents.RowHeadersVisible = false;
            dgvEvents.DataSource = _bsEvents;

            dgvEvents.Columns.Clear();
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCreatedAt", HeaderText = "When", DataPropertyName = "CreatedAt", Width = 150, DefaultCellStyle = { Format = "yyyy-MM-dd HH:mm" } });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "colType", HeaderText = "Type", DataPropertyName = "Type", Width = 140 });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNote", HeaderText = "Note", DataPropertyName = "Note", Width = 320 });
            dgvEvents.Columns.Add(new DataGridViewLinkColumn { Name = "colEvidenceUrl", HeaderText = "Evidence", DataPropertyName = "EvidenceUrl", Width = 220 });

            // Evidence link tıklanınca aç
            dgvEvents.CellContentClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgvEvents.Columns[e.ColumnIndex] is DataGridViewLinkColumn)
                {
                    var url = (string?)dgvEvents.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        try
                        {
                            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                                url = System.IO.Path.GetFullPath(url); // local path'i tam yola çevir

                            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Evidence açılamadı: {ex.Message}", "Chargeback");
                        }
                    }
                }
            };
        }

        private void WireEvents()
        {
            btnSearch.Click += async (s, e) => await LoadCasesAsync();
            btnRefresh.Click += async (s, e) => await LoadCasesAsync();
            btnOpen.Click += async (s, e) => await OpenCaseAsync();
            btnAddEvidence.Click += async (s, e) => await AddEvidenceAsync();
            btnDecide.Click += async (s, e) => await DecideAsync();

            dgvCases.SelectionChanged += async (s, e) => await LoadEventsForSelectedAsync();
        }

        #endregion

        #region Data Ops

        private async Task LoadCasesAsync()
        {
            ToggleBusy(true);
            try
            {
                int? txnId = null;
                if (int.TryParse(txtTxnFilter.Text?.Trim(), out var t)) txnId = t;
                var status = (string)(cboStatus.SelectedItem ?? string.Empty);

                // Query string'i kendimiz kuruyoruz
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(status))
                    parts.Add("status=" + Uri.EscapeDataString(status));
                if (txnId.HasValue)
                    parts.Add("transactionId=" + txnId.Value);

                var url = "/api/chargeback/cases" + (parts.Count > 0 ? "?" + string.Join("&", parts) : "");

                var list = await _client.GetAsync<List<SelectChargebackCaseDto>>(url);

                _bsCases.DataSource = list ?? new List<SelectChargebackCaseDto>();
                lblCount.Text = $"Count: {(_bsCases.List as IList<SelectChargebackCaseDto>)?.Count ?? 0}";

                // liste yenilenince event'leri temizle
                _bsEvents.DataSource = new List<SelectChargebackEventDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Liste yüklenemedi: {ex.Message}", "Chargeback", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { ToggleBusy(false); }
        }

        private async Task LoadEventsForSelectedAsync()
        {
            if (_bsCases.Current is not SelectChargebackCaseDto row) return;

            ToggleBusy(true);
            try
            {
                var evs = await _client.GetAsync<List<SelectChargebackEventDto>>(
                    $"/api/chargeback/cases/{row.Id}/events");

                _bsEvents.DataSource = evs ?? new List<SelectChargebackEventDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Event'ler yüklenemedi: {ex.Message}", "Chargeback", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { ToggleBusy(false); }
        }

        private async Task OpenCaseAsync()
        {
            if (!InputBox.Int("Yeni Case", "TransactionId gir:", out var txnId)) return;
            if (!InputBox.Text("Yeni Case", "ReasonCode (örn: 4853):", out var reason)) return;
            if (!InputBox.Decimal("Yeni Case", "DisputedAmount:", out var disputed)) return;

            var body = new CreateChargebackDto
            {
                TransactionId = txnId,
                ReasonCode = string.IsNullOrWhiteSpace(reason) ? "4853" : reason.Trim(),
                DisputedAmount = disputed
            };

            ToggleBusy(true);
            try
            {
                var created = await _client.PostAsync<CreateChargebackDto, SelectChargebackCaseDto>(
                    "/api/chargeback/open", body);

                await LoadCasesAsync();
                SelectRowById(created.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Case açılamadı: {ex.Message}", "Chargeback", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { ToggleBusy(false); }
        }

        private async Task AddEvidenceAsync()
        {
            if (_bsCases.Current is not SelectChargebackCaseDto row)
            {
                MessageBox.Show("Önce listeden bir case seç.", "Chargeback"); return;
            }

            if (!InputBox.Text("Kanıt Ekle", "Evidence URL/Yol:", out var url)) return;
            InputBox.Text("Kanıt Ekle", "Not (opsiyonel):", out var note);

            var req = new AddEvidenceDto { EvidenceUrl = url, Note = note };

            ToggleBusy(true);
            try
            {
                await _client.PostAsync<AddEvidenceDto, SelectChargebackCaseDto>(
                    $"/api/chargeback/cases/{row.Id}/evidence", req);

                await LoadEventsForSelectedAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kanıt eklenemedi: {ex.Message}", "Chargeback", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { ToggleBusy(false); }
        }

        private async Task DecideAsync()
        {
            if (_bsCases.Current is not SelectChargebackCaseDto row)
            {
                MessageBox.Show("Önce listeden bir case seç.", "Chargeback"); return;
            }

            if (!InputBox.Text("Karar Ver", "Decision (Won/Lost/Cancelled):", out var decision)) return;
            InputBox.Text("Karar Ver", "Not (opsiyonel):", out var note);

            var req = new DecideChargebackDto { Decision = decision?.Trim() ?? "", Note = note };

            ToggleBusy(true);
            try
            {
                await _client.PostAsync<DecideChargebackDto, SelectChargebackCaseDto>(
                    $"/api/chargeback/cases/{row.Id}/decide", req);

                await LoadCasesAsync();
                SelectRowById(row.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Karar verilemedi: {ex.Message}", "Chargeback", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { ToggleBusy(false); }
        }

        #endregion

        #region Helpers

        private void ToggleBusy(bool busy)
        {
            UseWaitCursor = busy;
            foreach (Control c in Controls) c.Enabled = !busy;
        }

        private void SelectRowById(int id)
        {
            for (int i = 0; i < dgvCases.Rows.Count; i++)
            {
                if (dgvCases.Rows[i].DataBoundItem is SelectChargebackCaseDto item && item.Id == id)
                {
                    dgvCases.ClearSelection();
                    dgvCases.Rows[i].Selected = true;
                    if (i >= 0) dgvCases.FirstDisplayedScrollingRowIndex = i;
                    break;
                }
            }
        }

        private void UpdateFormTitle()
        {
            var f = this.FindForm();
            if (f != null)
                f.Text = "SmartBank Ödeme Sistemleri / Chargeback";
        }

        // basit InputBox yardımcıları
        private static class InputBox
        {
            public static bool Text(string title, string prompt, out string value)
            {
                using var f = new Form { Width = 420, Height = 160, Text = title, StartPosition = FormStartPosition.CenterParent };
                var lbl = new Label { Left = 10, Top = 12, Text = prompt, AutoSize = true };
                var tb = new TextBox { Left = 10, Top = 36, Width = 380 };
                var ok = new Button { Text = "OK", Left = 230, Top = 70, Width = 70, DialogResult = DialogResult.OK };
                var cancel = new Button { Text = "İptal", Left = 320, Top = 70, Width = 70, DialogResult = DialogResult.Cancel };
                f.Controls.AddRange(new Control[] { lbl, tb, ok, cancel });
                f.AcceptButton = ok; f.CancelButton = cancel;
                var r = f.ShowDialog();
                value = tb.Text;
                return r == DialogResult.OK;
            }

            public static bool Int(string title, string prompt, out int value)
            {
                if (!Text(title, prompt, out var s)) { value = 0; return false; }
                return int.TryParse(s, out value);
            }

            public static bool Decimal(string title, string prompt, out decimal value)
            {
                if (!Text(title, prompt, out var s)) { value = 0; return false; }
                return decimal.TryParse(s, out value);
            }
        }

        #endregion
    }

    // === Basit detay dialogu (read-only) ===
    internal class CaseDetailDialog : Form
    {
        public CaseDetailDialog(SelectChargebackCaseDto d)
        {
            Text = $"Case #{d.Id}";
            Width = 560; Height = 420; StartPosition = FormStartPosition.CenterParent;

            var tb = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical
            };

            tb.Text =
            $@"TxnId: {d.TransactionId}
            Status: {d.Status}
            Reason: {d.ReasonCode}
            Disputed: {d.DisputedAmount:N2} {d.Currency}
            TxnAmt: {d.TransactionAmount:N2} {d.Currency}
            Merchant: {d.MerchantName}
            Opened: {d.OpenedAt:yyyy-MM-dd HH:mm}
            Closed: {(d.ClosedAt?.ToString("yyyy-MM-dd HH:mm") ?? "-")}
            ReplyBy: {(d.ReplyBy?.ToString("yyyy-MM-dd") ?? "-")}
            Note:
            {d.Note}";

            var ok = new Button { Text = "Kapat", Dock = DockStyle.Bottom, Height = 36, DialogResult = DialogResult.OK };

            Controls.Add(tb);
            Controls.Add(ok);
            AcceptButton = ok;
        }
    }


}
