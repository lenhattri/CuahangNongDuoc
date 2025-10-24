using System;
using System.Text.RegularExpressions;

namespace CuahangNongduoc.Utils.Helpers
{
    /// <summary>
    /// Chuẩn hóa chuỗi số: thêm dấu phẩy ngăn cách hàng nghìn, loại bỏ ký tự không hợp lệ
    /// </summary>
    public class NumberNormalizer
    {
        public string Normalize(string num)
        {
            if (string.IsNullOrWhiteSpace(num))
                return "0";

            // Loại bỏ ký tự không hợp lệ (chỉ giữ số, dấu . và -)
            num = num.Trim();
            num = Regex.Replace(num, @"[^0-9\.\-]", "");

            // Xử lý dấu âm
            bool isNegative = num.StartsWith("-");
            if (isNegative)
                num = num.Substring(1);

            // Tách phần nguyên và phần thập phân
            string[] parts = num.Split('.');
            string integerPart = parts[0];
            string result = "";

            // Thêm dấu phẩy ngăn cách hàng nghìn
            while (integerPart.Length > 3)
            {
                string group = integerPart.Substring(integerPart.Length - 3);
                integerPart = integerPart.Substring(0, integerPart.Length - 3);
                result = "," + group + result;
            }

            result = integerPart + result;

            // Thêm phần thập phân nếu có
            if (parts.Length == 2)
                result += "." + parts[1];

            // Thêm lại dấu âm nếu có
            if (isNegative)
                result = "-" + result;

            return result;
        }
    }
}
