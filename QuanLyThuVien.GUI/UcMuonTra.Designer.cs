namespace QuanLyThuVien.GUI;

// File giao diện do Windows Forms Designer quản lý. Có thể mở View Designer trong Visual Studio.
partial class UcMuonTra
{
    private System.ComponentModel.IContainer components = null;
    private ComboBox cboDG, cboSach, cboStatus;
    private DateTimePicker dtpMuon, dtpHan;
    private TextBox txtSearch;
    private DataGridView grid;
    private Button bMuon, bTra, bGh, bEx;

    private Panel list;
    private Panel gap;
    private Panel form;
    private Panel bar;
    private Label title;
    private Label lblcboDG;
    private Label lblcboSach;
    private Label lbldtpMuon;
    private Label lbldtpHan;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        this.SuspendLayout(); this.Size = new Size(1120,700); this.BackColor = Color.FromArgb(241,245,249); this.Padding = new Padding(20);
        list = new Panel(); list.Name="list"; list.Dock=DockStyle.Fill; list.BackColor=Color.White;
        gap = new Panel(); gap.Name="gap"; gap.Dock=DockStyle.Top; gap.Height=16; gap.BackColor=Color.FromArgb(241,245,249);
        form = new Panel(); form.Name="form"; form.Dock=DockStyle.Top; form.Height=128; form.BackColor=Color.White;
        this.Controls.Add(list); this.Controls.Add(gap); this.Controls.Add(form);
        bar=new Panel(); bar.Name="bar"; bar.Dock=DockStyle.Top; bar.Height=56; bar.BackColor=Color.White; list.Controls.Add(bar);
        grid = new DataGridView();
        grid.Name = "grid";
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.BackgroundColor = Color.White;
        list.Controls.Add(grid);
        list.Controls.SetChildIndex(bar, 0);
        txtSearch = new TextBox();
        txtSearch.Name = "txtSearch";
        txtSearch.Location = new Point(0, 10);
        txtSearch.Size = new Size(280, 30);
        bar.Controls.Add(txtSearch);
        cboStatus = new ComboBox();
        cboStatus.Name = "cboStatus";
        cboStatus.Location = new Point(292, 10);
        cboStatus.Size = new Size(150, 30);
        cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        bar.Controls.Add(cboStatus);
        bTra = new Button();
        bTra.Name = "bTra";
        bTra.Text = "✔ Trả sách";
        bTra.Location = new Point(462, 5);
        bTra.Size = new Size(120,38);
        bar.Controls.Add(bTra);
        bGh = new Button();
        bGh.Name = "bGh";
        bGh.Text = "Gia hạn +7 ngày";
        bGh.Location = new Point(592, 5);
        bGh.Size = new Size(170,38);
        bar.Controls.Add(bGh);
        bEx = new Button();
        bEx.Name = "bEx";
        bEx.Text = "⬇ Xuất CSV";
        bEx.Location = new Point(772, 5);
        bEx.Size = new Size(120,38);
        bar.Controls.Add(bEx);
        title=new Label(); title.Name="title"; title.Text="Lập phiếu mượn sách"; title.Font=new Font("Segoe UI",12F,FontStyle.Bold); title.Location=new Point(16,10); title.AutoSize=true; form.Controls.Add(title);
        lblcboDG=new Label(); lblcboDG.Name="lblcboDG"; lblcboDG.Text="Độc giả"; lblcboDG.AutoSize=true; lblcboDG.Location=new Point(16,40); form.Controls.Add(lblcboDG);
        cboDG = new ComboBox();
        cboDG.Name = "cboDG";
        cboDG.Location = new Point(16,62);
        cboDG.Size = new Size(250,30);
        form.Controls.Add(cboDG);
        lblcboSach=new Label(); lblcboSach.Name="lblcboSach"; lblcboSach.Text="Sách (còn trong kho)"; lblcboSach.AutoSize=true; lblcboSach.Location=new Point(282,40); form.Controls.Add(lblcboSach);
        cboSach = new ComboBox();
        cboSach.Name = "cboSach";
        cboSach.Location = new Point(282,62);
        cboSach.Size = new Size(290,30);
        form.Controls.Add(cboSach);
        lbldtpMuon=new Label(); lbldtpMuon.Name="lbldtpMuon"; lbldtpMuon.Text="Ngày mượn"; lbldtpMuon.AutoSize=true; lbldtpMuon.Location=new Point(588,40); form.Controls.Add(lbldtpMuon);
        dtpMuon = new DateTimePicker();
        dtpMuon.Name = "dtpMuon";
        dtpMuon.Location = new Point(588,62);
        dtpMuon.Size = new Size(120,30);
        form.Controls.Add(dtpMuon);
        lbldtpHan=new Label(); lbldtpHan.Name="lbldtpHan"; lbldtpHan.Text="Hạn trả"; lbldtpHan.AutoSize=true; lbldtpHan.Location=new Point(724,40); form.Controls.Add(lbldtpHan);
        dtpHan = new DateTimePicker();
        dtpHan.Name = "dtpHan";
        dtpHan.Location = new Point(724,62);
        dtpHan.Size = new Size(120,30);
        form.Controls.Add(dtpHan);
        bMuon = new Button();
        bMuon.Name = "bMuon";
        bMuon.Text = "＋ Cho mượn";
        bMuon.Location = new Point(862, 60);
        bMuon.Size = new Size(140,38);
        form.Controls.Add(bMuon);
        this.ResumeLayout(false);
    }
}
