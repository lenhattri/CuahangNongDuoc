using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuahangNongduoc.Domain.Entities;
using System.Windows.Forms;
namespace CuahangNongduoc.DAL.DataLayer
{
    public class PhieuBanChiPhiFactory : IPhieuBanChiPhiFactory
    {
        private readonly DbClient _db = DbClient.Instance;
        private readonly string TABLE = "[dbo].[PHIEU_BAN_CHI_PHI]";

        public DataTable LayDanhSachTheoPhieuBan(string maPhieuBan)
        {
            string sql = "SELECT c.ID, c.TEN_CHI_PHI, c.LOAI_CHI_PHI, c.SO_TIEN " +
                "FROM CHI_PHI_PHAT_SINH c  " +
                "JOIN PHIEU_BAN_CHI_PHI p ON c.ID = p.MA_CHI_PHI " +
                "WHERE p.MA_PHIEU_BAN = @MaPhieuBan";
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@MaPhieuBan", SqlDbType.NVarChar, maPhieuBan, 50));
        }
        public void LuuChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis)
        {
            if (chiPhis == null || chiPhis.Count == 0)
                return;
            foreach (var cp in chiPhis)
            {
                var p = new PhieuBanChiPhi
                {
                    PhieuBan = new PhieuBan { Id = maPhieuBan },
                    ChiPhiPhatSinh = new ChiPhiPhatSinh { Id = cp.Id }
                };
                Insert(p);
            }
        }

        public void DeletedByPhieuBan(string maPhieuBan)
        {
            string sql = $@"DELETE FROM {TABLE} WHERE MA_PHIEU_BAN = @MaPhieuBan";
            _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@MaPhieuBan", SqlDbType.NVarChar, maPhieuBan, 50));
        }
        public int Insert(PhieuBanChiPhi phieuBanChiPhi)
        {
            string sql = $@"INSERT INTO {TABLE} (MA_PHIEU_BAN, MA_CHI_PHI)
                        VALUES (@MaPhieuBan, @MaChiPhi)";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@MaPhieuBan", SqlDbType.NVarChar, phieuBanChiPhi.PhieuBan.Id, 50),
                _db.P("@MaChiPhi", SqlDbType.NVarChar, phieuBanChiPhi.ChiPhiPhatSinh.Id, 50));
        }

    }
}
