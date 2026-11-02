using System.Data;
using QuanLyThuVien.BLL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI;

public partial class UcDocGia : UserControl
{
    int id;
    bool loading = true;

    public UcDocGia()
    {
        InitializeComponent();
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        Theme.StyleGrid(grid);
        var profile=Theme.Btn("Hồ sơ & thẻ QR",Theme.Accent,180);profile.Location=new Point(470,8);bar.Controls.Add(profile);
        profile.Click+=(_,_)=>NovaStyle.Safe(()=>{
            if(grid.SelectedRows.Count==0)throw new NghiepVuException("Chọn độc giả để xem hồ sơ.");
            var row=grid.SelectedRows[0];using var f=new FrmReaderProfile(Convert.ToInt32(row.Cells["MaDG"].Value),Convert.ToString(row.Cells["HoTen"].Value));f.ShowDialog(this);
        });
        bDel.Visible = AuthBLL.CurrentUser.IsAdmin;
        btnExport.Click += (s, e) => Util.ExportGrid(grid, "DanhSachDocGia");
        cboGT.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" }); cboGT.SelectedIndex = 0;
        bAdd.Click += (s, e) => Run(() => { id = 0; DocGiaBLL.Them(Read()); Theme.Info("Đã thêm độc giả mới."); });
        bEdit.Click += (s, e) => Run(() => { DocGiaBLL.Sua(Read()); Theme.Info("Đã cập nhật độc giả."); });
        bDel.Click += (s, e) =>
        {
            if (id <= 0) { Theme.Error("Vui lòng chọn độc giả cần xóa."); return; }
            if (Theme.Confirm($"Xóa độc giả \"{txtTen.Text}\"?")) Run(() => DocGiaBLL.Xoa(id));
        };
        bClr.Click += (s, e) => { grid.ClearSelection(); ClearForm(); };

        grid.SelectionChanged += (s, e) => FillForm();
        txtSearch.TextChanged += (s, e) => { if (!loading) LoadData(); };
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
        grid.DataSource = DocGiaBLL.GetAll(txtSearch.Text);
        Theme.Cols(grid, "MaDG=Mã", "HoTen=Họ tên", "GioiTinh=Giới tính", "NgaySinh=Ngày sinh", "SDT=SĐT",
                   "Email=Email", "DiaChi=Địa chỉ", "NgayDangKy=Ngày ĐK");
        Theme.Fmt(grid, "dd/MM/yyyy", "NgaySinh", "NgayDangKy");
        Theme.Weight(grid, 35, "MaDG");
        Theme.Weight(grid, 70, "GioiTinh");
        Theme.Weight(grid, 150, "HoTen", "Email", "DiaChi");
        loading = false;
        grid.ClearSelection();
        ClearForm();
    }

    DocGiaDTO Read() => new()
    {
        MaDG = id, HoTen = txtTen.Text, GioiTinh = cboGT.Text, NgaySinh = dtpNs.Value.Date,
        SDT = txtSdt.Text.Trim(), Email = txtMail.Text, DiaChi = txtDiaChi.Text
    };

    void FillForm()
    {
        if (loading || grid.SelectedRows.Count == 0) return;
        var r = grid.SelectedRows[0];
        id = Convert.ToInt32(r.Cells["MaDG"].Value);
        txtTen.Text = Convert.ToString(r.Cells["HoTen"].Value);
        cboGT.Text = Convert.ToString(r.Cells["GioiTinh"].Value);
        dtpNs.Value = Convert.ToDateTime(r.Cells["NgaySinh"].Value);
        txtSdt.Text = Convert.ToString(r.Cells["SDT"].Value);
        txtMail.Text = Convert.ToString(r.Cells["Email"].Value);
        txtDiaChi.Text = Convert.ToString(r.Cells["DiaChi"].Value);
    }

    void ClearForm()
    {
        id = 0;
        txtTen.Clear(); txtSdt.Clear(); txtMail.Clear(); txtDiaChi.Clear();
        cboGT.SelectedIndex = 0;
        dtpNs.Value = new DateTime(2004, 1, 1);
    }
}
