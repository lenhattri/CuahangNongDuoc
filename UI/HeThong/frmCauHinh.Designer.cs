namespace CuahangNongduoc.UI.HeThong
{
    partial class frmCauHinh
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbChonLo = new System.Windows.Forms.RadioButton();
            this.rdbFIFO = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbBQGQ = new System.Windows.Forms.RadioButton();
            this.rdbGiaFIFO = new System.Windows.Forms.RadioButton();
            this.btnLuu = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbChonLo);
            this.groupBox1.Controls.Add(this.rdbFIFO);
            this.groupBox1.Location = new System.Drawing.Point(11, 56);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(452, 105);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Phương pháp xuất kho";
            // 
            // rdbChonLo
            // 
            this.rdbChonLo.AutoSize = true;
            this.rdbChonLo.Location = new System.Drawing.Point(270, 44);
            this.rdbChonLo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbChonLo.Name = "rdbChonLo";
            this.rdbChonLo.Size = new System.Drawing.Size(73, 20);
            this.rdbChonLo.TabIndex = 1;
            this.rdbChonLo.TabStop = true;
            this.rdbChonLo.Text = "Chọn lô";
            this.rdbChonLo.UseVisualStyleBackColor = true;
            // 
            // rdbFIFO
            // 
            this.rdbFIFO.AutoSize = true;
            this.rdbFIFO.Location = new System.Drawing.Point(104, 41);
            this.rdbFIFO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbFIFO.Name = "rdbFIFO";
            this.rdbFIFO.Size = new System.Drawing.Size(57, 20);
            this.rdbFIFO.TabIndex = 0;
            this.rdbFIFO.TabStop = true;
            this.rdbFIFO.Text = "FIFO";
            this.rdbFIFO.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdbBQGQ);
            this.groupBox2.Controls.Add(this.rdbGiaFIFO);
            this.groupBox2.Location = new System.Drawing.Point(11, 190);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(452, 106);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Phương pháp tính giá";
            // 
            // rdbBQGQ
            // 
            this.rdbBQGQ.AutoSize = true;
            this.rdbBQGQ.Location = new System.Drawing.Point(99, 50);
            this.rdbBQGQ.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbBQGQ.Name = "rdbBQGQ";
            this.rdbBQGQ.Size = new System.Drawing.Size(149, 20);
            this.rdbBQGQ.TabIndex = 3;
            this.rdbBQGQ.TabStop = true;
            this.rdbBQGQ.Text = "BÌnh quân gia quyền";
            this.rdbBQGQ.UseVisualStyleBackColor = true;
            // 
            // rdbGiaFIFO
            // 
            this.rdbGiaFIFO.AutoSize = true;
            this.rdbGiaFIFO.Location = new System.Drawing.Point(290, 50);
            this.rdbGiaFIFO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbGiaFIFO.Name = "rdbGiaFIFO";
            this.rdbGiaFIFO.Size = new System.Drawing.Size(57, 20);
            this.rdbGiaFIFO.TabIndex = 2;
            this.rdbGiaFIFO.TabStop = true;
            this.rdbGiaFIFO.Text = "FIFO";
            this.rdbGiaFIFO.UseVisualStyleBackColor = true;
            // 
            // btnLuu
            // 
            this.btnLuu.Location = new System.Drawing.Point(99, 322);
            this.btnLuu.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(104, 35);
            this.btnLuu.TabIndex = 2;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnHuy
            // 
            this.btnHuy.Location = new System.Drawing.Point(270, 322);
            this.btnHuy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(104, 35);
            this.btnHuy.TabIndex = 3;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // frmCauHinh
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 385);
            this.Controls.Add(this.btnHuy);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmCauHinh";
            this.Text = "Cấu Hình";
            this.Load += new System.EventHandler(this.frmCauHinh_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbChonLo;
        private System.Windows.Forms.RadioButton rdbFIFO;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdbBQGQ;
        private System.Windows.Forms.RadioButton rdbGiaFIFO;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Button btnHuy;
    }
}