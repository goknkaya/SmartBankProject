using SmartBank.Desktop.Win.Core;
using SmartBank.Desktop.Win.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartBank.Desktop.Win.Views
{
    public partial class CardView : UserControl
    {
        private readonly ApiClient _api;

        private enum ViewMode { List, Create, Edit }
        private ViewMode _mode = ViewMode.List;

        private List<SelectCardDto> _cards = new();
        private SelectCardDto? _selected;

        // form doldururken preview tetiklenmesini engellemek için
        private bool _suspendPreview = false;

        public CardView(ApiClient api)
        {
            InitializeComponent();
            _api = api;

            // MENÜ CLICKLERİ
            miList.Click += miList_Click;
            miInsert.Click += miInsert_Click;
            miUpdate.Click += miUpdate_Click;
            miDelete.Click += miDelete_Click;
            miSave.Click += miSave_Click;
            miCancel.Click += miCancel_Click;

            // UI init
            InitCombos();
            SetupGrid();
            WireEvents();

            // ilk ekran
            _ = LoadAllAsync();
            EnterViewMode();

            this.Load += (_, __) => UpdateFormTitle();
            this.VisibleChanged += (_, __) => { if (Visible) UpdateFormTitle(); };
        }

        #region Init

        private sealed class ComboItem { public string Text { get; } public string Value { get; } public ComboItem(string t, string v) { Text = t; Value = v; } }

        private void InitCombos()
        {
            // Sabit combo stilleri
            foreach (var cb in new[] { cboExpM, cboExpY, cboCurrency, cboProvider, cboType, cboStatus, cboBank })
                cb.DropDownStyle = ComboBoxStyle.DropDownList;

            // Ay (01..12)
            cboExpM.DataSource = Enumerable.Range(1, 12).Select(i => i.ToString("00")).ToList();

            // Yıl (bu yıl..+10) -> API "yy" bekliyor kabulü
            var twoDigit = true;
            int start = DateTime.Now.Year, end = start + 10;
            cboExpY.DataSource = Enumerable.Range(start, end - start + 1)
                                           .Select(y => twoDigit ? (y % 100).ToString("00") : y.ToString())
                                           .ToList();

            // Para birimi
            cboCurrency.DisplayMember = "Text"; cboCurrency.ValueMember = "Value";
            cboCurrency.DataSource = new List<ComboItem> {
                new("TRY","TRY"), new("USD","USD"), new("EUR","EUR")
            };
            cboCurrency.SelectedValue = "TRY";

            // Sağlayıcı (V/M/T)
            cboProvider.DisplayMember = "Text"; cboProvider.ValueMember = "Value";
            cboProvider.DataSource = new List<ComboItem> {
                new("V - Visa","V"), new("M - Mastercard","M"), new("T - Troy","T")
            };
            cboProvider.SelectedValue = "V";

            // Tip (D/C/P)
            cboType.DisplayMember = "Text"; cboType.ValueMember = "Value";
            cboType.DataSource = new List<ComboItem> {
                new("D - Debit","D"), new("C - Credit","C"), new("P - Prepaid","P")
            };
            cboType.SelectedValue = "D";

            // Durum (A/B/C)
            cboStatus.DisplayMember = "Text"; cboStatus.ValueMember = "Value";
            cboStatus.DataSource = new List<ComboItem> {
                new("A - Active","A"), new("B - Blocked","B"), new("C - Cancelled","C")
            };
            cboStatus.SelectedValue = "A";

            // Exp default
            cboExpM.SelectedItem = DateTime.Now.Month.ToString("00");
            cboExpY.SelectedItem = (DateTime.Now.Year % 100).ToString("00");

            // Banka: Designer'da Edit Items ile doldurduysan burada data source set etmene gerek yok.
            if (string.IsNullOrWhiteSpace(cboBank.Text) && cboBank.Items.Count > 0)
                cboBank.SelectedIndex = 0;
        }

        private void SetupGrid()
        {
            dgvCards.AutoGenerateColumns = false;
            dgvCards.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCards.MultiSelect = false;
            dgvCards.ReadOnly = true;

            colCustomer.DataPropertyName = nameof(SelectCardDto.CustomerFullName);
            colCard.DataPropertyName = nameof(SelectCardDto.MaskedCardNumber);
            colExpM.DataPropertyName = nameof(SelectCardDto.ExpiryMonth);
            colExpY.DataPropertyName = nameof(SelectCardDto.ExpiryYear);
            colStatus.DataPropertyName = nameof(SelectCardDto.CardStatus);
            colProvider.DataPropertyName = nameof(SelectCardDto.CardProvider);
            colCardLimit.DataPropertyName = nameof(SelectCardDto.CardLimit);
            colDaily.DataPropertyName = nameof(SelectCardDto.DailyLimit);
            colTxn.DataPropertyName = nameof(SelectCardDto.TransactionLimit);
            colVirtual.DataPropertyName = nameof(SelectCardDto.IsVirtual);
            colContactless.DataPropertyName = nameof(SelectCardDto.IsContactless);
            colBlocked.DataPropertyName = nameof(SelectCardDto.IsBlocked);
        }

        private void WireEvents()
        {
            dgvCards.SelectionChanged += async (_, __) =>
            {
                if (_mode != ViewMode.List) return;

                var dto = GetSelectedRow();
                if (dto == null) return;

                FillForm(dto); // maskeli verilerle doldur
                await LoadPanToPreviewAsync(dto.Id, dto.MaskedCardNumber ?? ""); // önizlemeye maskesiz PAN getir
            };

            // önizleme güncellemeleri
            txtCardHolder.TextChanged += (_, __) => UpdatePreview();
            txtCardNumber.TextChanged += (_, __) => UpdatePreview();
            cboExpM.SelectedIndexChanged += (_, __) => UpdatePreview();
            cboExpY.SelectedIndexChanged += (_, __) => UpdatePreview();
            cboProvider.SelectedIndexChanged += (_, __) => UpdatePreview();
            cboBank.SelectedIndexChanged += (_, __) => UpdatePreview();
        }

        #endregion

        #region Load

        private async Task LoadAllAsync()
        {
            await LoadCustomersAsync();
            await LoadCardsAsync();
        }

        private async Task LoadCustomersAsync()
        {
            var customers = await _api.GetAsync<List<SmartBank.Desktop.Win.Core.Contracts.SelectCustomerDto>>("api/Customer");
            customers ??= new();

            cboCustomer.DisplayMember = "FullName";
            cboCustomer.ValueMember = "Id";
            cboCustomer.DataSource = customers
                .Select(c => new
                {
                    c.Id,
                    FullName = string.IsNullOrWhiteSpace(c.FirstName) && string.IsNullOrWhiteSpace(c.LastName)
                               ? c.Email
                               : $"{c.FirstName} {c.LastName}"
                })
                .ToList();
        }

        private async Task LoadCardsAsync()
        {
            var list = await _api.GetAsync<List<SelectCardDto>>("api/Card");
            _cards = list ?? new();
            dgvCards.DataSource = _cards;

            if (_cards.Count > 0)
            {
                dgvCards.ClearSelection();
                dgvCards.Rows[0].Selected = true;
                FillForm(_cards[0]);
            }
            else
            {
                ClearForm();
            }
        }

        #endregion

        #region Helpers (form)

        // ---- Tema & Logo ----
        private sealed class BankTheme
        {
            public Color Bg { get; }
            public Color Fg { get; }
            public Image? Logo { get; }
            public BankTheme(Color bg, Color fg, Image? logo = null) { Bg = bg; Fg = fg; Logo = logo; }
        }

        private static string NormalizeBank(string s)
        {
            s = (s ?? "").Trim().ToLowerInvariant();
            s = s.Replace("ı", "i").Replace("ş", "s").Replace("ğ", "g")
                 .Replace("ü", "u").Replace("ö", "o").Replace("ç", "c");
            s = new string(s.Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            return s;
        }

        private void SelectOrAddBank(string? bankName)
        {
            bankName = (bankName ?? "").Trim();

            int foundIndex = -1;
            for (int i = 0; i < cboBank.Items.Count; i++)
            {
                var text = cboBank.GetItemText(cboBank.Items[i]);
                if (NormalizeBank(text) == NormalizeBank(bankName))
                {
                    foundIndex = i; break;
                }
            }

            if (foundIndex >= 0)
            {
                cboBank.SelectedIndex = foundIndex;
            }
            else
            {
                if (!string.IsNullOrEmpty(bankName))
                {
                    cboBank.Items.Add(bankName);
                    cboBank.SelectedItem = bankName;
                }
                else if (cboBank.Items.Count > 0)
                {
                    cboBank.SelectedIndex = 0;
                }
            }
        }

        private BankTheme GetBankTheme(string bank)
        {
            // normalize anahtarlarla eşle
            var map = new Dictionary<string, BankTheme>
            {
                ["akbank"] = new BankTheme(Color.FromArgb(0xE3, 0x06, 0x13), Color.White, Resource1.akbank),
                ["yapikredi"] = new BankTheme(Color.FromArgb(0x0A, 0x3B, 0x5C), Color.White, Resource1.yapikredi),
                ["garanti"] = new BankTheme(Color.FromArgb(0x0B, 0x7D, 0x3E), Color.White, Resource1.garanti),
                ["garanti"] = new BankTheme(Color.FromArgb(0x0B, 0x7D, 0x3E), Color.White, Resource1.garanti),
                ["ziraat"] = new BankTheme(Color.FromArgb(0xE3, 0x06, 0x13), Color.White, Resource1.ziraat),
                ["isbankasi"] = new BankTheme(Color.FromArgb(0x00, 0x3A, 0x70), Color.White, Resource1.isbankasi),
                ["qnbfinansbank"] = new BankTheme(Color.FromArgb(0x53, 0x2E, 0x6D), Color.White, Resource1.qnb),
                ["denizbank"] = new BankTheme(Color.FromArgb(0x00, 0x33, 0x8D), Color.White, Resource1.denizbank),
                ["halkbank"] = new BankTheme(Color.FromArgb(0x0C, 0x4D, 0xA2), Color.White, Resource1.halkbank),
                ["vakifbank"] = new BankTheme(Color.FromArgb(0xFD, 0xB9, 0x13), Color.Black, Resource1.vakifbank),
                ["teb"] = new BankTheme(Color.FromArgb(0x1B, 0xAA, 0x4B), Color.White, Resource1.teb),
                ["turkiyefinans"] = new BankTheme(ColorTranslator.FromHtml("#00A5A5"), Color.White, Resource1.turkiyefinans),
                ["ing"] = new BankTheme(Color.FromArgb(0xFF, 0x62, 0x00), Color.White, Resource1.ing),
            };

            var key = NormalizeBank(bank);
            if (map.TryGetValue(key, out var th)) return th;

            // nötr fallback (logo yok)
            return new BankTheme(Color.FromArgb(0x11, 0x2D, 0xB7), Color.White, null);
        }

        private SelectCardDto? GetSelectedRow()
        {
            if (dgvCards.CurrentRow?.DataBoundItem is SelectCardDto dto)
            {
                _selected = dto;
                return dto;
            }
            return null;
        }

        private void FillForm(SelectCardDto dto)
        {
            _suspendPreview = true;
            try
            {
                if (dto.CustomerId > 0)
                {
                    cboCustomer.SelectedValue = dto.CustomerId;
                }
                else
                {
                    var list = (IEnumerable<dynamic>)cboCustomer.DataSource;
                    var found = list.FirstOrDefault(x =>
                        string.Equals((string)x.FullName ?? "", dto.CustomerFullName ?? "", StringComparison.OrdinalIgnoreCase));
                    if (found != null) cboCustomer.SelectedValue = found.Id;
                    else cboCustomer.SelectedIndex = -1;
                }

                txtCardHolder.Text = dto.CustomerFullName ?? "";
                txtCardNumber.Text = dto.MaskedCardNumber ?? "";

                cboExpM.SelectedItem = dto.ExpiryMonth ?? DateTime.Now.Month.ToString("00");
                cboExpY.SelectedItem = dto.ExpiryYear ?? (DateTime.Now.Year % 100).ToString("00");

                // Banka: normalize ederek seç
                SelectOrAddBank(dto.CardIssuerBank);

                cboCurrency.SelectedValue = dto.Currency;
                cboProvider.SelectedValue = dto.CardProvider;
                cboType.SelectedValue = dto.CardType;
                cboStatus.SelectedValue = dto.CardStatus;

                numCardLimit.Value = SafeDecimal(dto.CardLimit);
                numDailyLimit.Value = SafeDecimal(dto.DailyLimit);
                numTxnLimit.Value = SafeDecimal(dto.TransactionLimit);

                chkVirtual.Checked = dto.IsVirtual;
                chkContactless.Checked = dto.IsContactless;
                chkBlocked.Checked = dto.IsBlocked;

                txtReason.Text = dto.CardStatusChangeReason ?? "";
            }
            finally
            {
                _suspendPreview = false;
            }

            UpdatePreview(); // tek sefer önizleme
        }

        private static decimal SafeDecimal(decimal d)
            => Math.Max(Math.Min(d, 100000000m), 0m);

        private void ClearForm()
        {
            _suspendPreview = true;
            try
            {
                cboCustomer.SelectedIndex = (cboCustomer.Items.Count > 0) ? 0 : -1;
                txtCardHolder.Clear();
                txtCardNumber.Clear();

                cboExpM.SelectedItem = DateTime.Now.Month.ToString("00");
                cboExpY.SelectedItem = (DateTime.Now.Year % 100).ToString("00");

                if (cboBank.Items.Count > 0) cboBank.SelectedIndex = 0;

                cboCurrency.SelectedValue = "TRY";
                cboProvider.SelectedValue = "V";
                cboType.SelectedValue = "D";
                cboStatus.SelectedValue = "A";

                numCardLimit.Value = 0;
                numDailyLimit.Value = 0;
                numTxnLimit.Value = 0;

                chkVirtual.Checked = false;
                chkContactless.Checked = false;
                chkBlocked.Checked = false;

                txtReason.Clear();
            }
            finally
            {
                _suspendPreview = false;
            }

            UpdatePreview();
        }

        private void EnterViewMode()
        {
            _mode = ViewMode.List;
            ToggleForm(false);
            miSave.Enabled = false;
            miCancel.Enabled = false;
            miInsert.Enabled = miUpdate.Enabled = miDelete.Enabled = true;
        }
        private void EnterCreateMode()
        {
            _mode = ViewMode.Create;
            ClearForm();
            ToggleForm(true);
            miSave.Enabled = miCancel.Enabled = true;
            miInsert.Enabled = miUpdate.Enabled = miDelete.Enabled = false;
        }
        private void EnterEditMode()
        {
            if (_selected == null) return;
            _mode = ViewMode.Edit;
            ToggleForm(true);
            miSave.Enabled = miCancel.Enabled = true;
            miInsert.Enabled = miUpdate.Enabled = miDelete.Enabled = false;
        }

        private void ToggleForm(bool enabled)
        {
            foreach (Control c in tlpForm.Controls) c.Enabled = enabled;
            pnlPreview.Enabled = true;

            // List/Edit modunda numara kutusunu kitle (sadece Create'de aktif)
            bool allowEditPan = _mode == ViewMode.Create;
            if (this.Controls.Find("txtCardNumber", true).FirstOrDefault() is TextBox t)
                t.ReadOnly = !allowEditPan;
        }

        private void UpdatePreview()
        {
            if (_suspendPreview) return;

            // 1) Kart numarası
            string raw = txtCardNumber.Text ?? "";
            lblCardNumber.Text = FormatCardNumberForPreview(raw);

            // 2) Holder
            string holder = (txtCardHolder.Text ?? "").Trim();
            lblCustomer.Text = string.IsNullOrWhiteSpace(holder) ? "AD SOYAD" : holder.ToUpperInvariant();

            // 3) Expiry
            lblExpMM.Text = (cboExpM.SelectedItem as string) ?? "MM";
            lblExpYY.Text = (cboExpY.SelectedItem as string) ?? "YY";

            // 4) Brand
            string code = (cboProvider.SelectedValue as string ?? "").ToUpperInvariant();
            var img = GetBrandImage(code);
            pbBrand.Image = img;
            pbBrand.Visible = img != null;

            // 5) Banka teması + logo
            ApplyBankTheme(cboBank.Text);
        }

        private static string FormatCardNumberForPreview(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "#### #### #### ####";

            var raw = input.Where(ch => ch != ' ' && ch != '-').ToArray();
            var mapped = raw.Select(ch => char.IsDigit(ch) ? ch : '•').ToArray();
            mapped = mapped.Take(19).ToArray();

            return string.Join(" ",
                Enumerable.Range(0, (mapped.Length + 3) / 4)
                          .Select(i => new string(mapped.Skip(i * 4).Take(4).ToArray())));
        }

        private Image? GetBrandImage(string code)
        {
            switch (code)
            {
                case "V": return Resource1.visa;
                case "M": return Resource1.mastercard;
                case "T": return Resource1.troy;
                default: return null;
            }
        }

        private static string FormatCardNumber(string digits)
        {
            if (string.IsNullOrEmpty(digits))
                return "#### #### #### ####";

            var groups = Enumerable.Range(0, (digits.Length + 3) / 4)
                                   .Select(i => digits.Skip(i * 4).Take(4))
                                   .Select(g => new string(g.ToArray()));
            return string.Join(" ", groups);
        }

        #endregion

        #region Menu handlers

        private async void miList_Click(object sender, EventArgs e)
        {
            try { await LoadCardsAsync(); EnterViewMode(); }
            catch (ApiException ex) { ShowApiError(ex, "Listeleme Hatası"); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Listeleme Hatası"); }
        }

        private void miInsert_Click(object sender, EventArgs e) => EnterCreateMode();

        private void miUpdate_Click(object sender, EventArgs e)
        {
            if (GetSelectedRow() == null) { MessageBox.Show("Seçim yok."); return; }
            EnterEditMode();
        }

        private async void miDelete_Click(object sender, EventArgs e)
        {
            var dto = GetSelectedRow();
            if (dto == null) { MessageBox.Show("Seçim yok."); return; }

            if (MessageBox.Show("Bu kartı pasifleştirmek (silmek) istiyor musun?", "Onay", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                await _api.DeleteAsync($"api/Card/{dto.Id}");
                await LoadCardsAsync();
                EnterViewMode();
            }
            catch (ApiException ex) { ShowApiError(ex, "Silme Hatası"); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Silme Hatası"); }
        }

        private async void miSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_mode == ViewMode.Create)
                {
                    if (!TryBuildCreate(out var create, out var err)) { MessageBox.Show(err); return; }
                    var created = await _api.PostAsync<CreateCardDto, SelectCardDto>("api/Card", create);
                }
                else if (_mode == ViewMode.Edit)
                {
                    if (_selected == null) { MessageBox.Show("Seçim yok."); return; }
                    if (!TryBuildUpdate(out var update, out var err)) { MessageBox.Show(err); return; }
                    await _api.PutAsync<UpdateCardDto, object?>($"api/Card/{_selected.Id}", update);
                }

                await LoadCardsAsync();
                EnterViewMode();
            }
            catch (ApiException ex) { ShowApiError(ex, "Kaydetme Hatası"); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Kaydetme Hatası"); }
        }

        private void miCancel_Click(object sender, EventArgs e)
        {
            EnterViewMode();
            if (_selected != null) FillForm(_selected);
        }

        #endregion

        #region DTO builders + validation

        private bool TryBuildCreate(out CreateCardDto dto, out string error)
        {
            dto = new CreateCardDto();
            error = "";

            if (cboCustomer.SelectedValue is not int customerId)
            {
                error = "Müşteri seçin.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCardHolder.Text))
            {
                error = "Kart sahibi gerekli.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCardNumber.Text))
            {
                error = "Kart numarası gerekli.";
                return false;
            }

            dto.CustomerId = customerId;
            dto.CardHolderName = txtCardHolder.Text.Trim();
            dto.CardNumber = txtCardNumber.Text.Replace(" ", "").Trim();
            dto.ExpiryMonth = (string)cboExpM.SelectedItem!;
            dto.ExpiryYear = (string)cboExpY.SelectedItem!;
            dto.CardIssuerBank = cboBank.Text.Trim();
            dto.Currency = (string)cboCurrency.SelectedValue!;
            dto.CardType = (string)cboType.SelectedValue!;
            dto.CardStatus = (string)cboStatus.SelectedValue!;
            dto.CardProvider = (string)cboProvider.SelectedValue!;
            dto.CardLimit = numCardLimit.Value;
            dto.DailyLimit = numDailyLimit.Value;
            dto.TransactionLimit = numTxnLimit.Value;
            dto.IsVirtual = chkVirtual.Checked;
            dto.IsContactless = chkContactless.Checked;
            dto.IsBlocked = chkBlocked.Checked;
            dto.CardStatusChangeReason = txtReason.Text.Trim();
            dto.ParentCardId = null;

            // PIN: txtPin varsa onu kullan, yoksa "0000"
            string pinPlain = this.Controls.Find("txtPin", true).FirstOrDefault() is TextBox txtPin && !string.IsNullOrWhiteSpace(txtPin.Text)
                                ? txtPin.Text.Trim()
                                : "0000";
            dto.PinHash = Sha256Hex(pinPlain);

            return true;
        }

        private bool TryBuildUpdate(out UpdateCardDto dto, out string error)
        {
            dto = new UpdateCardDto();
            error = "";

            // Yalnızca güncellenebilir alanlar
            dto.CardStatus = (string)cboStatus.SelectedValue!;
            dto.CardStatusDescription = txtReason.Text.Trim();
            dto.IsBlocked = chkBlocked.Checked;
            dto.IsContactless = chkContactless.Checked;
            dto.IsVirtual = chkVirtual.Checked;
            dto.CardLimit = numCardLimit.Value;
            dto.DailyLimit = numDailyLimit.Value;
            dto.TransactionLimit = numTxnLimit.Value;
            dto.CardProvider = (string)cboProvider.SelectedValue!;
            dto.CardIssuerBank = cboBank.Text.Trim();

            return true;
        }

        private static string Sha256Hex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private async Task LoadPanToPreviewAsync(int cardId, string fallbackMasked)
        {
            try
            {
                var panDto = await _api.GetAsync<CardPanDto>($"api/Card/{cardId}/pan");
                var pan = panDto?.CardNumber;

                txtCardNumber.Text = string.IsNullOrWhiteSpace(pan) ? fallbackMasked : pan;
            }
            catch
            {
                txtCardNumber.Text = fallbackMasked;
            }

            UpdatePreview();
        }

        private void ApplyBankTheme(string bankName)
        {
            var t = GetBankTheme(bankName);

            pnlPreview.BackColor = t.Bg;

            lblCardNumber.ForeColor = t.Fg;
            lblCustomer.ForeColor = t.Fg;
            lblExpMM.ForeColor = t.Fg;
            lblExpYY.ForeColor = t.Fg;

            if (this.Controls.Find("pbBank", true).FirstOrDefault() is PictureBox pbBank)
            {
                pbBank.SizeMode = PictureBoxSizeMode.Zoom;
                pbBank.BackColor = Color.Transparent;
                pbBank.Image = t.Logo;
                pbBank.Visible = t.Logo != null;
            }
        }

        private void ShowApiError(ApiException ex, string fallbackCaption = "Hata")
        {
            var msg = ExtractApiMessage(ex?.ResponseBody)
                      ?? (string.IsNullOrWhiteSpace(ex?.Message) ? "İşlem sırasında bir hata oluştu." : ex!.Message);
        
            var caption = BuildCaption(ex, fallbackCaption);
            MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        private static string? ExtractApiMessage(string? body)
        {
            if (string.IsNullOrWhiteSpace(body)) return null;
        
            // JSON gövde bekleniyor: { "message": "..."} veya { "detail": "..." }
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
        
                if (root.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String)
                    return m.GetString();
        
                if (root.TryGetProperty("detail", out var d) && d.ValueKind == JsonValueKind.String)
                    return d.GetString();
        
                // ASP.NET validation: { errors: { Field: ["msg1","msg2"] } }
                if (root.TryGetProperty("errors", out var errs) && errs.ValueKind == JsonValueKind.Object)
                {
                    var lines = new List<string>();
                    foreach (var p in errs.EnumerateObject())
                        foreach (var v in p.Value.EnumerateArray())
                            if (v.ValueKind == JsonValueKind.String) lines.Add(v.GetString()!);
        
                    if (lines.Count > 0) return string.Join(Environment.NewLine, lines);
                }
            }
            catch
            {
                // JSON değilse düşsün
            }
        
            // JSON değilse body düz yazı olabilir:
            var trimmed = body.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }
        
        private static string BuildCaption(ApiException ex, string fallback)
        {
            try
            {
                // ApiException içinde StatusCode varsa (senin ApiException.cs'inde var)
                var code = ex.StatusCode;
                if (code >= 500) return "Sunucu Hatası";
                if (code == 404) return "Bulunamadı";
                if (code == 409) return "Çakışma";
                if (code >= 400) return "İşlem Hatası";
            }
            catch { }
            return fallback;
        }

        private void UpdateFormTitle()
        {
            var f = this.FindForm();
            if (f != null)
                f.Text = "SmartBank Ödeme Sistemleri / Card";
        }
        #endregion
    }
}
