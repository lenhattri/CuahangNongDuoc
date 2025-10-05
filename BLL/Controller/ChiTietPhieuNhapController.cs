//using System;
//using System.Collections.Generic;
//using System.Text;
//using CuahangNongduoc.DataLayer;
//using CuahangNongduoc.BusinessObject;
//using System.Windows.Forms;
//using System.Data;

//namespace CuahangNongduoc.Controller
//{

//    public class ChiTietPhieuNhapController
//    {
//        ChiTietPhieuNhapFactory factory = new ChiTietPhieuNhapFactory();


//        public void ThemChiTietPhieuNhap(String idPhieuNhap, String idMaSP)
//        {
//            factory.LoadSchema();
//            DataRow row = factory.NewRow();
//            row["ID_PHIEU_NHAP"] = idPhieuNhap;
//            row["ID_MA_SAN_PHAM"] = idMaSP;

//            factory.Add(row);
//            factory.Save();
//        }
//        public int XoaChiTietPhieuNhap(String idPhieuNhap)
//        {
//            return factory.XoaChiTietPhieuNhap(idPhieuNhap);
//        }

//        public void HienThiChiTietPhieuNhap(String id, ListView lvw)
//        {
//            MaSanPhamController ctrlMSP = new MaSanPhamController();
//            PhieuNhapController ctrlPN = new PhieuNhapController();
//            DataTable tbl = factory.LayChiTietPhieuNhap(id);

//            lvw.Items.Clear();
//            foreach (DataRow row in tbl.Rows)
//            {
//                ChiTietPhieuNhap ct = new ChiTietPhieuNhap();
//                ct.MaSanPham = ctrlMSP.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]));
//                ct.PhieuNhap = ctrlPN.LayPhieuNhap(Convert.ToString((row["ID_PHIEU_NHAP"])));

//                ListViewItem item = new ListViewItem(Convert.ToString(lvw.Items.Count + 1));
//                item.SubItems.Add(ct.MaSanPham.SanPham.TenSanPham);
//                item.SubItems.Add(ct.MaSanPham.Id);
//                item.SubItems.Add(ct.MaSanPham.GiaNhap.ToString("#,###0"));
//                item.SubItems.Add(ct.MaSanPham.SoLuong.ToString("#,###0"));
//                long thanhtien = (ct.MaSanPham.SoLuong + ct.MaSanPham.GiaNhap);
//                item.SubItems.Add(thanhtien.ToString("#,###0"));
//                item.SubItems.Add(ct.MaSanPham.NgaySanXuat.ToString("dd/MM/yyyy"));
//                item.SubItems.Add(ct.MaSanPham.NgayHetHan.ToString("dd/MM/yyyy"));

//                item.Tag = ct;
//                lvw.Items.Add(item);
//            }
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
    public class ChiTietPhieuNhapController
    {
        private readonly ChiTietPhieuNhapDAL _dal = new ChiTietPhieuNhapDAL();
        private readonly DataTable _buffer;

        public ChiTietPhieuNhapController()
        {
            _buffer = CreateBufferSchema();
        }

        /* ===================== BINDING HI?N TH? ===================== */
        public void HienThiChiTiet(DataGridView dgv, string idPhieuNhap)
        {
            var bs = new BindingSource
            {
                DataSource = _dal.LayChiTietPhieuNhap(idPhieuNhap)
            };
            dgv.DataSource = bs;
        }

        /* ===================== API GI? NGUYÊN CHO UI ===================== */
        public DataRow NewRow()
        {
            return _buffer.NewRow();
        }

        public void Add(DataRow row)
        {
            if (row.Table != _buffer)
            {
                var newRow = _buffer.NewRow();
                foreach (DataColumn col in _buffer.Columns)
                {
                    if (row.Table.Columns.Contains(col.ColumnName))
                        newRow[col.ColumnName] = row[col.ColumnName];
                }
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

        public int XoaChiTietPhieuNhap(string idPhieuNhap)
        {
            return _dal.XoaChiTietPhieuNhap(idPhieuNhap);
        }

        /* ===================== TR? V? LIST DOMAIN OBJECT ===================== */
        public IList<ChiTietPhieuNhap> ChiTietPhieuNhap(string idPhieuNhap)
        {
            return MapToList(_dal.LayChiTietPhieuNhap(idPhieuNhap));
        }

        /* ===================== HELPERS ===================== */
        private static DataTable CreateBufferSchema()
        {
            var t = new DataTable("CHI_TIET_PHIEU_NHAP");
            t.Columns.Add("ID_PHIEU_NHAP", typeof(string));
            t.Columns.Add("ID_MA_SAN_PHAM", typeof(string));
            t.Columns.Add("SO_LUONG", typeof(int));
            t.Columns.Add("DON_GIA", typeof(decimal));
            t.Columns.Add("THANH_TIEN", typeof(decimal));
            return t;
        }

        private static IList<ChiTietPhieuNhap> MapToList(DataTable tbl)
        {
            var ds = new List<ChiTietPhieuNhap>();
            var ctrlMaSP = new MaSanPhamController();

            foreach (DataRow row in tbl.Rows)
            {
                var ct = new ChiTietPhieuNhap
                {
                    DonGia = Convert.ToInt64(row["DON_GIA"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    ThanhTien = Convert.ToInt64(row["THANH_TIEN"]),
                    MaSanPham = ctrlMaSP.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]))
                };
                ds.Add(ct);
            }
            return ds;
        }
    }
}
