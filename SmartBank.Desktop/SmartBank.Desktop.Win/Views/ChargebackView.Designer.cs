namespace SmartBank.Desktop.Win.Views
{
    partial class ChargebackView
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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            txtTxnFilter = new TextBox();
            cboStatus = new ComboBox();
            btnSearch = new Button();
            btnRefresh = new Button();
            btnOpen = new Button();
            btnAddEvidence = new Button();
            btnDecide = new Button();
            label1 = new Label();
            label2 = new Label();
            dgvCases = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colTransactionId = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colReasonCode = new DataGridViewTextBoxColumn();
            colDisputeAmount = new DataGridViewTextBoxColumn();
            colCurrency = new DataGridViewTextBoxColumn();
            colTransactionAmount = new DataGridViewTextBoxColumn();
            colMerchantName = new DataGridViewTextBoxColumn();
            colOpenedAt = new DataGridViewTextBoxColumn();
            colReplyBy = new DataGridViewTextBoxColumn();
            dgvEvents = new DataGridView();
            colCreatedAt = new DataGridViewTextBoxColumn();
            colType = new DataGridViewTextBoxColumn();
            colNote = new DataGridViewTextBoxColumn();
            colEvidenceUrl = new DataGridViewTextBoxColumn();
            lblCount = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvCases).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvEvents).BeginInit();
            SuspendLayout();
            // 
            // txtTxnFilter
            // 
            txtTxnFilter.Location = new Point(92, 14);
            txtTxnFilter.Name = "txtTxnFilter";
            txtTxnFilter.PlaceholderText = "TxnId";
            txtTxnFilter.Size = new Size(257, 23);
            txtTxnFilter.TabIndex = 0;
            // 
            // cboStatus
            // 
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.FormattingEnabled = true;
            cboStatus.Items.AddRange(new object[] { "", "Open", "Won", "Lost", "Cancelled" });
            cboStatus.Location = new Point(92, 40);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(257, 23);
            cboStatus.TabIndex = 1;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(387, 14);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(105, 49);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "ARA";
            btnSearch.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(498, 14);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(105, 23);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "YENİLE";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(498, 40);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(105, 23);
            btnOpen.TabIndex = 4;
            btnOpen.Text = "YENİ CASE AÇ";
            btnOpen.UseVisualStyleBackColor = true;
            // 
            // btnAddEvidence
            // 
            btnAddEvidence.Location = new Point(609, 14);
            btnAddEvidence.Name = "btnAddEvidence";
            btnAddEvidence.Size = new Size(105, 23);
            btnAddEvidence.TabIndex = 5;
            btnAddEvidence.Text = "KANIT EKLE";
            btnAddEvidence.UseVisualStyleBackColor = true;
            // 
            // btnDecide
            // 
            btnDecide.Location = new Point(609, 40);
            btnDecide.Name = "btnDecide";
            btnDecide.Size = new Size(105, 23);
            btnDecide.TabIndex = 6;
            btnDecide.Text = "KARAR VER";
            btnDecide.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(42, 17);
            label1.Name = "label1";
            label1.Size = new Size(44, 15);
            label1.TabIndex = 7;
            label1.Text = "Txn Id :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(41, 43);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 8;
            label2.Text = "Status :";
            // 
            // dgvCases
            // 
            dgvCases.AllowUserToAddRows = false;
            dgvCases.AllowUserToDeleteRows = false;
            dgvCases.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCases.Columns.AddRange(new DataGridViewColumn[] { colId, colTransactionId, colStatus, colReasonCode, colDisputeAmount, colCurrency, colTransactionAmount, colMerchantName, colOpenedAt, colReplyBy });
            dgvCases.Location = new Point(20, 85);
            dgvCases.MultiSelect = false;
            dgvCases.Name = "dgvCases";
            dgvCases.ReadOnly = true;
            dgvCases.RowHeadersVisible = false;
            dgvCases.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCases.Size = new Size(954, 272);
            dgvCases.TabIndex = 9;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.HeaderText = "Id";
            colId.Name = "colId";
            colId.ReadOnly = true;
            colId.Width = 60;
            // 
            // colTransactionId
            // 
            colTransactionId.DataPropertyName = "TransactionId";
            colTransactionId.HeaderText = "TxnId";
            colTransactionId.Name = "colTransactionId";
            colTransactionId.ReadOnly = true;
            colTransactionId.Width = 90;
            // 
            // colStatus
            // 
            colStatus.DataPropertyName = "Status";
            colStatus.HeaderText = "Status";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            colStatus.Width = 90;
            // 
            // colReasonCode
            // 
            colReasonCode.DataPropertyName = "ReasonCode";
            colReasonCode.HeaderText = "Reason";
            colReasonCode.Name = "colReasonCode";
            colReasonCode.ReadOnly = true;
            colReasonCode.Width = 90;
            // 
            // colDisputeAmount
            // 
            colDisputeAmount.DataPropertyName = "DisputedAmount";
            dataGridViewCellStyle1.Format = "N2";
            colDisputeAmount.DefaultCellStyle = dataGridViewCellStyle1;
            colDisputeAmount.HeaderText = "Disputed";
            colDisputeAmount.Name = "colDisputeAmount";
            colDisputeAmount.ReadOnly = true;
            colDisputeAmount.Width = 90;
            // 
            // colCurrency
            // 
            colCurrency.DataPropertyName = "Currency";
            colCurrency.HeaderText = "Cur";
            colCurrency.Name = "colCurrency";
            colCurrency.ReadOnly = true;
            colCurrency.Width = 50;
            // 
            // colTransactionAmount
            // 
            colTransactionAmount.DataPropertyName = "TransactionAmount";
            dataGridViewCellStyle2.Format = "N2";
            colTransactionAmount.DefaultCellStyle = dataGridViewCellStyle2;
            colTransactionAmount.HeaderText = "TxnAmt";
            colTransactionAmount.Name = "colTransactionAmount";
            colTransactionAmount.ReadOnly = true;
            colTransactionAmount.Width = 90;
            // 
            // colMerchantName
            // 
            colMerchantName.DataPropertyName = "MerchantName";
            colMerchantName.HeaderText = "Merchant";
            colMerchantName.Name = "colMerchantName";
            colMerchantName.ReadOnly = true;
            colMerchantName.Width = 150;
            // 
            // colOpenedAt
            // 
            colOpenedAt.DataPropertyName = "OpenedAt";
            dataGridViewCellStyle3.Format = "yyyy-MM-dd HH:mm";
            colOpenedAt.DefaultCellStyle = dataGridViewCellStyle3;
            colOpenedAt.HeaderText = "Opened";
            colOpenedAt.Name = "colOpenedAt";
            colOpenedAt.ReadOnly = true;
            colOpenedAt.Width = 130;
            // 
            // colReplyBy
            // 
            colReplyBy.DataPropertyName = "ReplyBy";
            dataGridViewCellStyle4.Format = "yyyy-MM-dd";
            colReplyBy.DefaultCellStyle = dataGridViewCellStyle4;
            colReplyBy.HeaderText = "ReplyBy";
            colReplyBy.Name = "colReplyBy";
            colReplyBy.ReadOnly = true;
            colReplyBy.Width = 110;
            // 
            // dgvEvents
            // 
            dgvEvents.AllowUserToAddRows = false;
            dgvEvents.AllowUserToDeleteRows = false;
            dgvEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEvents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEvents.Columns.AddRange(new DataGridViewColumn[] { colCreatedAt, colType, colNote, colEvidenceUrl });
            dgvEvents.Location = new Point(20, 412);
            dgvEvents.MultiSelect = false;
            dgvEvents.Name = "dgvEvents";
            dgvEvents.ReadOnly = true;
            dgvEvents.RowHeadersVisible = false;
            dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEvents.Size = new Size(954, 269);
            dgvEvents.TabIndex = 10;
            // 
            // colCreatedAt
            // 
            colCreatedAt.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colCreatedAt.DataPropertyName = "CreatedAt";
            colCreatedAt.HeaderText = "When";
            colCreatedAt.Name = "colCreatedAt";
            colCreatedAt.ReadOnly = true;
            // 
            // colType
            // 
            colType.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colType.DataPropertyName = "Type";
            colType.HeaderText = "Type";
            colType.Name = "colType";
            colType.ReadOnly = true;
            // 
            // colNote
            // 
            colNote.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colNote.DataPropertyName = "Note";
            colNote.HeaderText = "Note";
            colNote.Name = "colNote";
            colNote.ReadOnly = true;
            // 
            // colEvidenceUrl
            // 
            colEvidenceUrl.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colEvidenceUrl.DataPropertyName = "EvidenceUrl";
            colEvidenceUrl.HeaderText = "Evidence";
            colEvidenceUrl.Name = "colEvidenceUrl";
            colEvidenceUrl.ReadOnly = true;
            // 
            // lblCount
            // 
            lblCount.AutoSize = true;
            lblCount.Dock = DockStyle.Right;
            lblCount.Location = new Point(943, 0);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(52, 15);
            lblCount.TabIndex = 11;
            lblCount.Text = "Count: 0";
            // 
            // ChargebackView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblCount);
            Controls.Add(dgvEvents);
            Controls.Add(dgvCases);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnDecide);
            Controls.Add(btnAddEvidence);
            Controls.Add(btnOpen);
            Controls.Add(btnRefresh);
            Controls.Add(btnSearch);
            Controls.Add(cboStatus);
            Controls.Add(txtTxnFilter);
            Name = "ChargebackView";
            Size = new Size(995, 696);
            ((System.ComponentModel.ISupportInitialize)dgvCases).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvEvents).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtTxnFilter;
        private ComboBox cboStatus;
        private Button btnSearch;
        private Button btnRefresh;
        private Button btnOpen;
        private Button btnAddEvidence;
        private Button btnDecide;
        private Label label1;
        private Label label2;
        private DataGridView dgvCases;
        private DataGridView dgvEvents;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colTransactionId;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colReasonCode;
        private DataGridViewTextBoxColumn colDisputeAmount;
        private DataGridViewTextBoxColumn colCurrency;
        private DataGridViewTextBoxColumn colTransactionAmount;
        private DataGridViewTextBoxColumn colMerchantName;
        private DataGridViewTextBoxColumn colOpenedAt;
        private DataGridViewTextBoxColumn colReplyBy;
        private DataGridViewTextBoxColumn colCreatedAt;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn colNote;
        private DataGridViewTextBoxColumn colEvidenceUrl;
        private Label lblCount;
    }
}
