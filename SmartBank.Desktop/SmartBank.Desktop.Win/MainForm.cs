using SmartBank.Desktop.Win.Core;
using SmartBank.Desktop.Win.Views;

namespace SmartBank.Desktop.Win
{
    public partial class MainForm : Form
    {
        private readonly ApiClient _api;
        public MainForm(ApiClient api)
        {
            InitializeComponent();
            _api = api;
        }

        private Control? _currentView;

        private void OpenView(UserControl view)
        {
            panelContent.SuspendLayout();
            panelContent.Controls.Clear();
            view.Dock = DockStyle.Fill;
            panelContent.Controls.Add(view);
            panelContent.ResumeLayout();
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            OpenView(new CustomerView(_api));
        }

        private void btnCard_Click(object sender, EventArgs e)
        {
            OpenView(new CardView(_api));
        }

        private void btnTransaction_Click(object sender, EventArgs e)
        {
            OpenView(new TransactionView(_api));
        }

        private void btnReversal_Click(object sender, EventArgs e)
        {
            OpenView(new ReversalView(_api));
        }

        private void btnClearing_Click(object sender, EventArgs e)
        {
            OpenView(new ClearingView(_api));
        }
    }
}
