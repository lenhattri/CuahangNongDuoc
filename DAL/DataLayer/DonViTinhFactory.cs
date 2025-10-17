// DAL/DataLayer/DonViTinhDAL.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;

namespace CuahangNongduoc.DataLayer
{
    public class DonViTinhDAL
    {
        private readonly DbClient _db = DbClient.Instance; // CHANGED

        // SELECT * FROM DON_VI_TINH
        public DataTable DanhSachDVT()
        {
            const string sql = "SELECT * FROM DON_VI_TINH";
            return _db.ExecuteDataTable(sql, CommandType.Text); // CHANGED
        }

        // SELECT * FROM DON_VI_TINH WHERE ID = @id
        public DataTable LayDVT(int id)
        {
            const string sql = "SELECT * FROM DON_VI_TINH WHERE ID = @id";
            return _db.ExecuteDataTable(sql, CommandType.Text,       // CHANGED
                _db.P("@id", SqlDbType.Int, id));
        }

        /* ==== CRUD nhanh (tuỳ dùng) ==== */
        public int Insert(string ten, string ghiChu = null)
        {
            const string sql = @"
                INSERT INTO DON_VI_TINH (TEN, GHI_CHU)
                VALUES (@TEN, @GHI_CHU)";
            return _db.ExecuteNonQuery(sql, CommandType.Text,        // CHANGED
                _db.P("@TEN", SqlDbType.NVarChar, ten, 100),
                _db.P("@GHI_CHU", SqlDbType.NVarChar, (object)ghiChu ?? DBNull.Value, 255));
        }

        public int Update(int id, string ten, string ghiChu = null)
        {
            const string sql = @"
                UPDATE DON_VI_TINH
                   SET TEN = @TEN,
                       GHI_CHU = @GHI_CHU
                 WHERE ID = @ID";
            return _db.ExecuteNonQuery(sql, CommandType.Text,        // CHANGED
                _db.P("@TEN", SqlDbType.NVarChar, ten, 100),
                _db.P("@GHI_CHU", SqlDbType.NVarChar, (object)ghiChu ?? DBNull.Value, 255),
                _db.P("@ID", SqlDbType.Int, id));
        }

        public int Delete(int id)
        {
            const string sql = "DELETE FROM DON_VI_TINH WHERE ID = @ID";
            return _db.ExecuteNonQuery(sql, CommandType.Text,        // CHANGED
                _db.P("@ID", SqlDbType.Int, id));
        }

        public bool Save(DataTable table)
        {
            if (table == null) return false;

            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, "SELECT * FROM DON_VI_TINH", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            using (var cb = new SqlCommandBuilder(da))
            {
                da.MissingSchemaAction = MissingSchemaAction.AddWithKey; // NEW
                return da.Update(table) > 0;
            }
        }
    }
}
