//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using CuahangNongduoc.BusinessObject;
//using CuahangNongduoc.DataLayer;


//namespace CuahangNongduoc.Controller
//{
//    public class LyDoChiController
//    {
//        LyDoChiFactory factory = new LyDoChiFactory();

//        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb)
//        {
//            cmb.DataSource = factory.DanhsachLyDo();
//            cmb.DisplayMember = "LY_DO";
//            cmb.ValueMember = "ID";
//        }

//        public void HienthiDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
//        {
//            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
//            DataTable tbl = factory.DanhsachLyDo();
//            bs.DataSource = tbl;
//            bn.BindingSource = bs;
//            dg.DataSource = bs;

//        }

//        public void HienthiDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
//        {

//            cmb.DataSource = factory.DanhsachLyDo();
//            cmb.DisplayMember = "LY_DO";
//            cmb.ValueMember = "ID";
//            cmb.DataPropertyName = "ID_LY_DO_CHI";
//            cmb.HeaderText = "Lý do chi";

//        }

//        public LyDoChi LayLyDoChi(long id)
//        {
//            DataTable tbl = factory.LayLyDoChi(id);
//            LyDoChi lydo = new LyDoChi();
//            if (tbl.Rows.Count > 0)
//            {
//                lydo.Id = Convert.ToInt64(tbl.Rows[0]["ID"]);
//                lydo.LyDo = Convert.ToString(tbl.Rows[0]["LY_DO"]);
//            }
//            return lydo;
//        }

//        public bool Save()
//        {
//            return factory.Save();
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class LyDoChiController
    {
        // DAL ADO.NET (SqlClient) đã viết ở bước trước
        private readonly LyDoChiFactory _dal = new LyDoChiFactory();

        // Bảng hiện tại cho binding + add (thay cho DataService cũ)
        private DataTable _currentTable;

        /* ===================== BINDING HIỂN THỊ ===================== */
        public void HienthiAutoComboBox(ComboBox cmb)
        {
            cmb.DataSource = _dal.DanhsachLyDo();
            cmb.DisplayMember = "LY_DO";
            cmb.ValueMember = "ID";
        }

        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn)
        {
            _currentTable = _dal.DanhsachLyDo();
            var bs = new BindingSource
            {
                DataSource = _currentTable
            };
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiDataGridviewComboBox(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _dal.DanhsachLyDo();
            cmb.DisplayMember = "LY_DO";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_LY_DO_CHI";
            cmb.HeaderText = "Lý do chi";
        }

        /* ===================== API GIỮ NGUYÊN CHO UI ===================== */
        public DataRow NewRow()
        {
            if (_currentTable == null)
                throw new InvalidOperationException("Phải load dữ liệu trước bằng HienthiDataGridview.");

            return _currentTable.NewRow();
        }

        public void Add(DataRow row)
        {
            if (_currentTable == null)
                throw new InvalidOperationException("Phải load dữ liệu trước bằng HienthiDataGridview.");

            if (row == null)
                throw new ArgumentNullException(nameof(row));

            _currentTable.Rows.Add(row);
        }

        public bool Save()
        {
            if (_currentTable == null)
                throw new InvalidOperationException("Phải load dữ liệu trước bằng HienthiDataGridview.");

            // Chỉ ghi các row trạng thái Added → DAL sẽ Insert (transaction)
            bool ok = _dal.SaveChanges(_currentTable);

            if (ok)
            {
                _currentTable.AcceptChanges(); // reset RowState sau khi commit
            }

            return ok;
        }

        /* ===================== TRẢ VỀ SINGLE DOMAIN OBJECT ===================== */
        public LyDoChi LayLyDoChi(long id)
        {
            var tbl = _dal.LayLyDoChi(id);
            if (tbl.Rows.Count == 0)
                return null;

            var row = tbl.Rows[0];
            return new LyDoChi
            {
                Id = Convert.ToInt64(row["ID"]),
                LyDo = Convert.ToString(row["LY_DO"])
            };
        }

        /* ===================== TRẢ VỀ LIST DOMAIN OBJECT (THÊM MỚI) ===================== */
        public IList<LyDoChi> DanhSachLyDo()
        {
            return MapToList(_dal.DanhsachLyDo());
        }

        /* ===================== HELPERS ===================== */
        private static IList<LyDoChi> MapToList(DataTable tbl)
        {
            var ds = new List<LyDoChi>();

            foreach (DataRow row in tbl.Rows)
            {
                var lydo = new LyDoChi
                {
                    Id = Convert.ToInt64(row["ID"]),
                    LyDo = Convert.ToString(row["LY_DO"])
                };
                ds.Add(lydo);
            }
            return ds;
        }
    }
}