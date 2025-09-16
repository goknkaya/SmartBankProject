using System;
using System.Windows.Forms;
using SmartBank.Desktop.Win.Core;

namespace SmartBank.Desktop.Win
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Tek instance ApiClient
            var api = new ApiClient();

            // 1. Login penceresi
            using var login = new LoginForm(api);
            var result = login.ShowDialog();

            // 2. Baþarýlý giriþ -> MainForm
            if (result == DialogResult.OK)
            {
                Application.Run(new MainForm(api));
            }

            // aksi halde (cancel/close) uygulama kapanýr.
        }
    }
}