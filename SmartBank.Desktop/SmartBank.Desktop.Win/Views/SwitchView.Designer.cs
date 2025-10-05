namespace SmartBank.Desktop.Win.Views
{
    partial class SwitchView
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
            txtPAN = new TextBox();
            txtMerchant = new TextBox();
            txtAcquirer = new TextBox();
            nudAmount = new NumericUpDown();
            cboCurrency = new ComboBox();
            dtpTxnTime = new DateTimePicker();
            dgvMessages = new DataGridView();
            Id = new DataGridViewTextBoxColumn();
            PANMasked = new DataGridViewTextBoxColumn();
            Bin = new DataGridViewTextBoxColumn();
            Amount = new DataGridViewTextBoxColumn();
            Currency = new DataGridViewTextBoxColumn();
            Acquirer = new DataGridViewTextBoxColumn();
            Issuer = new DataGridViewTextBoxColumn();
            Status = new DataGridViewTextBoxColumn();
            CreatedAt = new DataGridViewTextBoxColumn();
            TransactionId = new DataGridViewTextBoxColumn();
            btnSend = new Button();
            btnGetMessages = new Button();
            btnGetLogs = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            dgvLogs = new DataGridView();
            Stage = new DataGridViewTextBoxColumn();
            Level = new DataGridViewTextBoxColumn();
            Note = new DataGridViewTextBoxColumn();
            Created = new DataGridViewTextBoxColumn();
            PayloadIn = new DataGridViewTextBoxColumn();
            PayloadOut = new DataGridViewTextBoxColumn();
            btnClear = new Button();
            ((System.ComponentModel.ISupportInitialize)nudAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMessages).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvLogs).BeginInit();
            SuspendLayout();
            // 
            // txtPAN
            // 
            txtPAN.Location = new Point(183, 38);
            txtPAN.MaxLength = 16;
            txtPAN.Name = "txtPAN";
            txtPAN.Size = new Size(349, 23);
            txtPAN.TabIndex = 0;
            // 
            // txtMerchant
            // 
            txtMerchant.Location = new Point(183, 96);
            txtMerchant.Name = "txtMerchant";
            txtMerchant.Size = new Size(349, 23);
            txtMerchant.TabIndex = 1;
            // 
            // txtAcquirer
            // 
            txtAcquirer.Location = new Point(183, 67);
            txtAcquirer.Name = "txtAcquirer";
            txtAcquirer.Size = new Size(349, 23);
            txtAcquirer.TabIndex = 2;
            // 
            // nudAmount
            // 
            nudAmount.DecimalPlaces = 2;
            nudAmount.Location = new Point(183, 125);
            nudAmount.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudAmount.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            nudAmount.Name = "nudAmount";
            nudAmount.Size = new Size(349, 23);
            nudAmount.TabIndex = 3;
            nudAmount.Value = new decimal(new int[] { 1, 0, 0, 131072 });
            // 
            // cboCurrency
            // 
            cboCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCurrency.FormattingEnabled = true;
            cboCurrency.Items.AddRange(new object[] { "TRY", "USD", "EUR" });
            cboCurrency.Location = new Point(183, 154);
            cboCurrency.Name = "cboCurrency";
            cboCurrency.Size = new Size(349, 23);
            cboCurrency.TabIndex = 4;
            // 
            // dtpTxnTime
            // 
            dtpTxnTime.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpTxnTime.Format = DateTimePickerFormat.Custom;
            dtpTxnTime.Location = new Point(183, 183);
            dtpTxnTime.Name = "dtpTxnTime";
            dtpTxnTime.ShowUpDown = true;
            dtpTxnTime.Size = new Size(349, 23);
            dtpTxnTime.TabIndex = 5;
            // 
            // dgvMessages
            // 
            dgvMessages.AllowUserToAddRows = false;
            dgvMessages.AllowUserToDeleteRows = false;
            dgvMessages.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvMessages.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMessages.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMessages.Columns.AddRange(new DataGridViewColumn[] { Id, PANMasked, Bin, Amount, Currency, Acquirer, Issuer, Status, CreatedAt, TransactionId });
            dgvMessages.Location = new Point(0, 307);
            dgvMessages.Name = "dgvMessages";
            dgvMessages.ReadOnly = true;
            dgvMessages.RowHeadersVisible = false;
            dgvMessages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMessages.Size = new Size(994, 180);
            dgvMessages.TabIndex = 6;
            // 
            // Id
            // 
            Id.DataPropertyName = "Id";
            Id.HeaderText = "ID";
            Id.Name = "Id";
            Id.ReadOnly = true;
            // 
            // PANMasked
            // 
            PANMasked.DataPropertyName = "PANMasked";
            PANMasked.HeaderText = "PAN (Maskeli)";
            PANMasked.Name = "PANMasked";
            PANMasked.ReadOnly = true;
            // 
            // Bin
            // 
            Bin.DataPropertyName = "Bin";
            Bin.HeaderText = "BIN";
            Bin.Name = "Bin";
            Bin.ReadOnly = true;
            // 
            // Amount
            // 
            Amount.DataPropertyName = "Amount";
            dataGridViewCellStyle1.Format = "N2";
            Amount.DefaultCellStyle = dataGridViewCellStyle1;
            Amount.HeaderText = "Tutar";
            Amount.Name = "Amount";
            Amount.ReadOnly = true;
            // 
            // Currency
            // 
            Currency.DataPropertyName = "Currency";
            Currency.HeaderText = "Para Birimi";
            Currency.Name = "Currency";
            Currency.ReadOnly = true;
            // 
            // Acquirer
            // 
            Acquirer.DataPropertyName = "Acquirer";
            Acquirer.HeaderText = "Üye İşyeri / Acquirer";
            Acquirer.Name = "Acquirer";
            Acquirer.ReadOnly = true;
            // 
            // Issuer
            // 
            Issuer.DataPropertyName = "Issuer";
            Issuer.HeaderText = "Issuer Bank";
            Issuer.Name = "Issuer";
            Issuer.ReadOnly = true;
            // 
            // Status
            // 
            Status.DataPropertyName = "Status";
            Status.HeaderText = "Durum";
            Status.Name = "Status";
            Status.ReadOnly = true;
            // 
            // CreatedAt
            // 
            CreatedAt.DataPropertyName = "CreatedAt";
            dataGridViewCellStyle2.Format = "dd.MM.yyyy HH:mm";
            CreatedAt.DefaultCellStyle = dataGridViewCellStyle2;
            CreatedAt.HeaderText = "Oluşturulma";
            CreatedAt.Name = "CreatedAt";
            CreatedAt.ReadOnly = true;
            // 
            // TransactionId
            // 
            TransactionId.DataPropertyName = "TransactionId";
            TransactionId.HeaderText = "TxnID";
            TransactionId.Name = "TransactionId";
            TransactionId.ReadOnly = true;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(183, 223);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 7;
            btnSend.Text = "GÖNDER";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // btnGetMessages
            // 
            btnGetMessages.Location = new Point(264, 223);
            btnGetMessages.Name = "btnGetMessages";
            btnGetMessages.Size = new Size(120, 23);
            btnGetMessages.TabIndex = 8;
            btnGetMessages.Text = "MESAJLARI YENİLE";
            btnGetMessages.UseVisualStyleBackColor = true;
            // 
            // btnGetLogs
            // 
            btnGetLogs.Location = new Point(390, 223);
            btnGetLogs.Name = "btnGetLogs";
            btnGetLogs.Size = new Size(142, 23);
            btnGetLogs.TabIndex = 9;
            btnGetLogs.Text = "SEÇİLİ MESAJ LOGLARI";
            btnGetLogs.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(48, 41);
            label1.Name = "label1";
            label1.Size = new Size(122, 15);
            label1.TabIndex = 10;
            label1.Text = "Kart Numarası (PAN) :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(51, 70);
            label2.Name = "label2";
            label2.Size = new Size(119, 15);
            label2.TabIndex = 11;
            label2.Text = "Üye İşyeri / Acquirer :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(129, 127);
            label3.Name = "label3";
            label3.Size = new Size(41, 15);
            label3.TabIndex = 12;
            label3.Text = "Tutar :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(100, 157);
            label4.Name = "label4";
            label4.Size = new Size(70, 15);
            label4.TabIndex = 13;
            label4.Text = "Para Birimi :";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(22, 99);
            label5.Name = "label5";
            label5.Size = new Size(148, 15);
            label5.TabIndex = 14;
            label5.Text = "Üye İşyeri Adı (Opsiyonel) :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(86, 189);
            label6.Name = "label6";
            label6.Size = new Size(84, 15);
            label6.TabIndex = 15;
            label6.Text = "İşlem Zamanı :";
            // 
            // dgvLogs
            // 
            dgvLogs.AllowUserToAddRows = false;
            dgvLogs.AllowUserToDeleteRows = false;
            dgvLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLogs.Columns.AddRange(new DataGridViewColumn[] { Stage, Level, Note, Created, PayloadIn, PayloadOut });
            dgvLogs.Location = new Point(0, 523);
            dgvLogs.MultiSelect = false;
            dgvLogs.Name = "dgvLogs";
            dgvLogs.ReadOnly = true;
            dgvLogs.RowHeadersVisible = false;
            dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLogs.Size = new Size(994, 260);
            dgvLogs.TabIndex = 16;
            // 
            // Stage
            // 
            Stage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Stage.DataPropertyName = "Stage";
            Stage.HeaderText = "Stage";
            Stage.Name = "Stage";
            Stage.ReadOnly = true;
            // 
            // Level
            // 
            Level.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Level.DataPropertyName = "Level";
            Level.HeaderText = "Level";
            Level.Name = "Level";
            Level.ReadOnly = true;
            // 
            // Note
            // 
            Note.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Note.DataPropertyName = "Note";
            Note.HeaderText = "Note";
            Note.Name = "Note";
            Note.ReadOnly = true;
            // 
            // Created
            // 
            Created.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Created.DataPropertyName = "Created";
            dataGridViewCellStyle3.Format = "dd.MM.yyyy HH:mm";
            Created.DefaultCellStyle = dataGridViewCellStyle3;
            Created.HeaderText = "Zaman";
            Created.Name = "Created";
            Created.ReadOnly = true;
            // 
            // PayloadIn
            // 
            PayloadIn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PayloadIn.DataPropertyName = "PayloadIn";
            PayloadIn.HeaderText = "PayloadIn";
            PayloadIn.Name = "PayloadIn";
            PayloadIn.ReadOnly = true;
            // 
            // PayloadOut
            // 
            PayloadOut.DataPropertyName = "PayloadOut";
            PayloadOut.HeaderText = "PayloadOut";
            PayloadOut.Name = "PayloadOut";
            PayloadOut.ReadOnly = true;
            PayloadOut.Width = 94;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(183, 252);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(349, 23);
            btnClear.TabIndex = 17;
            btnClear.Text = "TEMİZLE";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // SwitchView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnClear);
            Controls.Add(dgvLogs);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnGetLogs);
            Controls.Add(btnGetMessages);
            Controls.Add(btnSend);
            Controls.Add(dgvMessages);
            Controls.Add(dtpTxnTime);
            Controls.Add(cboCurrency);
            Controls.Add(nudAmount);
            Controls.Add(txtAcquirer);
            Controls.Add(txtMerchant);
            Controls.Add(txtPAN);
            Name = "SwitchView";
            Size = new Size(994, 783);
            ((System.ComponentModel.ISupportInitialize)nudAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMessages).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvLogs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtPAN;
        private TextBox txtMerchant;
        private TextBox txtAcquirer;
        private NumericUpDown nudAmount;
        private ComboBox cboCurrency;
        private DateTimePicker dtpTxnTime;
        private DataGridView dgvMessages;
        private Button btnSend;
        private Button btnGetMessages;
        private Button btnGetLogs;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn PANMasked;
        private DataGridViewTextBoxColumn Bin;
        private DataGridViewTextBoxColumn Amount;
        private DataGridViewTextBoxColumn Currency;
        private DataGridViewTextBoxColumn Acquirer;
        private DataGridViewTextBoxColumn Issuer;
        private DataGridViewTextBoxColumn Status;
        private DataGridViewTextBoxColumn CreatedAt;
        private DataGridViewTextBoxColumn TransactionId;
        private DataGridView dgvLogs;
        private DataGridViewTextBoxColumn Stage;
        private DataGridViewTextBoxColumn Level;
        private DataGridViewTextBoxColumn Note;
        private DataGridViewTextBoxColumn Created;
        private DataGridViewTextBoxColumn PayloadIn;
        private DataGridViewTextBoxColumn PayloadOut;
        private Button btnClear;
    }
}
