using System.ComponentModel;
using System.Drawing.Drawing2D;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI;

/// <summary>Nút bo góc, đổi màu khi rê chuột.</summary>
public class RoundButton : Button
{
    bool _hover;
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Radius { get; set; } = 10;

    public RoundButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        ForeColor = Color.White;
        Font = Theme.F(10f, FontStyle.Bold);
        Cursor = Cursors.Hand;
        SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Color.White);
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        var fill = !Enabled ? Color.FromArgb(203, 213, 225) : _hover ? ControlPaint.Dark(BackColor, 0.08f) : BackColor;
        using var path = Theme.Round(rect, Radius);
        using var br = new SolidBrush(fill);
        g.FillPath(br, path);
        TextRenderer.DrawText(g, Text, Font, rect, ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }
}

/// <summary>Thẻ nền trắng bo góc (giống "card" trên web).</summary>
public class Card : Panel
{
    public Card()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.White;
        Padding = new Padding(16);
    }

    protected override void OnPaintBackground(PaintEventArgs e) { }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Parent?.BackColor ?? Theme.Bg);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using var p = Theme.Round(r, 12);
        using var b = new SolidBrush(BackColor);
        using var pen = new Pen(Theme.Border);
        g.FillPath(b, p);
        g.DrawPath(pen, p);
    }
}

/// <summary>Biểu đồ cột tự vẽ bằng GDI+ (không cần thư viện ngoài).</summary>
public class BarChart : Control
{
    List<ChartPoint> _data = new();
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BarColor { get; set; } = Theme.Accent;

    public BarChart()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.White;
    }

    public void SetData(IEnumerable<ChartPoint> data)
    {
        _data = data.ToList();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(BackColor);
        using var fVal = Theme.F(9f, FontStyle.Bold);
        using var fLbl = Theme.F(8.5f);

        if (_data.Count == 0)
        {
            TextRenderer.DrawText(g, "Chưa có dữ liệu", fLbl, ClientRectangle, Theme.Muted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            return;
        }

        double max = Math.Max(1, _data.Max(d => d.Value));
        var plot = new Rectangle(4, 26, Math.Max(10, Width - 8), Math.Max(10, Height - 26 - 36));

        using (var pen = new Pen(Theme.Border))
            for (int i = 0; i <= 4; i++)
            {
                int y = plot.Bottom - plot.Height * i / 4;
                g.DrawLine(pen, plot.Left, y, plot.Right, y);
            }

        float slot = (float)plot.Width / _data.Count;
        float bw = Math.Min(64f, slot * 0.55f);
        for (int i = 0; i < _data.Count; i++)
        {
            float h = (float)(_data[i].Value / max * (plot.Height - 6));
            float x = plot.Left + slot * i + (slot - bw) / 2;

            if (h >= 4)
            {
                var r = new Rectangle((int)x, (int)(plot.Bottom - h), (int)bw, (int)h);
                using var br = new LinearGradientBrush(r, ControlPaint.Light(BarColor, 0.45f), BarColor, 90f);
                using var path = Theme.Round(r, Math.Min(6, r.Height / 2));
                g.FillPath(br, path);
            }

            TextRenderer.DrawText(g, _data[i].Value.ToString("N0"), fVal,
                new Rectangle((int)x - 20, (int)(plot.Bottom - Math.Max(h, 0)) - 22, (int)bw + 40, 20), Theme.Ink,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom);
            TextRenderer.DrawText(g, _data[i].Label, fLbl,
                new Rectangle((int)(plot.Left + slot * i), plot.Bottom + 6, (int)slot, 30), Theme.Muted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.WordBreak);
        }
    }
}
