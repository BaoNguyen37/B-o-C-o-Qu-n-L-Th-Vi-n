using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.Data.SqlClient;
namespace QuanLyThuVien.DAL;

/// <summary>Ảnh chụp dữ liệu mẫu để tham quan giao diện. Không thực thi SQL và không cho ghi.</summary>
public static class PreviewData
{
    public static bool Enabled { get; set; }
    public static void GuardWrite() { if(Enabled)throw new InvalidOperationException("Bạn đang xem thử chỉ đọc. Đăng xuất, cấu hình SQL Server và đăng nhập để dùng nghiệp vụ."); }
    static DataTable Make(string schema,params object[][] rows)
    {
        var t=new DataTable();foreach(var field in schema.Split('|')) {
            var a=field.Split(':');t.Columns.Add(a[0],a.Length<2?typeof(string):a[1] switch {"i"=>typeof(int),"d"=>typeof(DateTime),"m"=>typeof(decimal),_=>typeof(string)});
        }
        foreach(var r in rows)t.Rows.Add(r);return t;
    }
    static readonly string[] Genres={"Công nghệ thông tin","Văn học","Kinh tế","Khoa học","Lịch sử","Kỹ năng sống","Thiếu nhi"};
    static DataTable Books()
    {
        string[] titles={"Lập trình C# từ cơ bản đến nâng cao","Cấu trúc dữ liệu và giải thuật","Clean Code","Cơ sở dữ liệu SQL Server","Dế Mèn phiêu lưu ký","Số đỏ","Nhà giả kim","Đắc nhân tâm","Kinh tế học vĩ mô","Lược sử thời gian","Sapiens: Lược sử loài người","Tư duy nhanh và chậm"};
        string[] authors={"Phạm Hữu Khang","Nguyễn Đức Nghĩa","Robert C. Martin","Lê Minh Trí","Tô Hoài","Vũ Trọng Phụng","Paulo Coelho","Dale Carnegie","N. Gregory Mankiw","Stephen Hawking","Yuval Noah Harari","Daniel Kahneman"};
        int[] cats={1,1,1,1,7,2,2,6,3,4,5,6},stock={8,6,4,5,10,5,7,9,4,6,6,5};
        var t=Make("MaSach:i|TenSach|TacGia|TheLoai|NhaXuatBan|NamXB:i|SoLuong:i|GiaBia:m|ViTri|MaTL:i|LuotMuon:i|HangCho:i");
        for(int i=0;i<12;i++)t.Rows.Add(i+1,titles[i],authors[i],Genres[cats[i]-1],"NXB mẫu",2020+i%3,stock[i],100000+i*10000,"Kệ "+(char)('A'+cats[i]-1)+"1",cats[i],i%4+1,i==2?1:0);
        return t;
    }
    static DataTable Readers()
    {
        var t=Make("MaDG:i|HoTen|GioiTinh|NgaySinh:d|SDT|Email|DiaChi|NgayDangKy:d");
        string[] names={"Nguyễn Văn An","Trần Thị Bích","Lê Hoàng Cường","Phạm Thu Dung","Võ Minh Em","Đặng Quốc Phong"};
        for(int i=0;i<6;i++)t.Rows.Add(i+1,names[i],i%2==0?"Nam":"Nữ",new DateTime(2004,3,12),"090000000"+i,$"docgia{i+1}@example.invalid","TP. Hồ Chí Minh",DateTime.Today.AddDays(-90-i));return t;
    }
    static DataTable Loans()
    {
        var t=Make("MaPM:i|MaDG:i|DocGia|TenSach|NgayMuon:d|HanTra:d|NgayTra:d|TienPhat:m|SoLanGiaHan:i|TrangThai");
        var b=Books();var r=Readers();
        for(int i=0;i<8;i++) {
            int days=i==0?-6:i==5?-4:i==2?-16:4+i;
            bool returned=i==2||i==4||i==6||i==7;
            t.Rows.Add(i+1,i%6+1,r.Rows[i%6]["HoTen"],b.Rows[i]["TenSach"],DateTime.Today.AddDays(days-14),DateTime.Today.AddDays(days),returned?DateTime.Today.AddDays(Math.Min(-1,days+2)):DBNull.Value,i==2?4000:0,0,returned?"Đã trả":days<0?"Quá hạn":"Đang mượn");
        }return t;
    }
    static string Fold(string s)=>string.Concat(s.Normalize(NormalizationForm.FormD).Where(c=>CharUnicodeInfo.GetUnicodeCategory(c)!=UnicodeCategory.NonSpacingMark)).Replace('đ','d').Replace('Đ','D').ToLowerInvariant();
    public static DataTable Query(string sql,SqlParameter[] parameters)
    {
        string v(string name)=>Convert.ToString(parameters.FirstOrDefault(p=>p.ParameterName==name)?.Value)??"";
        string key=Fold(v("@k").Trim('%'));
        bool Has(DataRow r,params string[] cols)=>cols.Any(c=>Fold(Convert.ToString(r[c])).Contains(key));
        DataTable Filter(DataTable t,Func<DataRow,bool> predicate){var copy=t.Clone();foreach(DataRow r in t.Rows)if(predicate(r))copy.ImportRow(r);return copy;}
        string q=System.Text.RegularExpressions.Regex.Replace(sql,@"\s+"," ").ToLowerInvariant();
        var books=Books();var readers=Readers();var loans=Loans();
        if(q.Contains("as dausach"))return Make("DauSach:i|TrongKho:i|DocGia:i|DangMuon:i|QuaHan:i|TienPhat:m|TongPhieu:i",new object[]{12,books.AsEnumerable().Sum(r=>r.Field<int>("SoLuong")),6,4,2,4000m,8});
        if(q.Contains("as cho,"))return Make("Cho:i|CongNo:m|SapHet:i",new object[]{1,0m,0});
        if(q.Contains("as [mã phiếu]")) {
            var t=Make("Mã phiếu:i|Độc giả|Tên sách|Hạn trả:d|Tình trạng");
            foreach(DataRow r in loans.Rows)if(r.IsNull("NgayTra")&&(DateTime)r["HanTra"]<=DateTime.Today.AddDays(3))t.Rows.Add(r["MaPM"],r["DocGia"],r["TenSach"],r["HanTra"],"Quá hạn");return t;
        }
        if(q.Contains("from nhatkythaotac"))return Make("ThoiGian:d|NguoiDung|DoiTuong|HanhDong|MaDoiTuong:i",new object[]{DateTime.Today.AddHours(8),"demo","DatTruoc","Thêm",1});
        if(q.Contains("from dattruoc d join")) {
            var t=Make("MaDat:i|MaDG:i|MaSach:i|DocGia|TenSach|NgayDat:d|TrangThai|SoLuong:i|ThuTu:i");
            if(v("@st")=="Đang chờ"||v("@st")=="Tất cả")t.Rows.Add(1,4,3,readers.Rows[3]["HoTen"],books.Rows[2]["TenSach"],DateTime.Today,"Đang chờ",4,1);return t;
        }
        if(q.Contains("from sach s join theloai"))return Filter(books,r=>Has(r,"TenSach","TacGia","ViTri")&&(v("@tl")=="0"||v("@tl")==Convert.ToString(r["MaTL"]))&&(v("@available")!="True"||r.Field<int>("SoLuong")>0));
        if(q.Contains("from sach where soluong")){var t=Make("MaSach:i|Ten");foreach(DataRow r in books.Rows)t.Rows.Add(r["MaSach"],$"{r["TenSach"]} (còn {r["SoLuong"]})");return t;}
        if(q.Contains("from theloai order")){var t=Make("MaTL:i|TenTL");for(int i=0;i<Genres.Length;i++)t.Rows.Add(i+1,Genres[i]);return t;}
        if(q.Contains("as ten from docgia")){var t=Make("MaDG:i|Ten");foreach(DataRow r in readers.Rows)t.Rows.Add(r["MaDG"],$"{r["MaDG"]} - {r["HoTen"]}");return t;}
        if(q.Contains("from docgia where"))return Filter(readers,r=>Has(r,"HoTen","SDT","Email"));
        if(q.Contains("year(ngaymuon) as y")) {
            var t=Make("Y:i|M:i|C:i");for(int i=5;i>=0;i--){var d=DateTime.Today.AddMonths(-i);t.Rows.Add(d.Year,d.Month,new[]{1,0,0,2,1,4}[5-i]);}return t;
        }
        if(q.Contains("as sl from theloai")){var t=Make("TenTL|SL:i");for(int i=0;i<7;i++)t.Rows.Add(Genres[i],books.AsEnumerable().Where(r=>r.Field<int>("MaTL")==i+1).Sum(r=>r.Field<int>("SoLuong")));return t;}
        if(q.Contains("top 5 s.tensach")){var t=Make("TenSach|Luot:i");foreach(DataRow r in books.AsEnumerable().Take(5))t.Rows.Add(r["TenSach"],r["LuotMuon"]);return t;}
        if(q.Contains("top 8 dg.hoten")){var t=Make("DocGia|LuotMuon:i");foreach(DataRow r in readers.Rows)t.Rows.Add(r["HoTen"],2);return t;}
        if(q.Contains("where pm.mapm=@id")) {
            var t=loans.Clone();t.Columns.Add("GiaBia",typeof(decimal));t.Columns.Add("TinhTrangXuLy");
            foreach(DataRow r in loans.Rows)if(Convert.ToString(r["MaPM"])==v("@id")) {
                var row=t.NewRow();for(int i=0;i<loans.Columns.Count;i++)row[i]=r[i];row["GiaBia"]=100000m;row["TinhTrangXuLy"]="Bình thường";t.Rows.Add(row);
            }return t;
        }
        if(q.Contains("as trangthaiemail")) {
            var t=Make("MaPM:i|DocGia|Email|TenSach|HanTra:d|SoNgayCon:i|LoaiNhac|TrangThaiEmail");
            foreach(DataRow r in loans.Rows)if(r.IsNull("NgayTra")&&(DateTime)r["HanTra"]<=DateTime.Today.AddDays(3))t.Rows.Add(r["MaPM"],r["DocGia"],"demo@example.invalid",r["TenSach"],r["HanTra"],((DateTime)r["HanTra"]-DateTime.Today).Days,"Quá hạn","Chưa gửi");return t;
        }
        if(q.Contains("as tienchuathu"))return Make("ChuaThu:i|TienChuaThu:m|TienDaThu:m",new object[]{0,0m,0m});
        if(q.Contains("from dbo.phieuphat p"))return Make("MaPhat:i|MaPM:i|DocGia|TenSach|LoaiPhat|SoTien:m|TrangThai|NgayLap:d|NgayThu:d|LyDo|NguoiLap|PhuongThuc|MaGiaoDich");
        if(q.Contains("as trangthai from phieumuon"))return Filter(loans,r=>Has(r,"DocGia","TenSach","MaPM"));
        throw new InvalidOperationException("Mục này cần kết nối SQL Server. Đăng xuất và cấu hình database để tiếp tục.");
    }
}
