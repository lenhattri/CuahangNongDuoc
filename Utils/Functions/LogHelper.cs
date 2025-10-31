using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Utils.Functions
{
    public static class LogHelper
    {
        public static void LogError(string methodName, Exception ex)
        {
            // Here you can implement your logging logic, e.g., log to a file, database, or external service.
            // For demonstration purposes, we'll just write to the console.
            System.Diagnostics.Debug.WriteLine($"Error in {methodName}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }

        public static void LogInfo(string message)
        {
            // Implement your info logging logic here.
            System.Diagnostics.Debug.WriteLine($"Info: {message}");
        }
        
        public static void LogWarning(string message) {
            // Implement your warning logging logic here.
            System.Diagnostics.Debug.WriteLine($"Warning: {message}");
        }
    }
}
