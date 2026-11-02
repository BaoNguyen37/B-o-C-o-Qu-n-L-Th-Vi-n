using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;
public sealed class CauHinhGmail
{
    public string DiaChiGui {get;set;}="";
    public string MatKhauDaMaHoa {get;set;}="";
    public bool TuDongNhac {get;set;}=false;
    public static string Tep => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"QuanLyThuVien","gmail.json");
    public static CauHinhGmail Load()
    {
        try{return File.Exists(Tep) ? JsonSerializer.Deserialize<CauHinhGmail>(File.ReadAllText(Tep)) ?? new() : new();}
        catch {return new();}
    }
    public void SetAppPassword(string password)
    {
        if(string.IsNullOrWhiteSpace(password))return; // không ghi đè mật khẩu cũ nếu trường để trống
        var bytes=ProtectedData.Protect(Encoding.UTF8.GetBytes(password.Replace(" ", "")),null,DataProtectionScope.CurrentUser);
        MatKhauDaMaHoa=Convert.ToBase64String(bytes);
    }
    public string GetAppPassword()
    {
        if(string.IsNullOrWhiteSpace(MatKhauDaMaHoa))throw new InvalidOperationException("Hãy nhập Gmail App Password trong phần cấu hình.");
        try{return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(MatKhauDaMaHoa),null,DataProtectionScope.CurrentUser));}
        catch(Exception){throw new InvalidOperationException("Không giải mã được App Password trên tài khoản Windows này. Vui lòng nhập lại.");}
    }
    public void Validate()
    {
        if(!DiaChiGui.EndsWith("@gmail.com",StringComparison.OrdinalIgnoreCase) || !NhacTraBLL.EmailHopLe(DiaChiGui))
            throw new ArgumentException("Nhập địa chỉ Gmail hợp lệ để gửi thông báo.");
        _=GetAppPassword();
    }
    public void Save()
    {
        QuanLyThuVien.DAL.PreviewData.GuardWrite();
        Validate();Directory.CreateDirectory(Path.GetDirectoryName(Tep)!);
        File.WriteAllText(Tep,JsonSerializer.Serialize(this,new JsonSerializerOptions{WriteIndented=true}));
    }
}
