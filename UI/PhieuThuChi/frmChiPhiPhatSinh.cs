using CuahangNongduoc.BLL.Controller;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc.UI.PhieuThuChi
{
    public partial class frmChiPhiPhatSinh : Form
    {
        private readonly ChiPhiPhatSinhController ctrl;

        public frmChiPhiPhatSinh()
        {
            InitializeComponent();

            // Tạo controller trực tiếp với factory, không dùng DI
            ctrl = new ChiPhiPhatSinhController(new ChiPhiPhatSinhFactory());
        }

        private void frmChiPhiPhatSinh_Load(object sender, EventArgs e)
        {
            dataGridView.AutoGenerateColumns = false;
            ctrl.HienThiDataGridView(dataGridView, bindingNavigator);
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Chi phí phát sinh",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            string maChiPhi = Guid.NewGuid().ToString("N").Substring(0, 5);

            DataRowView row = (DataRowView)bindingNavigator.BindingSource.AddNew();
            row["ID"] = maChiPhi;
            row["TEN_CHI_PHI"] = "";
            row["LOAI_CHI_PHI"] = "";
            row["SO_TIEN"] = 0;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource.Current == null) return;

            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Chi phí phát sinh",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
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
                bindingNavigator.BindingSource.EndEdit();

                DataTable dt = (DataTable)((BindingSource)bindingNavigator.BindingSource).DataSource;
                DataTable changes = dt.GetChanges();

                if (changes == null || changes.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu thay đổi để lưu!");
                    return;
                }

                foreach (DataRow row in changes.Rows)
                {
                    ChiPhiPhatSinh chiPhi = new ChiPhiPhatSinh
                    {
                        Id = row["ID"].ToString(),
                        TenChiPhi = row["TEN_CHI_PHI"].ToString(),
                        LoaiChiPhi = row["LOAI_CHI_PHI"].ToString(),
                        SoTien = Convert.ToInt32(row["SO_TIEN"])
                    };

                    if (row.RowState == DataRowState.Added)
                        ctrl.InSert(chiPhi);
                    else if (row.RowState == DataRowState.Modified)
                        ctrl.Update(chiPhi);
                }

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
            this.Close();
        }
    }
}
