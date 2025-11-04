
﻿//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.OleDb;

﻿// DAL/DataLayer/LyDoChiFactory.cs
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;


//namespace CuahangNongduoc.DataLayer
//{
//    public class LyDoChiFactory
//    {
//        DataService m_Ds = new DataService();

//        public DataTable DanhsachLyDo()
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM LY_DO_CHI");
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public DataTable LayLyDoChi(long id)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM LY_DO_CHI WHERE ID = " + id);
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public bool Save()
//        {
//            return m_Ds.ExecuteNoneQuery() > 0;
//        }
//    }
//}
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class LyDoChiFactory
    {

        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        /// <summary>
        /// Retrieves all reasons for expenditure.
        /// </summary>
        /// <returns>A DataTable containing all records.</returns>
        public DataTable DanhsachLyDo()
        {
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM LY_DO_CHI", connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(dataTable);
            }

            return dataTable;
        }

        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table;  // DataTable nội bộ cho pattern Save()

        private const string SELECT_ALL = "SELECT ID, LY_DO FROM LY_DO_CHI";

        /* Helpers */
        private void EnsureSchema()
        {
            if (_table != null) return;
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("LY_DO_CHI");
                da.FillSchema(_table, SchemaType.Source);
            }
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn)
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da);
            return da;
        }

        /* API */

        /// <summary>
        /// Retrieves a specific reason for expenditure by ID.
        /// </summary>
        /// <param name="id">The ID of the reason.</param>
        /// <returns>A DataTable containing the matching record.</returns>
        public DataTable LayLyDoChi(long id)
        {

            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM LY_DO_CHI WHERE ID = @id", connection))
            {
                command.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt) { Value = id });

                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;

            const string sql = "SELECT ID, LY_DO FROM LY_DO_CHI WHERE ID = @id";
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, sql, CommandType.Text, null, 30,
                       _db.P("@id", SqlDbType.BigInt, id)))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable("LY_DO_CHI");
                da.Fill(dt);
                _table = dt;
                return dt;
            }

        }

        /// <summary>
        /// Creates a new DataRow for a reason for expenditure.
        /// </summary>
        /// <returns>A new DataRow with the schema of LY_DO_CHI table.</returns>
        public DataRow NewRow()
        {

            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM LY_DO_CHI WHERE 1=0", connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                adapter.FillSchema(dataTable, SchemaType.Source);
            }

            return dataTable.NewRow();
        }

        /// <summary>
        /// Adds a DataRow to a DataTable.
        /// </summary>
        /// <param name="dataTable">The DataTable to add the row to.</param>
        /// <param name="row">The DataRow to add.</param>
        public void Add(DataTable dataTable, DataRow row)
        {
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
            if (row == null) throw new ArgumentNullException(nameof(row));

            dataTable.Rows.Add(row);
        }

        /* ===================== INSERT/UPDATE/DELETE ===================== */
        // Gợi ý: dùng theo kiểu “row-based” cho nhanh; nếu team muốn DTO, mình viết thêm class DTO.

        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            // Giả định các cột đã tồn tại trong row (ID, TEN, etc.)
            const string sql = @"INSERT INTO LY_DO_CHI (LY_DO) VALUES(@LY_DO)";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = row["ID"];
                cmd.Parameters.Add("@LY_DO", SqlDbType.VarChar, 255).Value = row["LY_DO"] ?? (object)DBNull.Value; // Assuming max length 255, adjust as needed
                return cmd.ExecuteNonQuery();
            }
        }
        public int Update(DataRow row, SqlTransaction tx = null)
        {
            // Giả định các cột đã tồn tại trong row
            const string sql = @"UPDATE LY_DO_CHI SET LY_DO = @LY_DO WHERE ID = @ID";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@LY_DO", SqlDbType.VarChar, 255).Value = row["LY_DO"] ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = row["ID"];
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
        /// Thay cho adapter.Update(): duyệt bảng, thêm records trong 1 transaction.
        /// Chỉ xử lý các row trạng thái Added (giống code cũ).
        /// </summary>
        public bool SaveChanges(DataTable table)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
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
                                    var originalId = Convert.ToInt64(row["ID", DataRowVersion.Original]);
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
                        throw; // cho BLL bắt và báo lỗi UI
                    }
                }

                EnsureSchema();
                using (var cn = _db.Open())
                using (var da = CreateAdapter(cn))
                {
                    return da.Update(_table) > 0;

                }
            }

        }
    }
}