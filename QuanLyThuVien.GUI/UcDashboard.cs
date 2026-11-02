using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;
public partial class UcDashboard : UserControl
{
    public UcDashboard(Action<string> navigate=null)
    {
        InitializeComponent();BackColor=Theme.Bg;Padding=new Padding(24);AutoScroll=true;
        var root=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=1,RowCount=4,MinimumSize=new Size(850,680)};
        root.RowStyles.Add(new RowStyle(SizeType.Absolute,190));root.RowStyles.Add(new RowStyle(SizeType.Absolute,124));
        root.RowStyles.Add(new RowStyle(SizeType.Percent,100));root.RowStyles.Add(new RowStyle(SizeType.Absolute,42));Controls.Add(root);
        var hero=new LibraryBanner{Dock=DockStyle.Fill,Margin=new Padding(0,0,0,20)};
        hero.Controls.Add(NovaStyle.Text("Mỗi cuốn sách, một kết nối.",24,20,23,true,Color.White));
        hero.Controls.Add(NovaStyle.Text("Theo dõi lưu thông, kết nối độc giả và chăm sóc kho tri thức.",26,67,10,false,Color.FromArgb(177,206,225)));
        var browse=Theme.Btn("Khám phá kho sách",Theme.Accent,180);browse.Location=new Point(26,111);
        browse.Click+=(_,_)=>navigate?.Invoke("Khám phá kho sách");hero.Controls.Add(browse);
        var borrow=Theme.Btn("Lập phiếu mượn",Color.FromArgb(41,72,97),166);borrow.Location=new Point(218,111);
        borrow.Click+=(_,_)=>navigate?.Invoke("Mượn / Trả sách");hero.Controls.Add(borrow);root.Controls.Add(hero,0,0);
        var s=ThongKeBLL.Summary().Rows[0];var pulse=NovaBLL.Pulse().Rows[0];
        var stats=Columns(25,25,25,25);root.Controls.Add(stats,0,1);
        string[] labels={"Đầu sách trong thư viện","Độc giả đã đăng ký","Phiếu đang mượn","Phiếu quá hạn"};
        string[] fields={"DauSach","DocGia","DangMuon","QuaHan"};
        string[] notes={$"{s["TrongKho"]} cuốn sẵn sàng phục vụ","Kết nối cộng đồng đọc sách","Theo dõi đến khi hoàn trả","Ưu tiên nhắc trả hôm nay"};
        for(int i=0;i<4;i++) {
            var card=new Card{Dock=DockStyle.Fill,Margin=new Padding(0,0,i==3?0:14,18)};
            var color=i==3?Theme.Danger:Theme.Accent;
            card.Controls.Add(NovaStyle.Text(labels[i],16,12,9,false,Theme.Muted));
            card.Controls.Add(NovaStyle.Text(Convert.ToInt32(s[fields[i]]).ToString("N0"),14,30,27,true,color));
            card.Controls.Add(NovaStyle.Text(notes[i],16,79,8,false,Theme.Muted));stats.Controls.Add(card,i,0);
        }
        var main=Columns(45,55);root.Controls.Add(main,0,2);
        var chartCard=new Card{Dock=DockStyle.Fill,Padding=new Padding(20),Margin=new Padding(0,0,16,0)};
        var chart=new BarChart{Dock=DockStyle.Fill,BarColor=Theme.Accent};chart.SetData(ThongKeBLL.MuonTheoThang());
        var chartTitle=Theme.Title("Nhịp đọc của thư viện");
        var chartSub=new Label{Text="Lượt mượn trong 6 tháng gần nhất",Dock=DockStyle.Top,Height=32,ForeColor=Theme.Muted};
        chartCard.Controls.Add(chart);chartCard.Controls.Add(chartSub);chartCard.Controls.Add(chartTitle);main.Controls.Add(chartCard,0,0);
        var attention=new Card{Dock=DockStyle.Fill,Padding=new Padding(16),Margin=Padding.Empty};
        var grid=new DataGridView();Theme.StyleGrid(grid);grid.DataSource=NovaBLL.Attention();
        Theme.Fmt(grid,"dd/MM/yyyy","Hạn trả");Theme.Weight(grid,40,"Mã phiếu");Theme.Weight(grid,170,"Tên sách");
        grid.CellFormatting+=(_,e)=>{if(e.RowIndex>=0 && grid.Columns[e.ColumnIndex].Name=="Tình trạng")e.CellStyle.ForeColor=Convert.ToString(e.Value)=="Quá hạn"?Theme.Danger:Theme.Warning;};
        attention.Controls.Add(grid);attention.Controls.Add(Theme.Title("Cần bạn xử lý"));main.Controls.Add(attention,1,0);
        var footer=Theme.Lbl($"{pulse["Cho"]} lượt đặt trước đang chờ     •     {pulse["SapHet"]} đầu sách còn ≤ 2 cuốn     •     Công nợ: {Convert.ToDecimal(pulse["CongNo"]):N0} đ",9,false,Theme.Muted);
        footer.Dock=DockStyle.Fill;footer.TextAlign=ContentAlignment.MiddleLeft;root.Controls.Add(footer,0,3);
    }
    static TableLayoutPanel Columns(params float[] widths)
    {
        var p=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=widths.Length,RowCount=1,Margin=Padding.Empty};
        foreach(var w in widths)p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,w));
        p.RowStyles.Add(new RowStyle(SizeType.Percent,100));return p;
    }
}
