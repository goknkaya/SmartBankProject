using SmartBank.Desktop.Win.Core;
using System;
using System.Windows.Forms;

namespace SmartBank.Desktop.Win
{
    public partial class LoginForm : Form
    {
        private readonly ApiClient _api;

        public LoginForm(ApiClient api)
        {
            InitializeComponent();
            _api = api;

            txtUsername.Text = "";
            txtPassword.Text = "";
            lblError.Visible = false;

            chkShow.CheckedChanged += (_, __) =>
                txtPassword.UseSystemPasswordChar = !chkShow.Checked;

            // Enter ile login
            this.AcceptButton = btnLogin;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            HideError();
            ToggleUi(false);

            try
            {
                var u = txtUsername.Text?.Trim() ?? "";
                var p = txtPassword.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p))
                {
                    ShowError("Kullanıcı adı ve şifre gerekli.");
                    return;
                }

                var ok = await _api.LoginAsync(u, p);
                if (ok)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (ApiException ex) // bizim özel exception
            {
                if (ex.StatusCode == 401)
                {
                    ShowError("Kullanıcı adı veya şifre hatalı.");
                }
                else
                {
                    // API'den gelen gövde varsa göster; yoksa genel mesaj
                    var msg = string.IsNullOrWhiteSpace(ex.ResponseBody)
                              ? ex.Message
                              : ex.ResponseBody;
                    ShowError(msg);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                ToggleUi(true);
            }
        }

        private void ToggleUi(bool enabled)
        {
            UseWaitCursor = !enabled;
            btnLogin.Enabled = enabled;
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            chkShow.Enabled = enabled;
        }

        private void ShowError(string msg)
        {
            lblError.Text = msg;
            lblError.Visible = true;
        }

        private void HideError()
        {
            lblError.Text = "";
            lblError.Visible = false;
        }
    }
}
