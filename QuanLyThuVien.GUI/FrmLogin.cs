using System.Runtime.InteropServices;
using QuanLyThuVien.BLL;

namespace QuanLyThuVien.GUI;

public partial class FrmLogin : Form
{
    [DllImport("user32.dll")] static extern bool ReleaseCapture();
    [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr h, int msg, int w, int l);


    public FrmLogin()
    {
        InitializeComponent();
        NovaStyle.Apply(this);
        Text="NOVA Library · Đăng nhập";
        left.BackColor=Theme.Sidebar;
        logo.Text="N /";logo.Font=Theme.F(38,FontStyle.Bold);logo.ForeColor=Theme.Sky;
        t1.Text="NOVA Library";t1.Font=Theme.F(25,FontStyle.Bold);t1.Location=new Point(30,220);
        t2.Text="Kết nối tri thức.\nMở lối tương lai.";t2.Font=Theme.F(14);t2.Location=new Point(36,290);t2.Size=new Size(300,72);t2.AutoSize=false;
        hello.Text="Chào mừng trở lại";hello.Font=Theme.F(20,FontStyle.Bold);
        btnLogin.Text="Vào không gian thư viện";
        hint.Text="Bản mẫu: admin / admin123  |  thuthu / thuthu123";
        var config=new Button{Text="Cấu hình SQL Server",FlatStyle=FlatStyle.Flat,ForeColor=Theme.Accent,BackColor=Color.White,Location=new Point(70,486),Size=new Size(340,28)};
        config.FlatAppearance.BorderSize=0;config.Click+=(_,_)=>{using var f=new FrmConnection();f.ShowDialog(this);};right.Controls.Add(config);
        var demo=Theme.Btn("Xem thử giao diện",Color.FromArgb(41,72,97),300);demo.Location=new Point(36,420);
        demo.Click+=(_,_)=>{AuthBLL.StartPreview();OpenMain();};left.Controls.Add(demo);
        left.Controls.Add(NovaStyle.Text("Dữ liệu mẫu • Chỉ đọc • Không cần SQL",36,467,9,false,Color.FromArgb(177,206,225)));
        txtUser.Font=txtPass.Font=Theme.F(12);
        chk.CheckedChanged += (s, e) => txtPass.UseSystemPasswordChar = !chk.Checked;
        btnLogin.Click += (s, e) => DoLogin();
        Shown += (s, e) => txtUser.Focus();
    }

    void Drag(object s, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        ReleaseCapture();
        SendMessage(Handle, 0xA1, 0x2, 0);
    }

    async void DoLogin()
    {
        lblErr.Text = "";
        btnLogin.Enabled=false;left.Enabled=right.Enabled=false;
        try
        {
            string username=txtUser.Text,password=txtPass.Text;
            if (!await Task.Run(()=>AuthBLL.Login(username,password)))
            {
                lblErr.Text = "Sai tên đăng nhập hoặc mật khẩu.";
                return;
            }
            OpenMain();
        }
        catch (Exception)
        {
            lblErr.Text = "Không kết nối được CSDL. Mở Cấu hình SQL Server để kiểm tra.";
        }
        finally {btnLogin.Enabled=true;left.Enabled=right.Enabled=true;}
    }
    void OpenMain()
    {
            Hide();
            var main = new FrmMain();
            main.FormClosed += (s, e) =>
            {
                if (main.LoggedOut)
                {
                    AuthBLL.Logout();
                    txtPass.Clear();
                    Show();
                }
                else Close();
            };
            main.Show();
    }

}
