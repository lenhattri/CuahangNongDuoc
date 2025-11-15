using System.Data;

namespace CuahangNongduoc.Utils.Mapping
{
    public interface IDataRowMapper<out T>
    {
        T Map(DataRow row);
    }
}
