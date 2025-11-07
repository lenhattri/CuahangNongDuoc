namespace CuahangNongduoc.UI.PhieuThuChi
{
    partial class frmPhieuBanChiPhi
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
            this.dgvChiPhi = new System.Windows.Forms.DataGridView();
            this.btnLuu = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Chon = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TEN_CHI_PHI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LOAI_CHI_PHI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SO_TIEN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiPhi)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvChiPhi
            // 
            this.dgvChiPhi.AllowUserToAddRows = false;
            this.dgvChiPhi.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChiPhi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChiPhi.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Chon,
            this.ID,
            this.TEN_CHI_PHI,
            this.LOAI_CHI_PHI,
            this.SO_TIEN});
            this.dgvChiPhi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiPhi.Location = new System.Drawing.Point(0, 0);
            this.dgvChiPhi.Name = "dgvChiPhi";
            this.dgvChiPhi.RowHeadersWidth = 62;
            this.dgvChiPhi.RowTemplate.Height = 28;
            this.dgvChiPhi.Size = new System.Drawing.Size(800, 344);
            this.dgvChiPhi.TabIndex = 0;
            // 
            // btnLuu
            // 
            this.btnLuu.Location = new System.Drawing.Point(317, 24);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(166, 52);
            this.btnLuu.TabIndex = 1;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnLuu);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 244);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 100);
            this.panel1.TabIndex = 2;
            // 
            // Chon
            // 
            this.Chon.HeaderText = "";
            this.Chon.MinimumWidth = 8;
            this.Chon.Name = "Chon";
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            this.ID.HeaderText = "";
            this.ID.MinimumWidth = 8;
            this.ID.Name = "ID";
            this.ID.Visible = false;
            // 
            // TEN_CHI_PHI
            // 
            this.TEN_CHI_PHI.DataPropertyName = "TEN_CHI_PHI";
            this.TEN_CHI_PHI.HeaderText = "Tên chi phí";
            this.TEN_CHI_PHI.MinimumWidth = 8;
            this.TEN_CHI_PHI.Name = "TEN_CHI_PHI";
            this.TEN_CHI_PHI.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TEN_CHI_PHI.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // LOAI_CHI_PHI
            // 
            this.LOAI_CHI_PHI.DataPropertyName = "LOAI_CHI_PHI";
            this.LOAI_CHI_PHI.HeaderText = "Loại chi phí";
            this.LOAI_CHI_PHI.MinimumWidth = 8;
            this.LOAI_CHI_PHI.Name = "LOAI_CHI_PHI";
            this.LOAI_CHI_PHI.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LOAI_CHI_PHI.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SO_TIEN
            // 
            this.SO_TIEN.DataPropertyName = "SO_TIEN";
            this.SO_TIEN.HeaderText = "Số tiền";
            this.SO_TIEN.MinimumWidth = 8;
            this.SO_TIEN.Name = "SO_TIEN";
            this.SO_TIEN.ReadOnly = true;
            this.SO_TIEN.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SO_TIEN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // frmPhieuBanChiPhi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 344);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvChiPhi);
            this.Name = "frmPhieuBanChiPhi";
            this.Text = "PhieuBanChiPhi";
            this.Load += new System.EventHandler(this.frmPhieuBanChiPhi_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiPhi)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvChiPhi;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Chon;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn TEN_CHI_PHI;
        private System.Windows.Forms.DataGridViewTextBoxColumn LOAI_CHI_PHI;
        private System.Windows.Forms.DataGridViewTextBoxColumn SO_TIEN;
    }
}