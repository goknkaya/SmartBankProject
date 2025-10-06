using System.ComponentModel;
using SmartBank.Desktop.Win.Core;
using SmartBank.Desktop.Win.Core.Contracts;

namespace SmartBank.Desktop.Win.Views
{
    public partial class CustomerView : UserControl
    {
        private readonly ApiClient _api;
        private readonly BindingList<SelectCustomerDto> _list = new();

        private enum ViewMode { View, Create, Edit }
        private ViewMode _mode = ViewMode.View;
        private SelectCustomerDto? _selected;

        public CustomerView(ApiClient api)
        {
            InitializeComponent();
            _api = api;

            // GRID
            grid.AutoGenerateColumns = false;      // manuel kolon -> auto kapalı
            grid.Columns.Clear();

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FirstName", HeaderText = "Ad" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LastName", HeaderText = "Soyad" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TCKN", HeaderText = "TCKN" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PhoneNumber", HeaderText = "Telefon" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BirthDate", HeaderText = "Doğum Tarihi" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Gender", HeaderText = "Cinsiyet" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "City", HeaderText = "Şehir" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Country", HeaderText = "Ülke" });
            grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsActive", HeaderText = "Aktif" });

            grid.DataSource = _list;
            grid.SelectionChanged += (_, __) => OnGridSelectionChanged();
            grid.CellFormatting += Grid_CellFormatting;

            // MENÜ
            miList.Click += async (_, __) => await LoadListAsync();
            miInsert.Click += (_, __) => EnterCreateMode();
            miUpdate.Click += (_, __) => EnterEditMode();
            miDelete.Click += async (_, __) => await DeleteAsync();
            miSave.Click += async (_, __) => await SaveAsync();
            miCancel.Click += (_, __) => EnterViewMode();

            // CİNSİYET
            cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGender.DisplayMember = "Text";
            cmbGender.ValueMember = "Value";
            cmbGender.DataSource = new[]
            {
                new { Text = "Male",   Value = "M" },
                new { Text = "Female", Value = "F" },
            };
            cmbGender.SelectedIndex = -1;

            this.Load += (_, __) => UpdateFormTitle();
            this.VisibleChanged += (_, __) => { if (Visible) UpdateFormTitle(); };
        }

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (grid.Columns[e.ColumnIndex].DataPropertyName == nameof(SelectCustomerDto.BirthDate) && e.Value is DateTime dt)
            {
                if (dt == default || dt.Year < 1900)
                {
                    e.Value = ""; e.FormattingApplied = true;
                }
                else
                {
                    e.Value = dt.ToString("dd.MM.yyyy"); e.FormattingApplied = true;
                }
            }

            if (grid.Columns[e.ColumnIndex].DataPropertyName == nameof(SelectCustomerDto.Gender) && e.Value is string g)
            {
                var t = g.ToUpperInvariant();
                e.Value = t == "M" ? "Male" : t == "F" ? "Female" : g;
                e.FormattingApplied = true;
            }
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode)
            {
                EnterViewMode();
                await LoadListAsync();
            }
        }

        // ---- Mode Helpers ----
        private void EnterViewMode()
        {
            _mode = ViewMode.View;
            SetFormEnabled(false);
            miSave.Visible = miCancel.Visible = false;
            miInsert.Enabled = true;
            miUpdate.Enabled = miDelete.Enabled = (_selected != null);
            miList.Enabled = true;
        }

        private void EnterCreateMode()
        {
            _mode = ViewMode.Create;
            ClearForm();
            SetFormEnabled(true);
            miSave.Visible = miCancel.Visible = true;
            miInsert.Enabled = miUpdate.Enabled = miDelete.Enabled = false;
            miList.Enabled = false;
        }

        private void EnterEditMode()
        {
            if (_selected == null) { MessageBox.Show("Bir satır seçin."); return; }
            _mode = ViewMode.Edit;
            FillForm(_selected);
            SetFormEnabled(true, isCreate: false);
            miSave.Visible = miCancel.Visible = true;
            miInsert.Enabled = miUpdate.Enabled = miDelete.Enabled = false;
            miList.Enabled = false;
        }

        private void SetFormEnabled(bool enabled, bool isCreate = true)
        {
            // Update edilebilir alanlar
            txtEmail.Enabled = enabled;
            txtPhone.Enabled = enabled;
            txtAddress.Enabled = enabled;
            txtCity.Enabled = enabled;
            txtCountry.Enabled = enabled;

            // Sadece Create’te doldurulsun
            txtFirstName.Enabled = enabled && isCreate;
            txtLastName.Enabled = enabled && isCreate;
            txtIdentityNumber.Enabled = enabled && isCreate;
            dteBirth.Enabled = enabled && isCreate;
            cmbGender.Enabled = enabled && isCreate;
        }

        private void ClearForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtIdentityNumber.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtCountry.Text = "";

            dteBirth.ShowCheckBox = true;
            dteBirth.Checked = false;

            cmbGender.SelectedIndex = -1; // boş
        }

        private void FillForm(SelectCustomerDto dto)
        {
            txtFirstName.Text = dto.FirstName ?? "";
            txtLastName.Text = dto.LastName ?? "";
            txtIdentityNumber.Text = dto.TCKN ?? "";
            txtEmail.Text = dto.Email;
            txtPhone.Text = dto.PhoneNumber;
            txtAddress.Text = dto.AddressLine ?? "";
            txtCity.Text = dto.City ?? "";
            txtCountry.Text = dto.Country ?? "";

            // Doğum tarihi
            dteBirth.ShowCheckBox = true;
            if (dto.BirthDate == default || dto.BirthDate.Year < 1900)
            {
                dteBirth.Checked = false;
            }
            else
            {
                dteBirth.Checked = true;
                var v = dto.BirthDate;
                if (v < dteBirth.MinDate) v = dteBirth.MinDate;
                if (v > dteBirth.MaxDate) v = dteBirth.MaxDate;
                dteBirth.Value = v;
            }

            // Cinsiyet (M/F)
            var g = (dto.Gender ?? "").ToUpperInvariant();
            if (g == "M" || g == "MALE") cmbGender.SelectedValue = "M";
            else if (g == "F" || g == "FEMALE") cmbGender.SelectedValue = "F";
            else cmbGender.SelectedIndex = -1;

            if (Controls.Find("chkIsActive", true).FirstOrDefault() is CheckBox chk)
                chk.Checked = dto.IsActive;
        }

        private void OnGridSelectionChanged()
        {
            _selected = grid.CurrentRow?.DataBoundItem as SelectCustomerDto;
            if (_mode == ViewMode.View)
            {
                miUpdate.Enabled = miDelete.Enabled = (_selected != null);
                if (_selected != null) FillForm(_selected);
            }
        }

        // ---- API Ops ----
        private async Task LoadListAsync()
        {
            try
            {
                grid.SuspendLayout();
                grid.CurrentCell = null;
                grid.ClearSelection();

                var data = await _api.GetAsync<List<SelectCustomerDto>>("/api/Customer");

                _list.Clear();
                if (data != null)
                    foreach (var item in data) _list.Add(item);

                if (grid.Columns["Id"] != null) grid.Columns["Id"].Visible = false;
                if (grid.Columns["AddressLine"] != null) grid.Columns["AddressLine"].Visible = false;

                if (grid.Rows.Count > 0)
                {
                    int firstVisibleCol = grid.Columns
                        .Cast<DataGridViewColumn>()
                        .Where(c => c.Visible)
                        .OrderBy(c => c.DisplayIndex)
                        .Select(c => c.Index)
                        .FirstOrDefault();

                    if (firstVisibleCol < 0) firstVisibleCol = 0;

                    grid.CurrentCell = grid.Rows[0].Cells[firstVisibleCol];
                    grid.Rows[0].Selected = true;

                    OnGridSelectionChanged();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Liste yüklenemedi: {ex.Message}");
            }
            finally
            {
                grid.ResumeLayout();
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                if (_mode == ViewMode.Create)
                {
                    if (!ValidateCreate(out var create)) return;
                    await _api.PostAsync<CreateCustomerDto, SelectCustomerDto>("/api/Customer", create);
                }
                else if (_mode == ViewMode.Edit)
                {
                    if (_selected == null) { MessageBox.Show("Seçim yok."); return; }
                    var update = BuildUpdateDto(_selected.Id);

                    // Controller 204 döndüğü için body bekleme:
                    await _api.PutAsync<UpdateCustomerDto, object>($"/api/Customer/{_selected.Id}", update);
                }

                await LoadListAsync();
                EnterViewMode();
            }
            catch (ApiException ex)
            {
                try
                {
                    // JSON ValidationProblemDetails ise göster
                    var pd = System.Text.Json.JsonSerializer.Deserialize<ValidationProblem>(ex.ResponseBody);

                    if (pd?.errors != null && pd.errors.Count > 0)
                    {
                        var lines = new List<string>();
                        foreach (var kv in pd.errors)
                            foreach (var msg in kv.Value ?? Array.Empty<string>())
                                lines.Add(msg);

                        MessageBox.Show(string.Join(Environment.NewLine, lines), "Doğrulama Hatası",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // metin/plain mesaj veya başlık
                        var title = pd?.title ?? ex.ResponseBody;
                        MessageBox.Show(title, $"Hata {(pd?.status ?? ex.StatusCode)}",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch
                {
                    // JSON değilse ham gövdeyi yaz
                    MessageBox.Show(ex.ResponseBody, $"Hata {ex.StatusCode}",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme başarısız: {ex.Message}");
            }
        }

        private async Task DeleteAsync()
        {
            if (_selected == null) { MessageBox.Show("Seçim yok."); return; }
            if (MessageBox.Show("Silinsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                await _api.DeleteAsync($"/api/Customer/{_selected.Id}");
                await LoadListAsync();
                EnterViewMode();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Silme başarısız: {ex.Message}");
            }
        }

        // ---- DTO Helpers ----
        private bool ValidateCreate(out CreateCustomerDto dto)
        {
            dto = new CreateCustomerDto
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                TCKN = txtIdentityNumber.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PhoneNumber = txtPhone.Text.Trim(),
                BirthDate = dteBirth.Checked ? dteBirth.Value.Date : default,
                Gender = (cmbGender.SelectedValue as string) ?? null,
                AddressLine = txtAddress.Text.TrimOrNull(),
                City = txtCity.Text.TrimOrNull(),
                Country = txtCountry.Text.TrimOrNull()
            };

            if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName) ||
                string.IsNullOrWhiteSpace(dto.TCKN))
            {
                MessageBox.Show("Ad, Soyad ve TCKN zorunlu.");
                return false;
            }
            return true;
        }

        private UpdateCustomerDto BuildUpdateDto(int id) => new()
        {
            Id = id,
            Email = txtEmail.Text.TrimOrNull(),
            PhoneNumber = txtPhone.Text.TrimOrNull(),
            AddressLine = txtAddress.Text.TrimOrNull(),
            City = txtCity.Text.TrimOrNull(),
            Country = txtCountry.Text.TrimOrNull()
        };

        private void UpdateFormTitle()
        {
            var f = this.FindForm();
            if (f != null)
                f.Text = "SmartBank Ödeme Sistemleri / Customer";
        }
    }

    internal static class StrX
    {
        public static string? TrimOrNull(this string s)
        {
            var t = s?.Trim();
            return string.IsNullOrEmpty(t) ? null : t;
        }
    }

    // UI tarafında kullanmak için küçük model
    public class ValidationProblem
    {
        public string? type { get; set; }
        public string? title { get; set; }
        public int? status { get; set; }
        public string? traceId { get; set; }
        public Dictionary<string, string[]>? errors { get; set; }
    }
}
