namespace QuanLyThuVien.DTO;

public class TaiKhoanDTO
{
    public string TenDangNhap { get; set; } = "";
    public string HoTen { get; set; } = "";
    public string VaiTro { get; set; } = "";
    public bool IsAdmin => VaiTro == "Admin";
}

public class SachDTO
{
    public int MaSach { get; set; }
    public string TenSach { get; set; } = "";
    public string TacGia { get; set; } = "";
    public string NhaXuatBan { get; set; } = "";
    public int NamXB { get; set; } = DateTime.Now.Year;
    public int MaTL { get; set; }
    public int SoLuong { get; set; }
    public decimal GiaBia { get; set; }
    public string ViTri { get; set; } = "";
}

public class DocGiaDTO
{
    public int MaDG { get; set; }
    public string HoTen { get; set; } = "";
    public string GioiTinh { get; set; } = "Nam";
    public DateTime NgaySinh { get; set; } = new DateTime(2004, 1, 1);
    public string SDT { get; set; } = "";
    public string Email { get; set; } = "";
    public string DiaChi { get; set; } = "";
}

/// <summary>Một điểm dữ liệu cho biểu đồ cột.</summary>
public record ChartPoint(string Label, double Value);
