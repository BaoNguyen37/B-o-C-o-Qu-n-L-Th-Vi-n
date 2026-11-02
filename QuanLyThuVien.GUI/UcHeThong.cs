using System.Data;
using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;
public partial class UcHeThong:UserControl
{
    public UcHeThong()
    {
        InitializeComponent();
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        Theme.StyleGrid(grid);
        btnCheck.Click += (_,__) => Check();
        btnExport.Click += (_,__) => Util.ExportGrid(grid,"KiemTraHeThong");
        KeyDown += (_,e) => {if(e.KeyCode==Keys.F5){e.Handled=true;Check();}};
        KeyPreviewForGrid();
        Check();
    }
    void KeyPreviewForGrid() {
        grid.KeyDown += (_,e) => {if(e.KeyCode==Keys.F5){e.Handled=true;Check();}};
    }
    void Check()
    {
        var report=new DataTable();
        report.Columns.Add("HangMuc");report.Columns.Add("KetQua");report.Columns.Add("TrangThai");report.Columns.Add("GhiChu");
        void Add(string name,object result,string status,string note="") => report.Rows.Add(name,Convert.ToString(result),status,note);
        if(QuanLyThuVien.DAL.PreviewData.Enabled) Add("Chế độ xem thử","Dữ liệu mẫu","Chỉ đọc","Không kết nối SQL Server. Kết quả hiển thị không xác nhận trạng thái database thật.");
        else try {
            var d=HeThongBLL.TongQuan().Rows[0];
            Add("SQL Server",$"{d["TenServer"]} / {d["TenDatabase"]}","OK","Đã truy vấn được dữ liệu; chỉ SELECT");
            Add("Đầu sách",d["SoDauSach"],"OK");Add("Độc giả",d["SoDocGia"],"OK");
            int over=Convert.ToInt32(d["QuaHan"]),debt=Convert.ToInt32(d["ChuaThu"]);
            Add("Đang mượn",d["DangMuon"],"OK");
            Add("Phiếu quá hạn",over,over>0?"Cần xử lý":"OK","Mở Mượn/Trả hoặc Nhắc hạn qua Gmail");
            Add("Phiếu phạt chưa thu",debt,debt>0?"Cần xử lý":"OK","Mở Phiếu phạt/Đền sách để xác nhận thu");
            Add("Tiền phạt chưa thu",$"{Convert.ToDecimal(d["CongNo"]):N0} đ",debt>0?"Cần xử lý":"OK");
            Add("Email đã gửi hôm nay",d["EmailDaGuiHomNay"],"Thông tin","Số thư được ghi nhận trong SQL, không phải kết quả inbox");
        }
        catch (Exception ex) { Add("SQL Server / Cấu trúc bảng","Không kiểm tra được","Lỗi",ex.GetBaseException().Message); }
        var bank=CauHinhThanhToan.Load();
        Add("Tài khoản nhận tiền MB Bank",CauHinhThanhToan.SoTaiKhoanHopLe(bank.SoTaiKhoan)?"Đã cấu hình":"Chưa cấu hình",CauHinhThanhToan.SoTaiKhoanHopLe(bank.SoTaiKhoan)?"OK":"Cần xử lý","Chỉ lưu trên máy này; cần kiểm tra tên người nhận khi quét QR");
        var gmail=CauHinhGmail.Load();
        bool configured=!string.IsNullOrWhiteSpace(gmail.DiaChiGui) && !string.IsNullOrWhiteSpace(gmail.MatKhauDaMaHoa);
        Add("Gmail nhắc hạn",configured?"Đã cấu hình":"Chưa đủ cấu hình",configured?"Thông tin":"Cần xử lý","Không thử gửi thư và không hiển thị mật khẩu");
        Add("Tự gửi Gmail",gmail.TuDongNhac?"Đã bật (khi ứng dụng mở)":"Tắt","Thông tin","Tự gửi không hoạt động khi ứng dụng đóng");
        Add("Tình trạng thanh toán QR","Thủ công","Thông tin","QR không tự kiểm tra sao kê; cần người thu xác nhận");
        grid.DataSource=report;
        Theme.Cols(grid,"HangMuc=Hạng mục","KetQua=Kết quả","TrangThai=Tình trạng","GhiChu=Ghi chú");
        Theme.Weight(grid,140,"HangMuc");Theme.Weight(grid,135,"KetQua");Theme.Weight(grid,95,"TrangThai");Theme.Weight(grid,240,"GhiChu");
        grid.ClearSelection();lblTime.Text="Kiểm tra lúc "+DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
