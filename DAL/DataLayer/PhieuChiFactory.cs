using CuahangNongduoc;
using CuahangNongduoc.DAL.Infrastructure;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuChiFactory
    {
        private readonly DbClient _db = DbClient.Instance;   // dùng DbClient chung
        private DataTable _table;                             // DataTable nội bộ cho NewRow/Add/Save
        private const string SELECT_ALL = "SELECT * FROM PHIEU_CHI";

        /* ========== Helpers ========== */

        private void EnsureSchema()
        {
            if (_table != null) return;

            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("PHIEU_CHI");
                da.FillSchema(_table, SchemaType.Source); // lấy schema rỗng
            }
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn)
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            // Tự sinh InsertCommand/UpdateCommand/DeleteCommand
            _ = new SqlCommandBuilder(da);

            return da;
        }

        /* ========== SELECT ========== */

        // Lấy danh sách phiếu chi theo lý do và ngày (theo ngày, dạng [start, end))
        public DataTable TimPhieuChi(int lydo, DateTime ngay)
        {
            var start = ngay.Date;
            var end = start.AddDays(1);

            const string sql = @"
                SELECT * 
                FROM PHIEU_CHI
                WHERE ID_LY_DO_CHI = @lydo
                  AND NGAY_CHI >= @start AND NGAY_CHI < @end";

            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@lydo", SqlDbType.Int, lydo),
                _db.P("@start", SqlDbType.DateTime, start),
                _db.P("@end", SqlDbType.DateTime, end));

            _table = dt;
            return dt;
        }

        // Lấy toàn bộ phiếu chi
        public DataTable DanhsachPhieuChi()
        {
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);
            _table = dt;
            return dt;
        }

        // Lấy phiếu chi theo ID
        public DataTable LayPhieuChi(string id)
        {
            const string sql = "SELECT * FROM PHIEU_CHI WHERE ID = @id";

            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.VarChar, id, 50));

            _table = dt;
            return dt;
        }

        // Tính tổng tiền theo lý do, tháng, năm
        public static long LayTongTien(string lydo, int thang, int nam)
        {
            var db = DbClient.Instance;
            const string sql = @"
                SELECT SUM(TONG_TIEN)
                FROM PHIEU_CHI
                WHERE ID_LY_DO_CHI = @lydo
                  AND MONTH(NGAY_CHI) = @thang
                  AND YEAR(NGAY_CHI)  = @nam";

            var obj = db.ExecuteScalar<object>(sql, CommandType.Text,
                db.P("@lydo", SqlDbType.VarChar, lydo, 50),
                db.P("@thang", SqlDbType.Int, thang),
                db.P("@nam", SqlDbType.Int, nam));

            return (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
        }

        /* ========== ROW FACTORY STYLE (giống code cũ) ========== */

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

        // Lưu _table bằng SqlDataAdapter + CommandBuilder
        public bool Save()
        {
            EnsureSchema();
            using (var cn = _db.Open())
            using (var da = CreateAdapter(cn))
            {
                return da.Update(_table) > 0;
            }
        }

        /* ========== CRUD thủ công theo từng dòng (nếu muốn gọi riêng lẻ) ========== */

        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            const string sql = @"
                INSERT INTO PHIEU_CHI
                    (ID, ID_LY_DO_CHI, NGAY_CHI, TONG_TIEN, GHI_CHU)
                VALUES
                    (@ID, @ID_LY_DO_CHI, @NGAY_CHI, @TONG_TIEN, @GHI_CHU)";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = row["ID"];
                cmd.Parameters.Add("@ID_LY_DO_CHI", SqlDbType.Int).Value = row["ID_LY_DO_CHI"];
                cmd.Parameters.Add("@NGAY_CHI", SqlDbType.DateTime).Value = row["NGAY_CHI"];
                cmd.Parameters.Add("@TONG_TIEN", SqlDbType.BigInt).Value = row["TONG_TIEN"];
                cmd.Parameters.Add("@GHI_CHU", SqlDbType.VarChar, 255).Value = row["GHI_CHU"] ?? (object)DBNull.Value;

                return cmd.ExecuteNonQuery();
            }
        }

        public int Update(DataRow row, SqlTransaction tx = null)
        {
            const string sql = @"
                UPDATE PHIEU_CHI
                SET ID_LY_DO_CHI = @ID_LY_DO_CHI,
                    NGAY_CHI     = @NGAY_CHI,
                    TONG_TIEN    = @TONG_TIEN,
                    GHI_CHU      = @GHI_CHU
                WHERE ID = @ID";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID_LY_DO_CHI", SqlDbType.Int).Value = row["ID_LY_DO_CHI"];
                cmd.Parameters.Add("@NGAY_CHI", SqlDbType.DateTime).Value = row["NGAY_CHI"];
                cmd.Parameters.Add("@TONG_TIEN", SqlDbType.BigInt).Value = row["TONG_TIEN"];
                cmd.Parameters.Add("@GHI_CHU", SqlDbType.VarChar, 255).Value = row["GHI_CHU"] ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = row["ID"];

                return cmd.ExecuteNonQuery();
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
        /// Batch apply changes in 1 transaction (Added / Modified / Deleted)
        /// </summary>
        public bool SaveChanges(DataTable table)
        {
            using (var conn = _db.Open())
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
                    throw;
                }
            }
        }
    }
}
