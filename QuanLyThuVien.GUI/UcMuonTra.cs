using System.Data;
using QuanLyThuVien.BLL;

namespace QuanLyThuVien.GUI;

public partial class UcMuonTra : UserControl
{
    static readonly Font statusFont = Theme.F(10f, FontStyle.Bold);
    bool loading = true;

    static DateTimePicker Dtp() => new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Font = Theme.F(10.5f) };

    public UcMuonTra()
    {
        InitializeComponent();
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        Theme.StyleGrid(grid);
        bMuon.BackColor=Theme.Accent;bMuon.ForeColor=Color.White;
        bTra.BackColor=Theme.Success;bTra.ForeColor=Color.White;
        bGh.BackColor=Theme.Accent;bGh.ForeColor=Color.White;
        bEx.BackColor=Theme.Sidebar;bEx.ForeColor=Color.White;
        txtSearch.PlaceholderText="Tìm độc giả, tên sách, mã phiếu…";
        form.AutoScroll=false;form.Height=164;
        var scan=Theme.Input("Quét/dán mã thẻ NOVA:DG:000001 rồi Enter",430);scan.Location=new Point(16,113);form.Controls.Add(scan);
        var scanHint=NovaStyle.Text("Thẻ QR giúp chọn độc giả nhanh hơn.",462,118,9,false,Theme.Muted);form.Controls.Add(scanHint);
        void ArrangeLoanForm() {
            int space=Math.Max(400,form.ClientSize.Width)-410;
            int rw=(int)(space*0.44),bw=space-rw,x=16;
            cboDG.SetBounds(x,62,rw,28);lblcboDG.Location=new Point(x,40);x+=rw+12;
            cboSach.SetBounds(x,62,bw,28);lblcboSach.Location=new Point(x,40);x+=bw+12;
            dtpMuon.SetBounds(x,62,110,28);lbldtpMuon.Location=new Point(x,40);x+=122;
            dtpHan.SetBounds(x,62,110,28);lbldtpHan.Location=new Point(x,40);x+=122;
            bMuon.SetBounds(x,60,110,38);
        }
        form.Resize+=(_,_)=>ArrangeLoanForm();ArrangeLoanForm();
        scan.KeyDown+=(_,e)=>{if(e.KeyCode!=Keys.Enter)return;e.SuppressKeyPress=true;
            if(!ReaderCode.TryParse(scan.Text,out int readerId)){Theme.Error("Mã thẻ không hợp lệ. Định dạng: NOVA:DG:000001");return;}
            cboDG.SelectedValue=readerId;if(cboDG.SelectedIndex<0)Theme.Error("Không tìm thấy độc giả của thẻ này.");else scan.Clear();};
        cboStatus.Items.AddRange(new object[] { "Tất cả", "Đang mượn", "Quá hạn", "Đã trả", "Mất sách", "Hỏng sách" });
        cboStatus.SelectedIndex = 0;
        dtpMuon.Format = DateTimePickerFormat.Custom; dtpMuon.CustomFormat="dd/MM/yyyy";
        dtpHan.Format = DateTimePickerFormat.Custom; dtpHan.CustomFormat="dd/MM/yyyy";
        // ----- Sự kiện -----
        dtpHan.Value = DateTime.Today.AddDays(PhieuMuonBLL.SoNgayMuon);
        dtpMuon.ValueChanged += (s, e) => dtpHan.Value = dtpMuon.Value.AddDays(PhieuMuonBLL.SoNgayMuon);

        bMuon.Click += (s, e) =>
        {
            try
            {
                if (cboDG.SelectedValue == null || cboSach.SelectedValue == null)
                    throw new NghiepVuException("Vui lòng chọn độc giả và sách.");
                PhieuMuonBLL.Muon(Convert.ToInt32(cboDG.SelectedValue), Convert.ToInt32(cboSach.SelectedValue),
                                  dtpMuon.Value.Date, dtpHan.Value.Date);
                Theme.Info($"Đã lập phiếu mượn thành công!\nHạn trả: {dtpHan.Value:dd/MM/yyyy}");
                LoadCombos();
                LoadGrid();
            }
            catch (Exception ex) { Theme.Error(ex.Message); }
        };

        bTra.Click += (s, e) =>
        {
            try
            {
                int id = SelectedId();
                var phat = PhieuMuonBLL.DuKienPhat(id);
                var msg = phat > 0
                    ? $"Phiếu #{id} trả trễ hạn.\nTiền phạt: {phat:N0} đ ({PhieuMuonBLL.PhatMoiNgay:N0} đ/ngày).\n\nXác nhận trả sách?"
                    : $"Xác nhận trả sách cho phiếu #{id}?";
                if (!Theme.Confirm(msg)) return;
                var tienPhat=PhieuMuonBLL.Tra(id);
                Theme.Info(tienPhat > 0
                    ? $"Đã trả sách, lập phiếu phạt {tienPhat:N0} đ. Vào mục Phiếu phạt / Đền sách để tạo QR hoặc ghi nhận thu tiền."
                    : "Đã trả sách thành công. Không phát sinh tiền phạt.");
                LoadCombos();
                LoadGrid();
            }
            catch (Exception ex) { Theme.Error(ex.Message); }
        };

        bGh.Click += (s, e) =>
        {
            try
            {
                int id = SelectedId();
                PhieuMuonBLL.GiaHan(id);
                Theme.Info($"Đã gia hạn thêm {PhieuMuonBLL.SoNgayGiaHan} ngày cho phiếu #{id}.");
                LoadGrid();
            }
            catch (Exception ex) { Theme.Error(ex.Message); }
        };

        bEx.Click += (s, e) => Util.ExportGrid(grid, "PhieuMuonTra");

        grid.CellFormatting += (s, e) =>
        {
            if (e.RowIndex < 0 || grid.Columns[e.ColumnIndex].Name != "TrangThai") return;
            var v = Convert.ToString(e.Value);
            e.CellStyle.ForeColor = v == "Quá hạn" ? Theme.Danger : v == "Đã trả" ? Theme.Success : Theme.Sky;
            e.CellStyle.Font = statusFont;
        };

        txtSearch.TextChanged += (s, e) => { if (!loading) LoadGrid(); };
        cboStatus.SelectedIndexChanged += (s, e) => { if (!loading) LoadGrid(); };

        LoadCombos();
        loading = false;
        LoadGrid();
    }

    int SelectedId()
    {
        if (grid.SelectedRows.Count == 0) throw new NghiepVuException("Vui lòng chọn một phiếu trong danh sách.");
        return Convert.ToInt32(grid.SelectedRows[0].Cells["MaPM"].Value);
    }

    void LoadCombos()
    {
        cboDG.DisplayMember = "Ten"; cboDG.ValueMember = "MaDG"; cboDG.DataSource = DocGiaBLL.GetForCombo();
        cboSach.DisplayMember = "Ten"; cboSach.ValueMember = "MaSach"; cboSach.DataSource = SachBLL.GetConSach();
    }

    void LoadGrid()
    {
        grid.DataSource = PhieuMuonBLL.GetAll(txtSearch.Text, cboStatus.Text);
        Theme.Cols(grid, "SoLanGiaHan=Gia hạn", "MaPM=Mã phiếu", "DocGia=Độc giả", "TenSach=Tên sách", "NgayMuon=Ngày mượn", "HanTra=Hạn trả",
                   "NgayTra=Ngày trả", "TienPhat=Tiền phạt", "TrangThai=Trạng thái");
        Theme.Hide(grid, "MaDG");
        Theme.Fmt(grid, "dd/MM/yyyy", "NgayMuon", "HanTra", "NgayTra");
        Theme.Fmt(grid, "N0", "TienPhat");
        Theme.Weight(grid, 45, "MaPM");
        Theme.Weight(grid, 160, "TenSach");
        grid.ClearSelection();
    }
}
