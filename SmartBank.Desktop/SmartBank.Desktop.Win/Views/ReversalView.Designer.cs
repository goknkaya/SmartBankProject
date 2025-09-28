namespace SmartBank.Desktop.Win.Views
{
    partial class ReversalView
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
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            menuStrip1 = new MenuStrip();
            miList = new ToolStripMenuItem();
            miGetById = new ToolStripMenuItem();
            miGetByTxn = new ToolStripMenuItem();
            miCreate = new ToolStripMenuItem();
            miVoid = new ToolStripMenuItem();
            miClear = new ToolStripMenuItem();
            txtStatus = new TextBox();
            txtDate = new TextBox();
            txtReason = new TextBox();
            txtPerformedBy = new TextBox();
            txtSearchId = new TextBox();
            txtSearchTxnId = new TextBox();
            nudAmount = new NumericUpDown();
            cboSource = new ComboBox();
            dgvRev = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colTransactionId = new DataGridViewTextBoxColumn();
            colReversedAmount = new DataGridViewTextBoxColumn();
            colReason = new DataGridViewTextBoxColumn();
            colPerformedBy = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colReversalDate = new DataGridViewTextBoxColumn();
            colReversalSource = new DataGridViewTextBoxColumn();
            colIsCardLimitRestored = new DataGridViewCheckBoxColumn();
            colVoidedBy = new DataGridViewTextBoxColumn();
            colVoidedAt = new DataGridViewTextBoxColumn();
            colVoidReason = new DataGridViewTextBoxColumn();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            txtReversalId = new TextBox();
            label9 = new Label();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRev).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { miList, miGetById, miGetByTxn, miCreate, miVoid, miClear });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1455, 24);
            menuStrip1.TabIndex = 0;
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
            // miGetByTxn
            // 
            miGetByTxn.Name = "miGetByTxn";
            miGetByTxn.Size = new Size(80, 20);
            miGetByTxn.Text = "Txn ile Getir";
            // 
            // miCreate
            // 
            miCreate.Name = "miCreate";
            miCreate.Size = new Size(58, 20);
            miCreate.Text = "Oluştur";
            // 
            // miVoid
            // 
            miVoid.Name = "miVoid";
            miVoid.Size = new Size(89, 20);
            miVoid.Text = "Void (İptal) Et";
            // 
            // miClear
            // 
            miClear.Name = "miClear";
            miClear.Size = new Size(59, 20);
            miClear.Text = "Temizle";
            // 
            // txtStatus
            // 
            txtStatus.Location = new Point(127, 22);
            txtStatus.Name = "txtStatus";
            txtStatus.ReadOnly = true;
            txtStatus.Size = new Size(209, 23);
            txtStatus.TabIndex = 1;
            // 
            // txtDate
            // 
            txtDate.Location = new Point(127, 54);
            txtDate.Name = "txtDate";
            txtDate.ReadOnly = true;
            txtDate.Size = new Size(209, 23);
            txtDate.TabIndex = 2;
            // 
            // txtReason
            // 
            txtReason.Location = new Point(159, 22);
            txtReason.MaxLength = 200;
            txtReason.Name = "txtReason";
            txtReason.Size = new Size(209, 23);
            txtReason.TabIndex = 3;
            // 
            // txtPerformedBy
            // 
            txtPerformedBy.Location = new Point(159, 51);
            txtPerformedBy.MaxLength = 100;
            txtPerformedBy.Name = "txtPerformedBy";
            txtPerformedBy.Size = new Size(209, 23);
            txtPerformedBy.TabIndex = 4;
            // 
            // txtSearchId
            // 
            txtSearchId.Location = new Point(159, 32);
            txtSearchId.Name = "txtSearchId";
            txtSearchId.Size = new Size(209, 23);
            txtSearchId.TabIndex = 6;
            // 
            // txtSearchTxnId
            // 
            txtSearchTxnId.Location = new Point(159, 61);
            txtSearchTxnId.Name = "txtSearchTxnId";
            txtSearchTxnId.Size = new Size(209, 23);
            txtSearchTxnId.TabIndex = 7;
            // 
            // nudAmount
            // 
            nudAmount.DecimalPlaces = 2;
            nudAmount.Location = new Point(159, 80);
            nudAmount.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            nudAmount.Name = "nudAmount";
            nudAmount.Size = new Size(209, 23);
            nudAmount.TabIndex = 8;
            nudAmount.ThousandsSeparator = true;
            // 
            // cboSource
            // 
            cboSource.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSource.FormattingEnabled = true;
            cboSource.Items.AddRange(new object[] { "API", "MANUEL", "BATCH" });
            cboSource.Location = new Point(159, 109);
            cboSource.Name = "cboSource";
            cboSource.Size = new Size(209, 23);
            cboSource.TabIndex = 9;
            // 
            // dgvRev
            // 
            dgvRev.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRev.Columns.AddRange(new DataGridViewColumn[] { colId, colTransactionId, colReversedAmount, colReason, colPerformedBy, colStatus, colReversalDate, colReversalSource, colIsCardLimitRestored, colVoidedBy, colVoidedAt, colVoidReason });
            dgvRev.Location = new Point(19, 319);
            dgvRev.Name = "dgvRev";
            dgvRev.Size = new Size(1420, 388);
            dgvRev.TabIndex = 10;
            dgvRev.CellFormatting += dgvRev_CellFormatting;
            // 
            // colId
            // 
            colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleRight;
            colId.DefaultCellStyle = dataGridViewCellStyle6;
            colId.HeaderText = "ID";
            colId.Name = "colId";
            colId.ReadOnly = true;
            colId.Width = 43;
            // 
            // colTransactionId
            // 
            colTransactionId.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleRight;
            colTransactionId.DefaultCellStyle = dataGridViewCellStyle7;
            colTransactionId.HeaderText = "İşlem ID";
            colTransactionId.Name = "colTransactionId";
            colTransactionId.ReadOnly = true;
            colTransactionId.Width = 69;
            // 
            // colReversedAmount
            // 
            colReversedAmount.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N2";
            colReversedAmount.DefaultCellStyle = dataGridViewCellStyle8;
            colReversedAmount.HeaderText = "Tutar (Reversal)";
            colReversedAmount.Name = "colReversedAmount";
            colReversedAmount.ReadOnly = true;
            colReversedAmount.Width = 105;
            // 
            // colReason
            // 
            colReason.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colReason.HeaderText = "Reversal Nedeni";
            colReason.MaxInputLength = 200;
            colReason.Name = "colReason";
            colReason.ReadOnly = true;
            colReason.Width = 106;
            // 
            // colPerformedBy
            // 
            colPerformedBy.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colPerformedBy.HeaderText = "İşlemi Yapan";
            colPerformedBy.MaxInputLength = 100;
            colPerformedBy.Name = "colPerformedBy";
            colPerformedBy.ReadOnly = true;
            colPerformedBy.Width = 90;
            // 
            // colStatus
            // 
            colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colStatus.HeaderText = "Durum";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            colStatus.Width = 69;
            // 
            // colReversalDate
            // 
            colReversalDate.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle9.Format = "dd.MM.yyyy HH:mm";
            colReversalDate.DefaultCellStyle = dataGridViewCellStyle9;
            colReversalDate.HeaderText = "Reversal Tarihi";
            colReversalDate.Name = "colReversalDate";
            colReversalDate.ReadOnly = true;
            colReversalDate.Width = 98;
            // 
            // colReversalSource
            // 
            colReversalSource.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colReversalSource.HeaderText = "Kaynak";
            colReversalSource.Name = "colReversalSource";
            colReversalSource.ReadOnly = true;
            colReversalSource.Width = 70;
            // 
            // colIsCardLimitRestored
            // 
            colIsCardLimitRestored.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colIsCardLimitRestored.HeaderText = "Kart Limiti İade Edildi mi";
            colIsCardLimitRestored.Name = "colIsCardLimitRestored";
            colIsCardLimitRestored.ReadOnly = true;
            colIsCardLimitRestored.SortMode = DataGridViewColumnSortMode.Automatic;
            colIsCardLimitRestored.Width = 95;
            // 
            // colVoidedBy
            // 
            colVoidedBy.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colVoidedBy.HeaderText = "Void Eden";
            colVoidedBy.Name = "colVoidedBy";
            colVoidedBy.ReadOnly = true;
            colVoidedBy.Resizable = DataGridViewTriState.True;
            colVoidedBy.Width = 78;
            // 
            // colVoidedAt
            // 
            colVoidedAt.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle10.Format = "dd.MM.yyyy HH:mm";
            colVoidedAt.DefaultCellStyle = dataGridViewCellStyle10;
            colVoidedAt.HeaderText = "Void Tarihi";
            colVoidedAt.Name = "colVoidedAt";
            colVoidedAt.ReadOnly = true;
            colVoidedAt.Width = 80;
            // 
            // colVoidReason
            // 
            colVoidReason.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colVoidReason.HeaderText = "Void Sebebi";
            colVoidReason.Name = "colVoidReason";
            colVoidReason.ReadOnly = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(126, 35);
            label1.Name = "label1";
            label1.Size = new Size(24, 15);
            label1.TabIndex = 11;
            label1.Text = "ID :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 64);
            label2.Name = "label2";
            label2.Size = new Size(127, 15);
            label2.TabIndex = 12;
            label2.Text = "İşlem (Transaction) ID :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(53, 25);
            label3.Name = "label3";
            label3.Size = new Size(97, 15);
            label3.TabIndex = 13;
            label3.Text = "Reversal Nedeni :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(71, 54);
            label4.Name = "label4";
            label4.Size = new Size(79, 15);
            label4.TabIndex = 14;
            label4.Text = "İşlemi Yapan :";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(71, 28);
            label5.Name = "label5";
            label5.Size = new Size(50, 15);
            label5.TabIndex = 15;
            label5.Text = "Durum :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(53, 60);
            label6.Name = "label6";
            label6.Size = new Size(68, 15);
            label6.TabIndex = 16;
            label6.Text = "İptal Tarihi :";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(55, 82);
            label7.Name = "label7";
            label7.Size = new Size(95, 15);
            label7.TabIndex = 17;
            label7.Text = "Tutar (Reversal) :";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(99, 112);
            label8.Name = "label8";
            label8.Size = new Size(51, 15);
            label8.TabIndex = 18;
            label8.Text = "Kaynak :";
            // 
            // txtReversalId
            // 
            txtReversalId.Location = new Point(127, 83);
            txtReversalId.Name = "txtReversalId";
            txtReversalId.ReadOnly = true;
            txtReversalId.Size = new Size(209, 23);
            txtReversalId.TabIndex = 19;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(51, 86);
            label9.Name = "label9";
            label9.Size = new Size(70, 15);
            label9.TabIndex = 20;
            label9.Text = "Reversal ID :";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtSearchId);
            groupBox1.Controls.Add(txtSearchTxnId);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(37, 29);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(437, 111);
            groupBox1.TabIndex = 21;
            groupBox1.TabStop = false;
            groupBox1.Text = "ARA";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtReason);
            groupBox2.Controls.Add(txtPerformedBy);
            groupBox2.Controls.Add(nudAmount);
            groupBox2.Controls.Add(cboSource);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label4);
            groupBox2.Location = new Point(37, 146);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(437, 138);
            groupBox2.TabIndex = 22;
            groupBox2.TabStop = false;
            groupBox2.Text = "OLUŞTUR";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(txtStatus);
            groupBox3.Controls.Add(txtDate);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(label9);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(txtReversalId);
            groupBox3.Location = new Point(480, 29);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(358, 111);
            groupBox3.TabIndex = 23;
            groupBox3.TabStop = false;
            groupBox3.Text = "SEÇİLİ REVERSAL";
            // 
            // ReversalView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(dgvRev);
            Controls.Add(menuStrip1);
            Name = "ReversalView";
            Size = new Size(1455, 731);
            Load += ReversalView_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRev).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem miList;
        private ToolStripMenuItem miGetById;
        private ToolStripMenuItem miGetByTxn;
        private ToolStripMenuItem miCreate;
        private ToolStripMenuItem miVoid;
        private ToolStripMenuItem miClear;
        private TextBox txtStatus;
        private TextBox txtDate;
        private TextBox txtReason;
        private TextBox txtPerformedBy;
        private TextBox txtSearchId;
        private TextBox txtSearchTxnId;
        private NumericUpDown nudAmount;
        private ComboBox cboSource;
        private DataGridView dgvRev;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private TextBox txtReversalId;
        private Label label9;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colTransactionId;
        private DataGridViewTextBoxColumn colReversedAmount;
        private DataGridViewTextBoxColumn colReason;
        private DataGridViewTextBoxColumn colPerformedBy;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colReversalDate;
        private DataGridViewTextBoxColumn colReversalSource;
        private DataGridViewCheckBoxColumn colIsCardLimitRestored;
        private DataGridViewTextBoxColumn colVoidedBy;
        private DataGridViewTextBoxColumn colVoidedAt;
        private DataGridViewTextBoxColumn colVoidReason;
    }
}
