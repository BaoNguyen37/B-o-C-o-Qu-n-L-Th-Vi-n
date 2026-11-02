namespace QuanLyThuVien.GUI;

// File giao diện do Windows Forms Designer quản lý. Có thể mở View Designer trong Visual Studio.
partial class FrmDoiMatKhau
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtOld, txtNew, txtCf;
    private Button ok;

    private Label title;
    private Label lbltxtOld;
    private Label lbltxtNew;
    private Label lbltxtCf;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        this.SuspendLayout(); this.Text = "Đổi mật khẩu"; this.ClientSize = new Size(380, 340);
        this.FormBorderStyle = FormBorderStyle.FixedDialog; this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false; this.MinimizeBox = false; this.BackColor = Color.White;
        title = new Label(); title.Name = "title"; title.Text = "Đổi mật khẩu"; title.Font = new Font("Segoe UI", 15F, FontStyle.Bold); title.Location = new Point(30, 20); title.AutoSize = true; this.Controls.Add(title);
        lbltxtOld = new Label(); lbltxtOld.Name = "lbltxtOld"; lbltxtOld.Text = "Mật khẩu hiện tại"; lbltxtOld.Location = new Point(30, 70); lbltxtOld.AutoSize = true; this.Controls.Add(lbltxtOld);
        txtOld = new TextBox();
        txtOld.Name = "txtOld";
        txtOld.Location = new Point(30, 92);
        txtOld.Size = new Size(320, 30);
        txtOld.UseSystemPasswordChar = true;
        this.Controls.Add(txtOld);
        lbltxtNew = new Label(); lbltxtNew.Name = "lbltxtNew"; lbltxtNew.Text = "Mật khẩu mới (≥ 6 ký tự)"; lbltxtNew.Location = new Point(30, 132); lbltxtNew.AutoSize = true; this.Controls.Add(lbltxtNew);
        txtNew = new TextBox();
        txtNew.Name = "txtNew";
        txtNew.Location = new Point(30, 154);
        txtNew.Size = new Size(320, 30);
        txtNew.UseSystemPasswordChar = true;
        this.Controls.Add(txtNew);
        lbltxtCf = new Label(); lbltxtCf.Name = "lbltxtCf"; lbltxtCf.Text = "Nhập lại mật khẩu mới"; lbltxtCf.Location = new Point(30, 194); lbltxtCf.AutoSize = true; this.Controls.Add(lbltxtCf);
        txtCf = new TextBox();
        txtCf.Name = "txtCf";
        txtCf.Location = new Point(30, 216);
        txtCf.Size = new Size(320, 30);
        txtCf.UseSystemPasswordChar = true;
        this.Controls.Add(txtCf);
        ok = new Button();
        ok.Name = "ok";
        ok.Text = "Lưu thay đổi";
        ok.Location = new Point(30, 270);
        ok.Size = new Size(320, 38);
        ok.BackColor = Color.FromArgb(99, 102, 241);
        ok.ForeColor = Color.White;
        this.Controls.Add(ok);
        this.AcceptButton = ok; this.ResumeLayout(false);
    }
}
