namespace SmartBank.Desktop.Win.Views
{
    partial class CardView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            menuStrip1 = new MenuStrip();
            miList = new ToolStripMenuItem();
            miInsert = new ToolStripMenuItem();
            miUpdate = new ToolStripMenuItem();
            miDelete = new ToolStripMenuItem();
            miSave = new ToolStripMenuItem();
            miCancel = new ToolStripMenuItem();
            splitMain = new SplitContainer();
            dgvCards = new DataGridView();
            colCustomer = new DataGridViewTextBoxColumn();
            colCard = new DataGridViewTextBoxColumn();
            colExpM = new DataGridViewTextBoxColumn();
            colExpY = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colProvider = new DataGridViewTextBoxColumn();
            colCardLimit = new DataGridViewTextBoxColumn();
            colDaily = new DataGridViewTextBoxColumn();
            colTxn = new DataGridViewTextBoxColumn();
            colVirtual = new DataGridViewCheckBoxColumn();
            colContactless = new DataGridViewCheckBoxColumn();
            colBlocked = new DataGridViewCheckBoxColumn();
            pnlRight = new Panel();
            tlpForm = new TableLayoutPanel();
            cboBank = new ComboBox();
            txtReason = new TextBox();
            cboStatus = new ComboBox();
            cboType = new ComboBox();
            cboProvider = new ComboBox();
            cboCurrency = new ComboBox();
            txtCardNumber = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            pnlPreview = new Panel();
            pbBank = new PictureBox();
            pbBrand = new PictureBox();
            label21 = new Label();
            lblExpYY = new Label();
            lblExpMM = new Label();
            lblCustomer = new Label();
            lblCardNumber = new Label();
            label13 = new Label();
            cboCustomer = new ComboBox();
            txtCardHolder = new TextBox();
            panel1 = new Panel();
            label16 = new Label();
            label15 = new Label();
            label14 = new Label();
            numTxnLimit = new NumericUpDown();
            numDailyLimit = new NumericUpDown();
            numCardLimit = new NumericUpDown();
            pnlFlags = new Panel();
            chkBlocked = new CheckBox();
            chkContactless = new CheckBox();
            chkVirtual = new CheckBox();
            panel2 = new Panel();
            cboExpM = new ComboBox();
            cboExpY = new ComboBox();
            label1 = new Label();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCards).BeginInit();
            pnlRight.SuspendLayout();
            tlpForm.SuspendLayout();
            pnlPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbBank).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbBrand).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numTxnLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDailyLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCardLimit).BeginInit();
            pnlFlags.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { miList, miInsert, miUpdate, miDelete, miSave, miCancel });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1284, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // miList
            // 
            miList.Name = "miList";
            miList.Size = new Size(52, 20);
            miList.Text = "Listele";
            // 
            // miInsert
            // 
            miInsert.Name = "miInsert";
            miInsert.Size = new Size(40, 20);
            miInsert.Text = "Ekle";
            // 
            // miUpdate
            // 
            miUpdate.Name = "miUpdate";
            miUpdate.Size = new Size(65, 20);
            miUpdate.Text = "Güncelle";
            // 
            // miDelete
            // 
            miDelete.Name = "miDelete";
            miDelete.Size = new Size(31, 20);
            miDelete.Text = "Sil";
            // 
            // miSave
            // 
            miSave.Name = "miSave";
            miSave.Size = new Size(55, 20);
            miSave.Text = "Kaydet";
            // 
            // miCancel
            // 
            miCancel.Name = "miCancel";
            miCancel.Size = new Size(55, 20);
            miCancel.Text = "Vazgeç";
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 24);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(dgvCards);
            splitMain.Panel1MinSize = 300;
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(pnlRight);
            splitMain.Panel2MinSize = 300;
            splitMain.Size = new Size(1284, 737);
            splitMain.SplitterDistance = 600;
            splitMain.SplitterWidth = 5;
            splitMain.TabIndex = 3;
            // 
            // dgvCards
            // 
            dgvCards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCards.Columns.AddRange(new DataGridViewColumn[] { colCustomer, colCard, colExpM, colExpY, colStatus, colProvider, colCardLimit, colDaily, colTxn, colVirtual, colContactless, colBlocked });
            dgvCards.Dock = DockStyle.Fill;
            dgvCards.Location = new Point(0, 0);
            dgvCards.MultiSelect = false;
            dgvCards.Name = "dgvCards";
            dgvCards.ReadOnly = true;
            dgvCards.Size = new Size(600, 737);
            dgvCards.TabIndex = 0;
            // 
            // colCustomer
            // 
            colCustomer.HeaderText = "Müşteri";
            colCustomer.Name = "colCustomer";
            colCustomer.ReadOnly = true;
            // 
            // colCard
            // 
            colCard.HeaderText = "Kart";
            colCard.Name = "colCard";
            colCard.ReadOnly = true;
            // 
            // colExpM
            // 
            colExpM.HeaderText = "Son Kull. Ay";
            colExpM.Name = "colExpM";
            colExpM.ReadOnly = true;
            // 
            // colExpY
            // 
            colExpY.HeaderText = "Yıl";
            colExpY.Name = "colExpY";
            colExpY.ReadOnly = true;
            // 
            // colStatus
            // 
            colStatus.HeaderText = "Durum";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // colProvider
            // 
            colProvider.HeaderText = "Sağlayıcı";
            colProvider.Name = "colProvider";
            colProvider.ReadOnly = true;
            // 
            // colCardLimit
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "N2";
            dataGridViewCellStyle1.NullValue = null;
            colCardLimit.DefaultCellStyle = dataGridViewCellStyle1;
            colCardLimit.HeaderText = "Limit";
            colCardLimit.Name = "colCardLimit";
            colCardLimit.ReadOnly = true;
            // 
            // colDaily
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N2";
            dataGridViewCellStyle2.NullValue = null;
            colDaily.DefaultCellStyle = dataGridViewCellStyle2;
            colDaily.HeaderText = "Günlük";
            colDaily.Name = "colDaily";
            colDaily.ReadOnly = true;
            // 
            // colTxn
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.NullValue = null;
            colTxn.DefaultCellStyle = dataGridViewCellStyle3;
            colTxn.HeaderText = "İşlem";
            colTxn.Name = "colTxn";
            colTxn.ReadOnly = true;
            // 
            // colVirtual
            // 
            colVirtual.HeaderText = "Sanal";
            colVirtual.Name = "colVirtual";
            colVirtual.ReadOnly = true;
            // 
            // colContactless
            // 
            colContactless.HeaderText = "Temassız";
            colContactless.Name = "colContactless";
            colContactless.ReadOnly = true;
            // 
            // colBlocked
            // 
            colBlocked.HeaderText = "Blokeli";
            colBlocked.Name = "colBlocked";
            colBlocked.ReadOnly = true;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(tlpForm);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(0, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(8);
            pnlRight.Size = new Size(679, 737);
            pnlRight.TabIndex = 0;
            // 
            // tlpForm
            // 
            tlpForm.ColumnCount = 2;
            tlpForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tlpForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpForm.Controls.Add(cboBank, 1, 10);
            tlpForm.Controls.Add(txtReason, 1, 11);
            tlpForm.Controls.Add(cboStatus, 1, 7);
            tlpForm.Controls.Add(cboType, 1, 6);
            tlpForm.Controls.Add(cboProvider, 1, 5);
            tlpForm.Controls.Add(cboCurrency, 1, 4);
            tlpForm.Controls.Add(txtCardNumber, 1, 2);
            tlpForm.Controls.Add(label2, 0, 1);
            tlpForm.Controls.Add(label3, 0, 2);
            tlpForm.Controls.Add(label4, 0, 3);
            tlpForm.Controls.Add(label5, 0, 4);
            tlpForm.Controls.Add(label6, 0, 5);
            tlpForm.Controls.Add(label7, 0, 6);
            tlpForm.Controls.Add(label8, 0, 7);
            tlpForm.Controls.Add(label9, 0, 8);
            tlpForm.Controls.Add(label10, 0, 9);
            tlpForm.Controls.Add(label11, 0, 10);
            tlpForm.Controls.Add(label12, 0, 11);
            tlpForm.Controls.Add(pnlPreview, 1, 12);
            tlpForm.Controls.Add(label13, 0, 12);
            tlpForm.Controls.Add(cboCustomer, 1, 0);
            tlpForm.Controls.Add(txtCardHolder, 1, 1);
            tlpForm.Controls.Add(panel1, 1, 8);
            tlpForm.Controls.Add(pnlFlags, 1, 9);
            tlpForm.Controls.Add(panel2, 1, 3);
            tlpForm.Controls.Add(label1, 0, 0);
            tlpForm.Dock = DockStyle.Fill;
            tlpForm.Location = new Point(8, 8);
            tlpForm.Name = "tlpForm";
            tlpForm.RowCount = 13;
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpForm.Size = new Size(663, 721);
            tlpForm.TabIndex = 0;
            // 
            // cboBank
            // 
            cboBank.DisplayMember = "CustomerFullName";
            cboBank.Dock = DockStyle.Fill;
            cboBank.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBank.FormattingEnabled = true;
            cboBank.Items.AddRange(new object[] { "Akbank", "Yapı Kredi", "Garanti", "Ziraat", "İşbankası", "QNB Finansbank", "Denizbank", "Halkbank", "Vakıfbank", "TEB", "Türkiye Finans", "ING" });
            cboBank.Location = new Point(123, 403);
            cboBank.Name = "cboBank";
            cboBank.Size = new Size(537, 23);
            cboBank.TabIndex = 27;
            cboBank.ValueMember = "Id";
            // 
            // txtReason
            // 
            txtReason.Dock = DockStyle.Fill;
            txtReason.Location = new Point(123, 443);
            txtReason.MaxLength = 250;
            txtReason.Multiline = true;
            txtReason.Name = "txtReason";
            txtReason.ScrollBars = ScrollBars.Vertical;
            txtReason.Size = new Size(537, 54);
            txtReason.TabIndex = 25;
            // 
            // cboStatus
            // 
            cboStatus.DisplayMember = "CustomerFullName";
            cboStatus.Dock = DockStyle.Fill;
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.FormattingEnabled = true;
            cboStatus.Location = new Point(123, 283);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(537, 23);
            cboStatus.TabIndex = 21;
            cboStatus.ValueMember = "Id";
            // 
            // cboType
            // 
            cboType.DisplayMember = "CustomerFullName";
            cboType.Dock = DockStyle.Fill;
            cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboType.FormattingEnabled = true;
            cboType.Location = new Point(123, 243);
            cboType.Name = "cboType";
            cboType.Size = new Size(537, 23);
            cboType.TabIndex = 20;
            cboType.ValueMember = "Id";
            // 
            // cboProvider
            // 
            cboProvider.DisplayMember = "CustomerFullName";
            cboProvider.Dock = DockStyle.Fill;
            cboProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProvider.FormattingEnabled = true;
            cboProvider.Location = new Point(123, 203);
            cboProvider.Name = "cboProvider";
            cboProvider.Size = new Size(537, 23);
            cboProvider.TabIndex = 19;
            cboProvider.ValueMember = "Id";
            // 
            // cboCurrency
            // 
            cboCurrency.DisplayMember = "CustomerFullName";
            cboCurrency.Dock = DockStyle.Fill;
            cboCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCurrency.FormattingEnabled = true;
            cboCurrency.Location = new Point(123, 163);
            cboCurrency.Name = "cboCurrency";
            cboCurrency.Size = new Size(537, 23);
            cboCurrency.TabIndex = 18;
            cboCurrency.ValueMember = "Id";
            // 
            // txtCardNumber
            // 
            txtCardNumber.Dock = DockStyle.Fill;
            txtCardNumber.Location = new Point(123, 83);
            txtCardNumber.MaxLength = 16;
            txtCardNumber.Name = "txtCardNumber";
            txtCardNumber.Size = new Size(537, 23);
            txtCardNumber.TabIndex = 16;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(54, 52);
            label2.Name = "label2";
            label2.Size = new Size(63, 15);
            label2.TabIndex = 2;
            label2.Text = "Kart Sahibi";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(70, 92);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 3;
            label3.Text = "Kart No";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(40, 132);
            label4.Name = "label4";
            label4.Size = new Size(77, 15);
            label4.TabIndex = 4;
            label4.Text = "Son Kullanım";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(53, 172);
            label5.Name = "label5";
            label5.Size = new Size(64, 15);
            label5.TabIndex = 5;
            label5.Text = "Para Birimi";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(66, 212);
            label6.Name = "label6";
            label6.Size = new Size(51, 15);
            label6.TabIndex = 6;
            label6.Text = "Provider";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(66, 252);
            label7.Name = "label7";
            label7.Size = new Size(51, 15);
            label7.TabIndex = 7;
            label7.Text = "Kart Tipi";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(73, 292);
            label8.Name = "label8";
            label8.Size = new Size(44, 15);
            label8.TabIndex = 8;
            label8.Text = "Durum";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new Point(70, 332);
            label9.Name = "label9";
            label9.Size = new Size(47, 15);
            label9.TabIndex = 9;
            label9.Text = "Limitler";
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Location = new Point(75, 372);
            label10.Name = "label10";
            label10.Size = new Size(42, 15);
            label10.TabIndex = 10;
            label10.Text = "Flaglar";
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Location = new Point(78, 412);
            label11.Name = "label11";
            label11.Size = new Size(39, 15);
            label11.TabIndex = 11;
            label11.Text = "Banka";
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Location = new Point(61, 462);
            label12.Name = "label12";
            label12.Size = new Size(56, 15);
            label12.TabIndex = 12;
            label12.Text = "Açıklama";
            // 
            // pnlPreview
            // 
            pnlPreview.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pnlPreview.BackColor = Color.DarkBlue;
            pnlPreview.BorderStyle = BorderStyle.FixedSingle;
            pnlPreview.Controls.Add(pbBank);
            pnlPreview.Controls.Add(pbBrand);
            pnlPreview.Controls.Add(label21);
            pnlPreview.Controls.Add(lblExpYY);
            pnlPreview.Controls.Add(lblExpMM);
            pnlPreview.Controls.Add(lblCustomer);
            pnlPreview.Controls.Add(lblCardNumber);
            pnlPreview.Location = new Point(126, 503);
            pnlPreview.Name = "pnlPreview";
            pnlPreview.Size = new Size(534, 215);
            pnlPreview.TabIndex = 0;
            // 
            // pbBank
            // 
            pbBank.Location = new Point(310, 12);
            pbBank.Name = "pbBank";
            pbBank.Size = new Size(208, 89);
            pbBank.SizeMode = PictureBoxSizeMode.StretchImage;
            pbBank.TabIndex = 10;
            pbBank.TabStop = false;
            // 
            // pbBrand
            // 
            pbBrand.Location = new Point(310, 110);
            pbBrand.Name = "pbBrand";
            pbBrand.Size = new Size(208, 89);
            pbBrand.SizeMode = PictureBoxSizeMode.StretchImage;
            pbBrand.TabIndex = 9;
            pbBrand.TabStop = false;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label21.ForeColor = Color.White;
            label21.Location = new Point(75, 156);
            label21.Name = "label21";
            label21.Size = new Size(25, 32);
            label21.TabIndex = 8;
            label21.Text = "/";
            // 
            // lblExpYY
            // 
            lblExpYY.AutoSize = true;
            lblExpYY.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblExpYY.ForeColor = Color.White;
            lblExpYY.Location = new Point(106, 156);
            lblExpYY.Name = "lblExpYY";
            lblExpYY.Size = new Size(44, 32);
            lblExpYY.TabIndex = 7;
            lblExpYY.Text = "YY";
            // 
            // lblExpMM
            // 
            lblExpMM.AutoSize = true;
            lblExpMM.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblExpMM.ForeColor = Color.White;
            lblExpMM.Location = new Point(21, 156);
            lblExpMM.Name = "lblExpMM";
            lblExpMM.Size = new Size(48, 32);
            lblExpMM.TabIndex = 6;
            lblExpMM.Text = "AA";
            // 
            // lblCustomer
            // 
            lblCustomer.AutoSize = true;
            lblCustomer.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblCustomer.ForeColor = Color.White;
            lblCustomer.Location = new Point(21, 55);
            lblCustomer.Name = "lblCustomer";
            lblCustomer.Size = new Size(135, 32);
            lblCustomer.TabIndex = 5;
            lblCustomer.Text = "AD SOYAD";
            // 
            // lblCardNumber
            // 
            lblCardNumber.AutoSize = true;
            lblCardNumber.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblCardNumber.ForeColor = Color.White;
            lblCardNumber.Location = new Point(21, 12);
            lblCardNumber.Name = "lblCardNumber";
            lblCardNumber.Size = new Size(259, 32);
            lblCardNumber.TabIndex = 4;
            lblCardNumber.Text = "#### #### #### ####";
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Right;
            label13.AutoSize = true;
            label13.Location = new Point(60, 603);
            label13.Name = "label13";
            label13.Size = new Size(57, 15);
            label13.TabIndex = 13;
            label13.Text = "Önizleme";
            // 
            // cboCustomer
            // 
            cboCustomer.DisplayMember = "CustomerFullName";
            cboCustomer.Dock = DockStyle.Fill;
            cboCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCustomer.FormattingEnabled = true;
            cboCustomer.Location = new Point(123, 3);
            cboCustomer.Name = "cboCustomer";
            cboCustomer.Size = new Size(537, 23);
            cboCustomer.TabIndex = 14;
            cboCustomer.ValueMember = "Id";
            // 
            // txtCardHolder
            // 
            txtCardHolder.Dock = DockStyle.Fill;
            txtCardHolder.Location = new Point(123, 43);
            txtCardHolder.MaxLength = 50;
            txtCardHolder.Name = "txtCardHolder";
            txtCardHolder.Size = new Size(537, 23);
            txtCardHolder.TabIndex = 15;
            // 
            // panel1
            // 
            panel1.Controls.Add(label16);
            panel1.Controls.Add(label15);
            panel1.Controls.Add(label14);
            panel1.Controls.Add(numTxnLimit);
            panel1.Controls.Add(numDailyLimit);
            panel1.Controls.Add(numCardLimit);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(123, 323);
            panel1.Name = "panel1";
            panel1.Size = new Size(537, 34);
            panel1.TabIndex = 22;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(386, 9);
            label16.Name = "label16";
            label16.Size = new Size(35, 15);
            label16.TabIndex = 5;
            label16.Text = "İşlem";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(189, 9);
            label15.Name = "label15";
            label15.Size = new Size(45, 15);
            label15.TabIndex = 4;
            label15.Text = "Günlük";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(6, 9);
            label14.Name = "label14";
            label14.Size = new Size(28, 15);
            label14.TabIndex = 3;
            label14.Text = "Kart";
            // 
            // numTxnLimit
            // 
            numTxnLimit.DecimalPlaces = 2;
            numTxnLimit.Location = new Point(425, 7);
            numTxnLimit.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numTxnLimit.Name = "numTxnLimit";
            numTxnLimit.Size = new Size(109, 23);
            numTxnLimit.TabIndex = 2;
            numTxnLimit.ThousandsSeparator = true;
            // 
            // numDailyLimit
            // 
            numDailyLimit.DecimalPlaces = 2;
            numDailyLimit.Location = new Point(240, 7);
            numDailyLimit.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numDailyLimit.Name = "numDailyLimit";
            numDailyLimit.Size = new Size(109, 23);
            numDailyLimit.TabIndex = 1;
            numDailyLimit.ThousandsSeparator = true;
            // 
            // numCardLimit
            // 
            numCardLimit.DecimalPlaces = 2;
            numCardLimit.Location = new Point(40, 7);
            numCardLimit.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numCardLimit.Name = "numCardLimit";
            numCardLimit.Size = new Size(109, 23);
            numCardLimit.TabIndex = 0;
            numCardLimit.ThousandsSeparator = true;
            // 
            // pnlFlags
            // 
            pnlFlags.Controls.Add(chkBlocked);
            pnlFlags.Controls.Add(chkContactless);
            pnlFlags.Controls.Add(chkVirtual);
            pnlFlags.Dock = DockStyle.Fill;
            pnlFlags.Location = new Point(123, 363);
            pnlFlags.Name = "pnlFlags";
            pnlFlags.Size = new Size(537, 34);
            pnlFlags.TabIndex = 23;
            // 
            // chkBlocked
            // 
            chkBlocked.AutoSize = true;
            chkBlocked.Location = new Point(147, 8);
            chkBlocked.Name = "chkBlocked";
            chkBlocked.Size = new Size(61, 19);
            chkBlocked.TabIndex = 2;
            chkBlocked.Text = "Blokeli";
            chkBlocked.UseVisualStyleBackColor = true;
            // 
            // chkContactless
            // 
            chkContactless.AutoSize = true;
            chkContactless.Location = new Point(68, 8);
            chkContactless.Name = "chkContactless";
            chkContactless.Size = new Size(73, 19);
            chkContactless.TabIndex = 1;
            chkContactless.Text = "Temassız";
            chkContactless.UseVisualStyleBackColor = true;
            // 
            // chkVirtual
            // 
            chkVirtual.AutoSize = true;
            chkVirtual.Location = new Point(6, 9);
            chkVirtual.Name = "chkVirtual";
            chkVirtual.Size = new Size(54, 19);
            chkVirtual.TabIndex = 0;
            chkVirtual.Text = "Sanal";
            chkVirtual.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(cboExpM);
            panel2.Controls.Add(cboExpY);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(123, 123);
            panel2.Name = "panel2";
            panel2.Size = new Size(537, 34);
            panel2.TabIndex = 26;
            // 
            // cboExpM
            // 
            cboExpM.Anchor = AnchorStyles.Left;
            cboExpM.DisplayMember = "CustomerFullName";
            cboExpM.DropDownStyle = ComboBoxStyle.DropDownList;
            cboExpM.FormattingEnabled = true;
            cboExpM.Location = new Point(3, 3);
            cboExpM.Name = "cboExpM";
            cboExpM.Size = new Size(70, 23);
            cboExpM.TabIndex = 19;
            cboExpM.ValueMember = "Id";
            // 
            // cboExpY
            // 
            cboExpY.Anchor = AnchorStyles.Left;
            cboExpY.DisplayMember = "CustomerFullName";
            cboExpY.DropDownStyle = ComboBoxStyle.DropDownList;
            cboExpY.FormattingEnabled = true;
            cboExpY.Location = new Point(79, 3);
            cboExpY.Name = "cboExpY";
            cboExpY.Size = new Size(70, 23);
            cboExpY.TabIndex = 18;
            cboExpY.ValueMember = "Id";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(70, 12);
            label1.Name = "label1";
            label1.Size = new Size(47, 15);
            label1.TabIndex = 1;
            label1.Text = "Müşteri";
            // 
            // CardView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitMain);
            Controls.Add(menuStrip1);
            Name = "CardView";
            Size = new Size(1284, 761);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCards).EndInit();
            pnlRight.ResumeLayout(false);
            tlpForm.ResumeLayout(false);
            tlpForm.PerformLayout();
            pnlPreview.ResumeLayout(false);
            pnlPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbBank).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbBrand).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numTxnLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDailyLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCardLimit).EndInit();
            pnlFlags.ResumeLayout(false);
            pnlFlags.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem miList;
        private ToolStripMenuItem miInsert;
        private ToolStripMenuItem miUpdate;
        private ToolStripMenuItem miDelete;
        private ToolStripMenuItem miSave;
        private ToolStripMenuItem miCancel;
        private SplitContainer splitMain;
        private DataGridView dgvCards;
        private Panel pnlRight;
        private TableLayoutPanel tlpForm;
        private Panel pnlPreview;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private DataGridViewTextBoxColumn colCustomer;
        private DataGridViewTextBoxColumn colCard;
        private DataGridViewTextBoxColumn colExpM;
        private DataGridViewTextBoxColumn colExpY;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colProvider;
        private DataGridViewTextBoxColumn colCardLimit;
        private DataGridViewTextBoxColumn colDaily;
        private DataGridViewTextBoxColumn colTxn;
        private DataGridViewCheckBoxColumn colVirtual;
        private DataGridViewCheckBoxColumn colContactless;
        private DataGridViewCheckBoxColumn colBlocked;
        private TextBox txtCardNumber;
        private ComboBox cboCustomer;
        private TextBox txtCardHolder;
        private ComboBox cboStatus;
        private ComboBox cboType;
        private ComboBox cboProvider;
        private ComboBox cboCurrency;
        private Panel panel1;
        private NumericUpDown numTxnLimit;
        private NumericUpDown numDailyLimit;
        private NumericUpDown numCardLimit;
        private Panel pnlFlags;
        private CheckBox chkBlocked;
        private CheckBox chkContactless;
        private CheckBox chkVirtual;
        private TextBox txtReason;
        private Panel panel2;
        private ComboBox cboExpM;
        private ComboBox cboExpY;
        private Label label16;
        private Label label15;
        private Label label14;
        private Label lblExpYY;
        private Label lblExpMM;
        private Label lblCustomer;
        private Label lblCardNumber;
        private Label label21;
        private PictureBox pbBrand;
        private ComboBox cboBank;
        private PictureBox pbBank;
    }
}
