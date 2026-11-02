using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyThuVien.DAL;

/// <summary>Lớp truy cập dữ liệu dùng chung (tầng DAL).</summary>
public static class DbHelper
{
    // Mặc định cài riêng database NOVA. Có thể đổi tại màn cấu hình SQL Server.
    public static string ConnectionString { get; set; } =
        "Server=localhost;Database=QuanLyThuVienNova;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;";

    public static string Actor { get; set; } = "";
    public static void Open(SqlConnection cn)
    {
        cn.Open();
        using var actor = new SqlCommand("EXEC sys.sp_set_session_context @key=N'Actor', @value=@actor",cn);
        actor.Parameters.AddWithValue("@actor", Actor);
        actor.ExecuteNonQuery();
    }

    public static SqlParameter P(string name, object value) => new(name, value ?? DBNull.Value);

    public static DataTable Table(string sql, params SqlParameter[] ps)
    {
        if(PreviewData.Enabled)return PreviewData.Query(sql,ps);
        using var cn = new SqlConnection(ConnectionString);
        using var da = new SqlDataAdapter(sql, cn);
        da.SelectCommand.Parameters.AddRange(ps);
        var dt = new DataTable();
        Open(cn);
        da.Fill(dt);
        return dt;
    }

    public static int NonQuery(string sql, params SqlParameter[] ps)
    {
        PreviewData.GuardWrite();
        using var cn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddRange(ps);
        Open(cn);
        return cmd.ExecuteNonQuery();
    }

    public static object Scalar(string sql, params SqlParameter[] ps)
    {
        PreviewData.GuardWrite();
        using var cn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddRange(ps);
        Open(cn);
        return cmd.ExecuteScalar();
    }

    public static bool TestConnection(out string error)
    {
        try
        {
            using var cn = new SqlConnection(ConnectionString);
            cn.Open();
            error = "";
            return true;
        }
        catch (Exception ex) { error = ex.Message; return false; }
    }
}
