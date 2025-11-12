using CuahangNongduoc.DAL.DataLayer;
using CuahangNongduoc.Domain.Entities;
using CuahangNongduoc.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc.BLL.Controller
{
    public class UserController
    {
        private readonly IUserDAL _userDAL;

        // ✅ Inject IUserDAL qua constructor
        public UserController(IUserDAL userDAL)
        {
            _userDAL = userDAL ?? throw new ArgumentNullException(nameof(userDAL));
        }

        public void HienthiNguoiDungDataGridview(DataGridView dgv, BindingNavigator bn)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = _userDAL.LayDanhSachNguoiDung();
            bn.BindingSource = bs;
            dgv.DataSource = bs;
        }

        public DataRow LayNguoiDungTheoId(string id)
        {
            return _userDAL.LayNguoiDungTheoId(id);
        }

        public NguoiDungDTO LayNguoiDungTheoTenDangNhap(string tenDangNhap)
        {
            DataRow row = _userDAL.LayNguoiDungTheoTenDangNhap(tenDangNhap);
            if (row != null)
            {
                return new NguoiDungDTO
                {
                    Id = Convert.ToInt64(row["ID"]),
                    HoTen = Convert.ToString(row["HO_TEN"]),
                    DiaChi = Convert.ToString(row["DIA_CHI"]),
                    DienThoai = Convert.ToString(row["DIEN_THOAI"]),
                    TenDangNhap = Convert.ToString(row["TEN_DANG_NHAP"]),
                    Quyen = Convert.ToString(row["QUYEN"])
                };
            }
            return null;
        }

        public bool KiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            DataRow row = _userDAL.LayNguoiDungTheoTenDangNhap(tenDangNhap);
            if (row != null)
            {
                string matKhauHash = Convert.ToString(row["MAT_KHAU"]);
                return BCrypt.Net.BCrypt.Verify(matKhau, matKhauHash);
            }
            return false;
        }

        public DataRow NewRow()
        {
            return _userDAL.NewRow();
        }

        public void Add(DataRow row)
        {
            if (row != null)
            {
                row["MAT_KHAU"] = BCrypt.Net.BCrypt.HashPassword(Convert.ToString(row["MAT_KHAU"]));
                _userDAL.Add(row);
            }
            else
            {
                throw new ArgumentNullException(nameof(row), "DataRow cannot be null");
            }
        }

        public void Update(long id)
        {
            _userDAL.Update(id);
        }

        public bool Save()
        {
            return _userDAL.Save();
        }

        internal void Delete(long id)
        {
            _userDAL.Delete(id);
        }
    }
}
