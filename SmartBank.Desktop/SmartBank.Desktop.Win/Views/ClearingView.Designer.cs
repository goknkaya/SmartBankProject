namespace SmartBank.Desktop.Win.Views
{
    partial class ClearingView
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
            splitContainer1 = new SplitContainer();
            dgvBatches = new DataGridView();
            colB_BatchId = new DataGridViewTextBoxColumn();
            colB_Direction = new DataGridViewTextBoxColumn();
            colB_FileName = new DataGridViewTextBoxColumn();
            colB_FileHash = new DataGridViewTextBoxColumn();
            colB_SettlementDate = new DataGridViewTextBoxColumn();
            colB_Status = new DataGridViewTextBoxColumn();
            colB_TotalCount = new DataGridViewTextBoxColumn();
            colB_SuccessCount = new DataGridViewTextBoxColumn();
            colB_FailCount = new DataGridViewTextBoxColumn();
            colB_CreatedAt = new DataGridViewTextBoxColumn();
            colB_ProcessedAt = new DataGridViewTextBoxColumn();
            colB_Notes = new DataGridViewTextBoxColumn();
            btnRetryUnmatched = new Button();
            btnRefreshBatches = new Button();
            btnGenerateOut = new Button();
            btnUploadIn = new Button();
            dtpSettlement = new DateTimePicker();
            cboDirection = new ComboBox();
            dgvRecords = new DataGridView();
            colR_RecordId = new DataGridViewTextBoxColumn();
            colR_LineNumber = new DataGridViewTextBoxColumn();
            colR_TransactionId = new DataGridViewTextBoxColumn();
            colR_CardId = new DataGridViewTextBoxColumn();
            colR_CardLast4 = new DataGridViewTextBoxColumn();
            colR_Amount = new DataGridViewTextBoxColumn();
            colR_Currency = new DataGridViewTextBoxColumn();
            colR_TransactionDate = new DataGridViewTextBoxColumn();
            colR_MerchantName = new DataGridViewTextBoxColumn();
            colR_MatchStatus = new DataGridViewTextBoxColumn();
            colR_ErrorMessage = new DataGridViewTextBoxColumn();
            colR_CreatedAt = new DataGridViewTextBoxColumn();
            btnRefreshRecords = new Button();
            btnRetrySelected = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBatches).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRecords).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dgvBatches);
            splitContainer1.Panel1.Controls.Add(btnRetryUnmatched);
            splitContainer1.Panel1.Controls.Add(btnRefreshBatches);
            splitContainer1.Panel1.Controls.Add(btnGenerateOut);
            splitContainer1.Panel1.Controls.Add(btnUploadIn);
            splitContainer1.Panel1.Controls.Add(dtpSettlement);
            splitContainer1.Panel1.Controls.Add(cboDirection);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dgvRecords);
            splitContainer1.Panel2.Controls.Add(btnRefreshRecords);
            splitContainer1.Panel2.Controls.Add(btnRetrySelected);
            splitContainer1.Size = new Size(1267, 603);
            splitContainer1.SplitterDistance = 280;
            splitContainer1.TabIndex = 0;
            // 
            // dgvBatches
            // 
            dgvBatches.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBatches.Columns.AddRange(new DataGridViewColumn[] { colB_BatchId, colB_Direction, colB_FileName, colB_FileHash, colB_SettlementDate, colB_Status, colB_TotalCount, colB_SuccessCount, colB_FailCount, colB_CreatedAt, colB_ProcessedAt, colB_Notes });
            dgvBatches.Location = new Point(12, 83);
            dgvBatches.Name = "dgvBatches";
            dgvBatches.Size = new Size(1244, 189);
            dgvBatches.TabIndex = 6;
            // 
            // colB_BatchId
            // 
            colB_BatchId.HeaderText = "ID";
            colB_BatchId.Name = "colB_BatchId";
            // 
            // colB_Direction
            // 
            colB_Direction.HeaderText = "Direction";
            colB_Direction.Name = "colB_Direction";
            // 
            // colB_FileName
            // 
            colB_FileName.HeaderText = "Dosya Adı";
            colB_FileName.Name = "colB_FileName";
            // 
            // colB_FileHash
            // 
            colB_FileHash.HeaderText = "Hash";
            colB_FileHash.Name = "colB_FileHash";
            // 
            // colB_SettlementDate
            // 
            colB_SettlementDate.HeaderText = "Settlement Date";
            colB_SettlementDate.Name = "colB_SettlementDate";
            // 
            // colB_Status
            // 
            colB_Status.HeaderText = "Durum";
            colB_Status.Name = "colB_Status";
            // 
            // colB_TotalCount
            // 
            colB_TotalCount.HeaderText = "Toplam Satır";
            colB_TotalCount.Name = "colB_TotalCount";
            // 
            // colB_SuccessCount
            // 
            colB_SuccessCount.HeaderText = "Başarılı";
            colB_SuccessCount.Name = "colB_SuccessCount";
            // 
            // colB_FailCount
            // 
            colB_FailCount.HeaderText = "Başarısız";
            colB_FailCount.Name = "colB_FailCount";
            // 
            // colB_CreatedAt
            // 
            colB_CreatedAt.HeaderText = "Oluşturulma";
            colB_CreatedAt.Name = "colB_CreatedAt";
            // 
            // colB_ProcessedAt
            // 
            colB_ProcessedAt.HeaderText = "İşlenme";
            colB_ProcessedAt.Name = "colB_ProcessedAt";
            // 
            // colB_Notes
            // 
            colB_Notes.HeaderText = "Notlar";
            colB_Notes.Name = "colB_Notes";
            // 
            // btnRetryUnmatched
            // 
            btnRetryUnmatched.Location = new Point(430, 50);
            btnRetryUnmatched.Name = "btnRetryUnmatched";
            btnRetryUnmatched.Size = new Size(177, 23);
            btnRetryUnmatched.TabIndex = 5;
            btnRetryUnmatched.Text = "Eşleşmeyenleri Yeniden Dene";
            btnRetryUnmatched.UseVisualStyleBackColor = true;
            // 
            // btnRefreshBatches
            // 
            btnRefreshBatches.Location = new Point(252, 50);
            btnRefreshBatches.Name = "btnRefreshBatches";
            btnRefreshBatches.Size = new Size(177, 23);
            btnRefreshBatches.TabIndex = 4;
            btnRefreshBatches.Text = "Batch Listesini Yenile";
            btnRefreshBatches.UseVisualStyleBackColor = true;
            // 
            // btnGenerateOut
            // 
            btnGenerateOut.Location = new Point(430, 19);
            btnGenerateOut.Name = "btnGenerateOut";
            btnGenerateOut.Size = new Size(177, 23);
            btnGenerateOut.TabIndex = 3;
            btnGenerateOut.Text = "OUT Dosyası Oluştur";
            btnGenerateOut.UseVisualStyleBackColor = true;
            // 
            // btnUploadIn
            // 
            btnUploadIn.Location = new Point(252, 19);
            btnUploadIn.Name = "btnUploadIn";
            btnUploadIn.Size = new Size(177, 23);
            btnUploadIn.TabIndex = 2;
            btnUploadIn.Text = "IN Dosyası Yükle";
            btnUploadIn.UseVisualStyleBackColor = true;
            // 
            // dtpSettlement
            // 
            dtpSettlement.Location = new Point(29, 48);
            dtpSettlement.Name = "dtpSettlement";
            dtpSettlement.Size = new Size(196, 23);
            dtpSettlement.TabIndex = 1;
            // 
            // cboDirection
            // 
            cboDirection.FormattingEnabled = true;
            cboDirection.Items.AddRange(new object[] { "IN", "OUT" });
            cboDirection.Location = new Point(29, 19);
            cboDirection.Name = "cboDirection";
            cboDirection.Size = new Size(196, 23);
            cboDirection.TabIndex = 0;
            // 
            // dgvRecords
            // 
            dgvRecords.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRecords.Columns.AddRange(new DataGridViewColumn[] { colR_RecordId, colR_LineNumber, colR_TransactionId, colR_CardId, colR_CardLast4, colR_Amount, colR_Currency, colR_TransactionDate, colR_MerchantName, colR_MatchStatus, colR_ErrorMessage, colR_CreatedAt });
            dgvRecords.Location = new Point(12, 41);
            dgvRecords.Name = "dgvRecords";
            dgvRecords.Size = new Size(1244, 261);
            dgvRecords.TabIndex = 7;
            // 
            // colR_RecordId
            // 
            colR_RecordId.HeaderText = "ID";
            colR_RecordId.Name = "colR_RecordId";
            // 
            // colR_LineNumber
            // 
            colR_LineNumber.HeaderText = "Satır No";
            colR_LineNumber.Name = "colR_LineNumber";
            // 
            // colR_TransactionId
            // 
            colR_TransactionId.HeaderText = "Transaction ID";
            colR_TransactionId.Name = "colR_TransactionId";
            // 
            // colR_CardId
            // 
            colR_CardId.HeaderText = "Kart ID";
            colR_CardId.Name = "colR_CardId";
            // 
            // colR_CardLast4
            // 
            colR_CardLast4.HeaderText = "Kart Son 4 Hane";
            colR_CardLast4.Name = "colR_CardLast4";
            // 
            // colR_Amount
            // 
            colR_Amount.HeaderText = "Tutar";
            colR_Amount.Name = "colR_Amount";
            // 
            // colR_Currency
            // 
            colR_Currency.HeaderText = "Para Birimi";
            colR_Currency.Name = "colR_Currency";
            // 
            // colR_TransactionDate
            // 
            colR_TransactionDate.HeaderText = "İşlem Tarihi";
            colR_TransactionDate.Name = "colR_TransactionDate";
            // 
            // colR_MerchantName
            // 
            colR_MerchantName.HeaderText = "Üye İşyeri";
            colR_MerchantName.Name = "colR_MerchantName";
            // 
            // colR_MatchStatus
            // 
            colR_MatchStatus.HeaderText = "Eşleşme Durumu";
            colR_MatchStatus.Name = "colR_MatchStatus";
            // 
            // colR_ErrorMessage
            // 
            colR_ErrorMessage.HeaderText = "Hata Mesajı";
            colR_ErrorMessage.Name = "colR_ErrorMessage";
            // 
            // colR_CreatedAt
            // 
            colR_CreatedAt.HeaderText = "Oluşturulma";
            colR_CreatedAt.Name = "colR_CreatedAt";
            // 
            // btnRefreshRecords
            // 
            btnRefreshRecords.Location = new Point(252, 12);
            btnRefreshRecords.Name = "btnRefreshRecords";
            btnRefreshRecords.Size = new Size(177, 23);
            btnRefreshRecords.TabIndex = 7;
            btnRefreshRecords.Text = "Kayıtları Yenile";
            btnRefreshRecords.UseVisualStyleBackColor = true;
            // 
            // btnRetrySelected
            // 
            btnRetrySelected.Location = new Point(430, 12);
            btnRetrySelected.Name = "btnRetrySelected";
            btnRetrySelected.Size = new Size(177, 23);
            btnRetrySelected.TabIndex = 8;
            btnRetrySelected.Text = "Seçiliyi Yeniden Dene";
            btnRetrySelected.UseVisualStyleBackColor = true;
            // 
            // ClearingView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "ClearingView";
            Size = new Size(1267, 603);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBatches).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRecords).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private DataGridView dgvBatches;
        private Button btnRetryUnmatched;
        private Button btnRefreshBatches;
        private Button btnGenerateOut;
        private Button btnUploadIn;
        private DateTimePicker dtpSettlement;
        private ComboBox cboDirection;
        private DataGridView dgvRecords;
        private Button btnRefreshRecords;
        private Button btnRetrySelected;
        private DataGridViewTextBoxColumn colB_BatchId;
        private DataGridViewTextBoxColumn colB_Direction;
        private DataGridViewTextBoxColumn colB_FileName;
        private DataGridViewTextBoxColumn colB_FileHash;
        private DataGridViewTextBoxColumn colB_SettlementDate;
        private DataGridViewTextBoxColumn colB_Status;
        private DataGridViewTextBoxColumn colB_TotalCount;
        private DataGridViewTextBoxColumn colB_SuccessCount;
        private DataGridViewTextBoxColumn colB_FailCount;
        private DataGridViewTextBoxColumn colB_CreatedAt;
        private DataGridViewTextBoxColumn colB_ProcessedAt;
        private DataGridViewTextBoxColumn colB_Notes;
        private DataGridViewTextBoxColumn colR_RecordId;
        private DataGridViewTextBoxColumn colR_LineNumber;
        private DataGridViewTextBoxColumn colR_TransactionId;
        private DataGridViewTextBoxColumn colR_CardId;
        private DataGridViewTextBoxColumn colR_CardLast4;
        private DataGridViewTextBoxColumn colR_Amount;
        private DataGridViewTextBoxColumn colR_Currency;
        private DataGridViewTextBoxColumn colR_TransactionDate;
        private DataGridViewTextBoxColumn colR_MerchantName;
        private DataGridViewTextBoxColumn colR_MatchStatus;
        private DataGridViewTextBoxColumn colR_ErrorMessage;
        private DataGridViewTextBoxColumn colR_CreatedAt;
    }
}
