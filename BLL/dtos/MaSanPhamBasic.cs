/// <summary>
/// DTO cơ bản cho tra cứu tồn tại nhanh (C# 7.3: class reference có thể trả về null).
/// </summary>
public sealed class MaSanPhamBasic
{
    public string Id { get; set; }
    public string IdSanPham { get; set; }
    public int SoLuong { get; set; }
}