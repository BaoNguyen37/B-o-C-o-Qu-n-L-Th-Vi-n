using System.Data;
using Microsoft.Data.SqlClient;
using static QuanLyThuVien.DAL.DbHelper;

namespace QuanLyThuVien.DAL;

/// <summary>Lưu phiếu phạt/đền và thu tiền; không xóa lịch sử.</summary>
public static class PhieuPhatDAL
{
    public static DataTable GetAll(string keyword, string trangThai) => Table(@"
        SELECT p.MaPhat, p.MaPM, dg.HoTen AS DocGia, s.TenSach, p.LoaiPhat,
               p.SoTien, p.TrangThai, p.NgayLap, p.NgayThu, p.LyDo, p.NguoiLap, p.PhuongThuc, p.MaGiaoDich
        FROM dbo.PhieuPhat p
        JOIN dbo.PhieuMuon pm ON pm.MaPM=p.MaPM
        JOIN dbo.DocGia dg ON dg.MaDG=pm.MaDG
        JOIN dbo.Sach s ON s.MaSach=pm.MaSach
        WHERE (@st=N'Tất cả' OR p.TrangThai=@st)
          AND (dg.HoTen LIKE @k OR s.TenSach LIKE @k OR CAST(p.MaPhat AS NVARCHAR(20)) LIKE @k
               OR CAST(p.MaPM AS NVARCHAR(20)) LIKE @k)
        ORDER BY p.MaPhat DESC", P("@st", trangThai), P("@k", "%" + keyword + "%"));

    public static DataTable TongHop() => Table(@"
        SELECT COUNT(CASE WHEN TrangThai=N'Chưa thu' THEN 1 END) AS ChuaThu,
               ISNULL(SUM(CASE WHEN TrangThai=N'Chưa thu' THEN SoTien ELSE 0 END),0) AS TienChuaThu,
               ISNULL(SUM(CASE WHEN TrangThai=N'Đã thu' THEN SoTien ELSE 0 END),0) AS TienDaThu
        FROM dbo.PhieuPhat");

    public static DataTable ChiTietMuon(int maPM) => Table(@"
        SELECT pm.MaPM, pm.NgayTra, pm.HanTra, pm.TienPhat,
               dg.HoTen AS DocGia, s.TenSach, s.GiaBia, pm.TinhTrangXuLy
        FROM dbo.PhieuMuon pm JOIN dbo.DocGia dg ON dg.MaDG=pm.MaDG
             JOIN dbo.Sach s ON s.MaSach=pm.MaSach
        WHERE pm.MaPM=@id", P("@id", maPM));

    public static void LapPhatTraTreCu(int maPM, string nguoiLap)
    {
        using var cn = new SqlConnection(ConnectionString);
        PreviewData.GuardWrite();
        Open(cn);
        using var cmd = new SqlCommand(@"
            SET XACT_ABORT ON;
            BEGIN TRAN;
            IF NOT EXISTS (SELECT 1 FROM dbo.PhieuMuon WHERE MaPM=@id AND NgayTra IS NOT NULL AND TienPhat>0)
                THROW 52001, N'Phiếu mượn chưa trả trễ hoặc không có tiền phạt.', 1;
            IF EXISTS(SELECT 1 FROM dbo.PhieuPhat WHERE MaPM=@id AND LoaiPhat=N'Trả trễ')
                THROW 52002, N'Phiếu phạt trả trễ đã được lập, không lập trùng.', 1;
            INSERT dbo.PhieuPhat(MaPM,LoaiPhat,LyDo,SoTien,NguoiLap)
            SELECT MaPM,N'Trả trễ',N'Trả sách quá hạn',TienPhat,@user
            FROM dbo.PhieuMuon WHERE MaPM=@id;
            COMMIT;", cn);
        cmd.Parameters.AddWithValue("@id",maPM);
        cmd.Parameters.AddWithValue("@user",nguoiLap);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Kết thúc phiếu mượn vì mất/hỏng và tạo phiếu bồi thường (không cộng tồn kho).
    /// Nếu quá hạn còn tạo thêm phiếu phạt trễ, tất cả cùng một transaction.</summary>
    public static void DenSach(int maPM, string loai, string lyDo, decimal soTien, decimal phatTre, string nguoiLap)
    {
        using var cn = new SqlConnection(ConnectionString);
        PreviewData.GuardWrite();
        Open(cn);
        using var cmd = new SqlCommand(@"
            SET XACT_ABORT ON;
            BEGIN TRAN;
            UPDATE dbo.PhieuMuon
                SET NgayTra=CAST(GETDATE() AS DATE), TienPhat=@tre, TinhTrangXuLy=@loai
            WHERE MaPM=@id AND NgayTra IS NULL;
            IF @@ROWCOUNT=0 THROW 52003, N'Phiếu đã hoàn tất hoặc không tồn tại.', 1;
            INSERT dbo.PhieuPhat(MaPM,LoaiPhat,LyDo,SoTien,NguoiLap)
                VALUES(@id,@loai,@lydo,@tien,@user);
            IF @tre>0
            BEGIN
                INSERT dbo.PhieuPhat(MaPM,LoaiPhat,LyDo,SoTien,NguoiLap)
                    VALUES(@id,N'Trả trễ',N'Quá hạn trước khi lập phiếu bồi thường',@tre,@user);
            END;
            COMMIT;", cn);
        cmd.Parameters.AddWithValue("@id",maPM);
        cmd.Parameters.AddWithValue("@loai",loai);
        cmd.Parameters.AddWithValue("@lydo",lyDo);
        var m=cmd.Parameters.Add("@tien",SqlDbType.Decimal); m.Precision=18; m.Scale=0; m.Value=soTien;
        var f=cmd.Parameters.Add("@tre",SqlDbType.Decimal); f.Precision=18; f.Scale=0; f.Value=phatTre;
        cmd.Parameters.AddWithValue("@user",nguoiLap);
        cmd.ExecuteNonQuery();
    }

    public static bool DaThu(int maPhat, string phuongThuc, string maGiaoDich, string nguoiThu) => NonQuery(@"
        UPDATE dbo.PhieuPhat
        SET TrangThai=N'Đã thu', NgayThu=SYSDATETIME(),
            PhuongThuc=@pt, MaGiaoDich=@gd, NguoiThu=@user
        WHERE MaPhat=@id AND TrangThai=N'Chưa thu'",
        P("@id",maPhat), P("@pt",phuongThuc), P("@gd",maGiaoDich), P("@user",nguoiThu)) == 1;
}
