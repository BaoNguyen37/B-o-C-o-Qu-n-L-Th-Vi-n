using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;
public class FrmConnection : Form
{
    readonly TextBox server=Theme.Input("Ví dụ: localhost hoặc .\\SQLEXPRESS",490),database=Theme.Input("Tên database",490);
    readonly Label message=new(){AutoSize=false,Size=new Size(490,72),ForeColor=Theme.Muted};
    readonly Button check=Theme.Btn("Kiểm tra & lưu",Theme.Accent,180),install=Theme.Btn("Tạo database mẫu NOVA",Theme.Sidebar,260);
    public FrmConnection()
    {
        Text="Kết nối SQL Server";ClientSize=new Size(550,460);StartPosition=FormStartPosition.CenterParent;
        FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;BackColor=Theme.Bg;Font=Theme.F();
        var cs=new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(QuanLyThuVien.DAL.DbHelper.ConnectionString);
        server.Text=cs.DataSource;database.Text=cs.InitialCatalog;
        Controls.Add(NovaStyle.Text("Kết nối thư viện của bạn",26,24,19,true));
        Controls.Add(NovaStyle.Text("Sử dụng Windows Authentication của máy hiện tại.",28,66,10,false,Theme.Muted));
        int y=112;Theme.Field(this,"SQL Server",server,ref y,28,490);Theme.Field(this,"Database",database,ref y,28,490);
        check.Location=new Point(28,250);install.Location=new Point(218,250);message.Location=new Point(28,310);
        message.Text="Tạo mẫu sẽ tạo QuanLyThuVienNova cùng dữ liệu từ bản gốc và các bảng mới. Có thể chạy lại; dữ liệu hiện có được giữ nguyên.";
        Controls.AddRange(new Control[]{check,install,message});
        check.Click+=async(_,_)=>await Work(false);install.Click+=async(_,_)=>await Work(true);
    }
    async Task Work(bool create)
    {
        if(string.IsNullOrWhiteSpace(server.Text)||string.IsNullOrWhiteSpace(database.Text)){message.Text="Nhập SQL Server và tên database.";return;}
        if(create&&!Theme.Confirm($"Tạo/nâng cấp database QuanLyThuVienNova trên {server.Text}?\nDatabase khác được giữ nguyên."))return;
        check.Enabled=install.Enabled=false;message.Text="Đang kết nối và kiểm tra…";
        try {
            string host=server.Text,db=create?"QuanLyThuVienNova":database.Text;
            if(create) {
                string path=Path.Combine(AppContext.BaseDirectory,"Database","00_CAI_MOI_NOVA.sql");
                var sql=await File.ReadAllTextAsync(path);await Task.Run(()=>DatabaseSetupBLL.Install(host,sql));database.Text=db;
            }
            string connection=DatabaseSetupBLL.Connection(host,db);await Task.Run(()=>DatabaseSetupBLL.Check(connection));
            await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory,"connection.txt"),connection);
            CauHinhBLL.SetConnectionString(connection);Theme.Info("Kết nối thành công. Đã lưu cấu hình.");DialogResult=DialogResult.OK;Close();
        }catch(Exception ex){message.Text="Chưa kết nối được: "+ex.GetBaseException().Message;message.ForeColor=Theme.Danger;}
        finally {check.Enabled=install.Enabled=true;}
    }
}
