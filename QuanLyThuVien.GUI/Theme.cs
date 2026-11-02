using System.Drawing.Drawing2D;
using System.Text;

namespace QuanLyThuVien.GUI;

/// <summary>Bảng màu + các hàm dựng giao diện dùng chung.</summary>
public static class Theme
{
    public static readonly Color Bg = Color.FromArgb(237, 243, 249);
    public static readonly Color Sidebar = Color.FromArgb(16, 40, 63);
    public static readonly Color SidebarHover = Color.FromArgb(30, 41, 59);
    public static readonly Color Accent = Color.FromArgb(35, 100, 232);
    public static readonly Color Success = Color.FromArgb(16, 185, 129);
    public static readonly Color Danger = Color.FromArgb(239, 68, 68);
    public static readonly Color Warning = Color.FromArgb(245, 158, 11);
    public static readonly Color Sky = Color.FromArgb(49, 198, 212);
    public static readonly Color Ink = Color.FromArgb(16, 40, 63);
    public static readonly Color Muted = Color.FromArgb(100, 116, 139);
    public static readonly Color Border = Color.FromArgb(226, 232, 240);

    public static Font F(float size = 10f, FontStyle style = FontStyle.Regular) => new("Segoe UI", size, style);

    public static Label Lbl(string text, float size = 10f, bool bold = false, Color? color = null) => new()
    {
        Text = text,
        AutoSize = true,
        Font = F(size, bold ? FontStyle.Bold : FontStyle.Regular),
        ForeColor = color ?? Ink,
        BackColor = Color.Transparent
    };

    public static Label Title(string text) => new()
    {
        Text = text, AutoSize = false, Dock = DockStyle.Top, Height = 38,
        Font = F(12f, FontStyle.Bold), ForeColor = Ink, BackColor = Color.Transparent
    };

    public static TextBox Input(string placeholder = "", int width = 300) => new()
    {
        PlaceholderText = placeholder, Width = width, Font = F(10.5f), BorderStyle = BorderStyle.FixedSingle
    };

    public static ComboBox Combo(int width = 300) => new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList, Width = width, Font = F(10.5f), FlatStyle = FlatStyle.Flat
    };

    public static RoundButton Btn(string text, Color color, int width = 110) => new()
    {
        Text = text, BackColor = color, Width = width, Height = 38
    };

    /// <summary>Đặt nhãn + ô nhập theo chiều dọc, tự tăng y.</summary>
    public static void Field(Control parent, string label, Control input, ref int y, int x = 16, int w = 0)
    {
        var l = Lbl(label, 9f, false, Muted);
        l.Location = new Point(x, y);
        input.Location = new Point(x, y + 22);
        input.Width = w > 0 ? w : parent.Width - 32;
        parent.Controls.Add(l);
        parent.Controls.Add(input);
        y += 62;
    }

    public static GraphicsPath Round(Rectangle r, int rad)
    {
        rad = Math.Max(1, rad);
        int d = rad * 2;
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    // ---------- DataGridView ----------
    public static void StyleGrid(DataGridView g)
    {
        g.Dock = DockStyle.Fill;
        g.BorderStyle = BorderStyle.None;
        g.BackgroundColor = Color.White;
        g.EnableHeadersVisualStyles = false;
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.ColumnHeadersHeight = 42;
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        g.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
        g.ColumnHeadersDefaultCellStyle.Font = F(9.5f, FontStyle.Bold);
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
        g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
        g.RowHeadersVisible = false;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.AllowUserToResizeRows = false;
        g.ReadOnly = true;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.MultiSelect = false;
        g.GridColor = Border;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.RowTemplate.Height = 38;
        g.DefaultCellStyle.Font = F(10f);
        g.DefaultCellStyle.ForeColor = Ink;
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
        g.DefaultCellStyle.SelectionForeColor = Ink;
        g.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
        g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253);
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }

    /// <summary>pairs: "TenCot=Tiêu đề"</summary>
    public static void Cols(DataGridView g, params string[] pairs)
    {
        foreach (var p in pairs)
        {
            int i = p.IndexOf('=');
            var name = p[..i];
            if (g.Columns.Contains(name)) {g.Columns[name].HeaderText = p[(i + 1)..];}
        }
    }

    public static void Hide(DataGridView g, params string[] names)
    {
        foreach (var n in names) if (g.Columns.Contains(n)) g.Columns[n].Visible = false;
    }

    public static void Fmt(DataGridView g, string format, params string[] names)
    {
        foreach (var n in names)
            if (g.Columns.Contains(n))
            {
                g.Columns[n].DefaultCellStyle.Format = format;
                if(format.StartsWith("dd/"))g.Columns[n].AutoSizeMode=DataGridViewAutoSizeColumnMode.AllCells;
                if (format == "N0") g.Columns[n].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
    }

    public static void Weight(DataGridView g, float w, params string[] names)
    {
        foreach (var n in names) if (g.Columns.Contains(n)) {g.Columns[n].FillWeight = w;if(w<=45)g.Columns[n].AutoSizeMode=DataGridViewAutoSizeColumnMode.AllCells;}
    }

    // ---------- Hộp thoại ----------
    public static void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    public static void Error(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    public static bool Confirm(string m) =>
        MessageBox.Show(m, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
}

public static class Util
{
    public static string CsvCell(string s)
    {
        s ??= "";
        if (s.TrimStart().StartsWith("=") || s.TrimStart().StartsWith("+") || s.TrimStart().StartsWith("-") || s.TrimStart().StartsWith("@")) s="'"+s;
        return "\""+s.Replace("\"","\"\"")+"\"";
    }
    static string Esc(string s) => CsvCell(s);

    /// <summary>Xuất DataGridView (các cột đang hiển thị) ra CSV, mở được bằng Excel.</summary>
    public static void ExportGrid(DataGridView g, string name)
    {
        var cols = g.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", cols.Select(c => Esc(c.HeaderText))));
        foreach (DataGridViewRow r in g.Rows)
            sb.AppendLine(string.Join(",", cols.Select(c => Esc(Convert.ToString(r.Cells[c.Index].FormattedValue)))));
        Save(name, sb);
    }

    public static void ExportTable(System.Data.DataTable dt, string name)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", dt.Columns.Cast<System.Data.DataColumn>().Select(c => Esc(c.ColumnName))));
        foreach (System.Data.DataRow r in dt.Rows)
            sb.AppendLine(string.Join(",", r.ItemArray.Select(v => Esc(v is DateTime d ? d.ToString("dd/MM/yyyy") : Convert.ToString(v)))));
        Save(name, sb);
    }

    static void Save(string name, StringBuilder sb)
    {
        using var d = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"{name}_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (d.ShowDialog() != DialogResult.OK) return;
        File.WriteAllText(d.FileName, sb.ToString(), new UTF8Encoding(true)); // BOM để Excel hiển thị tiếng Việt đúng
        Theme.Info("Đã xuất file:\n" + d.FileName);
    }
}
