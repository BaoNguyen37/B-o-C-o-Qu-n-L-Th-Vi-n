using System.Data;
using QuanLyThuVien.DAL;

namespace QuanLyThuVien.BLL;

/// <summary>Nghiệp vụ xử lý phiếu phạt trả trễ và bồi thường sách.</summary>
public static class PhieuPhatBLL
{
    public static DataTable GetAll(string keyword = "", string trangThai = "Tất cả") =>
        PhieuPhatDAL.GetAll((keyword??"").Trim(), trangThai);

    public static DataTable TongHop() => PhieuPhatDAL.TongHop();

    public static DataTable ChiTietMuon(int maPM)
    {
        if(maPM<=0) throw new NghiepVuException("Vui lòng chọn một phiếu mượn.");
        var dt=PhieuPhatDAL.ChiTietMuon(maPM);
        if(dt.Rows.Count==0) throw new NghiepVuException("Không tìm thấy phiếu mượn.");
        return dt;
    }

    public static void LapPhatTraTreCu(int maPM)
    {
        if (maPM<=0) throw new NghiepVuException("Vui lòng chọn phiếu mượn.");
        PhieuPhatDAL.LapPhatTraTreCu(maPM, AuthBLL.CurrentUser.TenDangNhap);
    }

    public static void DenSach(int maPM, string loai, string lyDo, decimal soTien)
    {
        if (maPM<=0) throw new NghiepVuException("Vui lòng chọn phiếu mượn.");
        if (loai != "Mất sách" && loai != "Hỏng sách")
            throw new NghiepVuException("Loại bồi thường không hợp lệ.");
        if (soTien<=0 || soTien>1000000000)
            throw new NghiepVuException("Số tiền bồi thường phải lớn hơn 0 và không quá 1 tỷ đồng.");
        lyDo=(lyDo??"").Trim();
        if (lyDo.Length<4 || lyDo.Length>300)
            throw new NghiepVuException("Nhập lý do bồi thường từ 4 đến 300 ký tự.");
        var row=ChiTietMuon(maPM).Rows[0];
        if (row["NgayTra"] != DBNull.Value)
            throw new NghiepVuException("Phiếu đã trả/xử lý, không lập bồi thường trùng.");
        var phatTre=PhieuMuonBLL.TinhPhat(Convert.ToDateTime(row["HanTra"]),DateTime.Today);
        PhieuPhatDAL.DenSach(maPM,loai,lyDo,soTien,phatTre,AuthBLL.CurrentUser.TenDangNhap);
    }

    public static void ThuTien(int maPhat, string phuongThuc, string maGiaoDich)
    {
        if (maPhat<=0) throw new NghiepVuException("Vui lòng chọn một phiếu phạt.");
        if (phuongThuc != "Chuyển khoản" && phuongThuc != "Tiền mặt")
            throw new NghiepVuException("Chọn phương thức thanh toán.");
        maGiaoDich = (maGiaoDich ?? "").Trim();
        if (maGiaoDich.Length > 80) throw new NghiepVuException("Mã giao dịch tối đa 80 ký tự.");
        if (phuongThuc == "Chuyển khoản" && maGiaoDich.Length < 4)
            throw new NghiepVuException("Nhập mã tham chiếu giao dịch ngân hàng để đối chiếu.");
        if(!PhieuPhatDAL.DaThu(maPhat, phuongThuc, maGiaoDich, AuthBLL.CurrentUser.TenDangNhap))
            throw new NghiepVuException("Phiếu đã được thu hoặc không tồn tại.");
    }
}
