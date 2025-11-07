using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CuahangNongduoc.Utils.Functions
{
    /// <summary>
    /// Các loại quy tắc kiểm tra hợp lệ
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// Chuỗi không được null, rỗng hoặc chỉ có khoảng trắng
        /// </summary>
        NotEmpty,
        /// <summary>
        /// Đối tượng không được null (dùng cho khóa ngoại, v.v.)
        /// </summary>
        NotNull,
        /// <summary>
        /// Số (int) không được âm và phải là số hợp lệ
        /// </summary>
        NotNegativeInt,
        /// <summary>
        /// Số (long) không được âm và phải là số hợp lệ
        /// </summary>
        NotNegativeLong,
        /// <summary>
        /// Số không được bằng 0
        /// </summary>
        NotZero
    }

    /// <summary>
    /// Đại diện cho một quy tắc kiểm tra hợp lệ
    /// </summary>
    public class ValidationRule
    {
        public string ColumnName { get; }
        public ValidationType Type { get; }
        public string ErrorMessage { get; }

        public ValidationRule(string columnName, ValidationType type, string errorMessage)
        {
            ColumnName = columnName;
            Type = type;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Hàm helper chung để kiểm tra DataRow dựa trên một danh sách quy tắc.
        /// </summary>
        /// <returns>Trả về true nếu hợp lệ, false nếu có lỗi.</returns>
        public static bool ValidateRow(DataRow row, List<ValidationRule> rules)
        {
            bool hopLe = true;
            row.ClearErrors(); // Xóa lỗi cũ trước khi kiểm tra lại

            foreach (var rule in rules)
            {
                string colName = rule.ColumnName;
                object value = row[colName];
                bool isNull = row.IsNull(colName);

                switch (rule.Type)
                {
                    case ValidationType.NotEmpty:
                        if (isNull || string.IsNullOrWhiteSpace(value?.ToString()))
                        {
                            row.SetColumnError(colName, rule.ErrorMessage);
                            hopLe = false;
                        }
                        break;

                    case ValidationType.NotNull:
                        if (isNull)
                        {
                            row.SetColumnError(colName, rule.ErrorMessage);
                            hopLe = false;
                        }
                        break;

                    case ValidationType.NotZero:
                        if (isNull)
                        {
                            row.SetColumnError(colName, "Giá trị không được trống.");
                            hopLe = false;
                        }else
                        {
                            try
                            {
                                if (Convert.ToInt32(value) == 0)
                                {
                                    row.SetColumnError(colName, rule.ErrorMessage);
                                    hopLe = false;
                                }
                            }
                            catch (Exception)
                            {
                                row.SetColumnError(colName, "Giá trị phải lớn hơn không.");
                                hopLe = false;
                            }
                        }
                        break;

                    case ValidationType.NotNegativeInt:
                        if (isNull)
                        {
                            row.SetColumnError(colName, "Giá trị không được trống.");
                            hopLe = false;
                        }
                        else
                        {
                            try
                            {
                                if (Convert.ToInt32(value) < 0)
                                {
                                    row.SetColumnError(colName, rule.ErrorMessage);
                                    hopLe = false;
                                }
                            }
                            catch (Exception)
                            {
                                row.SetColumnError(colName, "Giá trị phải là số nguyên.");
                                hopLe = false;
                            }
                        }
                        break;

                    case ValidationType.NotNegativeLong:
                        if (isNull)
                        {
                            row.SetColumnError(colName, "Giá trị không được trống.");
                            hopLe = false;
                        }
                        else
                        {
                            try
                            {
                                if (Convert.ToInt64(value) < 0)
                                {
                                    row.SetColumnError(colName, rule.ErrorMessage);
                                    hopLe = false;
                                }
                            }
                            catch (Exception)
                            {
                                row.SetColumnError(colName, "Giá trị phải là số.");
                                hopLe = false;
                            }
                        }
                        break;
                }
            }
            return hopLe;
        }
    }
}
