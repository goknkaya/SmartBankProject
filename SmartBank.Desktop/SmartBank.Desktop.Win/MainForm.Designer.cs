namespace SmartBank.Desktop.Win
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnCustomer = new Button();
            btnCard = new Button();
            btnTransaction = new Button();
            btnSwitch = new Button();
            btnClearing = new Button();
            btnReversal = new Button();
            btnChargeback = new Button();
            panelMenu = new Panel();
            pictureBox1 = new PictureBox();
            panelContent = new Panel();
            panelMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btnCustomer
            // 
            btnCustomer.BackColor = Color.Silver;
            btnCustomer.Dock = DockStyle.Top;
            btnCustomer.FlatStyle = FlatStyle.Flat;
            btnCustomer.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnCustomer.ForeColor = Color.White;
            btnCustomer.Location = new Point(0, 0);
            btnCustomer.Name = "btnCustomer";
            btnCustomer.Padding = new Padding(10, 0, 0, 0);
            btnCustomer.Size = new Size(198, 40);
            btnCustomer.TabIndex = 0;
            btnCustomer.Text = "Customer";
            btnCustomer.TextAlign = ContentAlignment.MiddleLeft;
            btnCustomer.UseVisualStyleBackColor = true;
            btnCustomer.Click += btnCustomer_Click;
            // 
            // btnCard
            // 
            btnCard.BackColor = Color.Silver;
            btnCard.Dock = DockStyle.Top;
            btnCard.FlatStyle = FlatStyle.Flat;
            btnCard.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnCard.ForeColor = Color.White;
            btnCard.Location = new Point(0, 40);
            btnCard.Name = "btnCard";
            btnCard.Padding = new Padding(10, 0, 0, 0);
            btnCard.Size = new Size(198, 40);
            btnCard.TabIndex = 1;
            btnCard.Text = "Card";
            btnCard.TextAlign = ContentAlignment.MiddleLeft;
            btnCard.UseVisualStyleBackColor = true;
            btnCard.Click += btnCard_Click;
            // 
            // btnTransaction
            // 
            btnTransaction.BackColor = Color.Silver;
            btnTransaction.Dock = DockStyle.Top;
            btnTransaction.FlatStyle = FlatStyle.Flat;
            btnTransaction.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnTransaction.ForeColor = Color.White;
            btnTransaction.Location = new Point(0, 80);
            btnTransaction.Name = "btnTransaction";
            btnTransaction.Padding = new Padding(10, 0, 0, 0);
            btnTransaction.Size = new Size(198, 40);
            btnTransaction.TabIndex = 2;
            btnTransaction.Text = "Transaction";
            btnTransaction.TextAlign = ContentAlignment.MiddleLeft;
            btnTransaction.UseVisualStyleBackColor = true;
            btnTransaction.Click += btnTransaction_Click;
            // 
            // btnSwitch
            // 
            btnSwitch.BackColor = Color.Silver;
            btnSwitch.Dock = DockStyle.Top;
            btnSwitch.FlatStyle = FlatStyle.Flat;
            btnSwitch.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnSwitch.ForeColor = Color.White;
            btnSwitch.Location = new Point(0, 120);
            btnSwitch.Name = "btnSwitch";
            btnSwitch.Padding = new Padding(10, 0, 0, 0);
            btnSwitch.Size = new Size(198, 40);
            btnSwitch.TabIndex = 3;
            btnSwitch.Text = "Switch";
            btnSwitch.TextAlign = ContentAlignment.MiddleLeft;
            btnSwitch.UseVisualStyleBackColor = true;
            // 
            // btnClearing
            // 
            btnClearing.BackColor = Color.Silver;
            btnClearing.Dock = DockStyle.Top;
            btnClearing.FlatStyle = FlatStyle.Flat;
            btnClearing.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnClearing.ForeColor = Color.White;
            btnClearing.Location = new Point(0, 160);
            btnClearing.Name = "btnClearing";
            btnClearing.Padding = new Padding(10, 0, 0, 0);
            btnClearing.Size = new Size(198, 40);
            btnClearing.TabIndex = 4;
            btnClearing.Text = "Clearing";
            btnClearing.TextAlign = ContentAlignment.MiddleLeft;
            btnClearing.UseVisualStyleBackColor = true;
            // 
            // btnReversal
            // 
            btnReversal.BackColor = Color.Silver;
            btnReversal.Dock = DockStyle.Top;
            btnReversal.FlatStyle = FlatStyle.Flat;
            btnReversal.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnReversal.ForeColor = Color.White;
            btnReversal.Location = new Point(0, 200);
            btnReversal.Name = "btnReversal";
            btnReversal.Padding = new Padding(10, 0, 0, 0);
            btnReversal.Size = new Size(198, 40);
            btnReversal.TabIndex = 5;
            btnReversal.Text = "Reversal";
            btnReversal.TextAlign = ContentAlignment.MiddleLeft;
            btnReversal.UseVisualStyleBackColor = true;
            btnReversal.Click += btnReversal_Click;
            // 
            // btnChargeback
            // 
            btnChargeback.BackColor = Color.Silver;
            btnChargeback.Dock = DockStyle.Top;
            btnChargeback.FlatStyle = FlatStyle.Flat;
            btnChargeback.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnChargeback.ForeColor = Color.White;
            btnChargeback.Location = new Point(0, 240);
            btnChargeback.Name = "btnChargeback";
            btnChargeback.Padding = new Padding(10, 0, 0, 0);
            btnChargeback.Size = new Size(198, 40);
            btnChargeback.TabIndex = 6;
            btnChargeback.Text = "Chargeback";
            btnChargeback.TextAlign = ContentAlignment.MiddleLeft;
            btnChargeback.UseVisualStyleBackColor = true;
            // 
            // panelMenu
            // 
            panelMenu.BackColor = Color.Silver;
            panelMenu.BorderStyle = BorderStyle.FixedSingle;
            panelMenu.Controls.Add(pictureBox1);
            panelMenu.Controls.Add(btnChargeback);
            panelMenu.Controls.Add(btnReversal);
            panelMenu.Controls.Add(btnClearing);
            panelMenu.Controls.Add(btnSwitch);
            panelMenu.Controls.Add(btnTransaction);
            panelMenu.Controls.Add(btnCard);
            panelMenu.Controls.Add(btnCustomer);
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Location = new Point(0, 0);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(200, 450);
            panelMenu.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Bottom;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 289);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(198, 159);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panelContent
            // 
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(200, 0);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(528, 450);
            panelContent.TabIndex = 2;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(728, 450);
            Controls.Add(panelContent);
            Controls.Add(panelMenu);
            Name = "MainForm";
            Text = "SmartBank Ödeme Sistemleri";
            panelMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnCustomer;
        private Button btnCard;
        private Button btnTransaction;
        private Button btnSwitch;
        private Button btnClearing;
        private Button btnReversal;
        private Button btnChargeback;
        private Panel panelMenu;
        private Panel panelContent;
        private PictureBox pictureBox1;
    }
}
