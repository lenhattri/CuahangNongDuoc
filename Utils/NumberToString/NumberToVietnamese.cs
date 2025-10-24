using System;
using CuahangNongduoc.Utils.Helpers;

namespace CuahangNongduoc.Utils.NumberToString
{
    /// <summary>
    /// Lớp chính chịu trách nhiệm điều phối quá trình chuyển số sang chữ tiếng Việt.
    /// </summary>
    public class NumberToVietnamese
    {
        private readonly IntegerReader _intReader = new IntegerReader();
        private readonly FractionReader _fracReader = new FractionReader();

        /// <summary>
        /// Chuyển số thành chữ tiếng Việt, bao gồm cả phần thập phân (nếu có).
        /// </summary>
        /// <param name="number">Số cần chuyển (chuỗi số có thể có dấu âm và dấu phẩy/thập phân).</param>
        /// <returns>Chuỗi biểu diễn số bằng chữ tiếng Việt, ví dụ: "Một trăm hai mươi ba phảy bốn năm".</returns>
        public string NumberToString(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return "Không";

            // Chuẩn hóa: bỏ khoảng trắng, thay dấu phẩy thành dấu chấm
            number = number.Trim().Replace(",", ".");

            // Kiểm tra âm
            bool isNegative = number.StartsWith("-");
            if (isNegative)
                number = number.Substring(1);

            // Tách phần nguyên và phần thập phân
            string[] parts = number.Split('.');
            string result = _intReader.Convert(parts[0]);

            // Nếu có phần thập phân, đọc thêm
            if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
                result += " phảy " + _fracReader.Convert(parts[1]);

            // Nếu là số âm, thêm chữ "Âm"
            if (isNegative)
                result = "Âm " + result;

            // Viết hoa chữ cái đầu tiên
            return char.ToUpper(result[0]) + result.Substring(1);
        }
    }
}
