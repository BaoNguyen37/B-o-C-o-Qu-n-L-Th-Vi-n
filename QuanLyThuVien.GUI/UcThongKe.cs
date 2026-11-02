using System.Data;
using QuanLyThuVien.BLL;

namespace QuanLyThuVien.GUI;

/// <summary>Thống kê & báo cáo (chỉ Admin).</summary>
public partial class UcThongKe : UserControl
{
    public UcThongKe()
    {
        InitializeComponent();
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        ch1.BarColor = Theme.Success; ch1.SetData(ThongKeBLL.SachTheoTheLoai());
        ch2.BarColor = Theme.Warning; ch2.SetData(ThongKeBLL.TopSach());
        Theme.StyleGrid(grid); grid.DataSource = ThongKeBLL.TopDocGia();
        Theme.Cols(grid,"DocGia=Độc giả","LuotMuon=Số lượt mượn"); grid.ClearSelection();
        var s=ThongKeBLL.Summary().Rows[0];
        l1.Text=$"Tổng số phiếu mượn: {Convert.ToInt32(s["TongPhieu"]):N0}";
        l2.Text=$"Đang mượn / Quá hạn: {Convert.ToInt32(s["DangMuon"]):N0} / {Convert.ToInt32(s["QuaHan"]):N0}";
        l3.Text=$"Tổng tiền phạt đã ghi nhận: {Convert.ToDecimal(s["TienPhat"]):N0} đ";
        b.Click += (e,a) => Util.ExportTable(PhieuMuonBLL.GetAll(),"BaoCaoMuonTra");
    }

}
