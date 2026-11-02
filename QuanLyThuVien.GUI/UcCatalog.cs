using System.Data;
using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;

public class UcCatalog : UserControl
{
    readonly TextBox search=Theme.Input("Tên sách, tác giả, mã sách, vị trí kệ…",330);
    readonly ComboBox category=Theme.Combo(210);
    readonly CheckBox available=new(){Text="Chỉ còn sách",AutoSize=true,Margin=new Padding(16,8,8,0)};
    readonly FlowLayoutPanel books=new(){Dock=DockStyle.Fill,AutoScroll=true,Padding=new Padding(0,8,0,0)};
    readonly Label count=Theme.Lbl("",10,false,Theme.Muted);
    readonly System.Windows.Forms.Timer debounce=new(){Interval=280};
    bool ready;
    public UcCatalog()
    {
        BackColor=Theme.Bg;Padding=new Padding(24);Font=Theme.F();
        var bar=NovaStyle.Toolbar();bar.Controls.AddRange(new Control[]{search,category,available});
        var hint=Theme.Lbl("Tra cứu kho sách • Bấm vào sách để xem chi tiết và đặt trước",10,false,Theme.Muted);hint.Dock=DockStyle.Top;hint.Height=34;
        count.Dock=DockStyle.Bottom;count.Height=28;
        Controls.Add(books);Controls.Add(count);Controls.Add(bar);Controls.Add(hint);
        var categories=TheLoaiBLL.GetAll();var all=categories.NewRow();all["MaTL"]=0;all["TenTL"]="Tất cả thể loại";categories.Rows.InsertAt(all,0);
        category.DisplayMember="TenTL";category.ValueMember="MaTL";category.DataSource=categories;
        debounce.Tick+=(_,_)=>{debounce.Stop();LoadBooks();};
        search.TextChanged+=(_,_)=>{debounce.Stop();debounce.Start();};
        category.SelectedIndexChanged+=(_,_)=>{if(ready)LoadBooks();};available.CheckedChanged+=(_,_)=>LoadBooks();
        ready=true;LoadBooks();
    }
    void LoadBooks()=>NovaStyle.Safe(()=>{
        var data=NovaBLL.Catalog(search.Text,Convert.ToInt32(category.SelectedValue??0),available.Checked);
        books.SuspendLayout();foreach(Control c in books.Controls.Cast<Control>().ToArray())c.Dispose();books.Controls.Clear();
        foreach(DataRow row in data.Rows) books.Controls.Add(Book(row));
        if(data.Rows.Count==0)books.Controls.Add(new Label{Text="Không tìm thấy sách. Thử từ khóa khác hoặc đổi bộ lọc.",AutoSize=true,ForeColor=Theme.Muted,Margin=new Padding(8,30,8,8)});
        books.ResumeLayout();count.Text=$"{data.Rows.Count} đầu sách phù hợp • Tìm kiếm hỗ trợ không dấu";
    });
    Control Book(DataRow r)
    {
        var card=new Card{Size=new Size(278,274),Margin=new Padding(0,0,16,16),Padding=new Padding(14),Cursor=Cursors.Hand};
        int id=Convert.ToInt32(r["MaSach"]),stock=Convert.ToInt32(r["SoLuong"]);
        Color[] palette={Color.FromArgb(28,75,111),Color.FromArgb(30,103,112),Color.FromArgb(72,83,140),Color.FromArgb(120,78,62)};
        var cover=new Panel{Location=new Point(14,14),Size=new Size(250,104),BackColor=palette[id%4]};
        cover.Controls.Add(new Panel{Location=new Point(14,0),Size=new Size(2,104),BackColor=Color.FromArgb(106,155,185)});
        cover.Controls.Add(NovaStyle.Text($"N / {id:D4}",28,15,9,true,Color.FromArgb(185,217,230)));
        var genre=NovaStyle.Text(Convert.ToString(r["TheLoai"]),28,49,14,true,Color.White);genre.AutoSize=false;genre.Size=new Size(207,47);cover.Controls.Add(genre);
        var title=NovaStyle.Text(Convert.ToString(r["TenSach"]),14,132,12,true);title.AutoSize=false;title.Size=new Size(250,48);title.AutoEllipsis=true;
        var author=NovaStyle.Text(Convert.ToString(r["TacGia"]),14,187,9,false,Theme.Muted);author.AutoSize=false;author.Size=new Size(250,22);author.AutoEllipsis=true;
        var info=NovaStyle.Text(stock>0?$"Còn {stock} cuốn  /  {r["ViTri"]}":"Hết sách · Có thể đặt trước",14,218,9,true,stock>0?Theme.Success:Theme.Danger);
        var foot=NovaStyle.Text($"{r["LuotMuon"]} lượt mượn     {r["HangCho"]} đang chờ",14,246,8,false,Theme.Muted);
        card.Controls.AddRange(new Control[]{cover,title,author,info,foot});
        void Clickable(Control c){c.Click+=(_,_)=>Details(r);foreach(Control child in c.Controls)Clickable(child);}Clickable(card);return card;
    }
    void Details(DataRow r)
    {
        using var form=new Form{Text="Chi tiết sách",Size=new Size(620,530),StartPosition=FormStartPosition.CenterParent,BackColor=Theme.Bg,Font=Theme.F(),FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false};
        var title=NovaStyle.Text(Convert.ToString(r["TenSach"]),24,24,20,true);title.AutoSize=false;title.Size=new Size(550,76);
        var meta=NovaStyle.Text($"{r["TacGia"]}\n\n{r["TheLoai"]} • {r["NamXB"]} • {r["NhaXuatBan"]}\n\nVị trí: {r["ViTri"]}      Còn: {r["SoLuong"]} cuốn\n\nGiá bìa: {Convert.ToDecimal(r["GiaBia"]):N0} đ\n\nĐã mượn {r["LuotMuon"]} lần / {r["HangCho"]} lượt đang chờ",26,110,11,false,Theme.Muted);
        var reader=Theme.Combo(540);reader.Location=new Point(26,345);reader.DisplayMember="Ten";reader.ValueMember="MaDG";reader.DataSource=DocGiaBLL.GetForCombo();
        var reserve=Theme.Btn("Đặt trước cho độc giả",Theme.Accent,240);reserve.Location=new Point(26,402);
        reserve.Click+=(_,_)=>NovaStyle.Safe(()=>{NovaBLL.Reserve(Convert.ToInt32(reader.SelectedValue??0),Convert.ToInt32(r["MaSach"]));Theme.Info("Đã thêm vào hàng chờ. Vào Đặt trước / Hàng chờ để nhận sách theo thứ tự.");form.Close();LoadBooks();});
        form.Controls.AddRange(new Control[]{title,meta,reader,reserve});form.ShowDialog(this);
    }
    protected override void Dispose(bool disposing){if(disposing)debounce.Dispose();base.Dispose(disposing);}
}
