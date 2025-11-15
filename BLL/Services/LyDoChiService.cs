using System;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class LyDoChiService : ILyDoChiService
    {
        private readonly ILyDoChiFactory _dal;

        public LyDoChiService(ILyDoChiFactory dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public DataTable DanhSachLyDo() => _dal.DanhsachLyDo();

        public LyDoChi LayLyDo(long id)
        {
            var table = _dal.LayLyDoChi(id);
            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
            return new LyDoChi
            {
                Id = Convert.ToInt64(row["ID"]),
                LyDo = Convert.ToString(row["LY_DO"])
            };
        }

        public DataRow TaoDongMoi() => _dal.NewRow();

        public void Them(DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            _dal.Add(row);
        }

        public bool Luu() => _dal.Save();
    }
}
