using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuahangNongduoc.Utils.Functions
{
    public static class DataTableHelper
    {
        private static readonly string _cs =
            ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        public static DataTable GetDataTable(string sql, SqlParameter[] parameters = null)
        {
            // Gọi SqlHelper thay vì viết lại
            return SqlHelper.ExecuteDataTable(sql, parameters);
        }

        // Trong file DataTableHelper.cs

        public static long SaveAndGetId(DataTable table, SqlConnection conn, SqlTransaction tran)
        {
            long newId = -1;
            var tableName = table.TableName;
            var rowToInsert = table.Rows[0];

            // --- Bắt đầu xây dựng lệnh INSERT thủ công ---

            var columns = new List<string>();
            var parameters = new List<string>();
            var sqlParameters = new List<SqlParameter>();

            foreach (DataColumn column in table.Columns)
            {
                // Bỏ qua các cột tự tăng (như cột ID)
                if (column.AutoIncrement)
                {
                    continue;
                }

                columns.Add($"[{column.ColumnName}]");
                parameters.Add($"@{column.ColumnName}");
                sqlParameters.Add(new SqlParameter($"@{column.ColumnName}", rowToInsert[column]));
            }

            // Tạo câu lệnh SQL hoàn chỉnh
            string commandText = $@"
                INSERT INTO [{tableName}] ({string.Join(", ", columns)})
                VALUES ({string.Join(", ", parameters)});
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            // --- Kết thúc xây dựng lệnh INSERT thủ công ---

            using (var cmd = new SqlCommand(commandText, conn, tran))
            {
                cmd.Parameters.AddRange(sqlParameters.ToArray());
                var result = cmd.ExecuteScalar(); // Thực thi và lấy về ID

                if (result != null && result != DBNull.Value)
                {
                    newId = Convert.ToInt64(result);

                    // Cập nhật lại ID vào DataRow gốc nếu cần
                    if (table.Columns.Contains("ID"))
                    {
                        rowToInsert["ID"] = newId;
                        table.AcceptChanges();
                    }
                }
            }

            return newId;
        }
        public static void Save(DataTable table, SqlConnection conn = null, SqlTransaction tran = null)
        {
            bool shouldClose = false;
            if (conn == null)
            {
                conn = new SqlConnection(_cs);
                conn.Open();
                shouldClose = true;
            }

            var tableName = table.TableName;

            using (var adapter = new SqlDataAdapter($"SELECT * FROM {tableName} WHERE 1=0", conn))
            {
                if (tran != null)
                    adapter.SelectCommand.Transaction = tran;

                var builder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = builder.GetInsertCommand();
                if (tran != null)
                    adapter.InsertCommand.Transaction = tran;

                var addedRows = table.GetChanges(DataRowState.Added);
                if (addedRows != null)
                {
                    adapter.Update(addedRows);
                    table.AcceptChanges();
                }
            }

            if (shouldClose)
                conn.Close();
        }

    }
}
