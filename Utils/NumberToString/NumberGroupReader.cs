using System;
using CuahangNongduoc.Utils.Interfaces;

namespace CuahangNongduoc.Utils.Helpers
{
    /// <summary>
    /// Đọc nhóm 3 chữ số (VD: "305" -> "ba trăm linh năm")
    /// </summary>
    public class NumberGroupReader : INumberReader
    {
        private static readonly string[] No =
            { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

        public string Convert(string num)
        {
            if (num.Length != 3)
                throw new ArgumentException("Nhóm số phải gồm 3 chữ số");

            string tram = num[0] == '0' ? "" : No[num[0] - '0'] + " trăm ";
            string chuc, donvi;

            // Chục
            switch (num[1])
            {
                case '0': chuc = num[2] != '0' && num[0] != '0' ? "linh " : ""; break;
                case '1': chuc = "mười "; break;
                default: chuc = No[num[1] - '0'] + " mươi "; break;
            }

            // Đơn vị
            switch (num[2])
            {
                case '0': donvi = ""; break;
                case '1': donvi = (num[1] == '0' || num[1] == '1') ? "một" : "mốt"; break;
                case '5': donvi = num[1] != '0' ? "lăm" : "năm"; break;
                default: donvi = No[num[2] - '0']; break;
            }

            return (tram + chuc + donvi).Trim();
        }
    }
}
