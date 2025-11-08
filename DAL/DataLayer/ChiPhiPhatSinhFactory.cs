using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;

namespace CuahangNongduoc.DataLayer
{
    public class ChiPhiPhatSinhFactory : IChiPhiPhatSinhFactory
    {
        private readonly DbClient _db = DbClient.Instance;
        private const string TABLE = "[dbo].[CHI_PHI_PHAT_SINH]";
        public DataTable DanhSachChiPhiPhatSinh()
        {
            string sql = $"SELECT ID, TEN_CHI_PHI, LOAI_CHI_PHI, SO_TIEN FROM {TABLE}";
            return _db.ExecuteDataTable(sql, CommandType.Text);
        }
        public int InSert(BusinessObject.ChiPhiPhatSinh chiPhi)
        {
            string sql = $@"INSERT INTO {TABLE} (ID,TEN_CHI_PHI, LOAI_CHI_PHI, SO_TIEN)
                        VALUES (@Id, @TenChiPhi, @LoaiChiPhi, @SoTien)";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@Id", SqlDbType.NVarChar, chiPhi.Id, 50),
                _db.P("@TenChiPhi", SqlDbType.NVarChar, chiPhi.TenChiPhi, 200),
                _db.P("@LoaiChiPhi", SqlDbType.NVarChar, chiPhi.LoaiChiPhi, 50),
                _db.P("@SoTien", SqlDbType.Int, chiPhi.SoTien));
        }
        public int Update(BusinessObject.ChiPhiPhatSinh chiPhi)
        {
            string sql = $@"UPDATE {TABLE} 
                        SET TEN_CHI_PHI = @TenChiPhi, LOAI_CHI_PHI = @LoaiChiPhi, SO_TIEN = @SoTien
                        WHERE ID = @Id";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@Id", SqlDbType.NVarChar, chiPhi.Id, 50),
                _db.P("@TenChiPhi", SqlDbType.NVarChar, chiPhi.TenChiPhi, 200),
                _db.P("@LoaiChiPhi", SqlDbType.NVarChar, chiPhi.LoaiChiPhi, 50),
                _db.P("@SoTien", SqlDbType.Int, chiPhi.SoTien));
        }
        public int Delete(string id)
        {
            string sql = $"DELETE FROM {TABLE} WHERE ID = @Id";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@Id", SqlDbType.NVarChar, id, 50));
        }
    }
}
