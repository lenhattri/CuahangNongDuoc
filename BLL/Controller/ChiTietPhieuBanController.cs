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
        private readonly ChiTietPhieuBanDAL _dal = new ChiTietPhieuBanDAL();

        private readonly DataTable _buffer;

        public ChiTietPhieuBanController()
        {
            _buffer = CreateBufferSchema();
        }

        //BINDING HIỂN THỊ 
        public void HienThiChiTiet(DataGridView dgv, string idPhieuBan)
        {
            var bs = new BindingSource
            {
                DataSource = _dal.LayChiTietPhieuBan(idPhieuBan)
            };
            dgv.DataSource = bs;
        }

        public DataRow NewRow()
        {
            return _buffer.NewRow();
        }
        
        public void Add(DataRow row)
        {
            // Đảm bảo row thuộc schema _buffer
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
            // Chỉ ghi các row trạng thái Added → DAL sẽ:
            // 1) Trừ kho
            // 2) Insert CHI_TIET_PHIEU_BAN (transaction)
            bool ok = _dal.SaveAddedRows(_buffer);

            if (ok)
            {
                _buffer.Clear();        // reset buffer sau khi đã commit
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

        /* ===================== HELPERS ===================== */
        // Tính giá bình quân gia quyền của sản phẩm
        public decimal TinhGiaBinhQuanGiaQuyen(string idSanPham)
        {
            return _dal.TinhGiaBinhQuanGiaQuyen(idSanPham);
        }
        public decimal TinhGiaFIFO(string idSanPham)
        {
            return _dal.TinhGiaFIFO(idSanPham);
        }

        private static DataTable CreateBufferSchema()
        {
            // Tạo schema tối thiểu cần để Insert + cập nhật kho
            var t = new DataTable("CHI_TIET_PHIEU_BAN");
            t.Columns.Add("ID_PHIEU_BAN", typeof(string));
            t.Columns.Add("ID_MA_SAN_PHAM", typeof(string));
            t.Columns.Add("SO_LUONG", typeof(int));
            t.Columns.Add("DON_GIA", typeof(decimal));
            t.Columns.Add("THANH_TIEN", typeof(decimal));
            return t;
        }

        private static IList<ChiTietPhieuBan> MapToList(DataTable tbl)
        {
            var ds = new List<ChiTietPhieuBan>();
            var ctrlMaSP = new MaSanPhamController(); // tạo 1 lần, dùng lại

            foreach (DataRow row in tbl.Rows)
            {
                var ct = new ChiTietPhieuBan
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