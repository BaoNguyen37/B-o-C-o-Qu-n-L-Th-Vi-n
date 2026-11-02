using System.Data;
using static QuanLyThuVien.DAL.DbHelper;
namespace QuanLyThuVien.DAL;

// Chỉ thống kê (SELECT), không sửa database của người dùng.
public static class HeThongDAL
{
    public static DataTable TongQuan() => Table(@"
        SELECT DB_NAME() AS TenDatabase,
            @@SERVERNAME AS TenServer,
            (SELECT COUNT(*) FROM dbo.Sach) AS SoDauSach,
            (SELECT COUNT(*) FROM dbo.DocGia) AS SoDocGia,
            (SELECT COUNT(*) FROM dbo.PhieuMuon WHERE NgayTra IS NULL) AS DangMuon,
            (SELECT COUNT(*) FROM dbo.PhieuMuon WHERE NgayTra IS NULL AND HanTra<CAST(GETDATE() AS DATE)) AS QuaHan,
            (SELECT COUNT(*) FROM dbo.PhieuPhat WHERE TrangThai=N'Chưa thu') AS ChuaThu,
            (SELECT ISNULL(SUM(SoTien),0) FROM dbo.PhieuPhat WHERE TrangThai=N'Chưa thu') AS CongNo,
            (SELECT COUNT(*) FROM dbo.NhatKyEmail WHERE NgayNhac=CAST(GETDATE() AS DATE) AND TrangThai=N'Đã gửi') AS EmailDaGuiHomNay;");
}
