namespace QuanLyThuVien.GUI;

// File giao diện do Windows Forms Designer quản lý. Có thể mở View Designer trong Visual Studio.
partial class FrmLogin
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtUser, txtPass;
    private Label lblErr;
    private Button btnLogin;
    private CheckBox chk;
    private Panel left, right;

    private Label hello;
    private Label sub;
    private Label l1;
    private Label l2;
    private Label hint;
    private Label logo;
    private Label t1;
    private Label t2;
    private Button close;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        this.SuspendLayout(); this.Text = "Đăng nhập - Quản lý thư viện"; this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen; this.ClientSize = new Size(860, 520); this.BackColor = Color.White;
        right = new Panel(); right.Name = "right"; right.Dock = DockStyle.Fill; right.BackColor = Color.White;
        left = new Panel(); left.Name = "left"; left.Dock = DockStyle.Left; left.Width = 380; left.BackColor = Color.FromArgb(67,56,202);
        this.Controls.Add(right); this.Controls.Add(left);
        hello = new Label(); hello.Name = "hello"; hello.Text = "Chào mừng trở lại!"; hello.Font = new Font("Segoe UI", 22F, FontStyle.Bold); hello.Location = new Point(70, 80); hello.AutoSize = true; right.Controls.Add(hello);
        sub = new Label(); sub.Name = "sub"; sub.Text = "Đăng nhập để tiếp tục quản lý thư viện"; sub.Font = new Font("Segoe UI", 10F); sub.Location = new Point(70, 130); sub.AutoSize = true; right.Controls.Add(sub);
        l1 = new Label(); l1.Name = "l1"; l1.Text = "Tên đăng nhập"; l1.Font = new Font("Segoe UI", 10F, FontStyle.Bold); l1.Location = new Point(70, 184); l1.AutoSize = true; right.Controls.Add(l1);
        l2 = new Label(); l2.Name = "l2"; l2.Text = "Mật khẩu"; l2.Font = new Font("Segoe UI", 10F, FontStyle.Bold); l2.Location = new Point(70, 258); l2.AutoSize = true; right.Controls.Add(l2);
        hint = new Label(); hint.Name = "hint"; hint.Text = "Tài khoản demo: admin / admin123 · thuthu / thuthu123"; hint.Font = new Font("Segoe UI", 8F); hint.Location = new Point(70, 462); hint.AutoSize = true; right.Controls.Add(hint);
        txtUser = new TextBox();
        txtUser.Name = "txtUser";
        txtUser.Location = new Point(70,210);
        txtUser.Size = new Size(340, 30);
        txtUser.UseSystemPasswordChar = false;
        right.Controls.Add(txtUser);
        txtPass = new TextBox();
        txtPass.Name = "txtPass";
        txtPass.Location = new Point(70,284);
        txtPass.Size = new Size(340, 30);
        txtPass.UseSystemPasswordChar = true;
        right.Controls.Add(txtPass);
        chk = new CheckBox();
        chk.Name = "chk";
        chk.Text = "Hiện mật khẩu";
        chk.Location = new Point(70, 322);
        chk.AutoSize = true;
        right.Controls.Add(chk);
        lblErr = new Label();
        lblErr.Name = "lblErr";
        lblErr.Location = new Point(70, 352);
        lblErr.Size = new Size(340, 42);
        lblErr.ForeColor = Color.Red;
        right.Controls.Add(lblErr);
        btnLogin = new Button();
        btnLogin.Name = "btnLogin";
        btnLogin.Location = new Point(70, 400);
        btnLogin.Size = new Size(340, 44);
        btnLogin.Text = "ĐĂNG NHẬP";
        btnLogin.BackColor = Color.FromArgb(99, 102, 241);
        btnLogin.ForeColor = Color.White;
        right.Controls.Add(btnLogin);
        logo = new Label(); logo.Name = "logo"; logo.Text = "📚"; logo.Font = new Font("Segoe UI", 46F, FontStyle.Bold); logo.ForeColor = Color.White; logo.Location = new Point(36,140); logo.AutoSize = true; left.Controls.Add(logo);
        t1 = new Label(); t1.Name = "t1"; t1.Text = "THƯ VIỆN"; t1.Font = new Font("Segoe UI", 28F, FontStyle.Bold); t1.ForeColor = Color.White; t1.Location = new Point(36,226); t1.AutoSize = true; left.Controls.Add(t1);
        t2 = new Label(); t2.Name = "t2"; t2.Text = "Hệ thống quản lý thư viện"; t2.Font = new Font("Segoe UI", 11F, FontStyle.Bold); t2.ForeColor = Color.White; t2.Location = new Point(36,290); t2.AutoSize = true; left.Controls.Add(t2);
        close = new Button(); close.Name = "btnClose"; close.Text = "✕"; close.Size = new Size(32, 32); close.Location = new Point(440, 5); close.FlatStyle = FlatStyle.Flat; close.FlatAppearance.BorderSize = 0; right.Controls.Add(close);
        close.Click += (s, e) => Application.Exit(); this.AcceptButton = btnLogin; this.ResumeLayout(false);
    }
}
