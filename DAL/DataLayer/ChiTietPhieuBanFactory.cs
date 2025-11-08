// DAL/DataLayer/ChiTietPhieuBanFactory.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.BLL.Helpers;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DAL.Infrastructure; // CHANGED: dùng DbClient

namespace CuahangNongduoc.DataLayer
{
    /// <summary>
    /// DAL cho CHI_TIET_PHIEU_BAN – bản ADO.NET (SqlClient) qua DbClient
    /// </summary>
    public class ChiTietPhieuBanDAL
    {
        private readonly DbClient _db = DbClient.Instance; // CHANGED
        private DataTable _dataTable;                      // NEW: Thêm DataTable nội bộ như UserDAL

        // NEW: schema DataTable hợp nhất 2 bảng để NewRow/Add/Save giữ được cột
        private void EnsureSchema()
        {
            if (_dataTable != null) return;
            _dataTable = new DataTable("CHI_TIET_PHIEU_BAN");
            // Dựa trên các cột trong phương thức Insert
            _dataTable.Columns.Add("ID_PHIEU_BAN", typeof(string));
            _dataTable.Columns.Add("ID_MA_SAN_PHAM", typeof(string));
            _dataTable.Columns.Add("SO_LUONG", typeof(int));
            _dataTable.Columns.Add("DON_GIA", typeof(decimal));
            _dataTable.Columns.Add("THANH_TIEN", typeof(decimal));
        }

        /* ===================== SELECT ===================== */

        public DataTable LayChiTietPhieuBan(string idPhieuBan)
        {
            // CHANGED: dùng DbClient + chỉ chọn cột cần thiết (tùy ý)
            const string sql =
                "SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id";

            // CHANGED: Gán vào _dataTable và đồng bộ, giống UserDAL
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.VarChar, idPhieuBan, 50));
            _dataTable = dt; // đồng bộ
            return dt;
        }

        public DataTable LayChiTietPhieuBan(DateTime ngayBan)
        {
            // CHANGED: sargable theo khoảng [start, end)
            var start = ngayBan.Date;
            var end = start.AddDays(1);

            const string sql = @"
                SELECT CT.*
                FROM CHI_TIET_PHIEU_BAN CT
                JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID
                WHERE PB.NGAY_BAN >= @start AND PB.NGAY_BAN < @end";
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@start", SqlDbType.DateTime, start),
                _db.P("@end", SqlDbType.DateTime, end));
        }

        public DataTable LayChiTietPhieuBan(int thang, int nam)
        {
            // CHANGED: chuyển sang range đầu-tháng → đầu-tháng-sau (sargable)
            var start = new DateTime(nam, thang, 1);
            var end = start.AddMonths(1);

            const string sql = @"
                SELECT CT.*
                FROM CHI_TIET_PHIEU_BAN CT
                JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID
                WHERE PB.NGAY_BAN >= @start AND PB.NGAY_BAN < @end";
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@start", SqlDbType.DateTime, start),
                _db.P("@end", SqlDbType.DateTime, end));
        }
        /// <summary>
        /// Lấy danh sách các lô nhập (còn tồn) của sản phẩm,
        /// dùng khi xuất hàng theo FIFO hoặc chỉ định.
        /// </summary>

        public DataTable LayDanhSachMaTheoSanPham(string idSanPham)
        {
            const string sql = @"
            SELECT ID, SO_LUONG, DON_GIA_NHAP, NGAY_HET_HAN
            FROM MA_SAN_PHAM
            WHERE ID_SAN_PHAM = @idSanPham AND SO_LUONG > 0
            ORDER BY NGAY_HET_HAN ASC";

            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@idSanPham", SqlDbType.VarChar, idSanPham, 50));
        }
        public string LayIdSanPhamTuMaSanPham(string idMaSanPham)
        {
            const string sql = @"
            SELECT ID_SAN_PHAM
            FROM MA_SAN_PHAM
            WHERE ID = @idMaSanPham";
            DataTable dt =_db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@idMaSanPham", SqlDbType.VarChar, idMaSanPham, 50)
                );
            if (dt.Rows.Count > 0)
                return Convert.ToString(dt.Rows[0]["ID_SAN_PHAM"]);
            else
                return string.Empty;
        }
        public DataTable LayThongTinMotLo(string idMaSanPham)
        {
            const string sql = @"
            SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_HET_HAN
            FROM MA_SAN_PHAM
            WHERE ID = @idMaSanPham";
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@idMaSanPham", SqlDbType.VarChar, idMaSanPham, 50));
        }
        public decimal TinhTongThanhTienTheoPhieu(string maPhieuBan)
        {
            const string sql = @"
            SELECT SUM(THANH_TIEN)
            FROM CHI_TIET_PHIEU_BAN
            WHERE ID_PHIEU_BAN = @maPhieuBan";
            decimal? result = _db.ExecuteScalar<decimal>(sql, CommandType.Text,
                _db.P("@maPhieuBan", SqlDbType.VarChar, maPhieuBan, 50));
            return result ?? 0;
        }
        //Tính giá của sản phẩm theo phương pháp bình quân gia quyền
        public decimal TinhGiaBinhQuanGiaQuyen(string idSanPham)
        {
            const string sql = @"
            SELECT SUM(DON_GIA_NHAP * SO_LUONG) / NULLIF(SUM(SO_LUONG), 0)
            FROM MA_SAN_PHAM
            WHERE ID_SAN_PHAM = @idSanPham AND SO_LUONG > 0";
            decimal? result = _db.ExecuteScalar<decimal>(sql, CommandType.Text,
                _db.P("@idSanPham", SqlDbType.VarChar, idSanPham, 50));
            return result ?? 0;
        }
        //Tính giá của sản phẩm theo phương pháp nhập trước xuất trước (FIFO)
        public decimal TinhGiaFIFO(string idSanPham)
        {
            const string sql = @"
            SELECT TOP 1 GIA_NHAP
            FROM MA_SAN_PHAM
            WHERE ID_MA_SAN_PHAM = @idSanPham AND SO_LUONG > 0
            ORDER BY NGAY_NHAP ASC";
            decimal? result = _db.ExecuteScalar<decimal>(sql, CommandType.Text,
                _db.P("@idSanPham", SqlDbType.VarChar, idSanPham, 50));
            return result ?? 0;
        }

        /* ========================= DataTable pattern (NEW) ========================= */

        public DataRow NewRow()
        {
            EnsureSchema(); // NEW
            return _dataTable.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema(); // NEW
            _dataTable.Rows.Add(row);
        }

        /* ===================== LOGIC XUẤT KHO (FIFO/CHONLO) ===================== */

        public void XuatTheoFIFO(DataRow row, SqlTransaction tx, string idPhieuBan)
        {
          
          //duplicate
            string idMaSanPham = Convert.ToString(row["ID_MA_SAN_PHAM"]);
            //string idSanPham = Convert.ToString(row["ID_MA_SAN_PHAM"]);
          
            int soLuongConPhaiXuat = Convert.ToInt32(row["SO_LUONG"]);
            decimal donGia = Convert.ToDecimal(row["DON_GIA"]);

            string idSanPham = LayIdSanPhamTuMaSanPham(idMaSanPham);
            DataTable loSanPhams = LayDanhSachMaTheoSanPham(idSanPham);

            foreach (DataRow lo in loSanPhams.Rows)
            {
                if (soLuongConPhaiXuat <= 0)
                    break;
                // Lấy thông tin của Lô
                string idMaLoHienTai = lo["ID"].ToString();
                int soLuongTon = Convert.ToInt32(lo["SO_LUONG"]);
                int soLuongXuatTuLo = Math.Min(soLuongConPhaiXuat, soLuongTon);

                // Lấy đơn giá nhập của lô này (hoặc dùng đơn giá bán tùy logic)
                decimal donGiaNhapCuaLo = Convert.ToDecimal(lo["DON_GIA_NHAP"]);

                // Cập nhật số lượng tồn kho của Lô
                UpdateTonKho(idMaLoHienTai, -soLuongXuatTuLo, tx);
                // Cập nhật tổng số lượng tồn kho của Sản phẩm
                UpdateTongTonKhoSanPham(idSanPham, -soLuongXuatTuLo, tx);

                // Ghi chi tiết phiếu bán cho Lô
                // Sử dụng hàm Insert() thay vì tạo DataRow thủ công
                // (Vì _dataTable dùng để *quản lý* danh sách, không phải để *thêm* vào)
                // Chúng ta Insert trực tiếp vào DB qua Transaction
                const string sqlInsert = @"
                    INSERT INTO CHI_TIET_PHIEU_BAN (ID_PHIEU_BAN, ID_MA_SAN_PHAM, SO_LUONG, DON_GIA, THANH_TIEN)
                    VALUES(@ID_PHIEU_BAN, @ID_MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @THANH_TIEN)";

                using (var cmd = _db.Cmd(tx.Connection, sqlInsert, CommandType.Text, tx, 30,
                    _db.P("@ID_PHIEU_BAN", SqlDbType.VarChar, idPhieuBan, 50),
                    _db.P("@ID_MA_SAN_PHAM", SqlDbType.VarChar, idMaLoHienTai, 50),
                    _db.P("@SO_LUONG", SqlDbType.Int, soLuongXuatTuLo),
                    _db.PDec("@DON_GIA", donGia), // Dùng đơn giá bán (row["DON_GIA"])
                    _db.PDec("@THANH_TIEN", donGia * soLuongXuatTuLo)))
                {
                    cmd.ExecuteNonQuery();
                }

                // Cập nhật số lượng còn phải xuất
                soLuongConPhaiXuat -= soLuongXuatTuLo;
            }
            if (soLuongConPhaiXuat > 0)
            {
                // Ném lỗi để Transaction bên ngoài Rollback
                throw new Exception($"Không đủ tồn kho cho sản phẩm {idSanPham}. Vẫn còn thiếu {soLuongConPhaiXuat} sản phẩm.");
            }

        }
        public void XuatTheoChonLo(DataRow row, SqlTransaction tx, string idPhieuBan)
        {

            string idMaSanPham = Convert.ToString(row["ID_MA_SAN_PHAM"]);
            int soLuongCanXuat = Convert.ToInt32(row["SO_LUONG"]);
            decimal donGia = Convert.ToDecimal(row["DON_GIA"]);
            DataTable loSanPhams = LayThongTinMotLo(idMaSanPham);

            string idSanPham = LayIdSanPhamTuMaSanPham(idMaSanPham);
            int soLuongTon = Convert.ToInt32(loSanPhams.Rows[0]["SO_LUONG"]);
            if (soLuongTon < soLuongCanXuat)
            {
                throw new Exception($"Lô {idMaSanPham} không đủ hàng. (Cần {soLuongCanXuat}, Tồn {soLuongTon})");
            }
            // Cập nhật số lượng tồn kho của Lô
            UpdateTonKho(idMaSanPham, -soLuongCanXuat, tx);
            // Cập nhật tổng số lượng tồn kho của Sản phẩm
            UpdateTongTonKhoSanPham(idSanPham, -soLuongCanXuat, tx);

            // Ghi chi tiết phiếu bán cho Lô
            // Tương tự FIFO, gọi Insert trực tiếp
            const string sqlInsert = @"
                INSERT INTO CHI_TIET_PHIEU_BAN (ID_PHIEU_BAN, ID_MA_SAN_PHAM, SO_LUONG, DON_GIA, THANH_TIEN)
                VALUES(@ID_PHIEU_BAN, @ID_MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @THANH_TIEN)";

            using (var cmd = _db.Cmd(tx.Connection, sqlInsert, CommandType.Text, tx, 30,
                _db.P("@ID_PHIEU_BAN", SqlDbType.VarChar, idPhieuBan, 50),
                _db.P("@ID_MA_SAN_PHAM", SqlDbType.VarChar, idMaSanPham, 50),
                _db.P("@SO_LUONG", SqlDbType.Int, soLuongCanXuat),
                _db.PDec("@DON_GIA", donGia),
                _db.PDec("@THANH_TIEN", donGia * soLuongCanXuat)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /* ===================== INSERT/UPDATE/DELETE ===================== */

        // Phương thức này chủ yếu được gọi nội bộ từ XuatTheo...
        // Hoặc có thể bị loại bỏ nếu XuatTheo... tự Insert trực tiếp (như đã sửa ở trên)
        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            // CHANGED: dùng DbClient.Cmd + P/PDec
            const string sql = @"
                INSERT INTO CHI_TIET_PHIEU_BAN
                (ID_PHIEU_BAN, ID_MA_SAN_PHAM, SO_LUONG, DON_GIA, THANH_TIEN)
                VALUES(@ID_PHIEU_BAN, @ID_MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @THANH_TIEN)";

            using (var cmd = _db.Cmd(tx?.Connection, sql, CommandType.Text, tx, 30,
                _db.P("@ID_PHIEU_BAN", SqlDbType.VarChar, row["ID_PHIEU_BAN"], 50),
                _db.P("@ID_MA_SAN_PHAM", SqlDbType.VarChar, row["ID_MA_SAN_PHAM"], 50),
                _db.P("@SO_LUONG", SqlDbType.Int, row["SO_LUONG"]),
                _db.PDec("@DON_GIA", row["DON_GIA"]),       // NEW: decimal 18,2
                _db.PDec("@THANH_TIEN", row["THANH_TIEN"])))   // NEW: decimal 18,2
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public int UpdateTonKho(string idMaSanPham, int deltaSoLuong, SqlTransaction tx)
        {
            // Giữ đúng logic cũ: cộng/trừ tồn kho theo delta
            const string sql = @"
                UPDATE MA_SAN_PHAM 
                    SET SO_LUONG = SO_LUONG + @delta
                WHERE ID = @id";
            using (var cmd = _db.Cmd(tx.Connection, sql, CommandType.Text, tx, 30,
                _db.P("@delta", SqlDbType.Int, deltaSoLuong),
                _db.P("@id", SqlDbType.VarChar, idMaSanPham, 50)))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public int UpdateTongTonKhoSanPham(string idSanPham, int deltaSoLuong, SqlTransaction tx)
        {
            // Giữ đúng logic cũ: cộng/trừ tồn kho theo delta
            const string sql = @"
                UPDATE SAN_PHAM 
                    SET SO_LUONG = SO_LUONG + @delta
                WHERE ID = @idSanPham";
            using (var cmd = _db.Cmd(tx.Connection, sql, CommandType.Text, tx, 30,
                _db.P("@delta", SqlDbType.Int, deltaSoLuong),
                _db.P("@idSanPham", SqlDbType.VarChar, idSanPham, 50)))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Duyệt bảng, thêm CT + cập nhật kho trong 1 transaction.
        /// Chỉ xử lý các row trạng thái Added.
        /// (Phương thức này nhận DataTable từ ngoài, không dùng _dataTable nội bộ)
        /// </summary>
        public bool SaveAddedRows()
        {
            // CHANGED: dùng DbClient.InTx
            return _db.InTx((cn, tx) =>
            {
                int n = 0;
                var phuongPhapXuat = CauHinhCuaHang.PhuongThucXuatKhoHienTai;
                foreach (DataRow row in _dataTable.Rows)
                {
                    if (row.RowState != DataRowState.Added) continue;

                    // Lấy ID_PHIEU_BAN từ hàng
                    string idPhieuBan = Convert.ToString(row["ID_PHIEU_BAN"]);

                    if (phuongPhapXuat == CauHinhCuaHang.PhuongThucXuatKho.FIFO)
                    {
                        XuatTheoFIFO(row, tx, idPhieuBan);
                    }
                    else if (phuongPhapXuat == CauHinhCuaHang.PhuongThucXuatKho.ChonLo)
                    {
                        XuatTheoChonLo(row, tx, idPhieuBan);
                    }


                    n++;
                }
                // (Ghi chú: Phương thức này không gọi AcceptChanges() trên `table`
                // vì `table` được truyền từ bên ngoài vào.)
                return n;
            }) > 0;
        }
    }
}