// fix mapping tiền tệ, binding an toàn
using CuahangNongduoc.DAL.Interfaces;

using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class PhieuNhapController
    {
        private readonly IPhieuNhapFactory _factory;
        private readonly BindingSource bs = new BindingSource();

        public PhieuNhapController(IPhieuNhapFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            // Bind schema rỗng để tránh lỗi khi UI khởi tạo
            bs.DataSource = _factory.LayPhieuNhap("-1");
        }

        public DataRow NewRow() => _factory.NewRow();
        public void Add(DataRow row) => _factory.Add(row);

        public void Update()
        {
            //   EndCurrentEdit để commit dữ liệu từ UI -> DataRow
            var cm = GetCurrencyManager();
            cm?.EndCurrentEdit();
            _factory.Save();
        }

        public void Save() => _factory.Save();

        public PhieuNhap LayPhieuNhap(string id)
        {
            var tbl = _factory.LayPhieuNhap(id);
            PhieuNhap ph = null;
            var ctrlNCC = new NhaCungCapController(new NhaCungCapDAL());
            if (tbl.Rows.Count > 0)
            {
                var r = tbl.Rows[0];

                //  DB decimal -> long BO
                long ReadLong(object v)
                {
                    if (v == null || v == DBNull.Value) return 0L;
                    // Hỗ trợ decimal, string, int64
                    return Convert.ToInt64(v);
                }

                ph = new PhieuNhap
                {
                    Id = Convert.ToString(r["ID"]),
                    NgayNhap = (r["NGAY_NHAP"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(r["NGAY_NHAP"]),
                    TongTien = ReadLong(r["TONG_TIEN"]),
                    DaTra = ReadLong(r["DA_TRA"]),
                    ConNo = ReadLong(r["CON_NO"])
                };
                ph.NhaCungCap = ctrlNCC.LayNCC(Convert.ToString(r["ID_NHA_CUNG_CAP"]));

                var ctrl = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());
                ph.ChiTiet = ctrl.ChiTietPhieuNhap(ph.Id);
            }
            return ph;
        }

        public void HienthiPhieuNhap(BindingNavigator bn, DataGridView dg)
        {
            bs.DataSource = _factory.DanhsachPhieuNhap();
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiPhieuNhap(
            BindingNavigator bn,
            TextBox txt,
            ComboBox cmb,
            DateTimePicker dt,
            NumericUpDown numTongTien,
            NumericUpDown numDaTra,
            NumericUpDown numConNo)
        {
            bn.BindingSource = bs;

            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs, "ID", true, DataSourceUpdateMode.OnPropertyChanged);

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_NHA_CUNG_CAP", true, DataSourceUpdateMode.OnPropertyChanged);

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_NHAP", true, DataSourceUpdateMode.OnPropertyChanged);

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN", true, DataSourceUpdateMode.OnPropertyChanged, 0m);

            numDaTra.DataBindings.Clear();
            numDaTra.DataBindings.Add("Value", bs, "DA_TRA", true, DataSourceUpdateMode.OnPropertyChanged, 0m);

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", bs, "CON_NO", true, DataSourceUpdateMode.OnPropertyChanged, 0m);
        }

        public void TimPhieuNhap(string maNCC, DateTime dt)
        {
            bs.DataSource = _factory.TimPhieuNhap(maNCC, dt);
        }

        private CurrencyManager GetCurrencyManager()
        {
            foreach (Form f in Application.OpenForms)
            {
                var cm = f.BindingContext[bs] as CurrencyManager;
                if (cm != null) return cm;
            }
            return null;
        }
    }
}
