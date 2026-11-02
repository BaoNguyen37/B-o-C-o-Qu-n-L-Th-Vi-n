using System.Globalization;
using System.Text;

namespace QuanLyThuVien.GUI;

/// <summary>
/// Tạo dữ liệu chuyển khoản VietQR/NAPAS theo EMVCo TLV ngay trên máy.
/// Không truy cập ngân hàng, không tự xác nhận giao dịch.
/// </summary>
public static class VietQrNoiBo
{
    private static string Tag(string id, string value)
    {
        if (id.Length != 2 || value.Length > 99)
            throw new ArgumentException("Trường dữ liệu VietQR không hợp lệ.");
        return id + value.Length.ToString("D2", CultureInfo.InvariantCulture) + value;
    }

    public static string TaoDuLieu(string soTaiKhoan, int maPhat, decimal soTien)
    {
        if (!CauHinhThanhToan.SoTaiKhoanHopLe(soTaiKhoan))
            throw new ArgumentException("Số tài khoản MB Bank không hợp lệ (6–19 chữ số).");
        if (maPhat <= 0)
            throw new ArgumentException("Mã phiếu phạt không hợp lệ.");
        if (soTien <= 0 || soTien != decimal.Truncate(soTien))
            throw new ArgumentException("Số tiền chuyển khoản phải là số nguyên dương.");

        // BIN của MB Bank: 970422. QRIBFTTA = chuyển khoản vào tài khoản.
        string nguoiThuHuong = Tag("00", "970422") + Tag("01", soTaiKhoan);
        string thongTinNganHang = Tag("00", "A000000727")
                                  + Tag("01", nguoiThuHuong)
                                  + Tag("02", "QRIBFTTA");
        string noiDung = $"PHAT TL{maPhat}";
        string payload = Tag("00", "01")
                       + Tag("01", "12")
                       + Tag("38", thongTinNganHang)
                       + Tag("53", "704")
                       + Tag("54", soTien.ToString("0", CultureInfo.InvariantCulture))
                       + Tag("58", "VN")
                       + Tag("62", Tag("08", noiDung));

        // CRC-16/CCITT-FALSE: poly 0x1021, init 0xFFFF, bao gồm tiền tố tag 63.
        string truocCrc = payload + "6304";
        ushort crc = 0xFFFF;
        foreach (byte b in Encoding.ASCII.GetBytes(truocCrc))
        {
            crc ^= (ushort)(b << 8);
            for (int i = 0; i < 8; ++i)
                crc = (ushort)((crc & 0x8000) != 0 ? (crc << 1) ^ 0x1021 : crc << 1);
        }
        return truocCrc + crc.ToString("X4", CultureInfo.InvariantCulture);
    }
}
