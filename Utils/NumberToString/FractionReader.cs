using System;
using CuahangNongduoc.Utils.Interfaces;

namespace CuahangNongduoc.Utils.Helpers
{
    /// <summary>
    /// Đọc phần thập phân (VD: ".25" -> "hai năm")
    /// </summary>
    public class FractionReader : INumberReader
    {
        private static readonly string[] No =
            { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

        public string Convert(string num)
        {
            string result = "";
            foreach (char c in num)
            {
                if (char.IsDigit(c))
                    result += No[c - '0'] + " ";
            }
            return result.Trim();
        }
    }
}
