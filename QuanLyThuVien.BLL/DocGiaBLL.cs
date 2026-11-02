using System.Data;
using System.Text.RegularExpressions;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL;

public static class DocGiaBLL
{
    public static DataTable GetAll(string keyword = "") => DocGiaDAL.GetAll((keyword ?? "").Trim());
    public static DataTable GetForCombo() => DocGiaDAL.GetForCombo();

    static void Check(DocGiaDTO d)
    {
        if (string.IsNullOrWhiteSpace(d.HoTen)) throw new NghiepVuException("Vui lòng nhập họ tên độc giả.");
        if (d.NgaySinh.Date >= DateTime.Today) throw new NghiepVuException("Ngày sinh không hợp lệ.");
        if (!Regex.IsMatch(d.SDT ?? "", @"^0\d{9,10}$"))
            throw new NghiepVuException("Số điện thoại phải gồm 10-11 chữ số và bắt đầu bằng 0.");
        if (!string.IsNullOrWhiteSpace(d.Email) && !Regex.IsMatch(d.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new NghiepVuException("Email không hợp lệ.");
        d.HoTen = d.HoTen.Trim();
        d.Email = (d.Email ?? "").Trim();
        d.DiaChi = (d.DiaChi ?? "").Trim();
    }

    public static void Them(DocGiaDTO d) { Check(d); DocGiaDAL.Insert(d); }

    public static void Sua(DocGiaDTO d)
    {
        if (d.MaDG <= 0) throw new NghiepVuException("Vui lòng chọn độc giả cần sửa.");
        Check(d);
        DocGiaDAL.Update(d);
    }

    public static void Xoa(int id)
    {
        if (id <= 0) throw new NghiepVuException("Vui lòng chọn độc giả cần xóa.");
        if (DocGiaDAL.DaCoPhieu(id)) throw new NghiepVuException("Độc giả đã có lịch sử mượn sách, không thể xóa.");
        DocGiaDAL.Delete(id);
    }
}
