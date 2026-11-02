using System.Data;
using static QuanLyThuVien.DAL.DbHelper;

namespace QuanLyThuVien.DAL;

public static class PhieuMuonDAL
{
    public static DataTable GetAll(string keyword) => Table(@"
        SELECT pm.MaPM, dg.MaDG, dg.HoTen AS DocGia, s.TenSach, pm.NgayMuon, pm.HanTra, pm.NgayTra, pm.TienPhat, pm.SoLanGiaHan,
               CASE WHEN pm.NgayTra IS NOT NULL AND pm.TinhTrangXuLy IN (N'Mất sách',N'Hỏng sách') THEN pm.TinhTrangXuLy
                    WHEN pm.NgayTra IS NOT NULL THEN N'Đã trả'
                    WHEN pm.HanTra < CAST(GETDATE() AS DATE) THEN N'Quá hạn'
                    ELSE N'Đang mượn' END AS TrangThai
        FROM PhieuMuon pm
        JOIN DocGia dg ON pm.MaDG = dg.MaDG
        JOIN Sach s ON pm.MaSach = s.MaSach
        WHERE dg.HoTen LIKE @k OR s.TenSach LIKE @k OR CAST(pm.MaPM AS NVARCHAR(20)) LIKE @k
        ORDER BY pm.MaPM DESC", P("@k", "%" + keyword + "%"));

    /// <summary>Lập phiếu mượn + trừ tồn kho trong 1 transaction.</summary>
    public static void Muon(int maDG, int maSach, DateTime ngayMuon, DateTime hanTra) => NonQuery(
        "EXEC dbo.NovaMuon @dg,@s,@nm,@ht", P("@dg",maDG),P("@s",maSach),P("@nm",ngayMuon.Date),P("@ht",hanTra.Date));

    /// <summary>Ghi nhận trả sách + cộng lại tồn kho trong 1 transaction.</summary>
    public static void Tra(int maPM, DateTime ngayTra, decimal tienPhat, string nguoiLap) => NonQuery(@"
        SET XACT_ABORT ON;
        BEGIN TRAN;
        EXEC dbo.NovaLock;
        UPDATE PhieuMuon SET NgayTra = @nt, TienPhat = @tp WHERE MaPM = @id AND NgayTra IS NULL;
        IF @@ROWCOUNT = 0 THROW 50002, N'Phiếu này đã được trả trước đó.', 1;
        UPDATE Sach SET SoLuong = SoLuong + 1 WHERE MaSach = (SELECT MaSach FROM PhieuMuon WHERE MaPM = @id);
        IF @tp > 0
            INSERT dbo.PhieuPhat(MaPM, LoaiPhat, LyDo, SoTien, NguoiLap)
            VALUES(@id, N'Trả trễ', N'Trả sách quá hạn', @tp, @user);
        COMMIT;",
        P("@nt", ngayTra.Date), P("@tp", tienPhat), P("@id", maPM), P("@user", nguoiLap));

    public static void GiaHan(int maPM, int soNgay) => NonQuery("EXEC dbo.NovaGiaHan @id", P("@id",maPM));

    /// <summary>Hạn trả của phiếu chưa trả; null nếu phiếu đã trả / không tồn tại.</summary>
    public static DateTime? GetHanTraChuaTra(int maPM)
    {
        var o = Scalar("SELECT HanTra FROM PhieuMuon WHERE MaPM=@id AND NgayTra IS NULL", P("@id", maPM));
        return o == null || o == DBNull.Value ? null : Convert.ToDateTime(o);
    }

    public static int DemDangMuon(int maDG) => Convert.ToInt32(Scalar(
        "SELECT COUNT(*) FROM PhieuMuon WHERE MaDG=@id AND NgayTra IS NULL", P("@id", maDG)));

    public static int DemQuaHan(int maDG) => Convert.ToInt32(Scalar(
        "SELECT COUNT(*) FROM PhieuMuon WHERE MaDG=@id AND NgayTra IS NULL AND HanTra < CAST(GETDATE() AS DATE)", P("@id", maDG)));
}
