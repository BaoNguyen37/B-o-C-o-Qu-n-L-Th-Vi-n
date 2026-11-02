using System.Data;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL;

public static class TheLoaiBLL
{
    public static DataTable GetAll() => TheLoaiDAL.GetAll();
}

public static class SachBLL
{
    public static DataTable GetAll(string keyword = "", int maTL = 0) => SachDAL.GetAll((keyword ?? "").Trim(), maTL);
    public static DataTable GetConSach() => SachDAL.GetConSach();

    static void Check(SachDTO s)
    {
        if (string.IsNullOrWhiteSpace(s.TenSach)) throw new NghiepVuException("Vui lòng nhập tên sách.");
        if (string.IsNullOrWhiteSpace(s.TacGia)) throw new NghiepVuException("Vui lòng nhập tên tác giả.");
        if (s.MaTL <= 0) throw new NghiepVuException("Vui lòng chọn thể loại.");
        if (s.NamXB < 1000 || s.NamXB > DateTime.Now.Year) throw new NghiepVuException("Năm xuất bản không hợp lệ.");
        if (s.SoLuong < 0) throw new NghiepVuException("Số lượng không được âm.");
        s.TenSach = s.TenSach.Trim();
        s.TacGia = s.TacGia.Trim();
        s.NhaXuatBan = (s.NhaXuatBan ?? "").Trim();
        s.ViTri = (s.ViTri ?? "").Trim();
    }

    public static void Them(SachDTO s) { Check(s); SachDAL.Insert(s); }

    public static void Sua(SachDTO s)
    {
        if (s.MaSach <= 0) throw new NghiepVuException("Vui lòng chọn sách cần sửa.");
        Check(s);
        SachDAL.Update(s);
    }

    public static void Xoa(int id)
    {
        if (id <= 0) throw new NghiepVuException("Vui lòng chọn sách cần xóa.");
        if (SachDAL.DaCoPhieu(id)) throw new NghiepVuException("Sách đã có lịch sử mượn, không thể xóa.");
        SachDAL.Delete(id);
    }
}
