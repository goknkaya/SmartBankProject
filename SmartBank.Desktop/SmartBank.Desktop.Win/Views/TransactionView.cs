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
    public partial class TransactionView : UserControl
    {
        private readonly ApiClient _api;

        private List<SelectTransactionDto> _rows = new();
        private List<SelectCardDto> _cards = new();

        private readonly BindingSource _bs = new();

        // UI guard’ları
        private bool _suppressDetailFill = false;
        private bool _suppressUi = false;

        public TransactionView(ApiClient api)
        {
            InitializeComponent();
            _api = api;

            _bs.DataSource = typeof(SelectTransactionDto);

            SetupGrid();
            InitCombos();
            WireMenu();

            _ = LoadAllAsync();
            _ = LoadCardsAsync();

            this.Load += (_, __) => UpdateFormTitle();
            this.VisibleChanged += (_, __) => { if (Visible) UpdateFormTitle(); };
        }

        // ================== UI INIT ==================

        private void SetupGrid()
        {
            dgvTx.AutoGenerateColumns = false;
            dgvTx.ReadOnly = true;
            dgvTx.AllowUserToAddRows = false;
            dgvTx.AllowUserToDeleteRows = false;
            dgvTx.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Grid -> BindingSource
            dgvTx.DataSource = _bs;

            // Kart kolonu: CardId yerine “{Id} - {Masked}”
            colCardId.DataPropertyName = "Card";
            colCardId.HeaderText = "Kart";

            // Görsel formatlar
            colAmount.DefaultCellStyle.Format = "N2";
            colAmount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colTxnDate.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            colId.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Satır değişince sağdaki alanları ön-izleme olarak doldur
            dgvTx.SelectionChanged += (_, __) => FillDetailsFromCurrent();

            // Varsayılan DataGridView hata kutusunu bastır (format vs.)
            dgvTx.DataError += (s, e) => { e.ThrowException = false; };
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
            // Tüm eski bindingleri temizle
            txtDesc.DataBindings.Clear();
            nudAmount.DataBindings.Clear();
            cboCurrency.DataBindings.Clear();
            dtpDate.DataBindings.Clear();

            // ⚠ Arama kutularını binding’e bağlamıyoruz (manuel yöneteceğiz)
            txtSearchId.DataBindings.Clear();
            txtSearchCardId.DataBindings.Clear();

            // Detay alanlarını listeye “okuma” amaçlı bağla
            txtDesc.DataBindings.Add("Text", _bs, "Description", true, DataSourceUpdateMode.Never, "");
            nudAmount.DataBindings.Add("Value", _bs, "Amount", true, DataSourceUpdateMode.Never, 0m);
            cboCurrency.DataBindings.Add("Text", _bs, "Currency", true, DataSourceUpdateMode.Never, "TRY");
            dtpDate.DataBindings.Add("Value", _bs, "TransactionDate", true, DataSourceUpdateMode.Never, DateTime.Now);
        }

        private void WireMenu()
        {
            miList.Click += async (_, __) => await LoadAllAsync();
            miGetById.Click += async (_, __) => await FetchByIdAsync();
            miGetByCard.Click += async (_, __) => await FetchByCardAsync();
            miCreate.Click += async (_, __) => await CreateAsync();
            miClear.Click += async (_, __) => await ClearAndReloadAsync();
        }

        // ================== API CALLS ==================

        private async Task LoadAllAsync()
        {
            try
            {
                var list = await _api.GetAsync<List<SelectTransactionDto>>("api/Transaction") ?? new();
                _rows = list;

                // Kart listesi geldiyse Card metinlerini doldur
                FillCardTexts(_rows);

                // Grid’e bağla + desc sıralama
                BindRows(_rows);

                // Temizlik anındaysak, seçim olmasın
                if (_suppressUi)
                {
                    _bs.Position = -1;
                    dgvTx.CurrentCell = null;
                    dgvTx.ClearSelection();
                }
            }
            catch (ApiException ex) { ShowApiError("Listeleme Hatası", ex); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Listeleme Hatası"); }
        }

        private async Task LoadCardsAsync()
        {
            try
            {
                var list = await _api.GetAsync<List<SelectCardDto>>("api/Card");
                _cards = list ?? new();

                // Kart seçimi için combo: "Id - **** **** 1234"
                cboCard.DataSource = _cards
                    .Select(c => new { c.Id, Display = $"{c.Id} - {c.MaskedCardNumber}" })
                    .ToList();
                cboCard.DisplayMember = "Display";
                cboCard.ValueMember = "Id";

                // Transaction daha önce geldiyse, kart metinlerini güncelle
                if (_rows.Count > 0)
                {
                    FillCardTexts(_rows);
                    _bs.ResetBindings(false);
                }
            }
            catch (ApiException ex) { ShowApiError("Kartları Yüklerken", ex); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Kartları Yüklerken"); }
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
                BindRows(_rows);
            }
            catch (ApiException ex) { ShowApiError("ID ile getirme", ex); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata"); }
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
                BindRows(_rows);
            }
            catch (ApiException ex) { ShowApiError("Kart işlemlerini getirirken", ex); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata"); }
        }

        private async Task CreateAsync()
        {
            // Validasyon
            if (cboCard.SelectedValue is not int cardId || cardId <= 0)
            { MessageBox.Show("Kart seç.", "Uyarı"); return; }

            if (nudAmount.Value <= 0)
            { MessageBox.Show("Tutar > 0 olmalı.", "Uyarı"); return; }

            if (cboCurrency.SelectedItem is not string curr || curr.Length != 3)
            { MessageBox.Show("Para birimi hatalı.", "Uyarı"); return; }

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
                await _api.PostAsync<CreateTransactionDto, object?>("api/Transaction", dto);

                // Tüm liste kalsın (filtre yok)
                await LoadAllAsync();

                // Formu temizle (tek tık)
                await ClearAndReloadAsync();

                MessageBox.Show("İşlem oluşturuldu.", "Bilgi");
            }
            catch (ApiException ex) { ShowApiError("İşlem oluşturma", ex); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "İşlem oluşturma"); }
        }

        // ================== UI HELPERS ==================

        /// <summary>
        /// Grid’e veri bağlar. ID’ye göre azalan sıralama + BindingList ile güvenli binding.
        /// </summary>
        private void BindRows(IEnumerable<SelectTransactionDto> rows)
        {
            var ordered = rows
                .OrderBy(r => r.Id) // istersen .OrderByDescending(r => r.TransactionDate)
                .ToList();

            _bs.DataSource = new BindingList<SelectTransactionDto>(ordered);
            dgvTx.CurrentCell = null;
            dgvTx.ClearSelection();
        }

        private async Task ClearAndReloadAsync()
        {
            _suppressUi = true; // temizlik süresince UI doldurulmasın

            // 1) Kart seçimi ve aramalar
            if (cboCard.DataSource != null)
            {
                cboCard.SelectedIndex = -1;
                cboCard.SelectedItem = null;
                cboCard.Text = string.Empty;
            }
            txtSearchId.Clear();
            txtSearchCardId.Clear();

            // 2) Grid’i boşalt ve seçimi kaldır
            _bs.DataSource = new BindingList<SelectTransactionDto>();
            dgvTx.CurrentCell = null;
            dgvTx.ClearSelection();
            _bs.Position = -1;

            // 3) Tam listeyi tekrar yükle
            await LoadAllAsync();

            // 4) Yüklemeden sonra da seçimi sıfırla (kutular boş kalsın)
            _bs.Position = -1;
            dgvTx.CurrentCell = null;
            dgvTx.ClearSelection();

            // 5) Form alanları
            nudAmount.Value = 0;
            if (cboCurrency.Items.Count > 0) cboCurrency.SelectedIndex = 0;
            dtpDate.Value = DateTime.Now;
            txtDesc.Clear();

            _suppressUi = false;
        }

        private void FillCardTexts(List<SelectTransactionDto> rows)
        {
            if (_cards == null || _cards.Count == 0) return;

            var byId = _cards.ToDictionary(
                c => c.Id,
                c => c.MaskedCardNumber ?? $"#{c.Id}"
            );

            foreach (var t in rows)
            {
                t.Card = byId.TryGetValue(t.CardId, out var masked)
                    ? $"{t.CardId} - {masked}"
                    : $"#{t.CardId}";
            }
        }

        private void FillDetailsFromCurrent()
        {
            if (_suppressUi || _suppressDetailFill) return;

            var r = _bs.Current as SelectTransactionDto;

            if (r == null)
            {
                // Seçim yoksa arama kutularını boş bırak
                if (!txtSearchId.Focused) txtSearchId.Clear();
                if (!txtSearchCardId.Focused) txtSearchCardId.Clear();
                return;
            }

            // Arama kutularını manuel doldur (kullanıcı o anda yazmıyorsa)
            if (!txtSearchId.Focused) txtSearchId.Text = r.Id.ToString();
            if (!txtSearchCardId.Focused) txtSearchCardId.Text = r.CardId.ToString();

            if (cboCard.DataSource != null)
                cboCard.SelectedValue = r.CardId;

            // Önizleme
            cboCurrency.Text = r.Currency;
            nudAmount.Value = r.Amount;
            dtpDate.Value = r.TransactionDate == default ? DateTime.Now : r.TransactionDate;
            txtDesc.Text = r.Description ?? "";
        }

        // ================== ERRORS ==================

        private void ShowApiError(string title, ApiException ex)
        {
            var body = (ex.ResponseBody ?? "").Trim();
            if (body.Length > 400) body = body[..400] + "...";

            string userMsg = ex.StatusCode switch
            {
                400 => body != "" ? body : "Geçersiz istek.",
                404 => "Kayıt bulunamadı.",
                409 => body != "" ? body : "İş kuralı ihlali.",
                500 => body.Contains("limit", StringComparison.OrdinalIgnoreCase)
                        ? "İşlem tutarı limitleri aşıyor."
                        : "Sunucu hatası.",
                _ => body != "" ? body : $"İşlem başarısız. Kod: {ex.StatusCode}"
            };

            MessageBox.Show(userMsg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void UpdateFormTitle()
        {
            var f = this.FindForm();
            if (f != null)
                f.Text = "SmartBank Ödeme Sistemleri / Transaction";
        }
    }
}
