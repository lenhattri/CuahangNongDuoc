using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Utils.Functions
{
    public static class SafeExecutor
    {
        // Dùng cho các hàm trả về giá trị
        public static T Execute<T>(Func<T> func, string methodName)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(methodName, ex);
                return default;
            }
        }

        // Dùng cho các hàm KHÔNG trả về giá trị (void)
        public static void Execute(Action action, string methodName)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(methodName, ex);
            }
        }
    }
}
