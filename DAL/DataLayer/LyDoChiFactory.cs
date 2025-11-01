using CuahangNongduoc.DAL.Infrastructure;
using CuahangNongduoc.DataLayer;
using System;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class LyDoChiFactory
    {
        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table; // giữ dữ liệu tạm để Add()/Save() kiểu cũ

        // Chỉ lấy các cột cần thiết. Nếu controller cũ cần SELECT * thì đổi lại SELECT *.
        private const string SELECT_ALL = "SELECT ID, LY_DO FROM LY_DO_CHI";

        /* ===================== Helpers ===================== */

        private void EnsureSchema()
        {
            if (_table != null) return;

            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("LY_DO_CHI");
                da.FillSchema(_table, SchemaType.Source); // tạo schema rỗng có cùng cấu trúc cột
            }
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn)
        {
            // Dùng CommandBuilder để tự sinh Insert/Update/Delete theo _table
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };

            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da);

            return da;
        }

        /* ===================== SELECT ===================== */

        // Lấy tất cả lý do chi
        public DataTable DanhsachLyDo()
        {
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL, CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable("LY_DO_CHI");
                da.Fill(dt);
                _table = dt; // đồng bộ _table để tiếp tục Add()/Save()
                return dt;
            }
        }

        // Lấy 1 lý do chi theo ID
        public DataTable LayLyDoChi(long id)
        {
            const string sql = "SELECT ID, LY_DO FROM LY_DO_CHI WHERE ID = @id";

            var dt = _db.ExecuteDataTable(
                sql,
                CommandType.Text,
                _db.P("@id", SqlDbType.BigInt, id)
            );

            _table = dt; // đồng bộ _table nếu cần tiếp tục chỉnh/sửa dòng đó
            return dt;
        }

        /* ===================== ROW FACTORY STYLE ===================== */

        public DataRow NewRow()
        {
            EnsureSchema();
            return _table.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema();
            _table.Rows.Add(row);
        }

        // Lưu tất cả thay đổi trong _table thông qua DataAdapter
        // (giống m_Ds.ExecuteNoneQuery() > 0 trong code Access/OleDb cũ)
        public bool Save()
        {
            EnsureSchema();

            using (var cn = _db.Open())
            using (var da = CreateAdapter(cn))
            {
                return da.Update(_table) > 0;
            }
        }

        /* ===================== CRUD THỦ CÔNG (Transaction tay) ===================== */

        // Nếu ID là IDENTITY (tự tăng), KHÔNG đưa ID vào INSERT.
        // Nếu ID KHÔNG tự tăng và bạn tự cấp ID, đổi câu sql như bên dưới:
        //
        // const string sql = "INSERT INTO LY_DO_CHI (ID, LY_DO) VALUES (@ID, @LY_DO)";
        // và nhớ Add tham số @ID.
        //
        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            const string sql =
                @"INSERT INTO LY_DO_CHI (LY_DO)
                  VALUES (@LY_DO)";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@LY_DO", SqlDbType.NVarChar, 255)
                    .Value = row["LY_DO"] ?? (object)DBNull.Value;

                return cmd.ExecuteNonQuery();
            }
        }

        public int Update(DataRow row, SqlTransaction tx = null)
        {
            const string sql =
                @"UPDATE LY_DO_CHI
                  SET LY_DO = @LY_DO
                  WHERE ID = @ID";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@LY_DO", SqlDbType.NVarChar, 255)
                    .Value = row["LY_DO"] ?? (object)DBNull.Value;

                cmd.Parameters.Add("@ID", SqlDbType.BigInt)
                    .Value = row["ID"];

                return cmd.ExecuteNonQuery();
            }
        }

        public int Delete(long id, SqlTransaction tx = null)
        {
            const string sql = @"DELETE FROM LY_DO_CHI WHERE ID = @ID";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = id;
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Chạy Insert/Update/Delete theo RowState trong cùng 1 transaction.
        /// Dùng khi bạn có 1 DataTable tạm và muốn commit tất cả thay đổi.
        /// </summary>
        public bool SaveChanges(DataTable table)
        {
            using (var cn = _db.Open())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    int totalAffected = 0;

                    foreach (DataRow row in table.Rows)
                    {
                        switch (row.RowState)
                        {
                            case DataRowState.Added:
                                totalAffected += Insert(row, tx);
                                break;

                            case DataRowState.Modified:
                                totalAffected += Update(row, tx);
                                break;

                            case DataRowState.Deleted:
                                // Khi row bị Deleted, phải lấy ID bản gốc
                                var originalId =
                                    Convert.ToInt64(row["ID", DataRowVersion.Original]);
                                totalAffected += Delete(originalId, tx);
                                break;
                        }
                    }

                    tx.Commit();
                    return totalAffected > 0;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}

