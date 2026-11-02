using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyThuVien.DTO;
using static QuanLyThuVien.DAL.DbHelper;

namespace QuanLyThuVien.DAL;

public static class SachDAL
{
    public static DataTable GetAll(string keyword, int maTL) => Table(@"
        SELECT s.MaSach, s.TenSach, s.TacGia, t.TenTL AS TheLoai, s.NhaXuatBan, s.NamXB,
               s.SoLuong, s.GiaBia, s.ViTri, s.MaTL
        FROM Sach s JOIN TheLoai t ON s.MaTL = t.MaTL
        WHERE (s.TenSach LIKE @k OR s.TacGia LIKE @k OR s.NhaXuatBan LIKE @k)
          AND (@tl = 0 OR s.MaTL = @tl)
        ORDER BY s.MaSach DESC", P("@k", "%" + keyword + "%"), P("@tl", maTL));

    /// <summary>Danh sách sách còn trong kho (đổ vào ComboBox khi lập phiếu mượn).</summary>
    public static DataTable GetConSach() => Table(
        "SELECT MaSach, TenSach + N'  (còn ' + CAST(SoLuong AS NVARCHAR(10)) + N')' AS Ten FROM Sach WHERE SoLuong > 0 ORDER BY TenSach");

    static SqlParameter[] Ps(SachDTO s) => new[]
    {
        P("@a", s.TenSach), P("@b", s.TacGia), P("@c", s.NhaXuatBan), P("@d", s.NamXB),
        P("@e", s.MaTL), P("@f", s.SoLuong), P("@g", s.GiaBia), P("@h", s.ViTri)
    };

    public static void Insert(SachDTO s) => NonQuery(
        "INSERT INTO Sach(TenSach,TacGia,NhaXuatBan,NamXB,MaTL,SoLuong,GiaBia,ViTri) VALUES(@a,@b,@c,@d,@e,@f,@g,@h)", Ps(s));

    public static void Update(SachDTO s) => NonQuery(
        "UPDATE Sach SET TenSach=@a,TacGia=@b,NhaXuatBan=@c,NamXB=@d,MaTL=@e,SoLuong=@f,GiaBia=@g,ViTri=@h WHERE MaSach=@id",
        Ps(s).Append(P("@id", s.MaSach)).ToArray());

    public static void Delete(int id) => NonQuery("DELETE FROM Sach WHERE MaSach=@id", P("@id", id));

    public static bool DaCoPhieu(int id) =>
        Convert.ToInt32(Scalar("SELECT COUNT(*) FROM PhieuMuon WHERE MaSach=@id", P("@id", id))) > 0;
}
