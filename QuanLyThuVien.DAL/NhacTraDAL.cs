using System.Data;
using static QuanLyThuVien.DAL.DbHelper;
namespace QuanLyThuVien.DAL;
public static class NhacTraDAL
{
    public static DataTable DanhSach(int truocHan = 3) => Table(@"
        SELECT pm.MaPM, dg.HoTen AS DocGia, dg.Email, s.TenSach,
               pm.HanTra, DATEDIFF(DAY, CAST(GETDATE() AS DATE), pm.HanTra) AS SoNgayCon,
               CASE WHEN pm.HanTra < CAST(GETDATE() AS DATE) THEN N'Quá hạn' ELSE N'Sắp hạn' END AS LoaiNhac,
               ISNULL((SELECT MAX(n.TrangThai) FROM dbo.NhatKyEmail n WHERE n.MaPM=pm.MaPM
                 AND n.NgayNhac=CAST(GETDATE() AS DATE)
                 AND n.LoaiNhac=CASE WHEN pm.HanTra < CAST(GETDATE() AS DATE) THEN N'Quá hạn' ELSE N'Sắp hạn' END),N'Chưa gửi') AS TrangThaiEmail
        FROM dbo.PhieuMuon pm JOIN dbo.DocGia dg ON dg.MaDG=pm.MaDG
        JOIN dbo.Sach s ON s.MaSach=pm.MaSach
        WHERE pm.NgayTra IS NULL AND pm.HanTra <= DATEADD(DAY,@ngay,CAST(GETDATE() AS DATE))
        ORDER BY pm.HanTra, dg.HoTen", P("@ngay",truocHan));

    // Claim 1 bản ghi/ngày (unique) trước khi gửi; không gửi lại email đã gửi thành công.
    public static bool TryReserve(int maPM, string loai, string email)
    {
        var n=NonQuery(@"
            SET XACT_ABORT ON;
            UPDATE dbo.NhatKyEmail SET TrangThai=N'Đang gửi',Loi=NULL,ThoiGian=SYSDATETIME()
            WHERE MaPM=@id AND LoaiNhac=@loai AND NgayNhac=CAST(GETDATE() AS DATE)
              AND TrangThai=N'Lỗi';
            IF @@ROWCOUNT > 0 RETURN;
            IF NOT EXISTS (SELECT 1 FROM dbo.NhatKyEmail
               WHERE MaPM=@id AND LoaiNhac=@loai AND NgayNhac=CAST(GETDATE() AS DATE))
            INSERT dbo.NhatKyEmail(MaPM,LoaiNhac,EmailNhan) VALUES(@id,@loai,@email);",
            P("@id",maPM),P("@loai",loai),P("@email",email));
        return n>0;
    }
    public static void Done(int maPM,string loai,bool ok,string error) => NonQuery(@"
        UPDATE dbo.NhatKyEmail SET TrangThai=@tt, Loi=@loi,ThoiGian=SYSDATETIME()
        WHERE MaPM=@id AND LoaiNhac=@loai AND NgayNhac=CAST(GETDATE() AS DATE)",
        P("@id",maPM),P("@loai",loai),P("@tt",ok?"Đã gửi":"Lỗi"),P("@loi",error));
}
