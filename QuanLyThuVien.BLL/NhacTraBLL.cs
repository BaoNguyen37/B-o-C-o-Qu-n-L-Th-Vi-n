using System.Data;
using System.Net.Mail;
using QuanLyThuVien.DAL;
namespace QuanLyThuVien.BLL;
public static class NhacTraBLL
{
    public static DataTable DanhSach() => NhacTraDAL.DanhSach(3);
    public static bool EmailHopLe(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try { return new MailAddress(email).Address.Equals(email.Trim(),StringComparison.OrdinalIgnoreCase); }
        catch { return false; }
    }
    public static string NoiDung(string ten, string sach, int maPM, DateTime han, int ngayCon)
    {
        string status = ngayCon < 0 ? $"đã quá hạn {-ngayCon} ngày" : ngayCon==0 ? "đến hạn hôm nay" : $"còn {ngayCon} ngày là đến hạn";
        return $"Xin chào {ten},\n\nThư viện nhắc bạn phiếu mượn #{maPM} - sách: {sach}.\nHạn trả: {han:dd/MM/yyyy}; hiện tại {status}.\nVui lòng liên hệ thủ thư để trả sách hoặc trao đổi nếu có vấn đề.\n\nNếu bạn đã trả sách, vui lòng bỏ qua thư này.\nTrân trọng,\nBộ phận quản lý thư viện.";
    }
}
