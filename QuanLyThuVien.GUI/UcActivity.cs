using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;
public class UcActivity : UserControl
{
    readonly DataGridView grid=new();
    readonly TextBox search=Theme.Input("Tài khoản, bảng dữ liệu hoặc mã đối tượng…",400);
    public UcActivity()
    {
        Padding=new Padding(24);BackColor=Theme.Bg;
        var title=Theme.Title("Dấu vết thao tác, minh bạch dữ liệu");
        var note=Theme.Lbl("500 thao tác gần nhất • Ghi tự động khi thêm, sửa hoặc xóa dữ liệu nghiệp vụ",10,false,Theme.Muted);note.Dock=DockStyle.Top;note.Height=35;
        var bar=NovaStyle.Toolbar();var find=Theme.Btn("Tra cứu",Theme.Accent);var export=Theme.Btn("Xuất CSV",Theme.Sidebar);
        bar.Controls.AddRange(new Control[]{search,find,export});find.Click+=(_,_)=>LoadData();export.Click+=(_,_)=>Util.ExportGrid(grid,"NhatKyThaoTac");
        search.KeyDown+=(_,e)=>{if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;LoadData();}};
        Theme.StyleGrid(grid);Controls.Add(grid);Controls.Add(bar);Controls.Add(note);Controls.Add(title);LoadData();
    }
    void LoadData()=>NovaStyle.Safe(()=>{grid.DataSource=NovaBLL.Activity(search.Text);Theme.Cols(grid,"ThoiGian=Thời gian","NguoiDung=Tài khoản","DoiTuong=Đối tượng","HanhDong=Thao tác","MaDoiTuong=Mã đối tượng");Theme.Fmt(grid,"dd/MM/yyyy HH:mm:ss","ThoiGian");grid.ClearSelection();});
}
