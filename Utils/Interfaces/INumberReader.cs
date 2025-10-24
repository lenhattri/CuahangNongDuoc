using System;

namespace CuahangNongduoc.Utils.Interfaces
{
    /// <summary>
    /// Giao diện định nghĩa hành vi chuyển chuỗi số thành chữ
    /// </summary>
    public interface INumberReader
    {
        string Convert(string number);
    }
}
