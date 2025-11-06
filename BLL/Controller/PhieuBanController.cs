using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class PhieuBanController
    {
        private readonly IPhieuBanFactory _phieuBanFactory;
        private readonly BindingSource _bindingSource;
        // ✅ Constructor mặc định (dành cho WinForms, 3 lớp truyền thống)
        public PhieuBanController()
            : this(new PhieuBanFactory())
        { }
        // ✅ Constructor nhận dependency từ bên ngoài
        public PhieuBanController(IPhieuBanFactory phieuBanFactory)
        {
            _phieuBanFactory = phieuBanFactory ?? throw new ArgumentNullException(nameof(phieuBanFactory));
            _bindingSource = new BindingSource
            {
                DataSource = _phieuBanFactory.LayPhieuBan("-1")
            };
        }

        // ✅ Trả về DataRow mới từ DAL
        public DataRow NewRow() => _phieuBanFactory.NewRow();

        // ✅ Thêm phiếu bán mới
        public void Add(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            _phieuBanFactory.Add(row);
        }

        // ✅ Cập nhật thông tin phiếu bán
        public void Update()
        {
            _bindingSource.MoveNext();
            _phieuBanFactory.Save();
        }

        // ✅ Lưu phiếu bán (và hiển thị cảnh báo dùng thử)
        public void Save()
        {
            int soPhieu = _phieuBanFactory.LaySoPhieu();

            if (soPhieu >= 50)
            {
                MessageBox.Show(
                    "Đây là bản dùng thử! Chỉ lưu được 50 phiếu bán!",
                    "Phiếu Bán",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            MessageBox.Show(
                $"Đây là bản dùng thử! Chỉ lưu được thêm {50 - soPhieu} phiếu bán!",
                "Phiếu Bán",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            _phieuBanFactory.Save();
        }

        // ✅ Hiển thị danh sách phiếu bán lẻ
        public void HienThiPhieuBanLe(BindingNavigator navigator, DataGridView grid)
        {
            _bindingSource.DataSource = _phieuBanFactory.DanhsachPhieuBanLe();
            navigator.BindingSource = _bindingSource;
            grid.DataSource = _bindingSource;
        }

        // ✅ Hiển thị danh sách phiếu bán sỉ
        public void HienThiPhieuBanSi(BindingNavigator navigator, DataGridView grid)
        {
            _bindingSource.DataSource = _phieuBanFactory.DanhsachPhieuBanSi();
            navigator.BindingSource = _bindingSource;
            grid.DataSource = _bindingSource;
        }

        // ✅ Hiển thị thông tin phiếu bán chi tiết lên form
        public void HienThiPhieuBan(
            BindingNavigator navigator,
            ComboBox cmbKhachHang,
            TextBox txtId,
            DateTimePicker dtNgayBan,
            NumericUpDown numTongTien,
            NumericUpDown numDaTra,
            NumericUpDown numConNo)
        {
            navigator.BindingSource = _bindingSource;

            txtId.DataBindings.Clear();
            txtId.DataBindings.Add("Text", _bindingSource, "ID");

            cmbKhachHang.DataBindings.Clear();
            cmbKhachHang.DataBindings.Add("SelectedValue", _bindingSource, "ID_KHACH_HANG");

            dtNgayBan.DataBindings.Clear();
            dtNgayBan.DataBindings.Add("Value", _bindingSource, "NGAY_BAN");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", _bindingSource, "TONG_TIEN");

            numDaTra.DataBindings.Clear();
            numDaTra.DataBindings.Add("Value", _bindingSource, "DA_TRA");

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", _bindingSource, "CON_NO");
        }

        // ✅ Lấy chi tiết 1 phiếu bán theo ID
        public PhieuBan LayPhieuBan(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Mã phiếu bán không hợp lệ.", nameof(id));

            DataTable tbl = _phieuBanFactory.LayPhieuBan(id);
            if (tbl.Rows.Count == 0)
                return null;

            var row = tbl.Rows[0];
            var phieu = new PhieuBan
            {
                Id = row["ID"].ToString(),
                NgayBan = Convert.ToDateTime(row["NGAY_BAN"]),
                TongTien = Convert.ToInt64(row["TONG_TIEN"]),
                DaTra = Convert.ToInt64(row["DA_TRA"]),
                ConNo = Convert.ToInt64(row["CON_NO"])
            };

            var khCtrl = new KhachHangController(new KhachHangFactory());
            phieu.KhachHang = khCtrl.LayKhachHang(row["ID_KHACH_HANG"].ToString());

            var ctCtrl = new ChiTietPhieuBanController();
            phieu.ChiTiet = ctCtrl.ChiTietPhieuBan(phieu.Id);

            return phieu;
        }

        // ✅ Tìm phiếu bán theo mã KH + ngày bán
        public void TimPhieuBan(string maKH, DateTime ngayBan)
        {
            _phieuBanFactory.TimPhieuBan(maKH, ngayBan);
        }
    }
}
