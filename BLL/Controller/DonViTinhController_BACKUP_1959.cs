<<<<<<< Updated upstream
﻿//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using CuahangNongduoc.BusinessObject;
//using CuahangNongduoc.DataLayer;


//namespace CuahangNongduoc.Controller
//{
//    public class DonViTinhController
//    {
//        DonViTinhFactory factory = new DonViTinhFactory();

//        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb)
//        {
//            DataTable tbl = factory.DanhsachDVT();
//            cmb.DataSource = tbl;
//            cmb.DisplayMember = "TEN_DON_VI";
//            cmb.ValueMember = "ID";
//        }
//        public System.Windows.Forms.DataGridViewComboBoxColumn HienthiDataGridViewComboBoxColumn()
//        {
//            System.Windows.Forms.DataGridViewComboBoxColumn cmb = new System.Windows.Forms.DataGridViewComboBoxColumn();
//            DataTable tbl = factory.DanhsachDVT();
//            cmb.DataSource = tbl;
//            cmb.DisplayMember = "TEN_DON_VI";
//            cmb.ValueMember = "ID";
//            cmb.DataPropertyName = "ID_DON_VI_TINH";
//            cmb.HeaderText = "Đơn vị tính";
//            return cmb;
//        }
//        public void HienthiDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
//        {
//            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
//            bs.DataSource = factory.DanhsachDVT();
//            bn.BindingSource = bs;
//            dg.DataSource = bs;

//        }

//        public DonViTinh LayDVT(int id)
//        {
//            DataTable tbl = factory.LayDVT(id);
//            DonViTinh dvt = null;
//            if (tbl.Rows.Count > 0)
//            {
//                dvt = new DonViTinh(Convert.ToInt32(tbl.Rows[0]["ID"]), Convert.ToString(tbl.Rows[0]["TEN_DON_VI"]));
//            }
//            return dvt;
//        }

//        public bool Save()
//        {
//            return factory.Save();
//        }
//    }
//}
using System;
=======
﻿using System;
>>>>>>> Stashed changes
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class DonViTinhController
    {
        // Giờ không phụ thuộc trực tiếp vào DonViTinhDAL nữa
        private readonly IDonViTinhDAL _dal;
        private DataTable _tableForEdit; // bảng đang bind để Save()

<<<<<<< Updated upstream
=======
        // ====================== CONSTRUCTOR ==========================
        public DonViTinhController()
        {
            // Inject thủ công qua Factory (trường hợp WinForms chưa có IoC)
            _dal = DonViTinhDAL.Create();
        }

        // Cho phép inject test hoặc mock DAL từ ngoài
        public DonViTinhController(IDonViTinhDAL dal)
        {
            _dal = dal ?? DonViTinhDAL.Create();
        }

        // ====================== HÀM TIỆN ÍCH ==========================
        /// <summary>
        /// Chọn tên cột hiển thị phù hợp (tuỳ bảng)
        /// </summary>
        private static string PickColumn(DataTable dt, params string[] candidates)
        {
            if (dt == null) return null;
            foreach (var c in candidates)
            {
                if (!string.IsNullOrEmpty(c) && dt.Columns.Contains(c))
                    return c;
            }

            foreach (DataColumn col in dt.Columns)
            {
                if (col.DataType == typeof(string))
                    return col.ColumnName;
            }

            return dt.Columns.Count > 0 ? dt.Columns[0].ColumnName : null;
        }

        // ====================== BINDING COMBOBOX ==========================
>>>>>>> Stashed changes
        public void HienthiAutoComboBox(ComboBox cmb)
        {
            var dt = _dal.DanhSachDVT();
            cmb.DataSource = dt;
            cmb.DisplayMember = "TEN_DON_VI"; // giữ nguyên schema cũ
            cmb.ValueMember = "ID";
        }

        // ====================== BINDING DATAGRIDVIEW (COMBO COLUMN) ==========================
        public DataGridViewComboBoxColumn HienthiDataGridViewComboBoxColumn()
        {
            var col = new DataGridViewComboBoxColumn
            {
                DataSource = _dal.DanhSachDVT(),
                DisplayMember = "TEN_DON_VI",
                ValueMember = "ID",
                DataPropertyName = "ID_DON_VI_TINH",
                HeaderText = "Đơn vị tính"
            };
            return col;
        }

        // ====================== BINDING DATAGRIDVIEW (TOÀN BẢNG) ==========================
        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn)
        {
            _tableForEdit = _dal.DanhSachDVT();
            var bs = new BindingSource { DataSource = _tableForEdit };
            if (bn != null) bn.BindingSource = bs;
            dg.DataSource = bs;
<<<<<<< Updated upstream
=======
            // (Tuỳ chọn) dg.AutoGenerateColumns = true;
>>>>>>> Stashed changes
        }

        // ====================== LẤY 1 DVT CỤ THỂ ==========================
        public DonViTinh LayDVT(int id)
        {
            var tbl = _dal.LayDVT(id);
            if (tbl.Rows.Count == 0) return null;

            var r = tbl.Rows[0];
            return new DonViTinh(
                Convert.ToInt32(r["ID"]),
                Convert.ToString(r["TEN_DON_VI"])
            );
        }

        // ====================== LƯU THAY ĐỔI (UPDATE/INSERT/DELETE) ==========================
        public bool Save()
        {
<<<<<<< Updated upstream
            if (_tableForEdit == null) return false;     // chưa gọi HienthiDataGridview
            return _dal.Save(_tableForEdit);             // đẩy các Added/Modified/Deleted
        }
    }
}

=======
            if (_tableForEdit == null) return false; // chưa gọi HienthiDataGridview
            return _dal.Save(_tableForEdit);
        }
    }
}
>>>>>>> Stashed changes
