namespace SmartBank.Desktop.Win.Views
{
    partial class CustomerView
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
            menuStrip1 = new MenuStrip();
            miList = new ToolStripMenuItem();
            miInsert = new ToolStripMenuItem();
            miUpdate = new ToolStripMenuItem();
            miDelete = new ToolStripMenuItem();
            miSave = new ToolStripMenuItem();
            miCancel = new ToolStripMenuItem();
            split = new SplitContainer();
            grid = new DataGridView();
            form = new TableLayoutPanel();
            txtPhone = new TextBox();
            txtEmail = new TextBox();
            txtIdentityNumber = new TextBox();
            txtLastName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            txtFirstName = new TextBox();
            txtAddress = new TextBox();
            txtCity = new TextBox();
            txtCountry = new TextBox();
            dteBirth = new DateTimePicker();
            cmbGender = new ComboBox();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split).BeginInit();
            split.Panel1.SuspendLayout();
            split.Panel2.SuspendLayout();
            split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            form.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { miList, miInsert, miUpdate, miDelete, miSave, miCancel });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(726, 24);
            menuStrip1.TabIndex = 0;
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
            miSave.Visible = false;
            // 
            // miCancel
            // 
            miCancel.Name = "miCancel";
            miCancel.Size = new Size(55, 20);
            miCancel.Text = "Vazgeç";
            miCancel.Visible = false;
            // 
            // split
            // 
            split.Dock = DockStyle.Fill;
            split.Location = new Point(0, 24);
            split.Name = "split";
            // 
            // split.Panel1
            // 
            split.Panel1.Controls.Add(grid);
            // 
            // split.Panel2
            // 
            split.Panel2.Controls.Add(form);
            split.Size = new Size(726, 419);
            split.SplitterDistance = 360;
            split.TabIndex = 1;
            // 
            // grid
            // 
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.Dock = DockStyle.Fill;
            grid.Location = new Point(0, 0);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(360, 419);
            grid.TabIndex = 0;
            // 
            // form
            // 
            form.AutoScroll = true;
            form.AutoSize = true;
            form.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            form.ColumnCount = 2;
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            form.Controls.Add(txtPhone, 1, 4);
            form.Controls.Add(txtEmail, 1, 3);
            form.Controls.Add(txtIdentityNumber, 1, 2);
            form.Controls.Add(txtLastName, 1, 1);
            form.Controls.Add(label1, 0, 0);
            form.Controls.Add(label2, 0, 1);
            form.Controls.Add(label3, 0, 2);
            form.Controls.Add(label4, 0, 3);
            form.Controls.Add(label5, 0, 4);
            form.Controls.Add(label6, 0, 5);
            form.Controls.Add(label7, 0, 6);
            form.Controls.Add(label8, 0, 7);
            form.Controls.Add(label9, 0, 8);
            form.Controls.Add(label10, 0, 9);
            form.Controls.Add(txtFirstName, 1, 0);
            form.Controls.Add(txtAddress, 1, 7);
            form.Controls.Add(txtCity, 1, 8);
            form.Controls.Add(txtCountry, 1, 9);
            form.Controls.Add(dteBirth, 1, 5);
            form.Controls.Add(cmbGender, 1, 6);
            form.Dock = DockStyle.Fill;
            form.Location = new Point(0, 0);
            form.Name = "form";
            form.Padding = new Padding(10);
            form.RowCount = 10;
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            form.Size = new Size(362, 419);
            form.TabIndex = 0;
            // 
            // txtPhone
            // 
            txtPhone.Dock = DockStyle.Fill;
            txtPhone.Location = new Point(150, 170);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(198, 23);
            txtPhone.TabIndex = 14;
            // 
            // txtEmail
            // 
            txtEmail.Dock = DockStyle.Fill;
            txtEmail.Location = new Point(150, 131);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(198, 23);
            txtEmail.TabIndex = 13;
            // 
            // txtIdentityNumber
            // 
            txtIdentityNumber.Dock = DockStyle.Fill;
            txtIdentityNumber.Location = new Point(150, 92);
            txtIdentityNumber.Name = "txtIdentityNumber";
            txtIdentityNumber.Size = new Size(198, 23);
            txtIdentityNumber.TabIndex = 12;
            // 
            // txtLastName
            // 
            txtLastName.Dock = DockStyle.Fill;
            txtLastName.Location = new Point(150, 53);
            txtLastName.Name = "txtLastName";
            txtLastName.Size = new Size(198, 23);
            txtLastName.TabIndex = 11;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(121, 22);
            label1.Name = "label1";
            label1.Size = new Size(22, 15);
            label1.TabIndex = 0;
            label1.Text = "Ad";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(104, 61);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 1;
            label2.Text = "Soyad";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(106, 100);
            label3.Name = "label3";
            label3.Size = new Size(37, 15);
            label3.TabIndex = 2;
            label3.Text = "TCKN";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(107, 139);
            label4.Name = "label4";
            label4.Size = new Size(36, 15);
            label4.TabIndex = 3;
            label4.Text = "Email";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(97, 178);
            label5.Name = "label5";
            label5.Size = new Size(46, 15);
            label5.TabIndex = 4;
            label5.Text = "Telefon";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(64, 217);
            label6.Name = "label6";
            label6.Size = new Size(79, 15);
            label6.TabIndex = 5;
            label6.Text = "Doğum Tarihi";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(94, 256);
            label7.Name = "label7";
            label7.Size = new Size(49, 15);
            label7.TabIndex = 6;
            label7.Text = "Cinsiyet";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(106, 295);
            label8.Name = "label8";
            label8.Size = new Size(37, 15);
            label8.TabIndex = 7;
            label8.Text = "Adres";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new Point(110, 334);
            label9.Name = "label9";
            label9.Size = new Size(33, 15);
            label9.TabIndex = 8;
            label9.Text = "Şehir";
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Location = new Point(113, 377);
            label10.Name = "label10";
            label10.Size = new Size(30, 15);
            label10.TabIndex = 9;
            label10.Text = "Ülke";
            // 
            // txtFirstName
            // 
            txtFirstName.Dock = DockStyle.Fill;
            txtFirstName.Location = new Point(150, 14);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(198, 23);
            txtFirstName.TabIndex = 10;
            // 
            // txtAddress
            // 
            txtAddress.Dock = DockStyle.Fill;
            txtAddress.Location = new Point(150, 287);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(198, 23);
            txtAddress.TabIndex = 15;
            // 
            // txtCity
            // 
            txtCity.Dock = DockStyle.Fill;
            txtCity.Location = new Point(150, 326);
            txtCity.Name = "txtCity";
            txtCity.Size = new Size(198, 23);
            txtCity.TabIndex = 16;
            // 
            // txtCountry
            // 
            txtCountry.Dock = DockStyle.Fill;
            txtCountry.Location = new Point(150, 365);
            txtCountry.Name = "txtCountry";
            txtCountry.Size = new Size(198, 23);
            txtCountry.TabIndex = 17;
            // 
            // dteBirth
            // 
            dteBirth.Dock = DockStyle.Fill;
            dteBirth.Location = new Point(150, 209);
            dteBirth.Name = "dteBirth";
            dteBirth.Size = new Size(198, 23);
            dteBirth.TabIndex = 18;
            // 
            // cmbGender
            // 
            cmbGender.Dock = DockStyle.Fill;
            cmbGender.FormattingEnabled = true;
            cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            cmbGender.Location = new Point(150, 248);
            cmbGender.Name = "cmbGender";
            cmbGender.Size = new Size(198, 23);
            cmbGender.TabIndex = 19;
            // 
            // CustomerView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(split);
            Controls.Add(menuStrip1);
            Name = "CustomerView";
            Size = new Size(726, 443);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            split.Panel1.ResumeLayout(false);
            split.Panel2.ResumeLayout(false);
            split.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)split).EndInit();
            split.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            form.ResumeLayout(false);
            form.PerformLayout();
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
        private SplitContainer split;
        private DataGridView grid;
        private TableLayoutPanel form;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtIdentityNumber;
        private TextBox txtLastName;
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
        private TextBox txtFirstName;
        private TextBox txtAddress;
        private TextBox txtCity;
        private TextBox txtCountry;
        private DateTimePicker dteBirth;
        private ComboBox cmbGender;
    }
}
