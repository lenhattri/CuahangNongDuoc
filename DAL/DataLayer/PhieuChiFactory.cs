
﻿//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.OleDb;

//namespace CuahangNongduoc.DataLayer
//{
//    public class PhieuChiFactory
//    {
//        DataService m_Ds = new DataService();

//        public DataTable TimPhieuChi(int lydo, DateTime ngay)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_CHI WHERE ID_LY_DO_CHI = @lydo AND NGAY_CHI = @ngay");
//            cmd.Parameters.Add("lydo", OleDbType.Integer).Value = lydo;
//            cmd.Parameters.Add("ngay", OleDbType.Date).Value = ngay;

//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public DataTable DanhsachPhieuChi()
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_CHI ");
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public DataTable LayPhieuChi(String id)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_CHI WHERE ID = @id");
//            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = id;
//            m_Ds.Load(cmd);
//            return m_Ds;
//        }


//        public static long LayTongTien(String lydo, int thang, int nam)
//        {
//            DataService ds = new DataService();
//            OleDbCommand cmd = new OleDbCommand("SELECT SUM(TONG_TIEN) FROM PHIEU_CHI WHERE ID_LY_DO_CHI = @lydo AND MONTH(NGAY_CHI)=@thang AND YEAR(NGAY_CHI)= @nam");
//            cmd.Parameters.Add("lydo", OleDbType.VarChar, 50).Value = lydo;
//            cmd.Parameters.Add("thang", OleDbType.Integer).Value = thang;
//            cmd.Parameters.Add("nam", OleDbType.Integer).Value = nam;

//            object obj = ds.ExecuteScalar(cmd);

//            if (obj == null)
//                return 0;
//            else
//                return Convert.ToInt64(obj);
//        }

//        public DataRow NewRow()
//        {
//            return m_Ds.NewRow();
//        }
//        public void Add(DataRow row)
//        {
//            m_Ds.Rows.Add(row);
//        }
//       public bool Save()
//        {

//            return m_Ds.ExecuteNoneQuery() > 0;
//        }
//    }
//}
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

﻿// DAL/DataLayer/PhieuChiFactory.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure; 

namespace CuahangNongduoc.DataLayer
{
    public class PhieuChiFactory
    {

        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        /// <summary>
        /// Retrieves payment vouchers filtered by reason ID and date.
        /// </summary>
        /// <param name="lydo">The ID of the reason for payment.</param>
        /// <param name="ngay">The payment date.</param>
        /// <returns>A DataTable containing the matching records.</returns>
        public DataTable TimPhieuChi(int lydo, DateTime ngay)
        {
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM PHIEU_CHI WHERE ID_LY_DO_CHI = @lydo AND NGAY_CHI = @ngay", connection))
            {
                command.Parameters.Add(new SqlParameter("@lydo", SqlDbType.Int) { Value = lydo });
                command.Parameters.Add(new SqlParameter("@ngay", SqlDbType.Date) { Value = ngay });

                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable; 
        }

        
        private readonly DbClient _db = DbClient.Instance;   // CHANGED
        private DataTable _table;                            // NEW: DataTable nội bộ cho pattern NewRow/Add/Save

        private const string SELECT_ALL = "SELECT * FROM PHIEU_CHI"; // giữ nguyên SELECT * để CommandBuilder sinh CRUD

        /* ===================== Helpers ===================== */

        private void EnsureSchema() // NEW
        {
            if (_table != null) return;
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("PHIEU_CHI");
                da.FillSchema(_table, SchemaType.Source); // CHANGED: lấy schema rỗng rõ ràng
            }
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn) // NEW: phục vụ Save()
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da); // auto sinh Insert/Update/Delete
            return da;
        }

        /* ===================== SELECTs ===================== */

        /// <summary>
        /// Retrieves all payment vouchers.
        /// </summary>
        /// <returns>A DataTable containing all records.</returns>
        public DataTable DanhsachPhieuChi()
        {

            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM PHIEU_CHI", connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(dataTable);
            }

            return dataTable;
        }

        /// <summary>
        /// Retrieves a specific payment voucher by ID.
        /// </summary>
        /// <param name="id">The ID of the payment voucher.</param>
        /// <returns>A DataTable containing the matching record.</returns>
        public DataTable LayPhieuChi(string id)
        {
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM PHIEU_CHI WHERE ID = @id", connection))
            {
                using (var adapter = new SqlDataAdapter(command))
                {
                    command.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Calculates the total amount for a given reason, month, and year.
        /// </summary>
        /// <param name="lydo">The ID of the reason for payment (as string).</param>
        /// <param name="thang">The month (1-12).</param>
        /// <param name="nam">The year.</param>
        /// <returns>The total amount as long.</returns>
        public static long LayTongTien(string lydo, int thang, int nam)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("SELECT SUM(TONG_TIEN) FROM PHIEU_CHI WHERE ID_LY_DO_CHI = @lydo AND MONTH(NGAY_CHI)=@thang AND YEAR(NGAY_CHI)= @nam", connection))
            {
                command.Parameters.Add("@lydo", SqlDbType.VarChar, 50).Value = lydo;
                command.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
                command.Parameters.Add("@nam", SqlDbType.Int).Value = nam ;

                connection.Open();
                var result = command.ExecuteScalar();

                return result == null || result == DBNull.Value ? 0L : Convert.ToInt64(result);
            }
        }

        /// <summary>
        /// Creates a new DataRow for a payment voucher.
        /// </summary>
        /// <returns>A new DataRow with the schema of PHIEU_CHI table.</returns>
        public DataRow NewRow()
        {
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM PHIEU_CHI WHERE 1=0", connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                adapter.FillSchema(dataTable, SchemaType.Source);
            }

            return dataTable.NewRow();
        }

        /* ===================== INSERT/UPDATE/DELETE ===================== */
        // Gợi ý: dùng theo kiểu “row-based” cho nhanh; nếu team muốn DTO, mình viết thêm class DTO.

        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            // Giả định các cột đã tồn tại trong row
            const string sql =
                @"INSERT INTO PHIEU_CHI
                    (ID, ID_LY_DO_CHI, NGAY_CHI, TONG_TIEN, GHI_CHU)
                  VALUES(@ID, @ID_LY_DO_CHI, @NGAY_CHI, @TONG_TIEN, @GHI_CHU)";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = row["ID"];
                cmd.Parameters.Add("@ID_LY_DO_CHI", SqlDbType.Int).Value = row["ID_LY_DO_CHI"];
                cmd.Parameters.Add("@NGAY_CHI", SqlDbType.DateTime).Value = row["NGAY_CHI"];
                cmd.Parameters.Add("@TONG_TIEN", SqlDbType.BigInt).Value = row["TONG_TIEN"];
                cmd.Parameters.Add("@GHI_CHU", SqlDbType.VarChar, 255).Value = row["GHI_CHU"] ?? (object)DBNull.Value; // Assuming max length 255, adjust as needed
                return cmd.ExecuteNonQuery();
            }
        }
        public int Update(DataRow row, SqlTransaction tx = null)
        {
            // Giả định các cột đã tồn tại trong row
            const string sql =
                @"UPDATE PHIEU_CHI 
                  SET ID_LY_DO_CHI = @ID_LY_DO_CHI, NGAY_CHI = @NGAY_CHI, TONG_TIEN = @TONG_TIEN, GHI_CHU = @GHI_CHU
                  WHERE ID = @ID";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID_LY_DO_CHI", SqlDbType.VarChar, 50).Value = row["ID_LY_DO_CHI"];
                cmd.Parameters.Add("@NGAY_CHI", SqlDbType.DateTime).Value = row["NGAY_CHI"];
                cmd.Parameters.Add("@TONG_TIEN", SqlDbType.BigInt).Value = row["TONG_TIEN"];
                cmd.Parameters.Add("@GHI_CHU", SqlDbType.VarChar, 255).Value = row["GHI_CHU"] ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = row["ID"];
                return cmd.ExecuteNonQuery();
            }
        }

        public void Add(DataRow row)
        {
            EnsureSchema(); // CHANGED
            _table.Rows.Add(row);
        }

        public bool Save()
        {
            // thay m_Ds.ExecuteNoneQuery()
            EnsureSchema();
            using (var cn = _db.Open())
            using (var da = CreateAdapter(cn))
            {
                return da.Update(_table) > 0;

            }
        }

        public int Delete(string id, SqlTransaction tx = null)
        {
            const string sql = @"DELETE FROM PHIEU_CHI WHERE ID = @ID";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = id;
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
                                    var originalId = row["ID", DataRowVersion.Original].ToString();
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
            }
        }


    }
}