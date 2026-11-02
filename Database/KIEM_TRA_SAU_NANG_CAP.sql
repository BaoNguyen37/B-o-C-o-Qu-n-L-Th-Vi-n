-- CHỈ TRUY VẤN (SELECT), KHÔNG SỬA DỮ LIỆU
USE QuanLyThuVien;
GO
SELECT 'PhieuPhat' AS Bang, COUNT(*) AS SoDong FROM dbo.PhieuPhat;
SELECT 'NhatKyEmail' AS Bang, COUNT(*) AS SoDong FROM dbo.NhatKyEmail;
SELECT TOP 50 MaPhat,MaPM,LoaiPhat,SoTien,TrangThai,NgayThu,PhuongThuc,MaGiaoDich,NguoiLap,NguoiThu FROM dbo.PhieuPhat ORDER BY MaPhat DESC;
SELECT TOP 50 MaPM,LoaiNhac,NgayNhac,EmailNhan,TrangThai,Loi FROM dbo.NhatKyEmail ORDER BY MaLog DESC;
SELECT pm.MaPM,pm.TinhTrangXuLy,pm.NgayTra,pm.TienPhat, s.SoLuong FROM dbo.PhieuMuon pm
JOIN dbo.Sach s ON s.MaSach=pm.MaSach ORDER BY pm.MaPM DESC;
