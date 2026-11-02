using QuanLyThuVien.DTO;
using static QuanLyThuVien.DAL.DbHelper;

namespace QuanLyThuVien.DAL;

public static class TaiKhoanDAL
{
    public static TaiKhoanDTO Login(string user, string hash)
    {
        var dt = Table("SELECT TenDangNhap, HoTen, VaiTro FROM TaiKhoan WHERE TenDangNhap=@u AND MatKhau=@p",
            P("@u", user), P("@p", hash));
        if (dt.Rows.Count == 0) return null;
        var r = dt.Rows[0];
        return new TaiKhoanDTO
        {
            TenDangNhap = r["TenDangNhap"].ToString(),
            HoTen = r["HoTen"].ToString(),
            VaiTro = r["VaiTro"].ToString()
        };
    }

    public static bool DoiMatKhau(string user, string oldHash, string newHash) =>
        NonQuery("UPDATE TaiKhoan SET MatKhau=@n WHERE TenDangNhap=@u AND MatKhau=@o",
            P("@n", newHash), P("@u", user), P("@o", oldHash)) > 0;
}
