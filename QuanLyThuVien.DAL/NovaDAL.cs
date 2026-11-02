using System.Data;
using static QuanLyThuVien.DAL.DbHelper;
namespace QuanLyThuVien.DAL;

public static class NovaDAL
{
    public static DataTable Catalog(string keyword, int category, bool available) => Table(@"
        SELECT s.*,t.TenTL AS TheLoai,
        (SELECT COUNT(*) FROM PhieuMuon p WHERE p.MaSach=s.MaSach) AS LuotMuon,
        (SELECT COUNT(*) FROM DatTruoc d WHERE d.MaSach=s.MaSach AND d.TrangThai=N'Đang chờ') AS HangCho
        FROM Sach s JOIN TheLoai t ON s.MaTL=t.MaTL
        WHERE (@tl=0 OR s.MaTL=@tl) AND (@available=0 OR s.SoLuong>0)
        AND (s.TenSach COLLATE Vietnamese_CI_AI LIKE @k OR s.TacGia COLLATE Vietnamese_CI_AI LIKE @k
             OR CONVERT(nvarchar,s.MaSach)=@id OR s.ViTri COLLATE Vietnamese_CI_AI LIKE @k)
        ORDER BY LuotMuon DESC,s.MaSach",P("@k","%"+keyword.Trim()+"%"),P("@id",keyword.Trim()),P("@tl",category),P("@available",available));

    public static DataTable Reservations(string status) => Table(@"
        SELECT d.MaDat,d.MaDG,d.MaSach,g.HoTen AS DocGia,s.TenSach,d.NgayDat,d.TrangThai,s.SoLuong,
        CASE WHEN d.TrangThai=N'Đang chờ' THEN (SELECT COUNT(*) FROM DatTruoc q
            WHERE q.MaSach=d.MaSach AND q.TrangThai=N'Đang chờ' AND q.MaDat<=d.MaDat) END AS ThuTu
        FROM DatTruoc d JOIN DocGia g ON d.MaDG=g.MaDG JOIN Sach s ON d.MaSach=s.MaSach
        WHERE @st=N'Tất cả' OR d.TrangThai=@st ORDER BY d.MaDat DESC",P("@st",status));
    public static void Reserve(int reader,int book) => NonQuery("EXEC dbo.NovaDat @dg,@s",P("@dg",reader),P("@s",book));
    public static void Cancel(int id) => NonQuery("EXEC dbo.NovaHuyDat @id",P("@id",id));
    public static DataTable Activity(string keyword) => Table(@"
        SELECT TOP 500 ThoiGian,NguoiDung,DoiTuong,HanhDong,MaDoiTuong FROM NhatKyThaoTac
        WHERE NguoiDung LIKE @k OR DoiTuong LIKE @k OR CONVERT(nvarchar,MaDoiTuong) LIKE @k
        ORDER BY MaLog DESC",P("@k","%"+keyword.Trim()+"%"));
    public static DataTable Attention() => Table(@"
        SELECT p.MaPM AS [Mã phiếu],g.HoTen AS [Độc giả],s.TenSach AS [Tên sách],p.HanTra AS [Hạn trả],
        CASE WHEN p.HanTra<CAST(GETDATE() AS DATE) THEN N'Quá hạn' ELSE N'Sắp đến hạn' END AS [Tình trạng]
        FROM PhieuMuon p JOIN DocGia g ON p.MaDG=g.MaDG JOIN Sach s ON p.MaSach=s.MaSach
        WHERE p.NgayTra IS NULL AND p.HanTra<=DATEADD(DAY,3,CAST(GETDATE() AS DATE)) ORDER BY p.HanTra");
    public static DataTable Pulse() => Table(@"
        SELECT (SELECT COUNT(*) FROM DatTruoc WHERE TrangThai=N'Đang chờ') AS Cho,
        (SELECT ISNULL(SUM(SoTien),0) FROM PhieuPhat WHERE TrangThai=N'Chưa thu') AS CongNo,
        (SELECT COUNT(*) FROM Sach WHERE SoLuong<=2) AS SapHet");
}
