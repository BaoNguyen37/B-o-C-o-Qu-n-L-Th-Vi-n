using QuanLyThuVien.BLL;
using QuanLyThuVien.GUI;
int ok=0,failed=0;
void Assert(string description,bool value)
{
    if(value){Console.WriteLine("PASS " + description);ok++;}
    else{Console.WriteLine("FAIL " + description);failed++;}
}
void MustThrow(string description, Action action)
{
    try{action();Assert(description,false);}catch{Assert(description,true);}
}
var today=new DateTime(2026,10,29);
Assert("Trả đúng hạn: không phạt",PhieuMuonBLL.TinhPhat(today,today)==0);
Assert("Trả sớm: không phạt",PhieuMuonBLL.TinhPhat(today,today.AddDays(-1))==0);
Assert("Trả trễ 1 ngày: 2.000",PhieuMuonBLL.TinhPhat(today,today.AddDays(1))==2000);
Assert("Trả trễ 5 ngày: 10.000",PhieuMuonBLL.TinhPhat(today,today.AddDays(5))==10000);
Assert("Sang tháng vẫn tính đúng",PhieuMuonBLL.TinhPhat(new DateTime(2026,10,31),new DateTime(2026,11,2))==4000);
Assert("Email Gmail hợp lệ",NhacTraBLL.EmailHopLe("thuthu@gmail.com"));
Assert("Email trắng không hợp lệ",!NhacTraBLL.EmailHopLe(""));
Assert("Email sai định dạng bị chặn",!NhacTraBLL.EmailHopLe("not-an-email"));
Assert("Nội dung thư có hạn trả và tên sách",NhacTraBLL.NoiDung("Nguyen", "C#", 5,today,-2).Contains("quá hạn 2 ngày"));
var bank = new CauHinhThanhToan {SoTaiKhoan="1234567890"};
Assert("QR đúng MB BIN",bank.VietQr(99,10000).Contains("/970422-1234567890-"));
Assert("QR kèm đúng số tiền",bank.VietQr(99,10000).Contains("amount=10000"));
Assert("QR kèm mã phiếu phạt",bank.VietQr(99,10000).Contains("PHAT%20TL99"));
var qrOffline = VietQrNoiBo.TaoDuLieu("1234567890", 99, 10000);
Assert("QR nội bộ có NAPAS GUID", qrOffline.Contains("A000000727"));
Assert("QR nội bộ có mã MB Bank", qrOffline.Contains("970422"));
Assert("QR nội bộ có đúng STK", qrOffline.Contains("1234567890"));
Assert("QR nội bộ có số tiền 10.000đ", qrOffline.Contains("540510000"));
Assert("QR nội bộ có mã phiếu phạt", qrOffline.Contains("PHAT TL99"));
Assert("QR nội bộ có CRC 4 ký tự", qrOffline.Substring(qrOffline.Length - 8, 4) == "6304");
MustThrow("QR nội bộ từ chối STK sai", () => VietQrNoiBo.TaoDuLieu("abc", 1, 1000));
MustThrow("QR nội bộ từ chối tiền âm", () => VietQrNoiBo.TaoDuLieu("1234567890", 1, -1000));

MustThrow("QR từ chối số tiền 0",()=>bank.VietQr(99,0));
MustThrow("Không có STK không tạo QR",()=>new CauHinhThanhToan().VietQr(1,1000));
Assert("STK MB Bank hợp lệ",CauHinhThanhToan.SoTaiKhoanHopLe("160120083706"));
Assert("STK có số 0 đầu vẫn hợp lệ",CauHinhThanhToan.SoTaiKhoanHopLe("0012345678"));
Assert("STK thiếu số bị chặn",!CauHinhThanhToan.SoTaiKhoanHopLe("12345"));
Assert("STK vượt 19 số bị chặn",!CauHinhThanhToan.SoTaiKhoanHopLe(new string('1',20)));
Assert("STK chứa ký tự chữ bị chặn",!CauHinhThanhToan.SoTaiKhoanHopLe("123456ABC"));
Assert("STK chứa dấu cách bị chặn",!CauHinhThanhToan.SoTaiKhoanHopLe("123 456789"));
Assert("STK rỗng bị chặn",!CauHinhThanhToan.SoTaiKhoanHopLe(""));
MustThrow("VietQR từ chối STK sai định dạng",()=>new CauHinhThanhToan{SoTaiKhoan="XYZ123456"}.VietQr(1,5000));
MustThrow("VietQR từ chối tiền âm",()=>bank.VietQr(1,-2000));
MustThrow("VietQR từ chối tiền lẻ",()=>bank.VietQr(1,1200.5m));
Assert("Nội dung chuyển tiền có mã phiếu 8",bank.VietQr(8,4000).Contains("PHAT%20TL8"));
Assert("Ngày trả trễ 0 ngày không bị tính phạt",PhieuMuonBLL.TinhPhat(today,today)==0);
Assert("Mã thẻ QR round-trip", ReaderCode.TryParse(ReaderCode.Create(123),out var cardId)&&cardId==123);
Assert("Mã thẻ chấp nhận chữ thường", ReaderCode.TryParse("nova:dg:000001",out var lower)&&lower==1);
Assert("Mã thẻ từ chối số âm",!ReaderCode.TryParse("NOVA:DG:-1",out _));
Assert("Mã thẻ từ chối 0",!ReaderCode.TryParse("NOVA:DG:000000",out _));
Assert("Mã thẻ từ chối tràn int",!ReaderCode.TryParse("NOVA:DG:99999999999",out _));
Assert("Mã thẻ từ chối ký tự lạ",!ReaderCode.TryParse("NOVA:DG:1;DROP",out _));
MustThrow("Không tạo QR cho mã 0",()=>ReaderCode.Create(0));
Assert("CSV bảo vệ công thức",Util.CsvCell("=1+1").StartsWith("\"'="));
Assert("CSV giữ dấu ngoặc kép",Util.CsvCell("A\"B")=="\"A\"\"B\"");
AuthBLL.StartPreview();
Assert("Xem thử có 12 sách",NovaBLL.Catalog().Rows.Count==12);
Assert("Tìm kiếm không dấu",NovaBLL.Catalog("de men").Rows.Count==1);
Assert("Lọc thể loại",NovaBLL.Catalog("",1).Rows.Count==4);
Assert("Tìm kiếm không có kết quả",NovaBLL.Catalog("khongtimthaysach123").Rows.Count==0);
Assert("Xem thử có dữ liệu dashboard",ThongKeBLL.Summary().Rows.Count==1);
Assert("Xem thử có hàng chờ",NovaBLL.Reservations().Rows.Count==1);
MustThrow("Xem thử chặn đặt trước",()=>NovaBLL.Reserve(1,3));
MustThrow("Xem thử chặn hủy đặt",()=>NovaBLL.Cancel(1));
MustThrow("Xem thử chặn ghi dữ liệu",()=>QuanLyThuVien.DAL.DbHelper.NonQuery("DELETE FROM Sach"));
MustThrow("Xem thử chặn truy vấn scalar thực",()=>QuanLyThuVien.DAL.DbHelper.Scalar("SELECT 1"));
MustThrow("Xem thử chặn phiếu đền trực tiếp",()=>QuanLyThuVien.DAL.PhieuPhatDAL.DenSach(1,"Mất sách","test",10000,0,"demo"));
AuthBLL.Logout();
Assert("Đăng xuất tắt xem thử",!QuanLyThuVien.DAL.PreviewData.Enabled&&AuthBLL.CurrentUser==null);
MustThrow("Nhật ký yêu cầu Admin",()=>NovaBLL.Activity());
Console.WriteLine($"KẾT QUẢ: PASS {ok}, FAIL {failed}.");
Environment.ExitCode=failed==0?0:1;
