using CuahangNongduoc.BLL.Controller;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc.UI.PhieuThuChi
{
    public partial class frmChiPhiPhatSinh: Form
    {
        ChiPhiPhatSinhController ctrl = new ChiPhiPhatSinhController();

        

        public frmChiPhiPhatSinh()
        {
            InitializeComponent();
        }
        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Chi phi phat sinh", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        private void frmChiPhiPhatSinh_Load(object sender, EventArgs e)
        {
            dataGridView.AutoGenerateColumns = false;
            ctrl.HienThiDataGridView(dataGridView, bindingNavigator);
        }
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            string maChiPhi  = Guid.NewGuid().ToString("N").Substring(0,5);

            DataRowView row = (DataRowView)bindingNavigator.BindingSource.AddNew();
            row["ID"] = maChiPhi;
            row["TEN_CHI_PHI"] = "";
            row["LOAI_CHI_PHI"] = "";
            row["SO_TIEN"] = 0;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource.Current == null) return;

            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Chi phi phat sinh",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // Lấy ID của dòng cần xóa
                    string id = ((DataRowView)bindingNavigator.BindingSource.Current)["ID"].ToString();

                    // Xóa trong Database
                    ctrl.Delete(id);

                    // Xóa khỏi BindingSource (UI)
                    bindingNavigator.BindingSource.RemoveCurrent();
                    ((DataTable)((BindingSource)bindingNavigator.BindingSource).DataSource).AcceptChanges();

                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kết thúc edit mode trên DataGridView
                bindingNavigator.BindingSource.EndEdit();

                // Lấy DataTable từ BindingSource
                DataTable dt = (DataTable)((BindingSource)bindingNavigator.BindingSource).DataSource;

                // Lấy những dòng thay đổi
                DataTable changes = dt.GetChanges();

                if (changes == null || changes.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu thay đổi để lưu!");
                    return;
                }

                // Duyệt qua từng dòng thay đổi
                foreach (DataRow row in changes.Rows)
                {
                    ChiPhiPhatSinh chiPhi = new ChiPhiPhatSinh();
                    chiPhi.Id = row["ID"].ToString();
                    chiPhi.TenChiPhi = row["TEN_CHI_PHI"].ToString();
                    chiPhi.LoaiChiPhi = row["LOAI_CHI_PHI"].ToString();
                    //chiPhi.SoTien = Convert.ToInt32(row["SO_TIEN"]);

                    if (row.RowState == DataRowState.Added)
                    {
                        // INSERT vào database
                        ctrl.InSert(chiPhi);
                    }
                    else if (row.RowState == DataRowState.Modified)
                    {
                        // UPDATE vào database
                        ctrl.Update(chiPhi);
                    }
                }

                // Chấp nhận thay đổi sau khi lưu thành công
                dt.AcceptChanges();

                MessageBox.Show("Lưu dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message);
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {

        }

        
    }
}
