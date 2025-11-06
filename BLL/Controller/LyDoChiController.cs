using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System.Data.SqlClient;


namespace CuahangNongduoc.Controller
{
    public class LyDoChiController
    {
        private readonly ILyDoChiFactory _dal; // ✅ dùng interface thực tế
        private DataTable _currentTable;

        // ✅ Hỗ trợ Dependency Injection
        public LyDoChiController(ILyDoChiFactory dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // ✅ Constructor mặc định để tương thích code cũ
        public LyDoChiController() : this(new LyDoChiFactory())
        {
        }

        /* ===================== BINDING HIỂN THỊ ===================== */
        public void HienthiAutoComboBox(ComboBox cmb)
        {
            cmb.DataSource = factory.DanhsachLyDo();
            cmb.DisplayMember = "LY_DO";
            cmb.ValueMember = "ID";
        }

        public void HienthiDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
        {
            _currentTable = _dal.DanhsachLyDo();
            var bs = new BindingSource { DataSource = _currentTable };
            bn.BindingSource = bs;
            dg.DataSource = bs;
            
        }

        public void HienthiDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {

            cmb.DataSource = factory.DanhsachLyDo();
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

            bool ok = _dal.SaveChanges(_currentTable);
            if (ok) _currentTable.AcceptChanges();
            return ok;
        }

        /* ===================== DOMAIN OBJECT ===================== */
        public LyDoChi LayLyDoChi(long id)
        {
            var tbl = _dal.LayLyDoChi(id);
            if (tbl.Rows.Count == 0) return null;

            var row = tbl.Rows[0];
            return new LyDoChi
            {
                lydo.Id = Convert.ToInt64(tbl.Rows[0]["ID"]);
                lydo.LyDo = Convert.ToString(tbl.Rows[0]["LY_DO"]);
            }
            return lydo;
        }

        public IList<LyDoChi> DanhSachLyDo()
        {
            SqlCommand cmd1 = new SqlCommand();
            return Save(cmd);
        }

        private static IList<LyDoChi> MapToList(DataTable tbl)
        {
            var ds = new List<LyDoChi>();
            foreach (DataRow row in tbl.Rows)
            {
                ds.Add(new LyDoChi
                {
                    Id = Convert.ToInt64(row["ID"]),
                    LyDo = Convert.ToString(row["LY_DO"])
                });
            }
            return ds;
        }
    }
}
