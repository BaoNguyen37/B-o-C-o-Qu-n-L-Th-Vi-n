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
