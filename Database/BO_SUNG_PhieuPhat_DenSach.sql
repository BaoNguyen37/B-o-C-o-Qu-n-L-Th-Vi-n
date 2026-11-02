/* BỔ SUNG PHIẾU PHẠT / BỒI THƯỜNG - KHÔNG XÓA DỮ LIỆU CŨ.
   Server: BAONGUYEN\BAONGUYEN1 - Windows Authentication
   Chạy trong SSMS bằng Execute (F5) SAU KHI đã có database QuanLyThuVien.
   Có thể chạy lại an toàn: không xóa bảng, không xóa dữ liệu, không đổi tài khoản.
*/
USE QuanLyThuVien;
GO
IF OBJECT_ID(N'dbo.PhieuMuon', N'U') IS NULL
    THROW 51000, N'Chưa có bảng PhieuMuon. Hãy chạy QuanLyThuVien.sql trước.', 1;
GO
IF COL_LENGTH(N'dbo.PhieuMuon', N'TinhTrangXuLy') IS NULL
BEGIN
    ALTER TABLE dbo.PhieuMuon ADD TinhTrangXuLy NVARCHAR(20) NOT NULL
        CONSTRAINT DF_PhieuMuon_TinhTrangXuLy DEFAULT N'Bình thường';
END;
GO
IF OBJECT_ID(N'dbo.PhieuPhat', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhieuPhat (
        MaPhat      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MaPM        INT NOT NULL REFERENCES dbo.PhieuMuon(MaPM),
        LoaiPhat    NVARCHAR(20) NOT NULL
            CHECK(LoaiPhat IN (N'Trả trễ', N'Mất sách', N'Hỏng sách')),
        LyDo        NVARCHAR(300) NOT NULL,
        SoTien      DECIMAL(18,0) NOT NULL CHECK(SoTien > 0),
        NgayLap     DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
        TrangThai   NVARCHAR(20) NOT NULL DEFAULT N'Chưa thu'
            CHECK(TrangThai IN (N'Chưa thu', N'Đã thu')),
        NgayThu     DATETIME2(0) NULL,
        NguoiLap    NVARCHAR(50) NOT NULL,
        PhuongThuc  NVARCHAR(25) NULL,
        MaGiaoDich  NVARCHAR(80) NULL,
        NguoiThu    NVARCHAR(50) NULL
    );
END;
GO
IF COL_LENGTH(N'dbo.PhieuPhat', N'PhuongThuc') IS NULL
    ALTER TABLE dbo.PhieuPhat ADD PhuongThuc NVARCHAR(25) NULL;
IF COL_LENGTH(N'dbo.PhieuPhat', N'MaGiaoDich') IS NULL
    ALTER TABLE dbo.PhieuPhat ADD MaGiaoDich NVARCHAR(80) NULL;
IF COL_LENGTH(N'dbo.PhieuPhat', N'NguoiThu') IS NULL
    ALTER TABLE dbo.PhieuPhat ADD NguoiThu NVARCHAR(50) NULL;
GO
IF OBJECT_ID(N'dbo.NhatKyEmail', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhatKyEmail (
        MaLog INT IDENTITY(1,1) PRIMARY KEY,
        MaPM INT NOT NULL REFERENCES dbo.PhieuMuon(MaPM),
        LoaiNhac NVARCHAR(20) NOT NULL CHECK (LoaiNhac IN (N'Sắp hạn',N'Quá hạn')),
        NgayNhac DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
        EmailNhan NVARCHAR(200) NOT NULL,
        TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Đang gửi',
        Loi NVARCHAR(500) NULL,
        ThoiGian DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT UQ_Email_1Lan_Ngay UNIQUE(MaPM, LoaiNhac, NgayNhac)
    );
END;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.PhieuPhat') AND name=N'UX_PhieuPhat_PhieuLoai')
    CREATE UNIQUE INDEX UX_PhieuPhat_PhieuLoai ON dbo.PhieuPhat(MaPM,LoaiPhat);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.PhieuPhat') AND name=N'UX_PhieuPhat_GiaoDichNH')
    CREATE UNIQUE INDEX UX_PhieuPhat_GiaoDichNH ON dbo.PhieuPhat(MaGiaoDich)
    WHERE PhuongThuc=N'Chuyển khoản' AND MaGiaoDich IS NOT NULL;
GO
PRINT N'Đã bổ sung chức năng phiếu phạt/đền. Dữ liệu cũ được giữ nguyên.';
