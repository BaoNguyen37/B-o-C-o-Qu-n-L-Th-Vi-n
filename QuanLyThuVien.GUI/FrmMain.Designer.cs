namespace QuanLyThuVien.GUI;
partial class FrmMain
{
    private System.ComponentModel.IContainer components=null;
    Panel host,side,header,brand,bottom;
    FlowLayoutPanel nav;
    Label lblTitle,lblClock,lblUser,logo,version;
    Button btnPwd,btnOut,navDashboard,navSach,navDocGia,navMuonTra,navThongKe,navPhieuPhat,navNhacTra,navHeThong;
    protected override void Dispose(bool disposing) { if(disposing)components?.Dispose();base.Dispose(disposing); }
    void InitializeComponent()
    {
        SuspendLayout();Text="NOVA Library · Trung tâm điều hành";StartPosition=FormStartPosition.CenterScreen;
        ClientSize=new Size(1440,900);MinimumSize=new Size(1180,700);BackColor=Theme.Bg;Font=Theme.F();
        side=new Panel{Dock=DockStyle.Left,Width=232,BackColor=Theme.Sidebar};
        header=new Panel{Dock=DockStyle.Top,Height=82,BackColor=Color.White};
        host=new Panel{Dock=DockStyle.Fill,BackColor=Theme.Bg};
        Controls.Add(host);Controls.Add(header);Controls.Add(side);
        brand=new Panel{Dock=DockStyle.Top,Height=114,BackColor=Theme.Sidebar};
        bottom=new Panel{Dock=DockStyle.Bottom,Height=104,BackColor=Theme.Sidebar};
        nav=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,BackColor=Theme.Sidebar};
        side.Controls.Add(nav);side.Controls.Add(bottom);side.Controls.Add(brand);
        logo=NovaStyle.Text("NOVA / Library",22,27,19,true,Color.White);
        version=NovaStyle.Text("Không gian tri thức số",24,65,9,false,Color.FromArgb(150,183,209));
        brand.Controls.AddRange(new Control[]{logo,version});
        Button Add(string text,int symbol){var b=new NavButton(symbol){Text=text,BackColor=Theme.Sidebar,ForeColor=Color.FromArgb(183,205,223)};nav.Controls.Add(b);return b;}
        navDashboard=Add("Tổng quan",0);navSach=Add("Quản lý sách",1);navDocGia=Add("Độc giả",3);navMuonTra=Add("Mượn / Trả sách",4);
        navPhieuPhat=Add("Phiếu phạt / Đền sách",8);navNhacTra=Add("Nhắc hạn Gmail",6);navThongKe=Add("Thống kê & báo cáo",7);navHeThong=Add("Kiểm tra hệ thống",8);
        btnPwd=new NavButton(8){Text="Đổi mật khẩu",Location=new Point(8,5),BackColor=Theme.Sidebar,ForeColor=Color.White};
        btnOut=new NavButton(4){Text="Đăng xuất",Location=new Point(8,51),BackColor=Theme.Sidebar,ForeColor=Color.FromArgb(183,205,223)};
        bottom.Controls.AddRange(new Control[]{btnPwd,btnOut});
        lblTitle=NovaStyle.Text("Tổng quan",28,14,18,true);lblClock=NovaStyle.Text("",30,49,9,false,Theme.Muted);
        lblUser=NovaStyle.Text("",800,29,10,true,Theme.Accent);
        header.Controls.AddRange(new Control[]{lblTitle,lblClock,lblUser});ResumeLayout(false);
    }
}
