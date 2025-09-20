namespace SmartBank.Desktop.Win.Views
{
    partial class TransactionView
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
            dgvTx = new DataGridView();
            cboCard = new ComboBox();
            nudAmount = new NumericUpDown();
            cboCurrency = new ComboBox();
            dtpDate = new DateTimePicker();
            txtDesc = new TextBox();
            txtSearchId = new TextBox();
            menuStrip1 = new MenuStrip();
            miList = new ToolStripMenuItem();
            miGetById = new ToolStripMenuItem();
            miGetByCard = new ToolStripMenuItem();
            miCreate = new ToolStripMenuItem();
            miClear = new ToolStripMenuItem();
            tableLayoutPanel1 = new TableLayoutPanel();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            txtSearchCardId = new TextBox();
            label1 = new Label();
            label7 = new Label();
            colId = new DataGridViewTextBoxColumn();
            colCardId = new DataGridViewTextBoxColumn();
            colCurr = new DataGridViewTextBoxColumn();
            colAmount = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colTxnDate = new DataGridViewTextBoxColumn();
            colDesc = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvTx).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAmount).BeginInit();
            menuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvTx
            // 
            dgvTx.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTx.Columns.AddRange(new DataGridViewColumn[] { colId, colCardId, colCurr, colAmount, colStatus, colTxnDate, colDesc });
            dgvTx.Location = new Point(18, 39);
            dgvTx.Name = "dgvTx";
            dgvTx.Size = new Size(1018, 494);
            dgvTx.TabIndex = 0;
            // 
            // cboCard
            // 
            cboCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboCard.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCard.FormattingEnabled = true;
            cboCard.Location = new Point(163, 3);
            cboCard.Name = "cboCard";
            cboCard.Size = new Size(270, 23);
            cboCard.TabIndex = 2;
            // 
            // nudAmount
            // 
            nudAmount.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudAmount.DecimalPlaces = 2;
            nudAmount.Location = new Point(163, 28);
            nudAmount.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            nudAmount.Name = "nudAmount";
            nudAmount.Size = new Size(270, 23);
            nudAmount.TabIndex = 3;
            nudAmount.ThousandsSeparator = true;
            // 
            // cboCurrency
            // 
            cboCurrency.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCurrency.FormattingEnabled = true;
            cboCurrency.Items.AddRange(new object[] { "TRY", "USD", "EUR" });
            cboCurrency.Location = new Point(163, 53);
            cboCurrency.Name = "cboCurrency";
            cboCurrency.Size = new Size(270, 23);
            cboCurrency.TabIndex = 4;
            // 
            // dtpDate
            // 
            dtpDate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dtpDate.CustomFormat = "dd MMM yyyy HH:mm";
            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.Location = new Point(163, 78);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(270, 23);
            dtpDate.TabIndex = 5;
            // 
            // txtDesc
            // 
            txtDesc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDesc.Location = new Point(163, 103);
            txtDesc.MaxLength = 200;
            txtDesc.Name = "txtDesc";
            txtDesc.Size = new Size(270, 23);
            txtDesc.TabIndex = 6;
            // 
            // txtSearchId
            // 
            txtSearchId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearchId.Location = new Point(163, 128);
            txtSearchId.Name = "txtSearchId";
            txtSearchId.Size = new Size(270, 23);
            txtSearchId.TabIndex = 7;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { miList, miGetById, miGetByCard, miCreate, miClear });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1491, 24);
            menuStrip1.TabIndex = 8;
            menuStrip1.Text = "menuStrip1";
            // 
            // miList
            // 
            miList.Name = "miList";
            miList.Size = new Size(52, 20);
            miList.Text = "Listele";
            // 
            // miGetById
            // 
            miGetById.Name = "miGetById";
            miGetById.Size = new Size(73, 20);
            miGetById.Text = "ID ile Getir";
            // 
            // miGetByCard
            // 
            miGetByCard.Name = "miGetByCard";
            miGetByCard.Size = new Size(87, 20);
            miGetByCard.Text = "Kart İşlemleri";
            // 
            // miCreate
            // 
            miCreate.Name = "miCreate";
            miCreate.Size = new Size(58, 20);
            miCreate.Text = "Oluştur";
            // 
            // miClear
            // 
            miClear.Name = "miClear";
            miClear.Size = new Size(59, 20);
            miClear.Text = "Temizle";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(label6, 0, 5);
            tableLayoutPanel1.Controls.Add(label5, 0, 4);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(cboCard, 1, 0);
            tableLayoutPanel1.Controls.Add(txtSearchId, 1, 5);
            tableLayoutPanel1.Controls.Add(nudAmount, 1, 1);
            tableLayoutPanel1.Controls.Add(txtDesc, 1, 4);
            tableLayoutPanel1.Controls.Add(cboCurrency, 1, 2);
            tableLayoutPanel1.Controls.Add(dtpDate, 1, 3);
            tableLayoutPanel1.Controls.Add(txtSearchCardId, 1, 6);
            tableLayoutPanel1.Controls.Add(label7, 0, 6);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Location = new Point(1042, 39);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.Size = new Size(436, 176);
            tableLayoutPanel1.TabIndex = 9;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(3, 130);
            label6.Name = "label6";
            label6.Size = new Size(154, 15);
            label6.TabIndex = 14;
            label6.Text = "ID ile Getir:";
            label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(3, 105);
            label5.Name = "label5";
            label5.Size = new Size(154, 15);
            label5.TabIndex = 13;
            label5.Text = "Açıklama:";
            label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(3, 80);
            label4.Name = "label4";
            label4.Size = new Size(154, 15);
            label4.TabIndex = 12;
            label4.Text = "İşlem Tarihi:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(3, 55);
            label3.Name = "label3";
            label3.Size = new Size(154, 15);
            label3.TabIndex = 11;
            label3.Text = "Para Birimi:";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(3, 30);
            label2.Name = "label2";
            label2.Size = new Size(154, 15);
            label2.TabIndex = 10;
            label2.Text = "Tutar:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSearchCardId
            // 
            txtSearchCardId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearchCardId.Location = new Point(163, 153);
            txtSearchCardId.Name = "txtSearchCardId";
            txtSearchCardId.Size = new Size(270, 23);
            txtSearchCardId.TabIndex = 8;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(3, 5);
            label1.Name = "label1";
            label1.Size = new Size(154, 15);
            label1.TabIndex = 9;
            label1.Text = "Kart Seçimi:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(3, 155);
            label7.Name = "label7";
            label7.Size = new Size(154, 15);
            label7.TabIndex = 15;
            label7.Text = "Kart İşlemleri İçin:";
            label7.TextAlign = ContentAlignment.MiddleRight;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.HeaderText = "ID";
            colId.Name = "colId";
            // 
            // colCardId
            // 
            colCardId.DataPropertyName = "CardId";
            colCardId.HeaderText = "Kart";
            colCardId.Name = "colCardId";
            colCardId.Width = 200;
            // 
            // colCurr
            // 
            colCurr.DataPropertyName = "Currency";
            colCurr.HeaderText = "Para Birimi";
            colCurr.Name = "colCurr";
            // 
            // colAmount
            // 
            colAmount.DataPropertyName = "Amount";
            dataGridViewCellStyle1.Format = "N2";
            dataGridViewCellStyle1.NullValue = null;
            colAmount.DefaultCellStyle = dataGridViewCellStyle1;
            colAmount.HeaderText = "Tutar";
            colAmount.Name = "colAmount";
            // 
            // colStatus
            // 
            colStatus.DataPropertyName = "Status";
            colStatus.HeaderText = "Durum";
            colStatus.Name = "colStatus";
            // 
            // colTxnDate
            // 
            colTxnDate.DataPropertyName = "TransactionDate";
            dataGridViewCellStyle2.Format = "dd.MM.yyyy HH:mm";
            colTxnDate.DefaultCellStyle = dataGridViewCellStyle2;
            colTxnDate.HeaderText = "Tarih";
            colTxnDate.Name = "colTxnDate";
            // 
            // colDesc
            // 
            colDesc.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colDesc.DataPropertyName = "Description";
            colDesc.HeaderText = "Açıklama";
            colDesc.Name = "colDesc";
            // 
            // TransactionView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dgvTx);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            Name = "TransactionView";
            Size = new Size(1491, 580);
            ((System.ComponentModel.ISupportInitialize)dgvTx).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAmount).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvTx;
        private ComboBox cboCard;
        private NumericUpDown nudAmount;
        private ComboBox cboCurrency;
        private DateTimePicker dtpDate;
        private TextBox txtDesc;
        private TextBox txtSearchId;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem miList;
        private ToolStripMenuItem miGetById;
        private ToolStripMenuItem miGetByCard;
        private ToolStripMenuItem miCreate;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox txtSearchCardId;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private ToolStripMenuItem miClear;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colCardId;
        private DataGridViewTextBoxColumn colCurr;
        private DataGridViewTextBoxColumn colAmount;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colTxnDate;
        private DataGridViewTextBoxColumn colDesc;
    }
}
