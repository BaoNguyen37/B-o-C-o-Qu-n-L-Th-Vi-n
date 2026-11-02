namespace QuanLyThuVien.GUI;
// Designer: các control được khai báo cố định, có thể chỉnh sửa bằng Toolbox / Properties.
partial class UcPhieuPhat
{
    private System.ComponentModel.IContainer components=null;
    private TableLayoutPanel layout, left;
    private Panel pnlLoan,pnlFine, form;
    private Label lblLoan,lblFine,lblSelected,lblForm,lblInfo,lblBank,lblBankHelp,lblPreview,lblSummary,lblFineSelected;
    private LinkLabel lnkBankConfig;
    private TextBox txtLoan,txtFine,txtReason,txtAccount,txtReference;
    private ComboBox cboType,cboStatus,cboMethod;
    private NumericUpDown numAmount;
    private DataGridView gridLoans,gridFines;
    private Button btnCreate,btnPaid,btnQR,btnPrint,btnSaveAccount,btnExport,btnRefresh,btnQuickReturn,btnCopyTransfer;
    protected override void Dispose(bool d){if(d)components?.Dispose();base.Dispose(d);}
    private void InitializeComponent()
    {
        SuspendLayout(); Size=new Size(1120,710); BackColor=Color.FromArgb(241,245,249); Padding=new Padding(12);
        layout=new TableLayoutPanel{Name="layout",Dock=DockStyle.Fill,ColumnCount=2,RowCount=1};
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,366));
        left=new TableLayoutPanel{Name="left",Dock=DockStyle.Fill,RowCount=2,ColumnCount=1,Margin=new Padding(0,0,12,0)};
        left.RowStyles.Add(new RowStyle(SizeType.Percent,45));left.RowStyles.Add(new RowStyle(SizeType.Percent,55));
        pnlLoan=new Panel{Name="pnlLoan",Dock=DockStyle.Fill,BackColor=Color.White,Margin=new Padding(0,0,0,8)};
        pnlFine=new Panel{Name="pnlFine",Dock=DockStyle.Fill,BackColor=Color.White,Margin=new Padding(0,4,0,0)};
        form=new Panel{Name="form",Dock=DockStyle.Fill,AutoScroll=true,BackColor=Color.White,Margin=new Padding(0)};
        left.Controls.Add(pnlLoan,0,0);left.Controls.Add(pnlFine,0,1);layout.Controls.Add(left,0,0);layout.Controls.Add(form,1,0);Controls.Add(layout);
        lblLoan=new Label{Name="lblLoan",Text="TÌM NHANH PHIẾU MƯỢN (mã phiếu, độc giả, tên sách)",Location=new Point(12,12),AutoSize=true,Font=new Font("Segoe UI",10,FontStyle.Bold)};
        txtLoan=new TextBox{Name="txtLoan",PlaceholderText="Nhập mã phiếu / độc giả / tên sách...",Location=new Point(12,43),Size=new Size(540,30),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        gridLoans=new DataGridView{Name="gridLoans",Dock=DockStyle.Fill,ReadOnly=true};
        var loanHead=new Panel{Dock=DockStyle.Top,Height=82};loanHead.Controls.AddRange(new Control[]{lblLoan,txtLoan});pnlLoan.Controls.Add(gridLoans);pnlLoan.Controls.Add(loanHead);
        lblFine=new Label{Name="lblFine",Text="PHIẾU PHẠT / BỒI THƯỜNG",Location=new Point(12,12),AutoSize=true,Font=new Font("Segoe UI",10,FontStyle.Bold)};
        txtFine=new TextBox{Name="txtFine",PlaceholderText="Tìm mã phiếu, tên độc giả, tên sách...",Location=new Point(12,43),Size=new Size(285,30)};
        cboStatus=new ComboBox{Name="cboStatus",Location=new Point(310,43),Size=new Size(132,30),DropDownStyle=ComboBoxStyle.DropDownList};
        btnExport=new Button{Name="btnExport",Text="Xuất CSV",Location=new Point(453,42),Size=new Size(90,30)};
        gridFines=new DataGridView{Name="gridFines",Dock=DockStyle.Fill,ReadOnly=true};
        lblSummary=new Label{Name="lblSummary",Text="Đang tính công nợ...",Location=new Point(12,80),Size=new Size(700,25),ForeColor=Color.FromArgb(55,65,81),Font=new Font("Segoe UI",9,FontStyle.Bold),AutoEllipsis=true};
        var fineHead=new Panel{Dock=DockStyle.Top,Height=110};fineHead.Controls.AddRange(new Control[]{lblFine,txtFine,cboStatus,btnExport,lblSummary});pnlFine.Controls.Add(gridFines);pnlFine.Controls.Add(fineHead);
        int y=16;
        lblForm=new Label{Name="lblForm",Text="XỬ LÝ PHẠT / ĐỀN SÁCH",Font=new Font("Segoe UI",13,FontStyle.Bold),Location=new Point(16,y),AutoSize=true};form.Controls.Add(lblForm); y+=34;
        lnkBankConfig=new LinkLabel{Name="lnkBankConfig",Text="↓ Đến phần nhập STK MB Bank (Admin)",Location=new Point(16,y),Size=new Size(328,24),LinkColor=Color.FromArgb(79,70,229)};form.Controls.Add(lnkBankConfig);y+=30;
        lblSelected=new Label{Name="lblSelected",Text="Chưa chọn phiếu mượn",Location=new Point(16,y),Size=new Size(330,55),ForeColor=Color.FromArgb(51,65,85)};form.Controls.Add(lblSelected);y+=56;
        lblPreview=new Label{Name="lblPreview",Text="Chọn một phiếu để xem phạt dự kiến.",Location=new Point(16,y),Size=new Size(328,48),ForeColor=Color.FromArgb(180,83,9),Font=new Font("Segoe UI",9,FontStyle.Bold)};form.Controls.Add(lblPreview);y+=50;
        btnQuickReturn=new Button{Name="btnQuickReturn",Text="Trả sách nhanh + lập phạt (nếu có)",Location=new Point(16,y),Size=new Size(328,39),BackColor=Color.FromArgb(14,165,233),ForeColor=Color.White,FlatStyle=FlatStyle.Flat,Enabled=false};form.Controls.Add(btnQuickReturn);y+=49;
        form.Controls.Add(new Label{Text="Loại xử lý",Location=new Point(16,y),AutoSize=true});y+=22;
        cboType=new ComboBox{Name="cboType",Location=new Point(16,y),Width=328,DropDownStyle=ComboBoxStyle.DropDownList};form.Controls.Add(cboType);y+=42;
        form.Controls.Add(new Label{Text="Lý do / tình trạng",Location=new Point(16,y),AutoSize=true});y+=22;
        txtReason=new TextBox{Name="txtReason",Location=new Point(16,y),Size=new Size(328,56),Multiline=true,PlaceholderText="Ví dụ: Làm mất sách khi đang mượn"};form.Controls.Add(txtReason);y+=70;
        form.Controls.Add(new Label{Text="Số tiền bồi thường (đ)",Location=new Point(16,y),AutoSize=true});y+=22;
        numAmount=new NumericUpDown{Name="numAmount",Location=new Point(16,y),Width=328,Maximum=1000000000,ThousandsSeparator=true};form.Controls.Add(numAmount);y+=44;
        btnCreate=new Button{Name="btnCreate",Text="Lập phiếu phạt / bồi thường",Location=new Point(16,y),Size=new Size(328,40),ForeColor=Color.White,BackColor=Color.FromArgb(99,102,241),FlatStyle=FlatStyle.Flat};form.Controls.Add(btnCreate);y+=51;
        lblInfo=new Label{Name="lblInfo",Text="Trả trễ: tự lập phiếu khi bấm Trả sách.\nMất/hỏng: ghi nhận bồi thường, không cộng lại tồn kho.",Location=new Point(16,y),Size=new Size(328,48),ForeColor=Color.FromArgb(100,116,139)};form.Controls.Add(lblInfo);y+=57;
        form.Controls.Add(new Label{Text="THANH TOÁN PHIẾU PHẠT",Location=new Point(16,y),AutoSize=true,Font=new Font("Segoe UI",11,FontStyle.Bold)});y+=29;
        lblFineSelected=new Label{Name="lblFineSelected",Text="Chọn phiếu phạt chưa thu để thanh toán",Location=new Point(16,y),Size=new Size(328,50),ForeColor=Color.FromArgb(51,65,85)};form.Controls.Add(lblFineSelected);y+=55;
        cboMethod=new ComboBox{Name="cboMethod",Location=new Point(16,y),Size=new Size(328,28),DropDownStyle=ComboBoxStyle.DropDownList};form.Controls.Add(cboMethod);y+=35;
        txtReference=new TextBox{Name="txtReference",Location=new Point(16,y),Size=new Size(328,27),PlaceholderText="Mã giao dịch khi thanh toán chuyển khoản"};form.Controls.Add(txtReference);y+=37;
        btnPaid=new Button{Name="btnPaid",Text="Xác nhận đã thu (kiểm tra thủ công)",Location=new Point(16,y),Size=new Size(328,38),BackColor=Color.FromArgb(16,185,129),ForeColor=Color.White,FlatStyle=FlatStyle.Flat};form.Controls.Add(btnPaid);y+=50;
        btnQR=new Button{Name="btnQR",Text="Tạo mã QR thanh toán MB Bank",Location=new Point(16,y),Size=new Size(328,38)};form.Controls.Add(btnQR);y+=46;
        btnCopyTransfer=new Button{Name="btnCopyTransfer",Text="Sao chép thông tin chuyển khoản",Location=new Point(16,y),Size=new Size(328,35)};form.Controls.Add(btnCopyTransfer);y+=43;
        btnPrint=new Button{Name="btnPrint",Text="In phiếu phạt",Location=new Point(16,y),Size=new Size(328,38)};form.Controls.Add(btnPrint);y+=50;
        lblBank=new Label{Name="lblBank",Text="MB Bank • NGUYEN BAO NGUYEN",Location=new Point(16,y),Size=new Size(328,26),Font=new Font("Segoe UI",9,FontStyle.Bold)};form.Controls.Add(lblBank);y+=30;
        txtAccount=new TextBox{Name="txtAccount",PlaceholderText="Nhập số tài khoản MB Bank nhận tiền",Location=new Point(16,y),Size=new Size(328,27),MaxLength=19};form.Controls.Add(txtAccount);y+=33;
        lblBankHelp=new Label{Name="lblBankHelp",Text="Admin nhập và lưu STK trước khi tạo QR.",Location=new Point(16,y),Size=new Size(328,48),ForeColor=Color.FromArgb(100,116,139)};form.Controls.Add(lblBankHelp);y+=51;
        btnSaveAccount=new Button{Name="btnSaveAccount",Text="Lưu số tài khoản MB Bank",Location=new Point(16,y),Size=new Size(328,36)};form.Controls.Add(btnSaveAccount);y+=42;
        btnRefresh=new Button{Name="btnRefresh",Text="Làm mới dữ liệu",Location=new Point(16,y),Size=new Size(328,36)};form.Controls.Add(btnRefresh);
        ResumeLayout(false);
    }
}
