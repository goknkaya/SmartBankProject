using SmartBank.Desktop.Win.Core;
using SmartBank.Desktop.Win.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartBank.Desktop.Win.Views
{
    public partial class TransactionView : UserControl
    {
        private readonly ApiClient _api;
        private List<SelectTransactionDto> _rows = new();
        private List<SelectCardDto> _cards = new();

        private readonly BindingSource _bs = new();

        public TransactionView(ApiClient api)
        {
            InitializeComponent();
            _api = api;

            _bs.DataSource = typeof(SelectTransactionDto);

            // UI init
            SetupGrid();
            InitCombos();
            WireMenu();

            // ilk yük
            _ = LoadAllAsync();
            _ = LoadCardsAsync();
        }

        private void SetupGrid()
        {
            dgvTx.AutoGenerateColumns = false;
            dgvTx.ReadOnly = true;
            dgvTx.AllowUserToAddRows = false;
            dgvTx.AllowUserToDeleteRows = false;
            dgvTx.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTx.DataSource = _bs;

            colCardId.DataPropertyName = "Card";
            colCardId.HeaderText = "Kart";

            // görsel
            colAmount.DefaultCellStyle.Format = "N2";
            colAmount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colTxnDate.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            colId.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvTx.SelectionChanged += (_, __) => FillDetailsFromCurrent();
        }

        private void InitCombos()
        {
            // Para birimi
            cboCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCurrency.Items.Clear();
            cboCurrency.Items.AddRange(new object[] { "TRY", "USD", "EUR" });
            cboCurrency.SelectedItem = "TRY";

            // Tarih
            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = "dd MMM yyyy HH:mm";
            dtpDate.ShowUpDown = true;

            // Kart listesi (LoadCardsAsync dolduracak)
            cboCard.DropDownStyle = ComboBoxStyle.DropDownList;

            BindDetailControls();
        }

        private void BindDetailControls()
        {
            // önce eski bindingleri temizle
            txtDesc.DataBindings.Clear();
            nudAmount.DataBindings.Clear();
            cboCurrency.DataBindings.Clear();
            dtpDate.DataBindings.Clear();
            txtSearchId.DataBindings.Clear();
            txtSearchCardId.DataBindings.Clear();

            // sonra bağla
            txtDesc.DataBindings.Add("Text", _bs, "Description", true, DataSourceUpdateMode.Never, "");
            nudAmount.DataBindings.Add("Value", _bs, "Amount", true, DataSourceUpdateMode.Never, 0m);
            cboCurrency.DataBindings.Add("Text", _bs, "Currency", true, DataSourceUpdateMode.Never, "TRY");
            dtpDate.DataBindings.Add("Value", _bs, "TransactionDate", true, DataSourceUpdateMode.Never, DateTime.Now);
            txtSearchId.DataBindings.Add("Text", _bs, "Id", true, DataSourceUpdateMode.Never, "");
            txtSearchCardId.DataBindings.Add("Text", _bs, "CardId", true, DataSourceUpdateMode.Never, "");
        }

        private void WireMenu()
        {
            miList.Click += async (_, __) => await LoadAllAsync();
            miGetById.Click += async (_, __) => await FetchByIdAsync();
            miGetByCard.Click += async (_, __) => await FetchByCardAsync();
            miCreate.Click += async (_, __) => await CreateAsync();
            miClear.Click += (_, __) => ClearInputs(keepCard: true);
        }

        // ---- API CALLS ----

        private async Task LoadAllAsync()
        {
            try
            {
                var list = await _api.GetAsync<List<SelectTransactionDto>>("api/Transaction") ?? new();
                _rows = list;

                // Kart listesi hazırsa Card metinlerini doldur
                FillCardTexts(_rows);

                // BindingSource’a ver
                _bs.DataSource = _rows;
            }
            catch (ApiException ex)
            {
                ShowApiError("Listeleme Hatası", ex);
            }
        }

        private async Task LoadCardsAsync()
        {
            try
            {
                var list = await _api.GetAsync<List<SelectCardDto>>("api/Card");
                _cards = list ?? new();

                // kart seçimi için combo
                cboCard.DataSource = _cards
                    .Select(c => new { c.Id, Display = $"{c.Id} - {c.MaskedCardNumber}" })
                    .ToList();
                cboCard.DisplayMember = "Display";
                cboCard.ValueMember = "Id";

                // Eğer transactions daha önce geldiyse, Card metinlerini şimdi doldurup UI’yı tazele
                if (_rows.Count > 0)
                {
                    FillCardTexts(_rows);
                    _bs.ResetBindings(false);
                }
            }
            catch (ApiException ex)
            {
                ShowApiError("Kartları Yüklerken", ex);
            }
        }

        private async Task FetchByIdAsync()
        {
            if (!int.TryParse(txtSearchId.Text.Trim(), out var id) || id <= 0)
            { MessageBox.Show("Geçerli bir ID gir.", "Uyarı"); return; }

            try
            {
                var dto = await _api.GetAsync<SelectTransactionDto>($"api/Transaction/{id}");
                _rows = dto is null ? new() : new() { dto };
                FillCardTexts(_rows);
                _bs.DataSource = _rows;
            }
            catch (ApiException ex)
            {
                ShowApiError("ID ile getirme", ex);
            }
        }

        private async Task FetchByCardAsync()
        {
            int cardId = 0;

            if (!string.IsNullOrWhiteSpace(txtSearchCardId.Text) &&
                int.TryParse(txtSearchCardId.Text.Trim(), out var typed))
                cardId = typed;
            else if (cboCard.SelectedValue is int sel)
                cardId = sel;

            if (cardId <= 0) { MessageBox.Show("Geçerli bir Kart ID seç/gir.", "Uyarı"); return; }

            try
            {
                var list = await _api.GetAsync<List<SelectTransactionDto>>($"api/Transaction/card/{cardId}") ?? new();
                _rows = list;
                FillCardTexts(_rows);
                _bs.DataSource = _rows;
            }
            catch (ApiException ex)
            {
                ShowApiError("Kart işlemlerini getirirken", ex);
            }
        }

        private async Task CreateAsync()
        {
            if (cboCard.SelectedValue is not int cardId || cardId <= 0)
            { MessageBox.Show("Kart seç.", "Uyarı"); return; }
            if (nudAmount.Value <= 0)
            { MessageBox.Show("Tutar > 0 olmalı.", "Uyarı"); return; }
            if (cboCurrency.SelectedItem is not string curr || curr.Length != 3)
            { MessageBox.Show("Para birimi hatalı.", "Uyarı"); return; }

            var dto = new CreateTransactionDto
            {
                CardId = cardId, // ID ile
                Amount = nudAmount.Value,
                Currency = curr,
                Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim(),
                TransactionDate = dtpDate.Value
            };

            try
            {
                await _api.PostAsync<CreateTransactionDto, object?>("api/Transaction", dto);
                await FetchByCardAsync();                   // aynı kartın listesi
                MessageBox.Show("İşlem oluşturuldu.", "Bilgi");
            }
            catch (ApiException ex) { ShowApiError("İşlem oluşturma", ex); }
        }

        private void ClearInputs(bool keepCard = true)
        {
            // Kart seçimini koru istiyorsan dokunma; istemezsen -1 yap
            if (!keepCard)
            {
                cboCard.SelectedIndex = cboCard.Items.Count > 0 ? 0 : -1;
            }

            nudAmount.Value = 0;
            cboCurrency.SelectedItem = (cboCurrency.Items.Contains("TRY") ? "TRY" : cboCurrency.Items.Cast<object>().FirstOrDefault());
            dtpDate.Value = DateTime.Now;

            txtDesc.Clear();
            txtSearchId.Clear();
            txtSearchCardId.Clear();

            // Grid seçimini de sıfırla (veriler kalır)
            if (dgvTx.Rows.Count > 0)
                dgvTx.ClearSelection();

            // Odak ilk alana
            cboCard.Focus();
        }
        private void ClearAll()
        {
            ClearInputs(keepCard: false);

            // BindingSource'u boşalt (grid boş)
            _bs.DataSource = new List<SelectTransactionDto>();
            dgvTx.ClearSelection();
        }

        private void FillCardTexts(List<SelectTransactionDto> rows)
        {
            if (_cards == null || _cards.Count == 0) return;  // kartlar daha gelmediyse bekle

            var byId = _cards.ToDictionary(
                c => c.Id,
                c => c.MaskedCardNumber ?? $"#{c.Id}"
            );

            foreach (var t in rows)
            {
                // “123 - **** **** **** 4567” gibi
                t.Card = byId.TryGetValue(t.CardId, out var masked)
                    ? $"{t.CardId} - {masked}"
                    : $"#{t.CardId}";
            }
        }

        private void FillDetailsFromCurrent()
        {
            if (_bs.Current is not SelectTransactionDto r) return;

            txtSearchId.Text = r.Id.ToString();
            txtSearchCardId.Text = r.CardId.ToString();

            if (cboCard.DataSource != null)
                cboCard.SelectedValue = r.CardId;

            // readonly önizleme
            cboCurrency.Text = r.Currency;
            nudAmount.Value = r.Amount;
            dtpDate.Value = r.TransactionDate == default ? DateTime.Now : r.TransactionDate;
            txtDesc.Text = r.Description ?? "";
        }

        // ---- Helpers ----

        private void ShowApiError(string title, ApiException ex)
        {
            var msg = ex.StatusCode switch
            {
                400 => "Geçersiz istek. " + ex.ResponseBody,
                404 => "Kayıt bulunamadı.",
                409 => "Çakışma / iş kuralı ihlali. " + ex.ResponseBody,
                _ => $"Sunucu hatası ({ex.StatusCode}). " + ex.ResponseBody
            };
            MessageBox.Show(msg, title);
        }
    }
}
