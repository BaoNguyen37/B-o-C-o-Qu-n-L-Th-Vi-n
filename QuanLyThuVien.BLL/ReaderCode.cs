namespace QuanLyThuVien.BLL;
public static class ReaderCode
{
    public static string Create(int id) => id>0 ? $"NOVA:DG:{id:D6}" : throw new ArgumentOutOfRangeException(nameof(id));
    public static bool TryParse(string code,out int id)
    {
        id=0;code=(code??"").Trim();
        return code.StartsWith("NOVA:DG:",StringComparison.OrdinalIgnoreCase)
            && code.Length>8 && code[8..].All(char.IsAsciiDigit)
            && int.TryParse(code[8..],out id) && id>0;
    }
}
