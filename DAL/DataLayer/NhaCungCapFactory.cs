// DAL/DataLayer/NhaCungCapFactory.cs
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;

namespace CuahangNongduoc.DataLayer
{
    public class NhaCungCapDAL
    {
        private readonly DbClient _db = DbClient.Instance;            
        private const string TABLE = "[dbo].[NHA_CUNG_CAP]";          

        // Lấy toàn bộ danh sách NCC
        public DataTable DanhsachNCC()
        {
            string sql = $"SELECT ID, HO_TEN, DIEN_THOAI, DIA_CHI FROM {TABLE}"; // CHANGED
            return _db.ExecuteDataTable(sql, CommandType.Text);
        }

        // Lấy NCC theo ID
        public DataTable LayNCC(string id)
        {
            string sql = $"SELECT ID, HO_TEN, DIEN_THOAI, DIA_CHI FROM {TABLE} WHERE ID = @Id"; // CHANGED
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@Id", SqlDbType.NVarChar, id, 50));                                       // CHANGED
        }

        // Tìm theo địa chỉ (LIKE)
        public DataTable TimDiaChi(string diachi)
        {
            string sql = $"SELECT ID, HO_TEN, DIEN_THOAI, DIA_CHI FROM {TABLE} WHERE DIA_CHI LIKE @DiaChi"; // CHANGED
            var pattern = $"%{diachi ?? string.Empty}%";                                                     // NEW
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@DiaChi", SqlDbType.NVarChar, pattern, 255));                                        // CHANGED
        }

        // Tìm theo họ tên (LIKE)
        public DataTable TimHoTen(string hoten)
        {
            string sql = $"SELECT ID, HO_TEN, DIEN_THOAI, DIA_CHI FROM {TABLE} WHERE HO_TEN LIKE @HoTen"; // CHANGED
            var pattern = $"%{hoten ?? string.Empty}%";                                                   // NEW
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@HoTen", SqlDbType.NVarChar, pattern, 200));                                      // CHANGED
        }

        // Thêm mới
        public int Insert(BusinessObject.NhaCungCap ncc)
        {
            string sql = $@"
                INSERT INTO {TABLE}(ID, HO_TEN, DIEN_THOAI, DIA_CHI)     -- CHANGED
                VALUES (@Id, @HoTen, @DienThoai, @DiaChi)";
            return _db.ExecuteNonQuery(sql, CommandType.Text,             // CHANGED
                _db.P("@Id", SqlDbType.NVarChar, ncc.Id, 50),
                _db.P("@HoTen", SqlDbType.NVarChar, ncc.HoTen, 200),
                _db.P("@DienThoai", SqlDbType.NVarChar, ncc.DienThoai, 50),
                _db.P("@DiaChi", SqlDbType.NVarChar, ncc.DiaChi, 255));
        }

        // Cập nhật
        public int Update(BusinessObject.NhaCungCap ncc)
        {
            string sql = $@"
                UPDATE {TABLE}                                         -- CHANGED
                   SET HO_TEN=@HoTen, DIEN_THOAI=@DienThoai, DIA_CHI=@DiaChi
                 WHERE ID=@Id";
            return _db.ExecuteNonQuery(sql, CommandType.Text,           // CHANGED
                _db.P("@HoTen", SqlDbType.NVarChar, ncc.HoTen, 200),
                _db.P("@DienThoai", SqlDbType.NVarChar, ncc.DienThoai, 50),
                _db.P("@DiaChi", SqlDbType.NVarChar, ncc.DiaChi, 255),
                _db.P("@Id", SqlDbType.NVarChar, ncc.Id, 50));
        }

        // Xóa
        public int Delete(string id)
        {
            string sql = $"DELETE FROM {TABLE} WHERE ID=@Id";           // CHANGED
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@Id", SqlDbType.NVarChar, id, 50));
        }
    }
}
