namespace QuanLyThuVien.GUI;
// Các control cố định có thể mở bằng View Designer trong Visual Studio.
partial class UcHeThong
{
    private System.ComponentModel.IContainer components=null;
    private Panel head;
    private Label lblTitle, lblNote, lblTime;
    private DataGridView grid;
    private Button btnCheck, btnExport;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose();base.Dispose(disposing); }
    private void InitializeComponent()
    {
        SuspendLayout();
        Name="UcHeThong";BackColor=Color.FromArgb(241,245,249);Size=new Size(1050,700);Padding=new Padding(16);
        head=new Panel{Name="head",Height=155,Dock=DockStyle.Top,BackColor=Color.White};
        lblTitle=new Label{Name="lblTitle",Text="KIỂM TRA HỆ THỐNG & CÔNG NỢ",Location=new Point(20,15),AutoSize=true,Font=new Font("Segoe UI",14,FontStyle.Bold)};
        lblNote=new Label{Name="lblNote",Text="Chỉ đọc dữ liệu SQL; kiểm tra cấu hình Gmail, ngân hàng và cảnh báo trước khi trình bày. Không thay đổi dữ liệu.",Location=new Point(20,50),Size=new Size(950,45)};
        btnCheck=new Button{Name="btnCheck",Text="Kiểm tra ngay (F5)",Location=new Point(20,101),Size=new Size(180,36),BackColor=Color.FromArgb(99,102,241),ForeColor=Color.White,FlatStyle=FlatStyle.Flat};
        btnExport=new Button{Name="btnExport",Text="Xuất kết quả CSV",Location=new Point(215,101),Size=new Size(175,36)};
        lblTime=new Label{Name="lblTime",Text="Chưa kiểm tra",Location=new Point(410,109),AutoSize=true,ForeColor=Color.FromArgb(100,116,139)};
        head.Controls.AddRange(new Control[]{lblTitle,lblNote,btnCheck,btnExport,lblTime});
        grid=new DataGridView{Name="grid",Dock=DockStyle.Fill,ReadOnly=true};
        Controls.Add(grid);Controls.Add(head);
        ResumeLayout(false);
    }
}
