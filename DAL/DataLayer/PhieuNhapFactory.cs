using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;
namespace CuahangNongduoc.DataLayer
{
    public class PhieuNhapFactory
    {
        private readonly DbClient _db = DbClient.Instance;
        private readonly DataTable _buffer;


        public PhieuNhapFactory()
        {
            _buffer = CreateSchema();
        }


        //tương thích với bảng PHIEU_NHAP 
        private static DataTable CreateSchema()
        {
            var dt = new DataTable("PHIEU_NHAP");
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("ID_NHA_CUNG_CAP", typeof(string));
            dt.Columns.Add("NGAY_NHAP", typeof(DateTime));
            dt.Columns.Add("TONG_TIEN", typeof(decimal));
            dt.Columns.Add("DA_TRA", typeof(decimal));
            dt.Columns.Add("CON_NO", typeof(decimal));
            dt.PrimaryKey = new[] { dt.Columns["ID"] };
            return dt;
        }
        public DataRow NewRow() => _buffer.NewRow();


        public void Add(DataRow row)
        {
            if (row.RowState == DataRowState.Detached)
            {
                _buffer.Rows.Add(row);
                return;
            }
            if (!ReferenceEquals(row.Table, _buffer))
            {
                var clone = _buffer.NewRow();
                foreach (DataColumn c in _buffer.Columns)
                    clone[c.ColumnName] = row.Table.Columns.Contains(c.ColumnName) ? row[c.ColumnName] : DBNull.Value;
                _buffer.Rows.Add(clone);
            }
        }


        public DataTable DanhsachPhieuNhap()
        {
            var dt = _db.ExecuteDataTable("SELECT ID, ID_NHA_CUNG_CAP, NGAY_NHAP, TONG_TIEN, DA_TRA, CON_NO FROM PHIEU_NHAP");
            _buffer.Clear();
            foreach (DataRow r in dt.Rows) _buffer.ImportRow(r);
            _buffer.AcceptChanges();
            return _buffer;
        }
        public DataTable LayPhieuNhap(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == "-1")
                return _buffer.Clone(); // Trả về schema rỗng cho bind ban đầu


            return _db.ExecuteDataTable(
            "SELECT ID, ID_NHA_CUNG_CAP, NGAY_NHAP, TONG_TIEN, DA_TRA, CON_NO FROM PHIEU_NHAP WHERE ID=@ID",
            CommandType.Text,
            _db.P("@ID", SqlDbType.NVarChar, id, 50));
        }


        public DataTable TimPhieuNhap(string maNCC, DateTime ngay)
        {
            return _db.ExecuteDataTable(@"SELECT ID, ID_NHA_CUNG_CAP, NGAY_NHAP, TONG_TIEN, DA_TRA, CON_NO
                FROM PHIEU_NHAP
                WHERE (@NCC IS NULL OR ID_NHA_CUNG_CAP=@NCC)
                AND CAST(NGAY_NHAP AS DATE)=@NGAY",
            CommandType.Text,
            _db.P("@NCC", SqlDbType.NVarChar, string.IsNullOrWhiteSpace(maNCC) ? (object)DBNull.Value : maNCC, 50),
            _db.P("@NGAY", SqlDbType.Date, ngay.Date));
        }

        public bool Save()
        {
            // Lưu thay đổi trong _buffer theo RowState trong 1 transaction
            return _db.InTx((cn, tx) =>
            {
                int affected = 0;
                foreach (DataRow r in _buffer.Rows)
                {
                    if (r.RowState == DataRowState.Added)
                    {
                        using (var cmd = _db.Cmd(cn, @"INSERT INTO PHIEU_NHAP(ID, ID_NHA_CUNG_CAP, NGAY_NHAP, TONG_TIEN, DA_TRA, CON_NO)
                            VALUES(@ID,@NCC,@NGAY,@TONG,@DTRA,@CNO)",
                            CommandType.Text, tx, 30,
                        _db.P("@ID", SqlDbType.NVarChar, r["ID"], 50),
                        _db.P("@NCC", SqlDbType.NVarChar, r["ID_NHA_CUNG_CAP"], 50),
                        _db.P("@NGAY", SqlDbType.DateTime, r["NGAY_NHAP"]),
                        _db.PDec("@TONG", r["TONG_TIEN"]),
                        _db.PDec("@DTRA", r["DA_TRA"]),
                        _db.PDec("@CNO", r["CON_NO"])))
                        { affected += cmd.ExecuteNonQuery(); }
                    }
                    else if (r.RowState == DataRowState.Modified)
                    {
                        using (var cmd = _db.Cmd(cn, @"UPDATE PHIEU_NHAP SET ID_NHA_CUNG_CAP=@NCC, NGAY_NHAP=@NGAY,
                            TONG_TIEN=@TONG, DA_TRA=@DTRA, CON_NO=@CNO WHERE ID=@ID",
                        CommandType.Text, tx, 30,
                        _db.P("@NCC", SqlDbType.NVarChar, r["ID_NHA_CUNG_CAP"], 50),
                        _db.P("@NGAY", SqlDbType.DateTime, r["NGAY_NHAP"]),
                        _db.PDec("@TONG", r["TONG_TIEN"]),
                        _db.PDec("@DTRA", r["DA_TRA"]),
                        _db.PDec("@CNO", r["CON_NO"]),
                        _db.P("@ID", SqlDbType.NVarChar, r["ID"], 50)))
                        { affected += cmd.ExecuteNonQuery(); }
                    }
                    else if (r.RowState == DataRowState.Deleted)
                    {
                        using (var cmd = _db.Cmd(cn, "DELETE FROM PHIEU_NHAP WHERE ID=@ID", CommandType.Text, tx, 30,
                        _db.P("@ID", SqlDbType.NVarChar, r["ID", DataRowVersion.Original], 50)))
                        { affected += cmd.ExecuteNonQuery(); }
                    }
                }
                _buffer.AcceptChanges();
                return affected;
            }) > 0;
        }

    }
    }
    
