using System.Data;
using QuanLyThuVien.DAL;
namespace QuanLyThuVien.BLL;

public static class HeThongBLL
{
    public static DataTable TongQuan()
    {
        if (!AuthBLL.CurrentUser.IsAdmin)
            throw new NghiepVuException("Chỉ Admin được truy cập kiểm tra hệ thống.");
        return HeThongDAL.TongQuan();
    }
}
