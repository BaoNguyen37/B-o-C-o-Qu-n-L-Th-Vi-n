/* KIỂM THỬ TÍCH HỢP: chạy SAU 00_CAI_MOI_NOVA.sql.
Mỗi ca dùng dữ liệu tạm trong transaction rồi ROLLBACK. Không gửi email.
Khuyến nghị chạy trên database thử nghiệm. Chưa thực thi trên máy tạo bản bàn giao vì SQL Server không nhận kết nối.
*/
USE QuanLyThuVienNova;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
EXEC dbo.NovaMuon @dg,@s,@today,@due; IF (SELECT SoLuong FROM Sach WHERE MaSach=@s)<>1 THROW 52999,N'Tồn kho sai',1;
 ROLLBACK; PRINT N'PASS: Mượn trừ tồn kho';
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 THROW;
END CATCH;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
EXEC dbo.NovaDat @dg,@s; EXEC dbo.NovaDat @dg,@s;
 THROW 52999,N'Không chặn được thao tác không hợp lệ',1;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>52014 THROW;
 PRINT N'PASS: Chặn đặt trước trùng';
END CATCH;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
EXEC dbo.NovaDat @dg,@s;EXEC dbo.NovaDat @dg2,@s;EXEC dbo.NovaMuon @dg2,@s,@today,@due;
 THROW 52999,N'Không chặn được thao tác không hợp lệ',1;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>52006 THROW;
 PRINT N'PASS: Ưu tiên đầu hàng chờ';
END CATCH;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
EXEC dbo.NovaMuon @dg,@s,@today,@due;SELECT @pm=MaPM FROM PhieuMuon WHERE MaDG=@dg;EXEC dbo.NovaGiaHan @pm;EXEC dbo.NovaGiaHan @pm;EXEC dbo.NovaGiaHan @pm;
 THROW 52999,N'Không chặn được thao tác không hợp lệ',1;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>52010 THROW;
 PRINT N'PASS: Giới hạn 2 lần gia hạn';
END CATCH;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
INSERT PhieuMuon(MaDG,MaSach,NgayMuon,HanTra) VALUES(@dg,@s,DATEADD(DAY,-20,@today),DATEADD(DAY,-1,@today));SET @pm=SCOPE_IDENTITY();EXEC dbo.NovaGiaHan @pm;
 THROW 52999,N'Không chặn được thao tác không hợp lệ',1;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>52009 THROW;
 PRINT N'PASS: Chặn gia hạn quá hạn';
END CATCH;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
EXEC dbo.NovaMuon @dg,@s,@today,@due;SELECT @pm=MaPM FROM PhieuMuon WHERE MaDG=@dg;EXEC dbo.NovaDat @dg2,@s;EXEC dbo.NovaGiaHan @pm;
 THROW 52999,N'Không chặn được thao tác không hợp lệ',1;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>52011 THROW;
 PRINT N'PASS: Chặn gia hạn khi có người chờ';
END CATCH;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
UPDATE Sach SET SoLuong=0 WHERE MaSach=@s;EXEC dbo.NovaMuon @dg,@s,@today,@due;
 THROW 52999,N'Không chặn được thao tác không hợp lệ',1;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>52007 THROW;
 PRINT N'PASS: Chặn mượn hết sách';
END CATCH;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
EXEC dbo.NovaDat @dg,@s;EXEC dbo.NovaMuon @dg,@s,@today,@due;IF NOT EXISTS(SELECT 1 FROM DatTruoc WHERE MaDG=@dg AND MaSach=@s AND TrangThai=N'Đã nhận') THROW 52999,N'Chưa kết thúc hàng chờ',1;
 ROLLBACK; PRINT N'PASS: Nhận sách hoàn tất hàng chờ';
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 THROW;
END CATCH;
GO
IF @@TRANCOUNT<>0 THROW 52998,N'Hãy kết thúc transaction hiện tại trước khi chạy kiểm thử.',1;
BEGIN TRY
 BEGIN TRAN;
DECLARE @dg INT,@dg2 INT,@s INT,@tl INT=(SELECT TOP 1 MaTL FROM TheLoai ORDER BY MaTL);
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 1',N'Nam','20040101','0900000000'); SET @dg=SCOPE_IDENTITY();
INSERT DocGia(HoTen,GioiTinh,NgaySinh,SDT) VALUES(N'NOVA test 2',N'Nữ','20040101','0900000001'); SET @dg2=SCOPE_IDENTITY();
INSERT Sach(TenSach,TacGia,MaTL,SoLuong,GiaBia) VALUES(N'NOVA test',N'Test',@tl,2,100000);SET @s=SCOPE_IDENTITY();
DECLARE @today DATE=CAST(GETDATE() AS DATE),@due DATE=DATEADD(DAY,14,CAST(GETDATE() AS DATE)),@pm INT;
EXEC dbo.NovaMuon @dg,@s,@today,@due;EXEC dbo.NovaMuon @dg,@s,@today,@due;
 THROW 52999,N'Không chặn được thao tác không hợp lệ',1;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>52005 THROW;
 PRINT N'PASS: Chặn mượn trùng đầu sách';
END CATCH;
GO
