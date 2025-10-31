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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbFIFO = new System.Windows.Forms.RadioButton();
            this.rdbChonLo = new System.Windows.Forms.RadioButton();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(508, 131);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Phương pháp xuất kho";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdbBQGQ);
            this.groupBox2.Controls.Add(this.rdbGiaFIFO);
            this.groupBox2.Location = new System.Drawing.Point(12, 237);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(508, 132);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Phương pháp tính giá";
            // 
            // rdbFIFO
            // 
            this.rdbFIFO.AutoSize = true;
            this.rdbFIFO.Location = new System.Drawing.Point(117, 51);
            this.rdbFIFO.Name = "rdbFIFO";
            this.rdbFIFO.Size = new System.Drawing.Size(71, 24);
            this.rdbFIFO.TabIndex = 0;
            this.rdbFIFO.TabStop = true;
            this.rdbFIFO.Text = "FIFO";
            this.rdbFIFO.UseVisualStyleBackColor = true;
            // 
            // rdbChonLo
            // 
            this.rdbChonLo.AutoSize = true;
            this.rdbChonLo.Location = new System.Drawing.Point(304, 55);
            this.rdbChonLo.Name = "rdbChonLo";
            this.rdbChonLo.Size = new System.Drawing.Size(88, 24);
            this.rdbChonLo.TabIndex = 1;
            this.rdbChonLo.TabStop = true;
            this.rdbChonLo.Text = "Chọn lô";
            this.rdbChonLo.UseVisualStyleBackColor = true;
            // 
            // rdbBQGQ
            // 
            this.rdbBQGQ.AutoSize = true;
            this.rdbBQGQ.Location = new System.Drawing.Point(111, 63);
            this.rdbBQGQ.Name = "rdbBQGQ";
            this.rdbBQGQ.Size = new System.Drawing.Size(180, 24);
            this.rdbBQGQ.TabIndex = 3;
            this.rdbBQGQ.TabStop = true;
            this.rdbBQGQ.Text = "BÌnh quân gia quyền";
            this.rdbBQGQ.UseVisualStyleBackColor = true;
            // 
            // rdbGiaFIFO
            // 
            this.rdbGiaFIFO.AutoSize = true;
            this.rdbGiaFIFO.Location = new System.Drawing.Point(326, 63);
            this.rdbGiaFIFO.Name = "rdbGiaFIFO";
            this.rdbGiaFIFO.Size = new System.Drawing.Size(71, 24);
            this.rdbGiaFIFO.TabIndex = 2;
            this.rdbGiaFIFO.TabStop = true;
            this.rdbGiaFIFO.Text = "FIFO";
            this.rdbGiaFIFO.UseVisualStyleBackColor = true;
            // 
            // btnLuu
            // 
            this.btnLuu.Location = new System.Drawing.Point(111, 403);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(117, 44);
            this.btnLuu.TabIndex = 2;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnHuy
            // 
            this.btnHuy.Location = new System.Drawing.Point(304, 403);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(117, 44);
            this.btnHuy.TabIndex = 3;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // frmCauHinh
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 481);
            this.Controls.Add(this.btnHuy);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmCauHinh";
            this.Text = "frmCauHinh";
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