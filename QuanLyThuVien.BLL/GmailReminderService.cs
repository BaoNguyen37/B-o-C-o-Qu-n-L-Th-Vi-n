using System.Net;
using System.Net.Mail;
using System.Text;
using System.Data;
using QuanLyThuVien.DAL;
namespace QuanLyThuVien.BLL;
public static class GmailReminderService
{
    private static readonly SemaphoreSlim Sending=new(1,1);
    public static async Task SendMessageAsync(string from,string password,string recipient,string subject,string body)
    {
        PreviewData.GuardWrite();
        if(!NhacTraBLL.EmailHopLe(from))throw new InvalidOperationException("Gmail người gửi không hợp lệ.");
        if(string.IsNullOrWhiteSpace(password))throw new InvalidOperationException("Chưa cấu hình Gmail App Password.");
        if(!NhacTraBLL.EmailHopLe(recipient))throw new InvalidOperationException("Email độc giả không hợp lệ: "+recipient);
        using var smtp=new SmtpClient("smtp.gmail.com",587)
        {
            EnableSsl=true,
            UseDefaultCredentials=false,
            Credentials=new NetworkCredential(from,password),
            Timeout=20000
        };
        using var message=new MailMessage(new MailAddress(from,"Thư viện"),new MailAddress(recipient))
        {
            Subject=subject,
            Body=body,
            IsBodyHtml=false,
            BodyEncoding=Encoding.UTF8,
            SubjectEncoding=Encoding.UTF8
        };
        await smtp.SendMailAsync(message);
    }
    public static async Task<string> SendOneAsync(DataRow row,string from,string password)
    {
        int loan=Convert.ToInt32(row["MaPM"]);
        string loai=Convert.ToString(row["LoaiNhac"])!;
        string email=Convert.ToString(row["Email"])!.Trim();
        if(!NhacTraBLL.EmailHopLe(email))return "Bỏ qua: thiếu hoặc sai email.";
        var status=Convert.ToString(row["TrangThaiEmail"]);
        if(status=="Đã gửi" || status=="Đang gửi")return "Đã gửi/đang gửi hôm nay.";
        if(!NhacTraDAL.TryReserve(loan,loai,email))return "Đã có nhật ký gửi hôm nay.";
        try
        {
            string title=loai=="Quá hạn"?$"[THƯ VIỆN] Phiếu #{loan} quá hạn trả sách":$"[THƯ VIỆN] Nhắc hạn trả sách - phiếu #{loan}";
            await SendMessageAsync(from,password,email,title,
                NhacTraBLL.NoiDung(Convert.ToString(row["DocGia"])!,Convert.ToString(row["TenSach"])!,loan,Convert.ToDateTime(row["HanTra"]),Convert.ToInt32(row["SoNgayCon"])));
            NhacTraDAL.Done(loan,loai,true,"");
            return $"Đã gửi cho {email}.";
        }
        catch(Exception ex)
        {
            var detail=ex.GetBaseException().Message;
            NhacTraDAL.Done(loan,loai,false,detail.Length>480?detail[..480]:detail);
            return $"Lỗi gửi tới {email}: {detail}";
        }
    }
    public static async Task<(int sent,int skipped)> SendBatchAsync(string from,string password,int limit=20)
    {
        // Chạy khi ứng dụng mở. Không làm việc nền khi app bị đóng.
        if(!await Sending.WaitAsync(0))return (0,0);
        try
        {
            if(!NhacTraBLL.EmailHopLe(from) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Cấu hình Gmail chưa hợp lệ.");
            int success=0,skip=0;
            foreach(DataRow row in NhacTraBLL.DanhSach().Rows.Cast<DataRow>()
                .Where(x => Convert.ToString(x["TrangThaiEmail"]) != "Đã gửi"
                    && Convert.ToString(x["TrangThaiEmail"]) != "Đang gửi")
                .Take(limit))
            {
                var msg=await SendOneAsync(row,from,password);
                if(msg.StartsWith("Đã gửi cho "))success++;else skip++;
            }
            return(success,skip);
        }
        finally{Sending.Release();}
    }
}
