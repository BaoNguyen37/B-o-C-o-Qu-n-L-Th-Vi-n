/* QUAN LY THU VIEN - SQL SERVER BAONGUYEN\BAONGUYEN1
   KET NOI SSMS: Windows Authentication.
   Chay F5 toan bo file trong SSMS MOT LAN TRUOC KHI CHAY APP.
   Tao database rieng QuanLyThuVien; KHONG thay doi QLThuVienCongCong.
   Script co the chay lai: bao toan bang va du lieu da co.
*/
USE master;
GO
IF DB_ID(N'QuanLyThuVien') IS NULL
BEGIN
    CREATE DATABASE QuanLyThuVien;
END;
GO
USE QuanLyThuVien;
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
PRINT N'Hoàn tất tạo CSDL QuanLyThuVien.';
