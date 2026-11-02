using System.Data;
using QuanLyThuVien.DAL;

namespace QuanLyThuVien.BLL;

public static class PhieuMuonBLL
{
    public const int MaxSachMuon = 5;      // tối đa số sách mượn cùng lúc
    public const int SoNgayMuon = 14;      // thời hạn mượn mặc định
    public const int SoNgayGiaHan = 7;     // mỗi lần gia hạn
    public const decimal PhatMoiNgay = 2000; // đ / ngày trễ

    public static DataTable GetAll(string keyword = "", string status = "Tất cả")
    {
        var view = PhieuMuonDAL.GetAll((keyword ?? "").Trim()).DefaultView;
        if (!string.IsNullOrEmpty(status) && status != "Tất cả")
            view.RowFilter = $"TrangThai = '{status.Replace("'", "''")}'";
        return view.ToTable();
    }

    public static decimal TinhPhat(DateTime hanTra, DateTime ngayTra) =>
        ngayTra.Date > hanTra.Date ? (ngayTra.Date - hanTra.Date).Days * PhatMoiNgay : 0;

    public static void Muon(int maDG, int maSach, DateTime ngayMuon, DateTime hanTra)
    {
        if (maDG <= 0 || maSach <= 0) throw new NghiepVuException("Vui lòng chọn độc giả và sách.");
        if (hanTra.Date < ngayMuon.Date) throw new NghiepVuException("Hạn trả phải sau ngày mượn.");
        if (PhieuMuonDAL.DemQuaHan(maDG) > 0)
            throw new NghiepVuException("Độc giả đang có sách quá hạn chưa trả, không thể mượn thêm.");
        if (PhieuMuonDAL.DemDangMuon(maDG) >= MaxSachMuon)
            throw new NghiepVuException($"Mỗi độc giả chỉ được mượn tối đa {MaxSachMuon} cuốn cùng lúc.");
        PhieuMuonDAL.Muon(maDG, maSach, ngayMuon, hanTra);
    }

    public static decimal DuKienPhat(int maPM)
    {
        var han = PhieuMuonDAL.GetHanTraChuaTra(maPM) ?? throw new NghiepVuException("Phiếu này đã được trả rồi.");
        return TinhPhat(han, DateTime.Today);
    }

    public static decimal Tra(int maPM)
    {
        var phat = DuKienPhat(maPM);
        PhieuMuonDAL.Tra(maPM, DateTime.Today, phat, AuthBLL.CurrentUser.TenDangNhap);
        return phat;
    }

    public static void GiaHan(int maPM)
    {
        var han = PhieuMuonDAL.GetHanTraChuaTra(maPM) ?? throw new NghiepVuException("Phiếu này đã được trả rồi.");
        if (han.Date < DateTime.Today) throw new NghiepVuException("Phiếu đã quá hạn, không thể gia hạn. Vui lòng thu sách và tính phạt.");
        PhieuMuonDAL.GiaHan(maPM, SoNgayGiaHan);
    }
}
