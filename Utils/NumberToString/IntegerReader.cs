using System;
using CuahangNongduoc.Utils.Interfaces;

namespace CuahangNongduoc.Utils.Helpers
{
    public class IntegerReader : INumberReader
    {
        private readonly NumberGroupReader groupReader = new NumberGroupReader();

        private static readonly string[] Cap =
        { "", " nghìn ", " triệu ", " tỷ ", " nghìn tỷ ", " triệu tỷ ", " tỷ tỷ ", " nghìn tỷ tỷ " };

        public string Convert(string num)
        {
            if (string.IsNullOrEmpty(num)) return "không";

            string result = "";
            string str = num;
            int cap = 0;

            while (str.Length > 3)
            {
                // Lấy 3 chữ số cuối cùng (ví dụ: "123456" → "456")
                string group = str.Substring(str.Length - 3, 3);
                // Cắt bỏ 3 chữ số cuối cùng
                str = str.Substring(0, str.Length - 3);

                if (group != "000")
                    result = groupReader.Convert(group) + Cap[cap] + result;
                cap++;
            }

            // Thêm số 0 vào đầu nếu chưa đủ 3 chữ số
            while (str.Length < 3)
                str = "0" + str;

            string head = (str == "000" && num.Length <= 3)
                ? "không"
                : groupReader.Convert(str) + Cap[cap];

            return (head + result).Trim();
        }
    }
}
