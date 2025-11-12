using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class PhieuBanController
    {
        private readonly IPhieuBanFactory _phieuBanDal;
        private readonly KhachHangController _khachHangCtrl;
        private readonly BindingSource _bs = new BindingSource();

        // ✅ Inject qua constructor (theo DI)
        public PhieuBanController(IPhieuBanFactory phieuBanDal, KhachHangController khachHangCtrl)
        {
            _phieuBanDal = phieuBanDal;
            _khachHangCtrl = khachHangCtrl;
            _bs.DataSource = _phieuBanDal.LayPhieuBan("-1"); // khởi tạo BindingSource rỗng ban đầu
        }

        public DataRow NewRow() => _phieuBanDal.NewRow();

        public void Add(DataRow row) => _phieuBanDal.Add(row);

        public void Update() => _phieuBanDal.Save();

        public void Save()
        {
            int n = _phieuBanDal.LaySoPhieu(); // ✅ Gọi qua instance chứ không phải static
            if (n >= 50)
            {
                MessageBox.Show("Đây là bản dùng thử! Chỉ lưu được 50 phiếu bán!",
                    "Phiếu Bán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Đây là bản dùng thử! Chỉ lưu được thêm {50 - n} phiếu bán!",
                    "Phiếu Bán", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _phieuBanDal.Save();
            }
        }

        public void HienthiPhieuBanLe(BindingNavigator bn, DataGridView dg)
        {
            _bs.DataSource = _phieuBanDal.DanhsachPhieuBanLe();
            bn.BindingSource = _bs;
            dg.DataSource = _bs;
        }

        public void HienthiPhieuBanSi(BindingNavigator bn, DataGridView dg)
        {
            _bs.DataSource = _phieuBanDal.DanhsachPhieuBanSi();
            bn.BindingSource = _bs;
            dg.DataSource = _bs;
        }

        public void HienthiPhieuBan(
            BindingNavigator bn, ComboBox cmb, TextBox txt, DateTimePicker dt,
            NumericUpDown numTongTien, NumericUpDown numDatra, NumericUpDown numConNo)
        {
            bn.BindingSource = _bs;

            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", _bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", _bs, "ID_KHACH_HANG");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", _bs, "NGAY_BAN");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", _bs, "TONG_TIEN");

            numDatra.DataBindings.Clear();
            numDatra.DataBindings.Add("Value", _bs, "DA_TRA");

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", _bs, "CON_NO");
        }

        public PhieuBan LayPhieuBan(string id)
        {
            DataTable tbl = _phieuBanDal.LayPhieuBan(id);
            if (tbl.Rows.Count == 0) return null;

            var row = tbl.Rows[0];
            var ph = new PhieuBan
            {
                Id = Convert.ToString(row["ID"]),
                NgayBan = Convert.ToDateTime(row["NGAY_BAN"]),
                TongTien = Convert.ToInt64(row["TONG_TIEN"]),
                DaTra = Convert.ToInt64(row["DA_TRA"]),
                ConNo = Convert.ToInt64(row["CON_NO"]),
                KhachHang = _khachHangCtrl.LayKhachHang(Convert.ToString(row["ID_KHACH_HANG"]))
            };

            IChiTietPhieuBanDAL chiTietDal = new ChiTietPhieuBanDAL();
            MaSanPhamController maSpCtrl = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());

            var ctCtrl = new ChiTietPhieuBanController(chiTietDal, maSpCtrl);
            ph.ChiTiet = ctCtrl.ChiTietPhieuBan(ph.Id);

            return ph;
        }

        public void TimPhieuBan(string maKH, DateTime dt)
        {
            _phieuBanDal.TimPhieuBan(maKH, dt);
        }
    }
}
