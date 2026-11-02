using System.Data;
using QuanLyThuVien.DAL;
namespace QuanLyThuVien.BLL;

public static class NovaBLL
{
    public static DataTable Catalog(string q="",int category=0,bool available=false) => NovaDAL.Catalog(q,category,available);
    public static DataTable Reservations(string status="Đang chờ") => NovaDAL.Reservations(status);
    public static void Reserve(int reader,int book)
    {
        if(reader<=0 || book<=0) throw new NghiepVuException("Chọn độc giả và sách cần đặt trước.");
        NovaDAL.Reserve(reader,book);
    }
    public static void Cancel(int id) => NovaDAL.Cancel(id);
    public static void Receive(int id)
    {
        var row=NovaDAL.Reservations("Đang chờ").AsEnumerable().FirstOrDefault(r=>r.Field<int>("MaDat")==id)
            ?? throw new NghiepVuException("Đặt trước không còn trong hàng chờ.");
        PhieuMuonBLL.Muon(row.Field<int>("MaDG"),row.Field<int>("MaSach"),DateTime.Today,DateTime.Today.AddDays(14));
    }
    public static DataTable Activity(string keyword="")
    {
        if(AuthBLL.CurrentUser?.IsAdmin!=true) throw new NghiepVuException("Chỉ quản trị viên được xem nhật ký.");
        return NovaDAL.Activity(keyword);
    }
    public static DataTable Attention() => NovaDAL.Attention();
    public static DataTable Pulse() => NovaDAL.Pulse();
}
