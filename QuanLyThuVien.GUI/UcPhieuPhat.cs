using System.Data;
using System.Drawing.Printing;
using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;
public partial class UcPhieuPhat:UserControl
{
    bool busy=true;
    int selectedLoan;
    public UcPhieuPhat()
    {
        InitializeComponent();
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        Theme.StyleGrid(gridLoans);Theme.StyleGrid(gridFines);
        cboType.Items.AddRange(new object[]{"Trả trễ", "Mất sách", "Hỏng sách"});cboType.SelectedIndex=0;
        cboStatus.Items.AddRange(new object[]{"Tất cả","Chưa thu","Đã thu"});cboStatus.SelectedIndex=0;
        cboMethod.Items.AddRange(new object[]{"Chuyển khoản","Tiền mặt"});cboMethod.SelectedIndex=0;
        txtAccount.Text=CauHinhThanhToan.Load().SoTaiKhoan;
        lnkBankConfig.LinkClicked += (_,__) => {
            form.ScrollControlIntoView(txtAccount);
            if (AuthBLL.CurrentUser.IsAdmin) txtAccount.Focus();
        };
        // Admin sửa STK; Thủ thư chỉ xem, tránh thay đổi nơi nhận tiền.
        txtAccount.ReadOnly = !AuthBLL.CurrentUser.IsAdmin;
        btnSaveAccount.Enabled = AuthBLL.CurrentUser.IsAdmin;
        lblBankHelp.Text = AuthBLL.CurrentUser.IsAdmin
            ? "Admin: nhập 6–19 chữ số và bấm Lưu. Kiểm tra chủ tài khoản trên app MB Bank."
            : "Chỉ Admin được đổi STK nhận tiền; Thủ thư vẫn tạo QR cho phiếu chưa thu.";
        txtAccount.MaxLength = 19;
        txtAccount.KeyPress += (_, e) => { if (!char.IsControl(e.KeyChar) && !char.IsAsciiDigit(e.KeyChar)) e.Handled = true; };
        txtAccount.TextChanged += (_,__) => { if(AuthBLL.CurrentUser.IsAdmin) lblBankHelp.Text = "STK đang chỉnh sửa; hãy bấm Lưu trước khi tạo QR."; };
        btnSaveAccount.Click += (_,__) => Run(() => {
            if (!AuthBLL.CurrentUser.IsAdmin) throw new NghiepVuException("Chỉ Admin được sửa số tài khoản nhận tiền.");
            string account = txtAccount.Text.Trim();
            if (!CauHinhThanhToan.SoTaiKhoanHopLe(account))
                throw new NghiepVuException("STK MB Bank phải có 6–19 chữ số. Không nhập khoảng trắng hoặc chữ cái.");
            if (!Theme.Confirm($"Lưu tài khoản nhận tiền?\n\nNgân hàng: MB Bank\nSố tài khoản: {account}\nChủ tài khoản kỳ vọng: NGUYEN BAO NGUYEN\n\nQuan trọng: cần kiểm tra tên người nhận hiển thị trong app ngân hàng trước khi thu.")) return;
            new CauHinhThanhToan{SoTaiKhoan=account}.Save();
            lblBankHelp.Text = "Đã lưu STK trên máy này. QR không tự xác nhận tiền đã vào tài khoản.";
            Theme.Info("Đã lưu số tài khoản MB Bank. Hãy kiểm tra đúng người nhận khi quét.");
        });
        txtLoan.TextChanged += (_,__) => {if(!busy)LoadLoans();};
        txtFine.TextChanged += (_,__) => {if(!busy)LoadFines();};
        cboStatus.SelectedIndexChanged += (_,__) => {if(!busy)LoadFines();};
        gridLoans.SelectionChanged += (_,__) => {
            if(busy || gridLoans.SelectedRows.Count==0)return;
            try{
                var row=gridLoans.SelectedRows[0];
                selectedLoan=Convert.ToInt32(row.Cells["MaPM"].Value);
                lblSelected.Text=$"Phiếu mượn #{selectedLoan} • {row.Cells["DocGia"].Value}\n{row.Cells["TenSach"].Value}";
                var details=PhieuPhatBLL.ChiTietMuon(selectedLoan).Rows[0];
                numAmount.Value=Math.Clamp(Convert.ToDecimal(details["GiaBia"]),numAmount.Minimum,numAmount.Maximum);
                txtReason.Text="";
                UpdateLoanPreview();
            }catch(Exception ex){lblSelected.Text=ex.Message; lblPreview.Text="Không xem được thông tin phiếu.";}
        };
        cboType.SelectedIndexChanged += (_,__) => UpdateLoanPreview();
        btnQuickReturn.Click += (_,__) => Run(() => {
            if (selectedLoan <= 0) throw new NghiepVuException("Chọn một phiếu đang mượn ở bảng phía trên trước.");
            var detail = PhieuPhatBLL.ChiTietMuon(selectedLoan).Rows[0];
            if (detail["NgayTra"] != DBNull.Value)
                throw new NghiepVuException("Phiếu này đã kết thúc. Tìm phiếu phạt ở bảng dưới để thanh toán.");
            decimal estimate = PhieuMuonBLL.DuKienPhat(selectedLoan);
            if (!Theme.Confirm($"Trả sách nhanh – phiếu #{selectedLoan}\n\nDự kiến phạt: {estimate:N0} đ\n\nXác nhận sách đã được trả về và cộng 1 vào tồn kho?\nNếu sách MẤT hoặc HỎNG, bấm Không và chọn loại bồi thường thay vì trả sách.")) return;
            decimal actual = PhieuMuonBLL.Tra(selectedLoan);
            Theme.Info(actual > 0
                ? $"Đã trả sách và tạo phiếu phạt {actual:N0} đ trong cùng giao dịch SQL. Chọn phiếu dưới để tạo QR hoặc thu tiền."
                : "Đã trả sách. Không phát sinh tiền phạt.");
            LoadLoans(); LoadFines();
        });
        btnCreate.Click += (_,__) => Run(() => {
            if(selectedLoan<=0)throw new NghiepVuException("Tìm và chọn phiếu mượn phía bên trái trước.");
            if(cboType.Text=="Trả trễ") {
                var detail = PhieuPhatBLL.ChiTietMuon(selectedLoan).Rows[0];
                if(detail["NgayTra"] == DBNull.Value)
                    throw new NghiepVuException("Phiếu còn đang mượn. Chọn 'Trả sách nhanh + lập phạt' để vừa trả sách vừa lập phạt. Không thu phạt trước khi chốt ngày trả.");
                if(Convert.ToDecimal(detail["TienPhat"])<=0)
                    throw new NghiepVuException("Phiếu đã trả đúng hạn nên không có tiền phạt. Không cần lập phiếu.");
                PhieuPhatBLL.LapPhatTraTreCu(selectedLoan);
            }
            else if(Theme.Confirm($"Xác nhận sách {cboType.Text.ToLowerInvariant()}?\nPhiếu mượn #{selectedLoan} sẽ kết thúc, nhưng SỐ LƯỢNG SÁCH KHÔNG TĂNG.\nBồi thường: {numAmount.Value:N0} đ."))
                PhieuPhatBLL.DenSach(selectedLoan,cboType.Text,txtReason.Text,numAmount.Value);
            else return;
            Theme.Info("Đã lập phiếu phạt/bồi thường. Chọn phiếu ở danh sách bên dưới để thanh toán.");
            LoadLoans();LoadFines();
        });
        gridFines.SelectionChanged += (_,__) => UpdateFineSelection();
        cboMethod.SelectedIndexChanged += (_,__) => {
            txtReference.Enabled = cboMethod.Text == "Chuyển khoản";
            if (!txtReference.Enabled) txtReference.Clear();
        };
        btnPaid.Click += (_,__) => Run(() => {
            var f=SelectedFine(); var id=Convert.ToInt32(f.Cells["MaPhat"].Value);
            if(Convert.ToString(f.Cells["TrangThai"].Value)=="Đã thu")
                throw new NghiepVuException("Phiếu này đã được xác nhận thu trước đó.");
            if(cboMethod.Text == "Chuyển khoản" && txtReference.Text.Trim().Length < 4)
                throw new NghiepVuException("Chuyển khoản cần mã giao dịch/FT thực tế (tối thiểu 4 ký tự). Không nhập mã tự tạo.");
            if(!Theme.Confirm($"XÁC NHẬN ĐÃ NHẬN TIỀN?\n\nPhiếu phạt #{id}: {Convert.ToDecimal(f.Cells["SoTien"].Value):N0} đ\nPhương thức: {cboMethod.Text}\nMã tham chiếu: {(cboMethod.Text == "Chuyển khoản" ? txtReference.Text.Trim() : "Tiền mặt")}\n\nChỉ tiếp tục sau khi đã kiểm tra sao kê MB Bank hoặc nhận tiền mặt. QR không tự kiểm chứng giao dịch."))return;
            PhieuPhatBLL.ThuTien(id,cboMethod.Text,txtReference.Text);
            Theme.Info("Đã ghi nhận thanh toán thủ công. Mã phiếu được lưu trong SQL Server.");
            txtReference.Clear();
            LoadFines();
        });
        btnQR.Click += (_,__) => Run(() => {
            var f=SelectedFine();
            if(Convert.ToString(f.Cells["TrangThai"].Value)=="Đã thu")throw new NghiepVuException("Phiếu đã thu tiền, không tạo QR mới.");
            var cfg=CauHinhThanhToan.Load();
            if (!CauHinhThanhToan.SoTaiKhoanHopLe(cfg.SoTaiKhoan))
                throw new NghiepVuException("Chưa có STK MB Bank hợp lệ. Hãy đăng nhập Admin để nhập và LƯU STK trước khi tạo QR.");
            using var dlg=new FrmQrThanhToan(Convert.ToInt32(f.Cells["MaPhat"].Value),Convert.ToDecimal(f.Cells["SoTien"].Value));
            dlg.ShowDialog(this);
        });
        btnCopyTransfer.Click += (_,__) => Run(() => {
            var row=SelectedFine();
            if(Convert.ToString(row.Cells["TrangThai"].Value)=="Đã thu") throw new NghiepVuException("Phiếu đã thu, không cần chuyển khoản thêm.");
            var cfg=CauHinhThanhToan.Load();
            if(!CauHinhThanhToan.SoTaiKhoanHopLe(cfg.SoTaiKhoan))
                throw new NghiepVuException("Chưa cấu hình STK MB Bank hợp lệ.");
            var id=Convert.ToInt32(row.Cells["MaPhat"].Value);
            Clipboard.SetText($"MB Bank - {cfg.SoTaiKhoan} - {cfg.TenChuTaiKhoan}\r\nSố tiền: {Convert.ToDecimal(row.Cells["SoTien"].Value):N0} VND\r\nNội dung: PHAT TL{id}");
            Theme.Info("Đã sao chép thông tin chuyển khoản. Cần kiểm tra tên người nhận thực tế khi chuyển.");
        });
        btnPrint.Click += (_,__) => Run(PrintFine);
        btnExport.Click += (_,__) => Util.ExportGrid(gridFines,"DanhSachPhieuPhat");
        btnRefresh.Click += (_,__) => Run(() => {LoadLoans();LoadFines();});
        gridLoans.CellFormatting += (_, e) => {
            if (e.RowIndex < 0 || gridLoans.Columns[e.ColumnIndex].Name != "TrangThai") return;
            string value=Convert.ToString(e.Value) ?? "";
            e.CellStyle.ForeColor=value=="Quá hạn"?Theme.Danger:value=="Đã trả"?Theme.Success:Theme.Sky;
        };
        gridFines.CellFormatting += (_, e) => {
            if (e.RowIndex < 0 || gridFines.Columns[e.ColumnIndex].Name != "TrangThai") return;
            e.CellStyle.ForeColor=Convert.ToString(e.Value)=="Đã thu"?Theme.Success:Theme.Danger;
        };
        txtReference.Enabled = false;
        busy=false;LoadLoans();LoadFines();
    }
    void UpdateLoanPreview()
    {
        btnQuickReturn.Enabled = false;
        if (selectedLoan <= 0) {lblPreview.Text="Chọn một phiếu để xem trạng thái, tiền phạt dự kiến.";return;}
        try {
            var r = PhieuPhatBLL.ChiTietMuon(selectedLoan).Rows[0];
            bool returned = r["NgayTra"] != DBNull.Value;
            var amount = returned ? Convert.ToDecimal(r["TienPhat"])
                : PhieuMuonBLL.TinhPhat(Convert.ToDateTime(r["HanTra"]), DateTime.Today);
            lblPreview.Text = returned
                ? $"Phiếu đã kết thúc. Phạt đã chốt: {amount:N0} đ."
                : $"Đang mượn • Phạt dự kiến đến hôm nay: {amount:N0} đ.";
            btnQuickReturn.Enabled = !returned;
        } catch (Exception ex) {lblPreview.Text="Không xem được phạt dự kiến: "+ex.Message;}
    }
    void UpdateFineSelection()
    {
        if (busy || gridFines.SelectedRows.Count == 0) {
            lblFineSelected.Text="Chọn phiếu phạt chưa thu ở bảng dưới để tạo QR, in hoặc thanh toán.";
            return;
        }
        var row=gridFines.SelectedRows[0];
        lblFineSelected.Text=$"Phiếu #{row.Cells["MaPhat"].Value} • {row.Cells["TrangThai"].Value}\nSố tiền: {Convert.ToDecimal(row.Cells["SoTien"].Value):N0} đ";
        txtReference.Clear(); // Không giữ lại mã giao dịch khi chọn phiếu khác.
    }
    DataGridViewRow SelectedFine()
    {
        if(gridFines.SelectedRows.Count==0)throw new NghiepVuException("Chọn một phiếu phạt trong danh sách phía dưới bên trái.");
        return gridFines.SelectedRows[0];
    }
    void LoadLoans()
    {
        busy=true;
        gridLoans.DataSource=PhieuMuonBLL.GetAll(txtLoan.Text);
        Theme.Cols(gridLoans,"MaPM=Mã phiếu","DocGia=Độc giả","TenSach=Tên sách","HanTra=Hạn trả","NgayTra=Ngày trả","TienPhat=Phạt trễ","TrangThai=Trạng thái");
        Theme.Hide(gridLoans,"MaDG");Theme.Fmt(gridLoans,"dd/MM/yyyy","NgayMuon","HanTra","NgayTra");Theme.Fmt(gridLoans,"N0","TienPhat");
        gridLoans.ClearSelection();selectedLoan=0;lblSelected.Text="Chưa chọn phiếu mượn";busy=false;
        UpdateLoanPreview();
    }
    void LoadFines()
    {
        busy=true;
        gridFines.DataSource=PhieuPhatBLL.GetAll(txtFine.Text,cboStatus.Text);
        Theme.Cols(gridFines,"MaPhat=Mã phạt","MaPM=Mã mượn","DocGia=Độc giả","TenSach=Sách","LoaiPhat=Loại","SoTien=Số tiền","TrangThai=Đã thu?","NgayLap=Ngày lập","NgayThu=Ngày thu","MaGiaoDich=Mã GD");
        Theme.Hide(gridFines,"LyDo","NguoiLap","PhuongThuc");
        Theme.Fmt(gridFines,"dd/MM/yyyy HH:mm","NgayLap","NgayThu");Theme.Fmt(gridFines,"N0","SoTien");
        gridFines.ClearSelection();busy=false;
        UpdateFineSelection();
        var stats=PhieuPhatBLL.TongHop();
        var first=stats.Rows[0];
        lblSummary.Text=$"Công nợ: {first["ChuaThu"]} phiếu • {Convert.ToDecimal(first["TienChuaThu"]):N0} đ chưa thu   |   Đã thu: {Convert.ToDecimal(first["TienDaThu"]):N0} đ";
    }
    void Run(Action action)
    {
        try{action();}catch(Exception ex){Theme.Error(ex.Message);}
    }
    void PrintFine()
    {
        var row=SelectedFine();
        string[] text={
           "PHIẾU PHẠT / BỒI THƯỜNG THƯ VIỆN",
           $"Mã phiếu phạt: {row.Cells["MaPhat"].Value}",
           $"Phiếu mượn: {row.Cells["MaPM"].Value}",
           $"Độc giả: {row.Cells["DocGia"].Value}",
           $"Sách: {row.Cells["TenSach"].Value}",
           $"Loại: {row.Cells["LoaiPhat"].Value}",
           $"Số tiền: {Convert.ToDecimal(row.Cells["SoTien"].Value):N0} đồng",
           $"Trạng thái: {row.Cells["TrangThai"].Value}",
           $"Ngày lập: {row.Cells["NgayLap"].Value}",
           $"Mã giao dịch: {row.Cells["MaGiaoDich"].Value}",
           "", "Xác nhận của thủ thư: __________________________",
           "LƯU Ý: In phiếu không xác nhận giao dịch ngân hàng thành công."
        };
        using var printer=new PrintDocument();
        printer.DocumentName=$"PhieuPhat_{row.Cells["MaPhat"].Value}";
        printer.PrintPage += (_,e)=> {
            using var titleFont=new Font("Segoe UI",15,FontStyle.Bold);
            using var font=new Font("Segoe UI",11);
            int x=e.MarginBounds.Left,y=e.MarginBounds.Top;
            for(int i=0;i<text.Length;i++){
                var f=i==0?titleFont:font;
                e.Graphics!.DrawString(text[i],f,Brushes.Black,x,y);
                y+=i==0?58:36;
            }
            e.HasMorePages=false;
        };
        using var dialog=new PrintPreviewDialog{Document=printer,Width=850,Height=650};dialog.ShowDialog(this);
    }
}
