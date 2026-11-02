using System.Data;
using QRCoder;
using QuanLyThuVien.BLL;
namespace QuanLyThuVien.GUI;
public class FrmReaderProfile : Form
{
    readonly Bitmap cardImage;
    public FrmReaderProfile(int id,string name)
    {
        Text=$"Hồ sơ độc giả #{id}";ClientSize=new Size(1000,700);MinimumSize=new Size(850,600);StartPosition=FormStartPosition.CenterParent;Font=Theme.F();BackColor=Theme.Bg;Padding=new Padding(24);
        var top=new Panel{Dock=DockStyle.Top,Height=252};
        cardImage=CreateCard(id,name);
        top.Controls.Add(new PictureBox{Image=cardImage,SizeMode=PictureBoxSizeMode.Zoom,Location=new Point(0,0),Size=new Size(430,232)});
        top.Controls.Add(NovaStyle.Text(name,464,12,21,true));
        var loans=PhieuMuonBLL.GetAll();loans.DefaultView.RowFilter=$"MaDG={id}";var data=loans.DefaultView.ToTable();
        int active=data.AsEnumerable().Count(r=>r.IsNull("NgayTra"));int overdue=data.AsEnumerable().Count(r=>r.IsNull("NgayTra")&&r.Field<DateTime>("HanTra")<DateTime.Today);
        top.Controls.Add(NovaStyle.Text($"Mã độc giả: DG{id:D6}\n\n{data.Rows.Count} lượt mượn • {active} đang mượn • {overdue} quá hạn",466,63,11,false,Theme.Muted));
        var save=Theme.Btn("Lưu thẻ QR (.png)",Theme.Accent,210);save.Location=new Point(466,155);save.Click+=(_,_)=>NovaStyle.Safe(()=>{using var d=new SaveFileDialog{Filter="PNG (*.png)|*.png",FileName=$"TheDocGia_{id:D6}.png"};if(d.ShowDialog(this)==DialogResult.OK){cardImage.Save(d.FileName,System.Drawing.Imaging.ImageFormat.Png);Theme.Info("Đã lưu thẻ độc giả.");}});top.Controls.Add(save);
        top.Controls.Add(NovaStyle.Text("Quét thẻ bằng đầu đọc QR hoặc dán mã ở màn Mượn / Trả.",466,204,9,false,Theme.Muted));
        var grid=new DataGridView();Theme.StyleGrid(grid);Controls.Add(grid);grid.DataSource=data;Theme.Hide(grid,"MaDG","DocGia");
        Theme.Cols(grid,"MaPM=Mã phiếu","TenSach=Tên sách","NgayMuon=Ngày mượn","HanTra=Hạn trả","NgayTra=Ngày trả","TienPhat=Phạt trễ","SoLanGiaHan=Gia hạn","TrangThai=Trạng thái");Theme.Fmt(grid,"dd/MM/yyyy","NgayMuon","HanTra","NgayTra");Theme.Fmt(grid,"N0","TienPhat");Theme.Weight(grid,170,"TenSach");
        var title=Theme.Title("Lịch sử mượn sách của độc giả");
        Controls.Add(grid);Controls.Add(title);Controls.Add(top);
    }
    public static Bitmap CreateCard(int id,string name)
    {
        var image=new Bitmap(860,464);using var g=Graphics.FromImage(image);g.Clear(Theme.Sidebar);g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var white=new SolidBrush(Color.White);using var muted=new SolidBrush(Color.FromArgb(177,206,225));using var cyan=new SolidBrush(Theme.Sky);
        using var brand=Theme.F(30,FontStyle.Bold);using var font=Theme.F(20);using var small=Theme.F(15);
        g.FillRectangle(cyan,0,0,12,464);g.DrawString("NOVA / Library",brand,white,44,36);
        g.DrawString("Thẻ độc giả",small,muted,48,105);
        g.DrawString(name,font,white,new RectangleF(46,186,500,115));g.DrawString($"DG{id:D6}",font,cyan,46,311);
        g.DrawString("Kết nối tri thức. Mở lối tương lai.",small,muted,46,405);
        using var qrData=QRCodeGenerator.GenerateQrCode(ReaderCode.Create(id),QRCodeGenerator.ECCLevel.Q);using var qr=new PngByteQRCode(qrData);
        using var stream=new MemoryStream(qr.GetGraphic(8));using var qrImage=Image.FromStream(stream);
        g.DrawImage(qrImage,new Rectangle(580,154,232,232));return image;
    }
    protected override void Dispose(bool disposing){if(disposing)cardImage?.Dispose();base.Dispose(disposing);}
}
