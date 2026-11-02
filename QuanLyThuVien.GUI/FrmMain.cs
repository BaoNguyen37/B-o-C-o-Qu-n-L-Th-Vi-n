using System.Globalization;
using QuanLyThuVien.BLL;

namespace QuanLyThuVien.GUI;

public partial class FrmMain : Form
{
    public bool LoggedOut { get; private set; }

    readonly List<(Button btn, string title, Func<UserControl> make)> items = new();
    readonly System.Windows.Forms.Timer timer = new() { Interval = 1000 };
    readonly System.Windows.Forms.Timer mailTimer = new() { Interval = 1800000 };
    DateTime lastReminder = DateTime.MinValue;
    bool autoRunning;

    public FrmMain()
    {
        InitializeComponent();
        var screen=Screen.FromControl(this).WorkingArea;
        Size=new Size(Math.Min(Width,screen.Width),Math.Min(Height,screen.Height));
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        var user = AuthBLL.CurrentUser;
        lblUser.Text = $"{user.HoTen}  /  {user.VaiTro}";
        AddItem(navDashboard, "Tổng quan", () => new UcDashboard(Go));
        AddItem(navSach, "Quản lý sách", () => new UcSach());
        AddItem(navDocGia, "Quản lý độc giả", () => new UcDocGia());
        AddItem(navMuonTra, "Mượn / Trả sách", () => new UcMuonTra());
        AddItem(navPhieuPhat, "Phiếu phạt / Đền sách", () => new UcPhieuPhat());
        AddItem(navNhacTra, "Nhắc trả sách qua Gmail", () => new UcNhacTra());
        navThongKe.Visible = user.IsAdmin;
        if (user.IsAdmin) AddItem(navThongKe, "Thống kê & báo cáo", () => new UcThongKe());
        navHeThong.Visible = user.IsAdmin;
        if (user.IsAdmin) AddItem(navHeThong, "Kiểm tra hệ thống", () => new UcHeThong());
        var catalog = new NavButton(2){Text="Khám phá kho sách",BackColor=Theme.Sidebar};
        nav.Controls.Add(catalog); nav.Controls.SetChildIndex(catalog,1);
        AddItem(catalog,"Khám phá kho sách",()=>new UcCatalog());
        var reservations = new NavButton(5){Text="Đặt trước / Hàng chờ",BackColor=Theme.Sidebar};
        nav.Controls.Add(reservations); nav.Controls.SetChildIndex(reservations,5);
        AddItem(reservations,"Đặt trước / Hàng chờ",()=>new UcReservations());
        if(user.IsAdmin) {
            var audit=new NavButton(8){Text="Nhật ký thao tác",BackColor=Theme.Sidebar};nav.Controls.Add(audit);
            AddItem(audit,"Nhật ký thao tác",()=>new UcActivity());
        }
        KeyPreview=true;
        KeyDown += (_,e)=>{if(e.Control && e.KeyCode==Keys.K){Go("Khám phá kho sách");e.SuppressKeyPress=true;}
            if(e.KeyCode==Keys.F5){if(current>=0)Open(current);e.SuppressKeyPress=true;}};
        NovaStyle.Apply(side);
        foreach(Control b in nav.Controls)b.Width=190;
        btnPwd.Click += (s,e) => { using var f = new FrmDoiMatKhau(); f.ShowDialog(this); };
        btnOut.Click += (s,e) => { if (Theme.Confirm("Bạn muốn đăng xuất?")) { LoggedOut = true; Close(); } };
        header.Resize += (s,e) => lblUser.Location = new Point(header.Width - lblUser.Width - 28,26);
        timer.Tick += (s,e) => lblClock.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy  HH:mm:ss", new CultureInfo("vi-VN")) + (QuanLyThuVien.DAL.PreviewData.Enabled ? "  /  Xem thử · Dữ liệu mẫu · Chỉ đọc" : "  /  F5 Làm mới · Ctrl+K Kho sách");
        lblClock.Text=QuanLyThuVien.DAL.PreviewData.Enabled?"Xem thử · Dữ liệu mẫu · Chỉ đọc":"F5 Làm mới · Ctrl+K Kho sách";
        timer.Start();
        mailTimer.Tick += async (s,e) => await RunDailyReminders();
        mailTimer.Start();
        Shown += async (s,e) => await RunDailyReminders();
        FormClosed += (s,e) => { timer.Dispose();mailTimer.Dispose(); };
        Load += (s,e) => { Open(0); lblUser.Location = new Point(header.Width - lblUser.Width - 28,26); };
    }

    async Task RunDailyReminders()
    {
        if(QuanLyThuVien.DAL.PreviewData.Enabled || autoRunning || lastReminder==DateTime.Today)return;
        var conf=CauHinhGmail.Load();
        if(!conf.TuDongNhac)return; // mặc định tắt; người quản trị phải tự bật
        autoRunning=true;
        lastReminder=DateTime.Today;
        try {
            var summary=await GmailReminderService.SendBatchAsync(conf.DiaChiGui,conf.GetAppPassword());
            System.Diagnostics.Debug.WriteLine($"[Gmail] Tự nhắc: {summary.sent} thư; bỏ qua {summary.skipped}.");
        }
        catch(Exception ex){System.Diagnostics.Debug.WriteLine("[Gmail] Tự nhắc lỗi: "+ex.Message);}
        finally{autoRunning=false;}
    }

    void AddItem(Button b, string title, Func<UserControl> make)
    {
        int idx = items.Count;
        b.Click += (s,e) => Open(idx);
        items.Add((b,title,make));
    }

    int current=-1;
    void Go(string title) {int i=items.FindIndex(x=>x.title==title);if(i>=0)Open(i);}
    void Open(int i)
    {
        current=i;
        for (int k = 0; k < items.Count; k++)
        {
            bool sel = k == i;
            items[k].btn.BackColor = sel ? Theme.Accent : Theme.Sidebar;
            items[k].btn.ForeColor = sel ? Color.White : Color.FromArgb(203, 213, 225);
            items[k].btn.Font = Theme.F(10.5f, sel ? FontStyle.Bold : FontStyle.Regular);
        }
        lblTitle.Text = items[i].title;

        var old = host.Controls.Cast<Control>().ToList();
        host.Controls.Clear();
        foreach (var o in old) o.Dispose();

        try
        {
            var uc = items[i].make();
            NovaStyle.Apply(uc);
            uc.Dock = DockStyle.Fill;
            host.Controls.Add(uc);
        }
        catch (Exception ex) {
            var panel=new Panel{Dock=DockStyle.Fill,Padding=new Padding(28)};
            panel.Controls.Add(new Label{Dock=DockStyle.Fill,Text="Không tải được dữ liệu.\nKiểm tra cấu hình kết nối và chạy script nâng cấp database.\n\n"+ex.GetBaseException().Message,Font=Theme.F(12),ForeColor=Theme.Danger});
            host.Controls.Add(panel);
        }
    }
}
