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

        /* ===================== BINDING HIỂN THỊ ===================== */
        public void HienthiAutoComboBox(ComboBox cmb)
        {
            cmb.DataSource = _dal.DanhsachLyDo();
            cmb.DisplayMember = "LY_DO";
            cmb.ValueMember = "ID";
        }

        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn)
        {
            var bs = new BindingSource
            {
                DataSource = _dal.DanhsachLyDo()
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
            return _dal.NewRow();
        }

        public void Add(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            _dal.Add(row);
        }

        public bool Save()
        {
            return _dal.Save();
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
    }
}