using CuahangNongduoc.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Utils
{
    public class Session
    {
        private static Session _instance;
        public static NguoiDungDTO CurrentUser { get; set; }
        public static Session Instant
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Session();
                }
                return _instance;
            }
        }
    }
}
