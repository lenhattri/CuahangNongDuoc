using CuahangNongduoc.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Utils.Functions
{
    /// <summary>
    /// Lớp helper tĩnh chứa các logic DAL dùng chung
    /// </summary>
    public static class DataAccessHelper
    {
        /// <summary>
        /// Logic Save chung cho các Factory dùng pattern DataTable.
        /// </summary>
        /// <param name="table">DataTable nội bộ chứa các thay đổi.</param>
        /// <param name="rules">Danh sách quy tắc validation.</param>
        /// <param name="createAdapterFunc">Một hàm (delegate) trỏ đến phương thức CreateAdapter() của Factory.</param>
        /// <param name="db">Instance của DbClient.</param>
        /// <returns>True nếu lưu thành công, False nếu thất bại.</returns>
        public static bool PerformSave(
            DataTable table,
            List<ValidationRule> rules,
            Func<SqlConnection, SqlDataAdapter> createAdapterFunc,
            DbClient db)
        {
            // 1. Kiểm tra sơ bộ
            if (table.GetChanges() == null)
            {
                return true; // Không có gì để lưu
            }

            // 2. Duyệt và kiểm tra
            bool allRowsValid = true;
            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                {
                    // Giả định ValidationRule.ValidateRow là một phương thức tĩnh
                    if (!ValidationRule.ValidateRow(row, rules))
                    {
                        allRowsValid = false;
                        // Vẫn tiếp tục duyệt để tìm hết lỗi
                    }
                }
            }

            // 3. Nếu có lỗi, không lưu
            if (!allRowsValid)
            {
                return false;
            }

            // 4. Không có lỗi, tiến hành lưu
            using (var cn = db.Open())
            using (var da = createAdapterFunc(cn)) // Gọi hàm delegate được truyền vào
            {
                try
                {
                    int rowsAffected = da.Update(table);
                    table.AcceptChanges();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    //table.RowError = "Lỗi CSDL: " + ex.Message;
                    return false;
                }
            }
        }
    }
}
