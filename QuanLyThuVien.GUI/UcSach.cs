using System.Data;
using QuanLyThuVien.BLL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI;

public partial class UcSach : UserControl
{
    int id;
    bool loading = true;

    public UcSach()
    {
        InitializeComponent();
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        Theme.StyleGrid(grid);
        bDel.Visible = AuthBLL.CurrentUser.IsAdmin;
        btnExport.Click += (s, e) => Util.ExportGrid(grid, "DanhSachSach");
        numNam.Minimum = 1900; numNam.Maximum = 2100; numNam.Value = DateTime.Now.Year;
        numSL.Maximum = 9999; numGia.Maximum = 100000000; numGia.Increment = 1000; numGia.ThousandsSeparator = true;
        cboTL.DisplayMember = "TenTL"; cboTL.ValueMember = "MaTL"; cboTL.DataSource = TheLoaiBLL.GetAll();
        bAdd.Click += (s, e) => Run(() => { id = 0; SachBLL.Them(Read()); Theme.Info("Đã thêm sách mới."); });
        bEdit.Click += (s, e) => Run(() => { SachBLL.Sua(Read()); Theme.Info("Đã cập nhật sách."); });
        bDel.Click += (s, e) =>
        {
            if (id <= 0) { Theme.Error("Vui lòng chọn sách cần xóa."); return; }
            if (Theme.Confirm($"Xóa sách \"{txtTen.Text}\"?")) Run(() => SachBLL.Xoa(id));
        };
        bClr.Click += (s, e) => { grid.ClearSelection(); ClearForm(); };

        var dt = TheLoaiBLL.GetAll();
        var all = dt.NewRow(); all["MaTL"] = 0; all["TenTL"] = "Tất cả thể loại";
        dt.Rows.InsertAt(all, 0);
        cboFilter.DisplayMember = "TenTL"; cboFilter.ValueMember = "MaTL"; cboFilter.DataSource = dt;

        grid.SelectionChanged += (s, e) => FillForm();
        txtSearch.TextChanged += (s, e) => { if (!loading) LoadData(); };
        cboFilter.SelectedIndexChanged += (s, e) => { if (!loading) LoadData(); };
        loading = false;
        LoadData();
    }

    void Run(Action act)
    {
        try { act(); LoadData(); }
        catch (Exception ex) { Theme.Error(ex.Message); }
    }

    void LoadData()
    {
        loading = true;
        grid.DataSource = SachBLL.GetAll(txtSearch.Text, Convert.ToInt32(cboFilter.SelectedValue ?? 0));
        Theme.Cols(grid, "MaSach=Mã", "TenSach=Tên sách", "TacGia=Tác giả", "TheLoai=Thể loại", "NhaXuatBan=NXB",
                   "NamXB=Năm", "SoLuong=SL", "GiaBia=Giá bìa", "ViTri=Vị trí");
        Theme.Hide(grid, "MaTL", "LuotMuon", "HangCho");
        Theme.Fmt(grid, "N0", "GiaBia");
        Theme.Weight(grid, 35, "MaSach", "NamXB", "SoLuong");
        Theme.Weight(grid, 160, "TenSach");
        loading = false;
        grid.ClearSelection();
        ClearForm();
    }

    SachDTO Read() => new()
    {
        MaSach = id,
        TenSach = txtTen.Text,
        TacGia = txtTacGia.Text,
        NhaXuatBan = txtNxb.Text,
        NamXB = (int)numNam.Value,
        MaTL = Convert.ToInt32(cboTL.SelectedValue ?? 0),
        SoLuong = (int)numSL.Value,
        GiaBia = numGia.Value,
        ViTri = txtViTri.Text
    };

    void FillForm()
    {
        if (loading || grid.SelectedRows.Count == 0) return;
        var r = grid.SelectedRows[0];
        id = Convert.ToInt32(r.Cells["MaSach"].Value);
        txtTen.Text = Convert.ToString(r.Cells["TenSach"].Value);
        txtTacGia.Text = Convert.ToString(r.Cells["TacGia"].Value);
        txtNxb.Text = Convert.ToString(r.Cells["NhaXuatBan"].Value);
        txtViTri.Text = Convert.ToString(r.Cells["ViTri"].Value);
        cboTL.SelectedValue = Convert.ToInt32(r.Cells["MaTL"].Value);
        numNam.Value = Math.Clamp(Convert.ToDecimal(r.Cells["NamXB"].Value), numNam.Minimum, numNam.Maximum);
        numSL.Value = Math.Clamp(Convert.ToDecimal(r.Cells["SoLuong"].Value), numSL.Minimum, numSL.Maximum);
        numGia.Value = Math.Clamp(Convert.ToDecimal(r.Cells["GiaBia"].Value), numGia.Minimum, numGia.Maximum);
    }

    void ClearForm()
    {
        id = 0;
        txtTen.Clear(); txtTacGia.Clear(); txtNxb.Clear(); txtViTri.Clear();
        numNam.Value = DateTime.Now.Year; numSL.Value = 0; numGia.Value = 0;
        if (cboTL.Items.Count > 0) cboTL.SelectedIndex = 0;
        // Khong doi focus sang o Ten sach khi tim kiem lam moi danh sach.
    }
}
