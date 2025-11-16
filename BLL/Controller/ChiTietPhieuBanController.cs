//using System;
using CuahangNongduoc.DAL.Interfaces;
//using System.Collections.Generic;
//using System.Text;
//using CuahangNongduoc.DataLayer;
//using CuahangNongduoc.BusinessObject;
//using System.Windows.Forms;
//using System.Data;

//namespace CuahangNongduoc.Controller
//{

//    public class ChiTietPhieuBanController
//    {
//        ChiTietPhieuBanFactory factory = new ChiTietPhieuBanFactory();



//        public void HienThiChiTiet(DataGridView dgv, String idPhieuBan)
//        {
//            BindingSource bs = new BindingSource();
//            bs.DataSource = factory.LayChiTietPhieuBan(idPhieuBan);
//            dgv.DataSource = bs;
//        }
//        public DataRow NewRow()
//        {
//            return factory.NewRow();
//        }
//        public void Add(DataRow row)
//        {
//            factory.Add(row);
//        }
//        public void Save()
//        {
//            factory.Save();
//        }


//        public IList<ChiTietPhieuBan> ChiTietPhieuBan(String idPhieuBan)
//        {
//            IList<ChiTietPhieuBan> ds = new List<ChiTietPhieuBan>();

//            DataTable tbl = factory.LayChiTietPhieuBan(idPhieuBan);
//            foreach (DataRow row in tbl.Rows)
//            {
//                MaSanPhamController ctrl = new MaSanPhamController();
//                ChiTietPhieuBan ct = new ChiTietPhieuBan();
//                ct.DonGia = Convert.ToInt64(row["DON_GIA"]);
//                ct.SoLuong = Convert.ToInt32(row["SO_LUONG"]);
//                ct.ThanhTien = Convert.ToInt64(row["THANH_TIEN"]);
//                ct.MaSanPham = ctrl.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]));

//                ds.Add(ct);
//            }
//            return ds;
//        }

//        public IList<ChiTietPhieuBan> ChiTietPhieuBan(DateTime dtNgayBan)
//        {
//            IList<ChiTietPhieuBan> ds = new List<ChiTietPhieuBan>();

//            DataTable tbl = factory.LayChiTietPhieuBan(dtNgayBan);
//            foreach (DataRow row in tbl.Rows)
//            {
//                MaSanPhamController ctrl = new MaSanPhamController();
//                ChiTietPhieuBan ct = new ChiTietPhieuBan();
//                ct.DonGia = Convert.ToInt64(row["DON_GIA"]);
//                ct.SoLuong = Convert.ToInt32(row["SO_LUONG"]);
//                ct.ThanhTien = Convert.ToInt64(row["THANH_TIEN"]);
//                ct.MaSanPham = ctrl.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]));

//                ds.Add(ct);
//            }
//            return ds;
//        }
//        public IList<ChiTietPhieuBan> ChiTietPhieuBan(int thang, int nam)
//        {
//            IList<ChiTietPhieuBan> ds = new List<ChiTietPhieuBan>();

//            DataTable tbl = factory.LayChiTietPhieuBan(thang, nam);
//            foreach (DataRow row in tbl.Rows)
//            {
//                MaSanPhamController ctrl = new MaSanPhamController();
//                ChiTietPhieuBan ct = new ChiTietPhieuBan();
//                ct.DonGia = Convert.ToInt64(row["DON_GIA"]);
//                ct.SoLuong = Convert.ToInt32(row["SO_LUONG"]);
//                ct.ThanhTien = Convert.ToInt64(row["THANH_TIEN"]);
//                ct.MaSanPham = ctrl.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]));

//                ds.Add(ct);
//            }
//            return ds;
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
    public class ChiTietPhieuBanController
    {
        private readonly IChiTietPhieuBanDAL _dal;            // ✅ Interface DAL (inject)
        private readonly MaSanPhamController _maSanPhamCtrl;      // ✅ Inject controller phụ thuộc
        private readonly DataTable _buffer;

        // ✅ Constructor có tham số — Dấu hiệu rõ ràng là đã fix
        public ChiTietPhieuBanController(IChiTietPhieuBanDAL dal, MaSanPhamController maSanPhamCtrl)
        {
            _dal = dal;
            _maSanPhamCtrl = maSanPhamCtrl;
            _buffer = CreateBufferSchema();
        }

        public DataTable Buffer => _buffer;

        /* ===================== BINDING HIỂN THỊ ===================== */
        public void HienThiChiTiet(DataGridView dgv, string idPhieuBan)
        {
            var bs = new BindingSource
            {
                DataSource = _dal.LayChiTietPhieuBan(idPhieuBan)
            };
            dgv.DataSource = bs;
        }

        /* ===================== API GIỮ NGUYÊN CHO UI ===================== */
        public DataRow NewRow() => _buffer.NewRow();

        public void Add(DataRow row)
        {
            if (row.Table != _buffer)
            {
                var newRow = _buffer.NewRow();
                foreach (DataColumn col in _buffer.Columns)
                    if (row.Table.Columns.Contains(col.ColumnName))
                        newRow[col.ColumnName] = row[col.ColumnName];
                _buffer.Rows.Add(newRow);
            }
            else
            {
                _buffer.Rows.Add(row);
            }
        }

        public void Save()
        {
            bool ok = _dal.SaveAddedRows(_buffer);

            if (ok)
            {
                _buffer.Clear();
                _buffer.AcceptChanges();
            }
        }

        /* ===================== TRẢ VỀ LIST DOMAIN OBJECT ===================== */
        public IList<ChiTietPhieuBan> ChiTietPhieuBan(string idPhieuBan)
        {
            return MapToList(_dal.LayChiTietPhieuBan(idPhieuBan));
        }

        public IList<ChiTietPhieuBan> ChiTietPhieuBan(DateTime dtNgayBan)
        {
            return MapToList(_dal.LayChiTietPhieuBan(dtNgayBan));
        }

        public IList<ChiTietPhieuBan> ChiTietPhieuBan(int thang, int nam)
        {
            return MapToList(_dal.LayChiTietPhieuBan(thang, nam));
        }

        public decimal TinhTongTienBanTheoPhieuBan(string maPhieuBan)
        {
            return _dal.TinhTongThanhTienTheoPhieu(maPhieuBan);
        }

        public decimal TinhGiaBinhQuanGiaQuyen(string idSanPham)
        {
            return _dal.TinhGiaBinhQuanGiaQuyen(idSanPham);
        }

        public decimal TinhGiaFIFO(string idSanPham)
        {
            return _dal.TinhGiaFIFO(idSanPham);
        }
        // Lấy toàn bộ chi tiết phiếu bán
        public IList<ChiTietPhieuBan> LayTatCaChiTietPhieuBan()
        {
            return MapToList(_dal.LayTatCaChiTietPhieuBan());
        }
        // Báo cáo doanh thu
        public IList<ChiTietPhieuBan> BaoCaoDoanhThu()
        {
            return MapToList(_dal.BaoCaoDoanhThu());
        }
        private static DataTable CreateBufferSchema()
        {
            var t = new DataTable("CHI_TIET_PHIEU_BAN");
            t.Columns.Add("ID_PHIEU_BAN", typeof(string));
            t.Columns.Add("ID_MA_SAN_PHAM", typeof(string));
            t.Columns.Add("SO_LUONG", typeof(int));
            t.Columns.Add("DON_GIA", typeof(decimal));
            t.Columns.Add("THANH_TIEN", typeof(decimal));
            return t;
        }

        private IList<ChiTietPhieuBan> MapToList(DataTable tbl)
        {
            var ds = new List<ChiTietPhieuBan>();

            foreach (DataRow row in tbl.Rows)
            {
                var ct = new ChiTietPhieuBan
                {
                    DonGia = Convert.ToInt64(row["DON_GIA"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    ThanhTien = Convert.ToInt64(row["THANH_TIEN"]),
                    MaSanPham = _maSanPhamCtrl.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]))
                };
                ds.Add(ct);
            }
            return ds;
        }
    }
}

