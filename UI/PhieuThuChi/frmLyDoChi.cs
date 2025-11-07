using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmLyDoChi : Form
    {
        CuahangNongduoc.Controller.LyDoChiController ctrl = new CuahangNongduoc.Controller.LyDoChiController();
        public frmLyDoChi()
        {
            InitializeComponent();
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Ly Do Chi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Ly Do Chi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (bindingNavigator.BindingSource.Count > 0)
                {
                    bindingNavigator.BindingSource.RemoveCurrent();
                    ctrl.Save();
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void frmNhaCungCap_Load(object sender, EventArgs e)
        {
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
        }

      

        private void toolLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Đẩy các thay đổi từ cell đang edit xuống DataTable
                bindingNavigatorPositionItem.Focus();
                this.Validate();
                ((BindingSource)bindingNavigator.BindingSource).EndEdit();

                ctrl.Save();
            }
            catch (Exception ex)
            {
                // Bắt các lỗi CSDL (vd: trùng khóa,...)
                MessageBox.Show("Đã xảy ra lỗi trong quá trình lưu:\n" + ex.Message, "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            // ???
            //bindingNavigator.BindingSource.AddNew();

            var drv = bindingNavigator.BindingSource.Current as DataRowView;
            if (drv != null)
            {
                drv["LY_DO"] = "";
            }

            // Di chuyển đến cuối để bắt đầu nhập
            bindingNavigator.BindingSource.MoveLast();
        }
    }
}