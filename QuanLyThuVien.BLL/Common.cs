using QuanLyThuVien.DAL;

namespace QuanLyThuVien.BLL;

/// <summary>Lỗi nghiệp vụ - message hiển thị trực tiếp cho người dùng.</summary>
public class NghiepVuException : Exception
{
    public NghiepVuException(string message) : base(message) { }
}

public static class CauHinhBLL
{
    public static void SetConnectionString(string cs) => DbHelper.ConnectionString = cs;
    public static bool TestKetNoi(out string error) => DbHelper.TestConnection(out error);
}
