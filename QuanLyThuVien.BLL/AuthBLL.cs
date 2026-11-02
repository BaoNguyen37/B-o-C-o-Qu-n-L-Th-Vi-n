using System.Security.Cryptography;
using System.Text;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL;

public static class AuthBLL
{
    public static TaiKhoanDTO CurrentUser { get; private set; }

    public static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s ?? "")));

    public static bool Login(string user, string pass)
    {
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrEmpty(pass)) return false;
        PreviewData.Enabled=false;
        CurrentUser = TaiKhoanDAL.Login(user.Trim(), Hash(pass));
        DbHelper.Actor = CurrentUser?.TenDangNhap ?? "";
        return CurrentUser != null;
    }

    public static void StartPreview() { PreviewData.Enabled=true;CurrentUser=new TaiKhoanDTO{TenDangNhap="demo",HoTen="Khách xem thử",VaiTro="Admin"}; }
    public static void Logout() { CurrentUser = null; DbHelper.Actor = ""; PreviewData.Enabled=false; }

    public static void DoiMatKhau(string oldPass, string newPass, string confirm)
    {
        if (string.IsNullOrEmpty(newPass) || newPass.Length < 6)
            throw new NghiepVuException("Mật khẩu mới phải có ít nhất 6 ký tự.");
        if (newPass != confirm)
            throw new NghiepVuException("Xác nhận mật khẩu không khớp.");
        if (!TaiKhoanDAL.DoiMatKhau(CurrentUser.TenDangNhap, Hash(oldPass), Hash(newPass)))
            throw new NghiepVuException("Mật khẩu hiện tại không đúng.");
    }
}
