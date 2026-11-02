using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;

public class UcReservations : UserControl
{
    readonly DataGridView grid=new();
    readonly ComboBox readers=Theme.Combo(270),books=Theme.Combo(380),status=Theme.Combo(160);
    readonly Label note=Theme.Lbl("",10,false,Theme.Muted);
    public UcReservations()
    {
        Padding=new Padding(24);BackColor=Theme.Bg;Font=Theme.F();
        var title=Theme.Title("Đặt trước hôm nay, kết nối lượt đọc tiếp theo.");title.Height=42;
        var help=new Label{Text="Tối đa 3 đặt trước/độc giả. Người đứng đầu hàng chờ được ưu tiên nhận sách.\nNhận sách tạo phiếu mượn 14 ngày và trừ tồn kho trong cùng giao dịch.",Dock=DockStyle.Top,Height=56,ForeColor=Theme.Muted};
        readers.DisplayMember="Ten";readers.ValueMember="MaDG";readers.DataSource=DocGiaBLL.GetForCombo();
        var catalog=NovaBLL.Catalog();books.DisplayMember="TenSach";books.ValueMember="MaSach";books.DataSource=catalog;
        var create=Theme.Btn("Thêm đặt trước",Theme.Accent,155);create.Click+=(_,_)=>Run(()=>NovaBLL.Reserve(Convert.ToInt32(readers.SelectedValue??0),Convert.ToInt32(books.SelectedValue??0)));
        var add=NovaStyle.Toolbar();add.Controls.AddRange(new Control[]{readers,books,create});
        status.Items.AddRange(new object[]{"Đang chờ","Đã nhận","Đã hủy","Tất cả"});status.SelectedIndex=0;
        var receive=Theme.Btn("Nhận sách",Theme.Success,130);var cancel=Theme.Btn("Hủy đặt trước",Theme.Danger,150);var export=Theme.Btn("Xuất CSV",Theme.Sidebar,115);
        var bar=NovaStyle.Toolbar();bar.Controls.AddRange(new Control[]{status,receive,cancel,export});
        receive.Click+=(_,_)=>NovaStyle.Safe(()=>{int id=Selected();if(Theme.Confirm($"Tạo phiếu mượn cho đặt trước #{id}?"))Run(()=>NovaBLL.Receive(id));});
        cancel.Click+=(_,_)=>NovaStyle.Safe(()=>{int id=Selected();if(Theme.Confirm($"Hủy đặt trước #{id}?"))Run(()=>NovaBLL.Cancel(id));});
        export.Click+=(_,_)=>Util.ExportGrid(grid,"HangCho");status.SelectedIndexChanged+=(_,_)=>LoadData();
        note.Dock=DockStyle.Bottom;note.Height=34;Theme.StyleGrid(grid);
        Controls.Add(grid);Controls.Add(note);Controls.Add(bar);Controls.Add(add);Controls.Add(help);Controls.Add(title);LoadData();
    }
    int Selected()=>grid.SelectedRows.Count>0?Convert.ToInt32(grid.SelectedRows[0].Cells["MaDat"].Value):throw new NghiepVuException("Chọn một đặt trước trong danh sách.");
    void Run(Action act)=>NovaStyle.Safe(()=>{act();LoadData();Theme.Info("Đã cập nhật hàng chờ.");});
    void LoadData()=>NovaStyle.Safe(()=>{
        grid.DataSource=NovaBLL.Reservations(status.Text);Theme.Hide(grid,"MaDG","MaSach");
        Theme.Cols(grid,"MaDat=Mã đặt","DocGia=Độc giả","TenSach=Tên sách","NgayDat=Ngày đặt","TrangThai=Trạng thái","SoLuong=Còn trong kho","ThuTu=Thứ tự chờ");
        Theme.Fmt(grid,"dd/MM/yyyy HH:mm","NgayDat");Theme.Weight(grid,170,"TenSach");Theme.Weight(grid,45,"MaDat","ThuTu");
        note.Text=$"{grid.Rows.Count} lượt đặt trước • Chọn một dòng để nhận sách hoặc hủy";grid.ClearSelection();
    });
}
