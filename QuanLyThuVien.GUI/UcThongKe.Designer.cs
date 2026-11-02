namespace QuanLyThuVien.GUI;

// File giao diện do Windows Forms Designer quản lý. Có thể mở View Designer trong Visual Studio.
partial class UcThongKe
{
    private System.ComponentModel.IContainer components = null;
    private BarChart ch1,ch2;
    private DataGridView grid;
    private Label l1,l2,l3;
    private Button b;

    private TableLayoutPanel root;
    private Panel c0;
    private Label title0;
    private Panel c1;
    private Label title1;
    private Panel c2;
    private Label title2;
    private Panel c3;
    private Label title3;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        this.SuspendLayout(); this.BackColor=Color.FromArgb(241,245,249); this.Padding=new Padding(20); this.Size=new Size(1100,700);
        root=new TableLayoutPanel(); root.Name="root"; root.Dock=DockStyle.Fill; root.ColumnCount=2; root.RowCount=2;
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50)); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));
        root.RowStyles.Add(new RowStyle(SizeType.Percent,55)); root.RowStyles.Add(new RowStyle(SizeType.Percent,45)); this.Controls.Add(root);
        c0=new Panel(); c0.Name="c0"; c0.Dock=DockStyle.Fill; c0.BackColor=Color.White; c0.Margin=new Padding(0,0,16,16); root.Controls.Add(c0,0,0);
        title0=new Label(); title0.Name="title0"; title0.Text="Số cuốn trong kho theo thể loại"; title0.Font=new Font("Segoe UI",12F,FontStyle.Bold); title0.Dock=DockStyle.Top; title0.Height=38; c0.Controls.Add(title0);
        c1=new Panel(); c1.Name="c1"; c1.Dock=DockStyle.Fill; c1.BackColor=Color.White; c1.Margin=new Padding(0,0,16,16); root.Controls.Add(c1,1,0);
        title1=new Label(); title1.Name="title1"; title1.Text="Top 5 sách được mượn nhiều nhất"; title1.Font=new Font("Segoe UI",12F,FontStyle.Bold); title1.Dock=DockStyle.Top; title1.Height=38; c1.Controls.Add(title1);
        c2=new Panel(); c2.Name="c2"; c2.Dock=DockStyle.Fill; c2.BackColor=Color.White; c2.Margin=new Padding(0,0,16,16); root.Controls.Add(c2,0,1);
        title2=new Label(); title2.Name="title2"; title2.Text="Độc giả tích cực nhất"; title2.Font=new Font("Segoe UI",12F,FontStyle.Bold); title2.Dock=DockStyle.Top; title2.Height=38; c2.Controls.Add(title2);
        c3=new Panel(); c3.Name="c3"; c3.Dock=DockStyle.Fill; c3.BackColor=Color.White; c3.Margin=new Padding(0,0,16,16); root.Controls.Add(c3,1,1);
        title3=new Label(); title3.Name="title3"; title3.Text="Báo cáo tổng hợp"; title3.Font=new Font("Segoe UI",12F,FontStyle.Bold); title3.Dock=DockStyle.Top; title3.Height=38; c3.Controls.Add(title3);
        ch1 = new BarChart();
        ch1.Name = "ch1";
        ch1.Dock = DockStyle.Fill;
        c0.Controls.Add(ch1);
        ch2 = new BarChart();
        ch2.Name = "ch2";
        ch2.Dock = DockStyle.Fill;
        c1.Controls.Add(ch2);
        grid = new DataGridView();
        grid.Name = "grid";
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        c2.Controls.Add(grid);
        l1 = new Label();
        l1.Name = "l1";
        l1.AutoSize = true;
        l1.Location = new Point(18,56);
        l1.Text = "";
        c3.Controls.Add(l1);
        l2 = new Label();
        l2.Name = "l2";
        l2.AutoSize = true;
        l2.Location = new Point(18,96);
        l2.Text = "";
        c3.Controls.Add(l2);
        l3 = new Label();
        l3.Name = "l3";
        l3.AutoSize = true;
        l3.Location = new Point(18,136);
        l3.Text = "";
        c3.Controls.Add(l3);
        b = new Button();
        b.Name = "b";
        b.Text = "⬇ Xuất báo cáo mượn/trả (CSV)";
        b.Location = new Point(18,172);
        b.Size = new Size(290,38);
        c3.Controls.Add(b);
        c0.Controls.SetChildIndex(title0,0);
        c1.Controls.SetChildIndex(title1,0);
        c2.Controls.SetChildIndex(title2,0);
        this.ResumeLayout(false);
    }
}
