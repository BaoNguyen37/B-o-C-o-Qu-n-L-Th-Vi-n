using System.Data;
using static QuanLyThuVien.DAL.DbHelper;

namespace QuanLyThuVien.DAL;

public static class ThongKeDAL
{
    public static DataTable Summary() => Table(@"
        SELECT (SELECT COUNT(*) FROM Sach) AS DauSach,
               (SELECT ISNULL(SUM(SoLuong),0) FROM Sach) AS TrongKho,
               (SELECT COUNT(*) FROM DocGia) AS DocGia,
               (SELECT COUNT(*) FROM PhieuMuon WHERE NgayTra IS NULL) AS DangMuon,
               (SELECT COUNT(*) FROM PhieuMuon WHERE NgayTra IS NULL AND HanTra < CAST(GETDATE() AS DATE)) AS QuaHan,
               (SELECT ISNULL(SUM(TienPhat),0) FROM PhieuMuon) AS TienPhat,
               (SELECT COUNT(*) FROM PhieuMuon) AS TongPhieu");

    public static DataTable MuonTheoThang() => Table(@"
        SELECT YEAR(NgayMuon) AS Y, MONTH(NgayMuon) AS M, COUNT(*) AS C
        FROM PhieuMuon
        WHERE NgayMuon >= DATEADD(MONTH, -5, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
        GROUP BY YEAR(NgayMuon), MONTH(NgayMuon)");

    public static DataTable SachTheoTheLoai() => Table(@"
        SELECT t.TenTL, ISNULL(SUM(s.SoLuong),0) AS SL
        FROM TheLoai t LEFT JOIN Sach s ON s.MaTL = t.MaTL
        GROUP BY t.TenTL ORDER BY SL DESC");

    public static DataTable TopSach() => Table(@"
        SELECT TOP 5 s.TenSach, COUNT(*) AS Luot
        FROM PhieuMuon pm JOIN Sach s ON pm.MaSach = s.MaSach
        GROUP BY s.TenSach ORDER BY COUNT(*) DESC");

    public static DataTable TopDocGia() => Table(@"
        SELECT TOP 8 dg.HoTen AS DocGia, COUNT(*) AS LuotMuon
        FROM PhieuMuon pm JOIN DocGia dg ON pm.MaDG = dg.MaDG
        GROUP BY dg.HoTen ORDER BY COUNT(*) DESC");
}
