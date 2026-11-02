using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
namespace QuanLyThuVien.BLL;
public static class DatabaseSetupBLL
{
    public static string Connection(string server,string database) => new SqlConnectionStringBuilder {
        DataSource=server.Trim(),InitialCatalog=database.Trim(),IntegratedSecurity=true,
        TrustServerCertificate=true,Encrypt=false,ConnectTimeout=8
    }.ConnectionString;
    public static void Check(string connection)
    {
        using var cn=new SqlConnection(connection);cn.Open();
        using var cmd=new SqlCommand(@"SELECT COUNT(*) FROM dbo.PhieuMuon;
        SELECT SoLanGiaHan FROM dbo.PhieuMuon WHERE 1=0;
        SELECT MaDat FROM dbo.DatTruoc WHERE 1=0;
        IF OBJECT_ID('dbo.NovaMuon','P') IS NULL OR OBJECT_ID('dbo.NovaGiaHan','P') IS NULL
          OR OBJECT_ID('dbo.NovaDat','P') IS NULL OR OBJECT_ID('dbo.NovaHuyDat','P') IS NULL
          OR OBJECT_ID('dbo.NovaLock','P') IS NULL OR OBJECT_ID('dbo.NhatKyThaoTac','U') IS NULL
          OR OBJECT_ID('dbo.PhieuPhat','U') IS NULL OR OBJECT_ID('dbo.NhatKyEmail','U') IS NULL
          THROW 52990,N'Database chưa đủ cấu trúc NOVA. Chạy script cài mới hoặc nâng cấp.',1;",cn);
        cmd.ExecuteNonQuery();
    }
    public static void Install(string server,string script)
    {
        using var cn=new SqlConnection(Connection(server,"master"));cn.Open();
        foreach(var batch in Regex.Split(script,@"^\s*GO\s*\r?$",RegexOptions.Multiline|RegexOptions.IgnoreCase))
        {
            if(string.IsNullOrWhiteSpace(batch))continue;
            using var cmd=new SqlCommand(batch,cn){CommandTimeout=120};cmd.ExecuteNonQuery();
        }
    }
}
