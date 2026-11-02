using QuanLyThuVien.BLL;

namespace QuanLyThuVien.GUI;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Tuỳ chọn: đặt file "connection.txt" cạnh file .exe để đổi chuỗi kết nối mà không cần build lại.
        var cfg = Path.Combine(AppContext.BaseDirectory, "connection.txt");
        if (File.Exists(cfg)) CauHinhBLL.SetConnectionString(File.ReadAllText(cfg).Trim());

        Application.Run(new FrmLogin());
    }
}
