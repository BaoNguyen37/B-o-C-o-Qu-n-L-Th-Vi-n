# NOVA Library 2.0

Bản nâng cấp từ QuanLyThuVien.zip, giữ kiến trúc C# WinForms / DTO / DAL / BLL / SQL Server và toàn bộ nhóm chức năng gốc.

## Mở nhanh

1. Clone hoặc tải mã nguồn từ GitHub. Mở `QuanLyThuVien.slnx` bằng Visual Studio, đặt `QuanLyThuVien.GUI` làm Startup Project rồi nhấn F5. Nếu dùng gói phát hành có thư mục `ChayNgay`, có thể mở `QuanLyThuVien.exe` trong thư mục đó.
2. Máy cần **.NET Desktop Runtime 10 x64**. Phát triển mã nguồn cần **.NET SDK 10** và Visual Studio hỗ trợ .NET 10 (hoặc dùng lệnh dotnet).
3. Chưa có SQL Server: chọn **Xem thử giao diện**. Đây là dữ liệu mẫu tách biệt, chỉ đọc. Không lưu nghiệp vụ, không gửi email, không kết nối database thật.
4. Dùng đầy đủ: chọn **Cấu hình SQL Server** tại màn đăng nhập, nhập tên instance đang chạy, ví dụ `localhost`, `.\SQLEXPRESS` hoặc `BAONGUYEN\BAONGUYEN1`.
5. Chọn **Tạo database mẫu NOVA**. Tài khoản Windows cần quyền tạo database. App tạo `QuanLyThuVienNova`, các bảng, thủ tục, trigger và dữ liệu mẫu dựa trên ZIP gốc. Có thể chạy lại; không xóa dữ liệu hiện có.
6. Đăng nhập: `admin / admin123` hoặc `thuthu / thuthu123`. Nên đổi mật khẩu sau khi cài.

Nếu cấu hình SQL Server chưa đúng, ứng dụng vẫn mở được phần xem thử. Cấu hình được lưu trong `connection.txt` cạnh EXE, sử dụng Windows Authentication. Có `connection.example.txt` để tham khảo.

## Điểm mới

- Nhận diện NOVA với sidebar navy, icon vẽ nét, dashboard và minh họa kệ sách; thống kê lấy từ database khi đăng nhập thật.
- Kho sách dạng thẻ; tìm tên/tác giả/vị trí không dấu, lọc thể loại, lọc sách còn kho, chi tiết và đặt trước.
- Hàng chờ đặt trước: tối đa 3 đầu sách/độc giả, tránh trùng, ưu tiên người đứng đầu; nhận sách tạo phiếu mượn, trừ tồn và hoàn tất đặt trước trong cùng transaction.
- Mượn tối đa 5 cuốn, chặn sách trùng và độc giả có sách quá hạn. Kiểm tra lại ngay tại SQL để tránh hai máy cùng bỏ qua giới hạn.
- Gia hạn tối đa 2 lần, mỗi lần 7 ngày; không gia hạn phiếu đã trả/quá hạn hoặc sách có người chờ.
- Hồ sơ độc giả với lịch sử mượn, số sách đang mượn/quá hạn, thẻ QR xuất PNG. Vào **Độc giả**, chọn dòng rồi **Hồ sơ & thẻ QR**.
- Quét mã thẻ bằng đầu đọc QR kiểu bàn phím, hoặc dán `NOVA:DG:000001` và Enter tại màn mượn/trả. Không tích hợp camera quét QR.
- Nhật ký thao tác cho Admin: trigger ghi thêm/sửa/xóa sách, độc giả, mượn, phạt và đặt trước, gồm tài khoản, thời gian và mã đối tượng. Không lưu mật khẩu vào nhật ký.
- F5 làm mới trang; Ctrl+K mở kho sách. Sửa lỗi dock che thanh tìm kiếm và nút thao tác của bản gốc.
- Giữ xuất CSV tiếng Việt; thêm xử lý ô có ký tự mở đầu công thức.
- Thêm trình cấu hình và khởi tạo SQL Server, không phải sửa chuỗi kết nối trong mã nguồn.

## Database

- **Cài mới:** `Database/00_CAI_MOI_NOVA.sql` chạy toàn bộ trong SSMS. Tạo database riêng `QuanLyThuVienNova` với 12 sách, 6 độc giả, tài khoản và phiếu mượn mẫu từ bản gốc.
- **Nâng cấp database cũ QuanLyThuVien:** sao lưu trước, chạy `Database/01_NANG_CAP_BAN_CU.sql`, rồi chọn kết nối tới `QuanLyThuVien` trong ứng dụng.
- **Bổ sung vào database khác:** chọn đúng database trong SSMS, chạy phần bổ sung gốc và `03_NOVA.sql`. File `03_NOVA.sql` không chứa USE để có thể dùng với database đã chọn.
- **Kiểm tra nghiệp vụ:** `04_TEST_NGHIEPVU.sql` gồm 9 ca dùng transaction và rollback; nên chạy trên database thử nghiệm đã cài schema.
- Giữ nguyên các script gốc trong thư mục Database để đối chiếu. Với bản 2.0, ưu tiên script 00/01 ở trên; một mình script gốc chưa đủ schema mới.

`03_NOVA.sql` bổ sung bảng DatTruoc, NhatKyThaoTac, SoLanGiaHan, 5 thủ tục và trigger nhật ký. NovaLock dùng khóa ứng dụng trong transaction để tuần tự hóa các thao tác hàng chờ/mượn/gia hạn. Trả sách cũng dùng khóa này.

## Build và kiểm tra

```powershell
dotnet build QuanLyThuVien.GUI/QuanLyThuVien.GUI.csproj -m:1 -p:UseSharedCompilation=false -p:MSBuildEnableWorkloadResolver=false
dotnet run --project Tests/QuanLyThuVien.Tests.csproj
dotnet publish QuanLyThuVien.GUI/QuanLyThuVien.GUI.csproj -c Release --self-contained false -o ChayNgay
```

Bản bàn giao đã build thành công và có 56 kiểm tra tự động đạt (tính phạt, QR, mã thẻ, CSV, dữ liệu xem thử, chặn ghi ở chế độ xem thử). Các script SQL qua kiểm tra cú pháp bằng Microsoft ScriptDom. Đã mở/render các màn chính với dữ liệu mẫu và kiểm tra hình ảnh.

**Giới hạn kiểm chứng:** SQL Server trên máy tạo bản bàn giao không nhận kết nối; chưa khởi tạo được database thật trên máy đó và chưa chạy kiểm thử giao dịch SQL. Script cài và ca kiểm thử được cung cấp để chạy khi SQL Server hoạt động. Không gọi đây là kết quả đã kiểm thử tích hợp thành công.

## Gmail và thanh toán kế thừa

Gmail cần địa chỉ thật và App Password do bạn cấu hình; tự nhắc chỉ hoạt động khi ứng dụng mở và được người quản trị bật. QR thanh toán MB Bank tạo trên máy, cần cấu hình STK và kiểm tra người nhận. Xác nhận đã thu là thao tác thủ công, không tự đối soát ngân hàng. Chưa gửi email hay thực hiện thanh toán trong quá trình nâng cấp.

Các tài khoản mẫu và cách băm mật khẩu được giữ tương thích bản gốc. Đây là đồ án chạy cục bộ; trước khi triển khai công khai cần thiết kế lại xác thực và quyền truy cập SQL theo môi trường triển khai.
