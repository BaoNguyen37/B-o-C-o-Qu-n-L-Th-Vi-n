using QuanLyThuVien.BLL;

namespace QuanLyThuVien.GUI;

public partial class FrmDoiMatKhau : Form
{

    public FrmDoiMatKhau()
    {
        InitializeComponent();
        ok.Click += (s, e) =>
        {
            try
            {
                AuthBLL.DoiMatKhau(txtOld.Text, txtNew.Text, txtCf.Text);
                Theme.Info("Đổi mật khẩu thành công!");
                Close();
            }
            catch (Exception ex) { Theme.Error(ex.Message); }
        };
    }
}
