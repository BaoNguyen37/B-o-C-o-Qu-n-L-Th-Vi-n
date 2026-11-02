using System.Text.Json;
namespace QuanLyThuVien.GUI;
public sealed class CauHinhThanhToan
{
    public string SoTaiKhoan {get;set;} = "";
    public string TenChuTaiKhoan {get;set;} = "NGUYEN BAO NGUYEN";
    public static string Tep => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"QuanLyThuVien","thanhtoan.json");
    public static CauHinhThanhToan Load()
    {
        try { return File.Exists(Tep) ? JsonSerializer.Deserialize<CauHinhThanhToan>(File.ReadAllText(Tep)) ?? new() : new(); }
        catch { return new(); }
    }
    public static bool SoTaiKhoanHopLe(string value) =>
        !string.IsNullOrWhiteSpace(value) && value.Length >= 6 && value.Length <= 19
        && value.All(char.IsAsciiDigit);
    public void Save()
    {
        QuanLyThuVien.DAL.PreviewData.GuardWrite();
        SoTaiKhoan = (SoTaiKhoan ?? "").Trim();
        if (!SoTaiKhoanHopLe(SoTaiKhoan))
            throw new ArgumentException("Số tài khoản MB Bank phải gồm 6–19 chữ số. Vui lòng kiểm tra trên ứng dụng ngân hàng.");
        var folder=Path.GetDirectoryName(Tep)!;Directory.CreateDirectory(folder);
        File.WriteAllText(Tep,JsonSerializer.Serialize(this, new JsonSerializerOptions {WriteIndented=true}));
    }
    public string VietQr(int maPhat, decimal amount)
    {
        if (!SoTaiKhoanHopLe(SoTaiKhoan))
            throw new InvalidOperationException("Chưa cấu hình số tài khoản MB Bank hợp lệ (6–19 chữ số). Admin cần lưu STK trước khi tạo QR.");
        if (amount<=0 || amount!=decimal.Truncate(amount)) throw new ArgumentException("Số tiền thanh toán không hợp lệ.");
        // VietQR Quick Link, BIN MB Bank = 970422. Phải kiểm tra tên người thụ hưởng khi quét mã.
        var info=Uri.EscapeDataString($"PHAT TL{maPhat}");
        var holder=Uri.EscapeDataString(TenChuTaiKhoan);
        return $"https://img.vietqr.io/image/970422-{SoTaiKhoan}-compact2.png?amount={amount:0}&addInfo={info}&accountName={holder}";
    }
}
