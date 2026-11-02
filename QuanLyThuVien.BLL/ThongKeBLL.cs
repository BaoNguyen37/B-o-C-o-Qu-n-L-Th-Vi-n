using System.Data;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL;

public static class ThongKeBLL
{
    public static DataTable Summary() => ThongKeDAL.Summary();

    /// <summary>Lượt mượn 6 tháng gần nhất (tháng không có dữ liệu = 0).</summary>
    public static List<ChartPoint> MuonTheoThang()
    {
        var dt = ThongKeDAL.MuonTheoThang();
        var list = new List<ChartPoint>();
        for (int i = 5; i >= 0; i--)
        {
            var d = DateTime.Today.AddMonths(-i);
            var rows = dt.Select($"Y = {d.Year} AND M = {d.Month}");
            list.Add(new ChartPoint($"T{d.Month}/{d.Year % 100:00}", rows.Length > 0 ? Convert.ToDouble(rows[0]["C"]) : 0));
        }
        return list;
    }

    public static List<ChartPoint> SachTheoTheLoai() => ToPoints(ThongKeDAL.SachTheoTheLoai());
    public static List<ChartPoint> TopSach() => ToPoints(ThongKeDAL.TopSach());
    public static DataTable TopDocGia() => ThongKeDAL.TopDocGia();

    static List<ChartPoint> ToPoints(DataTable dt) =>
        dt.Rows.Cast<DataRow>().Select(r => new ChartPoint(Convert.ToString(r[0]), Convert.ToDouble(r[1]))).ToList();
}
