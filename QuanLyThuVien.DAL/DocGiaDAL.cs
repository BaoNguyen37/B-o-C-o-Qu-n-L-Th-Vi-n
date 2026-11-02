using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyThuVien.DTO;
using static QuanLyThuVien.DAL.DbHelper;

namespace QuanLyThuVien.DAL;

public static class DocGiaDAL
{
    public static DataTable GetAll(string keyword) => Table(@"
        SELECT MaDG, HoTen, GioiTinh, NgaySinh, SDT, Email, DiaChi, NgayDangKy
        FROM DocGia
        WHERE HoTen LIKE @k OR SDT LIKE @k OR Email LIKE @k
        ORDER BY MaDG DESC", P("@k", "%" + keyword + "%"));

    public static DataTable GetForCombo() => Table(
        "SELECT MaDG, CAST(MaDG AS NVARCHAR(10)) + N' - ' + HoTen AS Ten FROM DocGia ORDER BY HoTen");

    static SqlParameter[] Ps(DocGiaDTO d) => new[]
    {
        P("@a", d.HoTen), P("@b", d.GioiTinh), P("@c", d.NgaySinh.Date),
        P("@d", d.SDT), P("@e", d.Email), P("@f", d.DiaChi)
    };

    public static void Insert(DocGiaDTO d) => NonQuery(
        "INSERT INTO DocGia(HoTen,GioiTinh,NgaySinh,SDT,Email,DiaChi) VALUES(@a,@b,@c,@d,@e,@f)", Ps(d));

    public static void Update(DocGiaDTO d) => NonQuery(
        "UPDATE DocGia SET HoTen=@a,GioiTinh=@b,NgaySinh=@c,SDT=@d,Email=@e,DiaChi=@f WHERE MaDG=@id",
        Ps(d).Append(P("@id", d.MaDG)).ToArray());

    public static void Delete(int id) => NonQuery("DELETE FROM DocGia WHERE MaDG=@id", P("@id", id));

    public static bool DaCoPhieu(int id) =>
        Convert.ToInt32(Scalar("SELECT COUNT(*) FROM PhieuMuon WHERE MaDG=@id", P("@id", id))) > 0;
}
