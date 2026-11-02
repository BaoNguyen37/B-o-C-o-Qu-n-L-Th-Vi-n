using System.Diagnostics;
using QRCoder;

namespace QuanLyThuVien.GUI;

public partial class FrmQrThanhToan : Form
{
    readonly string link = "";

    public FrmQrThanhToan()
    {
        InitializeComponent();
        lblDetail.Text = "MB BANK  |  NGUYEN BAO NGUYEN\nMã QR được tạo khi chọn phiếu phạt trong ứng dụng.";
        lblStatus.Text = "Xem trước giao diện trong Windows Forms Designer.";
    }

    public FrmQrThanhToan(int maPhat, decimal amount) : this()
    {
        var cfg = CauHinhThanhToan.Load();
        // Giữ nguyên liên kết VietQR cho chức năng mở trên web và sao chép.
        link = cfg.VietQr(maPhat, amount);
        lblDetail.Text = $"MB BANK  |  Chủ tài khoản: {cfg.TenChuTaiKhoan}\n" +
                         $"Số tài khoản: {cfg.SoTaiKhoan}\n" +
                         $"Phiếu phạt #{maPhat}  •  Số tiền: {amount:N0} đ";

        btnCopy.Click += (_, __) =>
        {
            Clipboard.SetText(link);
            Theme.Info("Đã sao chép liên kết QR. Không chia sẻ nếu không muốn lộ thông tin chuyển khoản.");
        };
        btnOpen.Click += (_, __) =>
        {
            try { Process.Start(new ProcessStartInfo(link) { UseShellExecute = true }); }
            catch (Exception ex) { Theme.Error("Không mở được trình duyệt: " + ex.Message); }
        };
        btnClose.Click += (_, __) => Close();

        // Tạo QR VietQR hợp lệ TRỰC TIẾP trên máy. Không phụ thuộc img.vietqr.io,
        // không tải dữ liệu HTTP về rồi cố đọc HTML thành ảnh PNG nữa.
        Shown += (_, __) =>
        {
            try
            {
                string payload = VietQrNoiBo.TaoDuLieu(cfg.SoTaiKhoan, maPhat, amount);
                using var qrData = QRCodeGenerator.GenerateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                using var qr = new PngByteQRCode(qrData);
                byte[] png = qr.GetGraphic(8);
                using var stream = new MemoryStream(png);
                using var image = Image.FromStream(stream);
                picture.Image?.Dispose();
                picture.Image = new Bitmap(image);
                lblStatus.Text = "QR tạo trên máy. Quét thử và kiểm tra ĐÚNG MB Bank, tên, STK, số tiền trước khi chuyển.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Không tạo được QR: " + ex.GetBaseException().Message;
            }
        };
    }
}
