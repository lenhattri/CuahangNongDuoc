using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class PhieuChiReport
    {
        public string Id { get; set; }
        public DateTime NgayChi { get; set; }
        public long TongTien { get; set; }
        public string LyDoChi { get; set; }
        public string GhiChu { get; set; }
    }
}
