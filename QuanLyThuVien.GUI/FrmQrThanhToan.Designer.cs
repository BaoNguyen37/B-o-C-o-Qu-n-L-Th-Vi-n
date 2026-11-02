namespace QuanLyThuVien.GUI;
// UI có thể sửa bằng WinForms Designer / Toolbox
partial class FrmQrThanhToan
{
    private System.ComponentModel.IContainer components = null;
    private PictureBox picture;
    private Label lblTitle,lblStatus,lblDetail;
    private Button btnCopy,btnOpen,btnClose;
    protected override void Dispose(bool disposing) { if(disposing){picture?.Image?.Dispose(); components?.Dispose();} base.Dispose(disposing); }
    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.Text="Mã VietQR thanh toán MB Bank";this.Size=new Size(610,760);
        this.StartPosition=FormStartPosition.CenterParent;this.MinimumSize=new Size(540,650);
        this.BackColor=Color.White;
        lblTitle=new Label{ Name="lblTitle", Text="THANH TOÁN PHIẾU PHẠT", Font=new Font("Segoe UI",16F,FontStyle.Bold),AutoSize=true,Location=new Point(36,20)};
        lblDetail=new Label{Name="lblDetail",Location=new Point(36,70),Size=new Size(520,85),Font=new Font("Segoe UI",10.5F)};
        picture=new PictureBox{Name="picture",Location=new Point(85,155),Size=new Size(420,450),SizeMode=PictureBoxSizeMode.Zoom,BackColor=Color.White,BorderStyle=BorderStyle.FixedSingle};
        lblStatus=new Label{Name="lblStatus",Text="Đang tải VietQR...",Location=new Point(36,612),Size=new Size(530,34),TextAlign=ContentAlignment.MiddleCenter};
        btnCopy=new Button{Name="btnCopy",Text="Sao chép liên kết",Location=new Point(34,660),Size=new Size(165,37),BackColor=Color.FromArgb(99,102,241),ForeColor=Color.White,FlatStyle=FlatStyle.Flat};
        btnOpen=new Button{Name="btnOpen",Text="Mở QR trên web",Location=new Point(213,660),Size=new Size(160,37),BackColor=Color.FromArgb(14,165,233),ForeColor=Color.White,FlatStyle=FlatStyle.Flat};
        btnClose=new Button{Name="btnClose",Text="Đóng",Location=new Point(387,660),Size=new Size(165,37)};
        this.Controls.AddRange(new Control[]{lblTitle,lblDetail,picture,lblStatus,btnCopy,btnOpen,btnClose});
        this.ResumeLayout(false);
    }
}
