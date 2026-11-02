namespace QuanLyThuVien.GUI;
// Giao diện có thể mở trong Visual Studio WinForms Designer (Shift+F7)
partial class UcNhacTra
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel root;
    private Panel panelList,panelSettings,top;
    private DataGridView grid;
    private TextBox txtGmail,txtPassword;
    private CheckBox chkAuto;
    private Button btnSave,btnTest,btnSend,btnAll,btnReload,btnExport;
    private Label lblStatus,lblHeader,lblHelp;
    protected override void Dispose(bool disposing){if(disposing) components?.Dispose();base.Dispose(disposing);}
    private void InitializeComponent()
    {
        SuspendLayout();Size=new Size(1120,700);Padding=new Padding(14);BackColor=Color.FromArgb(241,245,249);
        root=new TableLayoutPanel{Name="root",Dock=DockStyle.Fill,ColumnCount=2,RowCount=1};
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,365));
        panelList=new Panel{Name="panelList",Dock=DockStyle.Fill,BackColor=Color.White,Margin=new Padding(0,0,12,0)};
        panelSettings=new Panel{Name="panelSettings",Dock=DockStyle.Fill,BackColor=Color.White,AutoScroll=true};
        root.Controls.Add(panelList,0,0);root.Controls.Add(panelSettings,1,0);Controls.Add(root);
        top=new Panel{Name="top",Dock=DockStyle.Top,Height=132,BackColor=Color.White};
        lblHeader=new Label{Name="lblHeader",Text="NHẮC TRẢ SÁCH QUA GMAIL",Location=new Point(15,14),AutoSize=true,Font=new Font("Segoe UI",13,FontStyle.Bold)};
        btnReload=new Button{Name="btnReload",Text="Làm mới",Location=new Point(15,53),Size=new Size(105,32)};
        btnSend=new Button{Name="btnSend",Text="Gửi dòng đã chọn",Location=new Point(128,53),Size=new Size(155,32),BackColor=Color.FromArgb(99,102,241),ForeColor=Color.White,FlatStyle=FlatStyle.Flat};
        btnAll=new Button{Name="btnAll",Text="Gửi tối đa 20",Location=new Point(293,53),Size=new Size(140,32)};
        btnExport=new Button{Name="btnExport",Text="Xuất CSV",Location=new Point(443,53),Size=new Size(100,32)};
        lblStatus=new Label{Name="lblStatus",Text="Hiển thị: sắp hạn trong 3 ngày và quá hạn chưa trả.",Location=new Point(15,98),Size=new Size(600,24),ForeColor=Color.FromArgb(100,116,139)};
        top.Controls.AddRange(new Control[]{lblHeader,btnReload,btnSend,btnAll,btnExport,lblStatus});
        grid=new DataGridView{Name="grid",Dock=DockStyle.Fill,ReadOnly=true};panelList.Controls.Add(grid);panelList.Controls.Add(top);
        int y=18;
        panelSettings.Controls.Add(new Label{Text="CẤU HÌNH GỬI GMAIL",Font=new Font("Segoe UI",13,FontStyle.Bold),Location=new Point(17,y),AutoSize=true});y+=48;
        panelSettings.Controls.Add(new Label{Text="Địa chỉ Gmail người gửi",Location=new Point(17,y),AutoSize=true});y+=24;
        txtGmail=new TextBox{Name="txtGmail",Location=new Point(17,y),Size=new Size(326,29),PlaceholderText="ten@gmail.com"};panelSettings.Controls.Add(txtGmail);y+=52;
        panelSettings.Controls.Add(new Label{Text="Gmail App Password (không phải mật khẩu chính)",Location=new Point(17,y),AutoSize=true});y+=24;
        txtPassword=new TextBox{Name="txtPassword",Location=new Point(17,y),Size=new Size(326,29),UseSystemPasswordChar=true,PlaceholderText="Nhập 16 ký tự, để trống để giữ mật khẩu cũ"};panelSettings.Controls.Add(txtPassword);y+=50;
        chkAuto=new CheckBox{Name="chkAuto",Text="Tự gửi mỗi ngày KHI APP ĐANG MỞ",Location=new Point(17,y),Size=new Size(328,32)};panelSettings.Controls.Add(chkAuto);y+=41;
        btnSave=new Button{Name="btnSave",Text="Lưu cấu hình Gmail",Location=new Point(17,y),Size=new Size(326,38),BackColor=Color.FromArgb(16,185,129),ForeColor=Color.White,FlatStyle=FlatStyle.Flat};panelSettings.Controls.Add(btnSave);y+=50;
        btnTest=new Button{Name="btnTest",Text="Gửi email thử tới chính mình",Location=new Point(17,y),Size=new Size(326,37)};panelSettings.Controls.Add(btnTest);y+=54;
        lblHelp=new Label{Name="lblHelp",Location=new Point(17,y),Size=new Size(326,270),AutoSize=false,ForeColor=Color.FromArgb(71,85,105),Text="CÁCH SỬ DỤNG\n\n1. Bật xác minh 2 bước trong Google Account.\n2. Tạo App Password cho ứng dụng (nếu tài khoản hỗ trợ).\n3. Nhập Gmail và App Password, bấm Lưu.\n4. Bấm Gửi email thử, sau đó gửi từng độc giả.\n\nTự gửi mặc định TẮT; chỉ chạy khi phần mềm đang mở và đã bật tùy chọn.\nTối đa 1 email/phiếu/loại/ngày, có nhật ký SQL.\nEmail chỉ gửi nếu độc giả có địa chỉ hợp lệ.\nKhông tự gửi email khi chưa được quản trị bật."};panelSettings.Controls.Add(lblHelp);
        ResumeLayout(false);
    }
}
