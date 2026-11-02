/* QUAN LY THU VIEN - SQL SERVER BAONGUYEN\BAONGUYEN1
   KET NOI SSMS: Windows Authentication.
   Chay F5 toan bo file trong SSMS MOT LAN TRUOC KHI CHAY APP.
   Tao database rieng QuanLyThuVienNova; KHONG thay doi QLThuVienCongCong.
   Script co the chay lai: bao toan bang va du lieu da co.
*/
USE master;
GO
IF DB_ID(N'QuanLyThuVienNova') IS NULL
BEGIN
    CREATE DATABASE QuanLyThuVienNova;
END;
GO
USE QuanLyThuVienNova;
GO

/* ---------- BẢNG ---------- */
IF OBJECT_ID(N'TaiKhoan') IS NULL
CREATE TABLE TaiKhoan (
    TenDangNhap NVARCHAR(50)  PRIMARY KEY,
    MatKhau     VARCHAR(64)   NOT NULL,          -- SHA-256 (hex)
    HoTen       NVARCHAR(100) NOT NULL,
    VaiTro      NVARCHAR(20)  NOT NULL CHECK (VaiTro IN (N'Admin', N'Thủ thư'))
);
IF OBJECT_ID(N'TheLoai') IS NULL
CREATE TABLE TheLoai (
    MaTL  INT IDENTITY(1,1) PRIMARY KEY,
    TenTL NVARCHAR(100) NOT NULL UNIQUE
);
IF OBJECT_ID(N'Sach') IS NULL
CREATE TABLE Sach (
    MaSach     INT IDENTITY(1,1) PRIMARY KEY,
    TenSach    NVARCHAR(200) NOT NULL,
    TacGia     NVARCHAR(100) NOT NULL,
    NhaXuatBan NVARCHAR(100) NULL,
    NamXB      INT NULL,
    MaTL       INT NOT NULL REFERENCES TheLoai(MaTL),
    SoLuong    INT NOT NULL DEFAULT 0 CHECK (SoLuong >= 0),   -- số cuốn còn trong kho
    GiaBia     DECIMAL(18,0) NOT NULL DEFAULT 0,
    ViTri      NVARCHAR(50) NULL
);
IF OBJECT_ID(N'DocGia') IS NULL
CREATE TABLE DocGia (
    MaDG        INT IDENTITY(1,1) PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    GioiTinh    NVARCHAR(10)  NOT NULL,
    NgaySinh    DATE NOT NULL,
    SDT         VARCHAR(15)   NOT NULL,
    Email       NVARCHAR(100) NULL,
    DiaChi      NVARCHAR(200) NULL,
    NgayDangKy  DATE NOT NULL DEFAULT (CAST(GETDATE() AS DATE))
);
IF OBJECT_ID(N'PhieuMuon') IS NULL
CREATE TABLE PhieuMuon (
    MaPM     INT IDENTITY(1,1) PRIMARY KEY,
    MaDG     INT NOT NULL REFERENCES DocGia(MaDG),
    MaSach   INT NOT NULL REFERENCES Sach(MaSach),
    NgayMuon DATE NOT NULL,
    HanTra   DATE NOT NULL,
    NgayTra  DATE NULL,
    TienPhat DECIMAL(18,0) NOT NULL DEFAULT 0
);
GO

/* ---------- DỮ LIỆU MẪU ---------- */
IF NOT EXISTS (SELECT 1 FROM TaiKhoan)
INSERT TaiKhoan VALUES
 (N'admin',  CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2),  N'Bảo Nguyễn',      N'Admin'),
 (N'thuthu', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'thuthu123'), 2), N'Thủ thư Minh Anh', N'Thủ thư');

IF NOT EXISTS (SELECT 1 FROM TheLoai)
INSERT TheLoai(TenTL) VALUES (N'Công nghệ thông tin'),(N'Văn học'),(N'Kinh tế'),(N'Khoa học'),(N'Lịch sử'),(N'Kỹ năng sống'),(N'Thiếu nhi');

IF NOT EXISTS (SELECT 1 FROM Sach)
INSERT Sach(TenSach,TacGia,NhaXuatBan,NamXB,MaTL,SoLuong,GiaBia,ViTri) VALUES
 (N'Lập trình C# từ cơ bản đến nâng cao', N'Phạm Hữu Khang', N'NXB Lao Động', 2020, 1, 8, 185000, N'Kệ A1'),
 (N'Cấu trúc dữ liệu và giải thuật', N'Nguyễn Đức Nghĩa', N'NXB Giáo Dục', 2019, 1, 6, 120000, N'Kệ A1'),
 (N'Clean Code', N'Robert C. Martin', N'NXB Thế Giới', 2021, 1, 4, 250000, N'Kệ A2'),
 (N'Cơ sở dữ liệu SQL Server', N'Lê Minh Trí', N'NXB Thanh Niên', 2022, 1, 5, 160000, N'Kệ A2'),
 (N'Dế Mèn phiêu lưu ký', N'Tô Hoài', N'NXB Kim Đồng', 2018, 7, 10, 55000, N'Kệ D1'),
 (N'Số đỏ', N'Vũ Trọng Phụng', N'NXB Văn Học', 2017, 2, 5, 75000, N'Kệ B1'),
 (N'Nhà giả kim', N'Paulo Coelho', N'NXB Hội Nhà Văn', 2020, 2, 7, 79000, N'Kệ B1'),
 (N'Đắc nhân tâm', N'Dale Carnegie', N'NXB Tổng Hợp TP.HCM', 2019, 6, 9, 86000, N'Kệ E1'),
 (N'Kinh tế học vĩ mô', N'N. Gregory Mankiw', N'NXB Hồng Đức', 2021, 3, 4, 210000, N'Kệ C1'),
 (N'Lược sử thời gian', N'Stephen Hawking', N'NXB Trẻ', 2018, 4, 6, 98000, N'Kệ F1'),
 (N'Sapiens: Lược sử loài người', N'Yuval Noah Harari', N'NXB Tổng Hợp', 2020, 5, 6, 189000, N'Kệ G1'),
 (N'Tư duy nhanh và chậm', N'Daniel Kahneman', N'NXB Thế Giới', 2021, 6, 5, 169000, N'Kệ E1');

IF NOT EXISTS (SELECT 1 FROM DocGia)
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT,Email,DiaChi) VALUES
 (N'Nguyễn Văn An',   N'Nam',  '2004-03-12', '0901234567', N'an.nguyen@gmail.com',   N'Quận 1, TP.HCM'),
 (N'Trần Thị Bích',   N'Nữ',   '2003-07-25', '0912345678', N'bich.tran@gmail.com',   N'Quận 3, TP.HCM'),
 (N'Lê Hoàng Cường',  N'Nam',  '2002-11-02', '0933456789', N'cuong.le@gmail.com',    N'Thủ Đức, TP.HCM'),
 (N'Phạm Thu Dung',   N'Nữ',   '2005-01-18', '0944567890', N'dung.pham@gmail.com',   N'Quận 7, TP.HCM'),
 (N'Võ Minh Em',      N'Khác', '2001-09-30', '0955678901', N'em.vo@gmail.com',       N'Bình Thạnh, TP.HCM'),
 (N'Đặng Quốc Phong', N'Nam',  '2004-05-09', '0966789012', N'phong.dang@gmail.com',  N'Gò Vấp, TP.HCM');

IF NOT EXISTS (SELECT 1 FROM PhieuMuon)
BEGIN
  DECLARE @t DATE = CAST(GETDATE() AS DATE);
  INSERT PhieuMuon(MaDG,MaSach,NgayMuon,HanTra,NgayTra,TienPhat) VALUES
   (1, 1,  DATEADD(DAY,-20,@t), DATEADD(DAY,-6,@t),  NULL, 0),                      -- quá hạn
   (2, 3,  DATEADD(DAY,-10,@t), DATEADD(DAY,4,@t),   NULL, 0),                      -- đang mượn
   (3, 5,  DATEADD(DAY,-30,@t), DATEADD(DAY,-16,@t), DATEADD(DAY,-14,@t), 4000),    -- trả trễ
   (4, 8,  DATEADD(DAY,-5,@t),  DATEADD(DAY,9,@t),   NULL, 0),
   (1, 2,  DATEADD(DAY,-45,@t), DATEADD(DAY,-31,@t), DATEADD(DAY,-33,@t), 0),
   (5, 7,  DATEADD(DAY,-18,@t), DATEADD(DAY,-4,@t),  NULL, 0),                      -- quá hạn
   (6, 12, DATEADD(DAY,-60,@t), DATEADD(DAY,-46,@t), DATEADD(DAY,-50,@t), 0),
   (2, 8,  DATEADD(DAY,-75,@t), DATEADD(DAY,-61,@t), DATEADD(DAY,-62,@t), 0);
END
GO
PRINT N'Hoàn tất tạo CSDL QuanLyThuVienNova.';

/* BỔ SUNG PHIẾU PHẠT / BỒI THƯỜNG - KHÔNG XÓA DỮ LIỆU CŨ.
   Server: BAONGUYEN\BAONGUYEN1 - Windows Authentication
   Chạy trong SSMS bằng Execute (F5) SAU KHI đã có database QuanLyThuVienNova.
   Có thể chạy lại an toàn: không xóa bảng, không xóa dữ liệu, không đổi tài khoản.
*/
USE QuanLyThuVienNova;
GO
IF OBJECT_ID(N'dbo.PhieuMuon', N'U') IS NULL
    THROW 51000, N'Chưa có bảng PhieuMuon. Hãy chạy QuanLyThuVienNova.sql trước.', 1;
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

/* Chạy sau 2 script gốc. Bổ sung, không xóa dữ liệu. SQL Server 2016 SP1+. */
IF COL_LENGTH('dbo.PhieuMuon','SoLanGiaHan') IS NULL
 ALTER TABLE dbo.PhieuMuon ADD SoLanGiaHan INT NOT NULL CONSTRAINT DF_PM_GiaHan DEFAULT 0;
GO
IF OBJECT_ID('dbo.DatTruoc') IS NULL
 CREATE TABLE dbo.DatTruoc(
  MaDat INT IDENTITY PRIMARY KEY, MaDG INT NOT NULL REFERENCES dbo.DocGia(MaDG),
  MaSach INT NOT NULL REFERENCES dbo.Sach(MaSach), NgayDat DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Đang chờ' CHECK(TrangThai IN(N'Đang chờ',N'Đã nhận',N'Đã hủy')),
  NgayXuLy DATETIME2 NULL);
GO
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='UX_DatTruoc_Cho' AND object_id=OBJECT_ID('dbo.DatTruoc'))
 CREATE UNIQUE INDEX UX_DatTruoc_Cho ON dbo.DatTruoc(MaDG,MaSach) WHERE TrangThai=N'Đang chờ';
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_DatTruoc_HangCho' AND object_id=OBJECT_ID('dbo.DatTruoc'))
 CREATE INDEX IX_DatTruoc_HangCho ON dbo.DatTruoc(MaSach,TrangThai,MaDat);
IF OBJECT_ID('dbo.NhatKyThaoTac') IS NULL
 CREATE TABLE dbo.NhatKyThaoTac(MaLog BIGINT IDENTITY PRIMARY KEY, ThoiGian DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
 NguoiDung NVARCHAR(128) NOT NULL, DoiTuong NVARCHAR(50) NOT NULL, HanhDong NVARCHAR(20) NOT NULL, MaDoiTuong INT NOT NULL);
GO
CREATE OR ALTER PROCEDURE dbo.NovaLock AS
BEGIN
 DECLARE @r INT;
 EXEC @r=sys.sp_getapplock @Resource=N'NovaCirculation',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=10000;
 IF @r<0 THROW 52000,N'Hệ thống đang xử lý giao dịch khác. Vui lòng thử lại.',1;
END;
GO
CREATE OR ALTER PROCEDURE dbo.NovaMuon @dg INT,@s INT,@nm DATE,@ht DATE AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN;
 EXEC dbo.NovaLock;
 IF @ht<@nm OR @nm>CAST(GETDATE() AS DATE) THROW 52001,N'Ngày mượn hoặc hạn trả không hợp lệ.',1;
 IF NOT EXISTS(SELECT 1 FROM DocGia WHERE MaDG=@dg) THROW 52002,N'Độc giả không tồn tại.',1;
 IF EXISTS(SELECT 1 FROM PhieuMuon WHERE MaDG=@dg AND NgayTra IS NULL AND HanTra<CAST(GETDATE() AS DATE))
  THROW 52003,N'Độc giả có sách quá hạn, cần trả trước khi mượn thêm.',1;
 IF (SELECT COUNT(*) FROM PhieuMuon WHERE MaDG=@dg AND NgayTra IS NULL)>=5
  THROW 52004,N'Mỗi độc giả được mượn tối đa 5 cuốn.',1;
 IF EXISTS(SELECT 1 FROM PhieuMuon WHERE MaDG=@dg AND MaSach=@s AND NgayTra IS NULL)
  THROW 52005,N'Độc giả đang mượn đầu sách này.',1;
 DECLARE @first INT=(SELECT TOP 1 MaDG FROM DatTruoc WHERE MaSach=@s AND TrangThai=N'Đang chờ' ORDER BY MaDat);
 IF @first IS NOT NULL AND @first<>@dg THROW 52006,N'Sách được ưu tiên cho độc giả đứng đầu hàng chờ.',1;
 UPDATE Sach SET SoLuong=SoLuong-1 WHERE MaSach=@s AND SoLuong>0;
 IF @@ROWCOUNT=0 THROW 52007,N'Sách đã hết, hãy lập đặt trước.',1;
 INSERT PhieuMuon(MaDG,MaSach,NgayMuon,HanTra) VALUES(@dg,@s,@nm,@ht);
 UPDATE DatTruoc SET TrangThai=N'Đã nhận',NgayXuLy=SYSDATETIME() WHERE MaDG=@dg AND MaSach=@s AND TrangThai=N'Đang chờ';
 COMMIT;
END;
GO
CREATE OR ALTER PROCEDURE dbo.NovaGiaHan @id INT AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN; EXEC dbo.NovaLock;
 DECLARE @s INT,@han DATE,@lan INT;
 SELECT @s=MaSach,@han=HanTra,@lan=SoLanGiaHan FROM PhieuMuon WITH(UPDLOCK) WHERE MaPM=@id AND NgayTra IS NULL;
 IF @s IS NULL THROW 52008,N'Phiếu không tồn tại hoặc đã trả.',1;
 IF @han<CAST(GETDATE() AS DATE) THROW 52009,N'Phiếu quá hạn không được gia hạn.',1;
 IF @lan>=2 THROW 52010,N'Đã đủ 2 lần gia hạn.',1;
 IF EXISTS(SELECT 1 FROM DatTruoc WHERE MaSach=@s AND TrangThai=N'Đang chờ')
  THROW 52011,N'Có độc giả đang chờ sách, không thể gia hạn.',1;
 UPDATE PhieuMuon SET HanTra=DATEADD(DAY,7,HanTra),SoLanGiaHan=SoLanGiaHan+1 WHERE MaPM=@id;
 COMMIT;
END;
GO
CREATE OR ALTER PROCEDURE dbo.NovaDat @dg INT,@s INT AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN; EXEC dbo.NovaLock;
 IF NOT EXISTS(SELECT 1 FROM Sach WHERE MaSach=@s) OR NOT EXISTS(SELECT 1 FROM DocGia WHERE MaDG=@dg)
  THROW 52012,N'Độc giả hoặc sách không tồn tại.',1;
 IF EXISTS(SELECT 1 FROM PhieuMuon WHERE MaDG=@dg AND MaSach=@s AND NgayTra IS NULL)
  THROW 52013,N'Độc giả đang mượn đầu sách này.',1;
 IF EXISTS(SELECT 1 FROM DatTruoc WHERE MaDG=@dg AND MaSach=@s AND TrangThai=N'Đang chờ')
  THROW 52014,N'Độc giả đã có trong hàng chờ.',1;
 IF (SELECT COUNT(*) FROM DatTruoc WHERE MaDG=@dg AND TrangThai=N'Đang chờ')>=3
  THROW 52015,N'Mỗi độc giả được đặt trước tối đa 3 đầu sách.',1;
 INSERT DatTruoc(MaDG,MaSach) VALUES(@dg,@s);
 COMMIT;
END;
GO
CREATE OR ALTER PROCEDURE dbo.NovaHuyDat @id INT AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN; EXEC dbo.NovaLock;
 UPDATE DatTruoc SET TrangThai=N'Đã hủy',NgayXuLy=SYSDATETIME() WHERE MaDat=@id AND TrangThai=N'Đang chờ';
 IF @@ROWCOUNT=0 THROW 52016,N'Chỉ hủy được đặt trước đang chờ.',1;
 COMMIT;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_Nova_Sach ON dbo.Sach AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.NhatKyThaoTac(NguoiDung,DoiTuong,HanhDong,MaDoiTuong)
 SELECT COALESCE(CONVERT(nvarchar(128),SESSION_CONTEXT(N'Actor')),ORIGINAL_LOGIN()),N'Sach',
 CASE WHEN i.MaSach IS NULL THEN N'Xóa' WHEN d.MaSach IS NULL THEN N'Thêm' ELSE N'Cập nhật' END,
 COALESCE(i.MaSach,d.MaSach) FROM inserted i FULL JOIN deleted d ON i.MaSach=d.MaSach;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_Nova_DocGia ON dbo.DocGia AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.NhatKyThaoTac(NguoiDung,DoiTuong,HanhDong,MaDoiTuong)
 SELECT COALESCE(CONVERT(nvarchar(128),SESSION_CONTEXT(N'Actor')),ORIGINAL_LOGIN()),N'DocGia',
 CASE WHEN i.MaDG IS NULL THEN N'Xóa' WHEN d.MaDG IS NULL THEN N'Thêm' ELSE N'Cập nhật' END,
 COALESCE(i.MaDG,d.MaDG) FROM inserted i FULL JOIN deleted d ON i.MaDG=d.MaDG;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_Nova_PhieuMuon ON dbo.PhieuMuon AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.NhatKyThaoTac(NguoiDung,DoiTuong,HanhDong,MaDoiTuong)
 SELECT COALESCE(CONVERT(nvarchar(128),SESSION_CONTEXT(N'Actor')),ORIGINAL_LOGIN()),N'PhieuMuon',
 CASE WHEN i.MaPM IS NULL THEN N'Xóa' WHEN d.MaPM IS NULL THEN N'Thêm' ELSE N'Cập nhật' END,
 COALESCE(i.MaPM,d.MaPM) FROM inserted i FULL JOIN deleted d ON i.MaPM=d.MaPM;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_Nova_PhieuPhat ON dbo.PhieuPhat AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.NhatKyThaoTac(NguoiDung,DoiTuong,HanhDong,MaDoiTuong)
 SELECT COALESCE(CONVERT(nvarchar(128),SESSION_CONTEXT(N'Actor')),ORIGINAL_LOGIN()),N'PhieuPhat',
 CASE WHEN i.MaPhat IS NULL THEN N'Xóa' WHEN d.MaPhat IS NULL THEN N'Thêm' ELSE N'Cập nhật' END,
 COALESCE(i.MaPhat,d.MaPhat) FROM inserted i FULL JOIN deleted d ON i.MaPhat=d.MaPhat;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_Nova_DatTruoc ON dbo.DatTruoc AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.NhatKyThaoTac(NguoiDung,DoiTuong,HanhDong,MaDoiTuong)
 SELECT COALESCE(CONVERT(nvarchar(128),SESSION_CONTEXT(N'Actor')),ORIGINAL_LOGIN()),N'DatTruoc',
 CASE WHEN i.MaDat IS NULL THEN N'Xóa' WHEN d.MaDat IS NULL THEN N'Thêm' ELSE N'Cập nhật' END,
 COALESCE(i.MaDat,d.MaDat) FROM inserted i FULL JOIN deleted d ON i.MaDat=d.MaDat;
END;
GO
