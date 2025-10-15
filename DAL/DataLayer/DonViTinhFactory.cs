// Path: DataLayer/DonViTinhDAL.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;

namespace CuahangNongduoc.DataLayer
{
    public class DonViTinhDAL
    {
        private readonly DbClient _db = DbClient.Instance;

        // SELECT ID, TEN, GHI_CHU FROM DON_VI_TINH
        public DataTable DanhSachDVT()
        {
            const string sql = @"SELECT ID, TEN, GHI_CHU FROM DON_VI_TINH ORDER BY ID DESC";
            return _db.ExecuteDataTable(sql, CommandType.Text);
        }

        // SELECT ... WHERE ID = @id
        public DataTable LayDVT(int id)
        {
            const string sql = @"SELECT ID, TEN, GHI_CHU FROM DON_VI_TINH WHERE ID = @ID";
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@ID", SqlDbType.Int, id));
        }

        /* ============ CRUD ============ */

        // Trả về số dòng ảnh hưởng (hoặc dùng ExecuteScalar<int> để lấy ID mới)
        public int Insert(string ten, string ghiChu = null)
        {
            const string sql = @"INSERT INTO DON_VI_TINH(TEN, GHI_CHU)
                                 VALUES(@TEN, @GHI_CHU)";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@TEN", SqlDbType.NVarChar, ten, 100),
                _db.P("@GHI_CHU", SqlDbType.NVarChar, (object)ghiChu ?? DBNull.Value, 255));
        }

        // Nếu muốn trả về ID mới tạo:
        public int InsertReturnId(string ten, string ghiChu = null)
        {
            const string sql = @"INSERT INTO DON_VI_TINH(TEN, GHI_CHU)
                                 VALUES(@TEN, @GHI_CHU);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return _db.ExecuteScalar<int>(sql, CommandType.Text,
                _db.P("@TEN", SqlDbType.NVarChar, ten, 100),
                _db.P("@GHI_CHU", SqlDbType.NVarChar, (object)ghiChu ?? DBNull.Value, 255));
        }

        public int Update(int id, string ten, string ghiChu = null)
        {
            const string sql = @"UPDATE DON_VI_TINH
                                 SET TEN = @TEN, GHI_CHU = @GHI_CHU
                                 WHERE ID = @ID";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@TEN", SqlDbType.NVarChar, ten, 100),
                _db.P("@GHI_CHU", SqlDbType.NVarChar, (object)ghiChu ?? DBNull.Value, 255),
                _db.P("@ID", SqlDbType.Int, id));
        }

        public int Delete(int id)
        {
            const string sql = @"DELETE FROM DON_VI_TINH WHERE ID = @ID";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@ID", SqlDbType.Int, id));
        }

        //Save(DataTable) tự sinh Insert/Update/Delete.
        
        public bool Save(DataTable table)
        {
            using (var cn = _db.Open())
            using (var da = new SqlDataAdapter())
            {
                // SelectCommand tối thiểu phải có cột khóa (ID) để CommandBuilder sinh lệnh
                da.SelectCommand = _db.Cmd(cn, "SELECT ID, TEN, GHI_CHU FROM DON_VI_TINH", CommandType.Text);

                using (var cb = new SqlCommandBuilder(da))
                {
                   
                    // da.InsertCommand = cb.GetInsertCommand();
                    // da.UpdateCommand = cb.GetUpdateCommand();
                    // da.DeleteCommand = cb.GetDeleteCommand();

                    return da.Update(table) > 0;
                }
            }
        }
    }
}
