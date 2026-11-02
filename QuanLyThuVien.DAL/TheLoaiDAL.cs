using System.Data;
using static QuanLyThuVien.DAL.DbHelper;

namespace QuanLyThuVien.DAL;

public static class TheLoaiDAL
{
    public static DataTable GetAll() => Table("SELECT MaTL, TenTL FROM TheLoai ORDER BY TenTL");
}
