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
        public TransactionView(ApiClient api)
        {
            InitializeComponent();
            _api = api;

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
        }

        private void WireMenu()
        {
            miList.Click += async (_, __) => await LoadAllAsync();
            miGetById.Click += async (_, __) => await FetchByIdAsync();
            miGetByCard.Click += async (_, __) => await FetchByCardAsync();
            miCreate.Click += async (_, __) => await CreateAsync();
        }

        // ---- API CALLS ----

        private async Task LoadAllAsync()
        {
            try
            {
                var list = await _api.GetAsync<List<SelectTransactionDto>>("api/Transaction");
                _rows = list ?? new();
                dgvTx.DataSource = _rows;
            }
            catch (ApiException ex)
            {
                ShowApiError("Listeleme Hatası", ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Listeleme Hatası");
            }
        }

        private async Task LoadCardsAsync()
        {
            try
            {
                var list = await _api.GetAsync<List<SelectCardDto>>("api/Card");
                _cards = list ?? new();

                // Kart seçimi için: "Id - Masked"
                cboCard.DataSource = _cards
                    .Select(c => new
                    {
                        Id = c.Id,
                        Display = $"{c.Id} - {c.MaskedCardNumber}"
                    })
                    .ToList();
                cboCard.DisplayMember = "Display";
                cboCard.ValueMember = "Id";
            }
            catch (ApiException ex)
            {
                ShowApiError("Kartları Yüklerken", ex);
            }
        }

        private async Task FetchByIdAsync()
        {
            if (!int.TryParse(txtSearchId.Text.Trim(), out var id) || id <= 0)
            {
                MessageBox.Show("Geçerli bir ID gir.", "Uyarı");
                return;
            }

            try
            {
                var dto = await _api.GetAsync<SelectTransactionDto>($"api/Transaction/{id}");
                dgvTx.DataSource = new List<SelectTransactionDto> { dto };
            }
            catch (ApiException ex)
            {
                ShowApiError("ID ile getirme", ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata");
            }
        }

        private async Task FetchByCardAsync()
        {
            // Öncelik arama kutusu; boşsa combodaki değer
            int cardId = 0;
            if (!string.IsNullOrWhiteSpace(txtSearchCardId.Text) &&
                int.TryParse(txtSearchCardId.Text.Trim(), out var sId))
            {
                cardId = sId;
            }
            else if (cboCard.SelectedValue is int vId)
            {
                cardId = vId;
            }

            if (cardId <= 0)
            {
                MessageBox.Show("Geçerli bir Kart ID seç/gir.", "Uyarı");
                return;
            }

            try
            {
                var list = await _api.GetAsync<List<SelectTransactionDto>>($"api/Transaction/card/{cardId}");
                dgvTx.DataSource = list ?? new List<SelectTransactionDto>();
            }
            catch (ApiException ex)
            {
                ShowApiError("Kart işlemlerini getirirken", ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata");
            }
        }

        private async Task CreateAsync()
        {
            // Validasyon
            if (cboCard.SelectedValue is not int cardId || cardId <= 0)
            {
                MessageBox.Show("Kart seç.", "Uyarı"); return;
            }
            if (nudAmount.Value <= 0)
            {
                MessageBox.Show("Tutar > 0 olmalı.", "Uyarı"); return;
            }
            if (cboCurrency.SelectedItem is not string curr || curr.Length != 3)
            {
                MessageBox.Show("Para birimi hatalı.", "Uyarı"); return;
            }

            var dto = new CreateTransactionDto
            {
                CardId = cardId,
                Amount = nudAmount.Value,
                Currency = curr,
                Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim(),
                TransactionDate = dtpDate.Value
            };

            try
            {
                // API OK/BadRequest string döndürüyor; burada 200 bekliyoruz
                var resp = await _api.PostAsync<CreateTransactionDto, object?>("api/Transaction", dto);

                // Yeniden yükle
                await FetchByCardAsync();
                MessageBox.Show("İşlem oluşturuldu.", "Bilgi");
            }
            catch (ApiException ex)
            {
                ShowApiError("İşlem oluşturma", ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata");
            }
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
