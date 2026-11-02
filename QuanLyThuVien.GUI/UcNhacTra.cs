using System.Data;
using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;
public partial class UcNhacTra:UserControl
{
    bool sending;
    public UcNhacTra()
    {
        InitializeComponent();
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;Theme.StyleGrid(grid);
        var cfg=CauHinhGmail.Load();txtGmail.Text=cfg.DiaChiGui;chkAuto.Checked=cfg.TuDongNhac;
        btnReload.Click+=(_,__)=>Reload();
        btnExport.Click+=(_,__)=>Util.ExportGrid(grid,"DanhSachNhacTra");
        btnSave.Click+=(_,__) => {
            try {
                if(!AuthBLL.CurrentUser.IsAdmin)throw new InvalidOperationException("Chỉ Admin được sửa cấu hình Gmail.");
                var oldSender=cfg.DiaChiGui;
                var newSender=txtGmail.Text.Trim();
                if (!string.Equals(oldSender,newSender,StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(txtPassword.Text))
                    throw new InvalidOperationException("Đổi Gmail người gửi phải nhập App Password của Gmail mới. Không dùng lại mật khẩu Gmail cũ.");
                cfg.DiaChiGui=newSender;cfg.SetAppPassword(txtPassword.Text);
                if(chkAuto.Checked && !cfg.TuDongNhac &&
                   !Theme.Confirm("Bật gửi NHẮC TRẢ SÁCH TỰ ĐỘNG tới email độc giả khi ứng dụng đang mở?\nLưu ý: dữ liệu email mẫu cũng có thể nhận thư. Chỉ bật sau khi kiểm tra danh sách!"))return;
                cfg.TuDongNhac=chkAuto.Checked;cfg.Save();txtPassword.Clear();
                Theme.Info("Đã lưu cấu hình Gmail; App Password được mã hóa cho tài khoản Windows hiện tại.");
            }catch(Exception ex){Theme.Error(ex.Message);}
        };
        if(!AuthBLL.CurrentUser.IsAdmin){txtGmail.ReadOnly=true;txtPassword.Enabled=false;chkAuto.Enabled=false;btnSave.Enabled=false;}
        btnTest.Click+=async(_,__) => {
            if(sending)return;
            sending=true;btnTest.Enabled=false;
            try{
                var testCfg=CauHinhGmail.Load();
                var newSender=txtGmail.Text.Trim();
                if (!string.Equals(testCfg.DiaChiGui,newSender,StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(txtPassword.Text))
                    throw new InvalidOperationException("Đổi Gmail người gửi phải nhập App Password của Gmail mới.");
                testCfg.DiaChiGui=newSender;testCfg.SetAppPassword(txtPassword.Text);
                await GmailReminderService.SendMessageAsync(testCfg.DiaChiGui,testCfg.GetAppPassword(),testCfg.DiaChiGui,"[THƯ VIỆN] Kiểm tra gửi Gmail","Đây là email kiểm tra cấu hình nhắc trả sách từ ứng dụng Quản Lý Thư Viện.");
                Theme.Info("Đã gửi email thử. Kiểm tra Hộp thư đến và Spam.");}
            catch(Exception ex){Theme.Error("Chưa gửi được Gmail: "+ex.GetBaseException().Message);}
            finally{sending=false;btnTest.Enabled=true;}
        };
        btnSend.Click+=async(_,__) => {
            if(sending)return;
            if(grid.SelectedRows.Count==0){Theme.Error("Chọn một phiếu sắp hạn hoặc quá hạn.");return;}
            var row=((DataRowView)grid.SelectedRows[0].DataBoundItem).Row;
            if(!Theme.Confirm($"Gửi email nhắc trả phiếu #{row["MaPM"]} tới {row["Email"]}?"))return;
            sending=true;btnSend.Enabled=false;
            try { var result=await GmailReminderService.SendOneAsync(row,CauHinhGmail.Load().DiaChiGui,CauHinhGmail.Load().GetAppPassword());Theme.Info(result);Reload(); }
            catch(Exception ex){Theme.Error(ex.Message);}
            finally {sending=false;btnSend.Enabled=true;}
        };
        btnAll.Click+=async(_,__) => {
            if(sending)return;
            if(!Theme.Confirm("Xác nhận gửi Gmail tới tối đa 20 phiếu sắp hạn/quá hạn chưa trả?\nChỉ gửi cho email hợp lệ. Không gửi lại phiếu đã gửi thành công hôm nay."))return;
            sending=true;btnAll.Enabled=false;
            try {var (sent,skipped)=await GmailReminderService.SendBatchAsync(CauHinhGmail.Load().DiaChiGui,CauHinhGmail.Load().GetAppPassword());
                Theme.Info($"Gửi hoàn tất: {sent} email; bỏ qua/chưa gửi: {skipped}.");Reload();}
            catch(Exception ex){Theme.Error("Không thể gửi: "+ex.Message);}
            finally{sending=false;btnAll.Enabled=true;}
        };
        Reload();
    }
    void Reload()
    {
        try {
            grid.DataSource=NhacTraBLL.DanhSach();
            Theme.Cols(grid,"MaPM=Mã phiếu","DocGia=Độc giả","Email=Email người nhận","TenSach=Sách","HanTra=Hạn trả","SoNgayCon=Số ngày còn","LoaiNhac=Loại nhắc","TrangThaiEmail=Gửi hôm nay");
            Theme.Fmt(grid,"dd/MM/yyyy","HanTra");grid.ClearSelection();
            lblStatus.Text=$"Có {grid.Rows.Count} phiếu cần nhắc. Số ngày âm nghĩa là quá hạn.";
        }catch(Exception ex){Theme.Error("Không tải được nhắc trả: "+ex.Message);}
    }
}
