using System.Drawing.Drawing2D;
namespace QuanLyThuVien.GUI;

public class LibraryBanner : Panel
{
    public LibraryBanner() { DoubleBuffered=true; BackColor=Theme.Sidebar; ResizeRedraw=true; }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); var g=e.Graphics; g.SmoothingMode=SmoothingMode.AntiAlias;
        int start=Width-320;
        using var line=new Pen(Color.FromArgb(55,91,119),1);
        for(int y=14;y<Height;y+=24) g.DrawLine(line,start,y,Width-15,y);
        Color[] colors={Theme.Sky,Theme.Accent,Color.FromArgb(98,145,182),Theme.Success,Color.FromArgb(210,167,84)};
        for(int row=0;row<2;row++)
        {
            int x=start+34;
            for(int i=0;i<8;i++)
            {
                int h=38+(i*17+row*13)%30,w=16+(i%3)*6,bottom=85+row*86;
                using var b=new SolidBrush(colors[(i+row)%colors.Length]);
                g.FillRectangle(b,x,bottom-h,w,h);
                using var detail=new Pen(Color.FromArgb(130,255,255,255),2);
                g.DrawLine(detail,x+4,bottom-h+9,x+w-4,bottom-h+9);
                x+=w+7;
            }
            using var shelf=new SolidBrush(Color.FromArgb(65,104,133));
            g.FillRectangle(shelf,start+20,87+row*86,270,4);
        }
    }
}

public class NavButton : Button
{
    public int Symbol;
    public NavButton(int symbol) { Symbol=symbol; FlatStyle=FlatStyle.Flat; FlatAppearance.BorderSize=0; TextAlign=ContentAlignment.MiddleLeft; Padding=new Padding(44,0,0,0); Cursor=Cursors.Hand; Height=43; Width=210; Margin=new Padding(8,2,8,2); SetStyle(ControlStyles.OptimizedDoubleBuffer,true); }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;
        using var p=new Pen(ForeColor,1.6f); int x=16,y=Height/2-8;
        if(Symbol==0) { for(int i=0;i<4;i++)g.DrawRectangle(p,x+(i%2)*10,y+(i/2)*10,6,6); }
        else if(Symbol==1 || Symbol==2) { g.DrawRectangle(p,x,y,17,17);g.DrawLine(p,x+5,y,x+5,y+17);g.DrawLine(p,x+9,y+5,x+14,y+5); }
        else if(Symbol==3) {g.DrawEllipse(p,x+5,y,8,8);g.DrawArc(p,x,y+9,18,14,180,180);}
        else if(Symbol==4) {g.DrawLine(p,x,y+4,x+18,y+4);g.DrawLine(p,x+14,y,x+18,y+4);g.DrawLine(p,x,y+14,x+18,y+14);g.DrawLine(p,x,y+14,x+4,y+18);}
        else if(Symbol==5) {g.DrawRectangle(p,x,y+2,18,15);g.DrawLine(p,x,y+7,x+18,y+7);g.DrawLine(p,x+4,y,x+4,y+4);g.DrawLine(p,x+14,y,x+14,y+4);}
        else if(Symbol==6) {g.DrawRectangle(p,x,y+2,18,14);g.DrawLine(p,x,y+2,x+9,y+10);g.DrawLine(p,x+18,y+2,x+9,y+10);}
        else if(Symbol==7) {for(int i=0;i<3;i++)g.DrawRectangle(p,x+i*7,y+12-i*5,3,6+i*5);}
        else {g.DrawRectangle(p,x+2,y,14,18);for(int i=0;i<3;i++)g.DrawLine(p,x+6,y+5+i*4,x+12,y+5+i*4);}
    }
}

public static class NovaStyle
{
    public static void Apply(Control c)
    {
        if(c is not TextBoxBase && c is not NumericUpDown && c is not DateTimePicker)
            c.Font=Theme.F(Math.Max(9.5f,c.Font.Size), c.Font.Style);
        if(c.BackColor.ToArgb()==Color.FromArgb(241,245,249).ToArgb())c.BackColor=Theme.Bg;
        if(c.BackColor.ToArgb()==Color.FromArgb(15,23,42).ToArgb())c.BackColor=Theme.Sidebar;
        if(c.BackColor.ToArgb()==Color.FromArgb(99,102,241).ToArgb())c.BackColor=Theme.Accent;
        if(c is DataGridView grid) { Theme.StyleGrid(grid);return; }
        if(c is Button b) { b.FlatStyle=FlatStyle.Flat;b.FlatAppearance.BorderSize=0;b.Cursor=Cursors.Hand; }
        foreach(Control child in c.Controls)Apply(child);
        // Docking is evaluated in reverse Z order: fill must be first.
        foreach(Control child in c.Controls.Cast<Control>().Where(x=>x.Dock==DockStyle.Fill).ToArray())c.Controls.SetChildIndex(child,0);
    }
    public static Label Text(string value,int x,int y,float size=10,bool bold=false,Color? color=null)
    { var l=Theme.Lbl(value,size,bold,color);l.Location=new Point(x,y);return l; }
    public static FlowLayoutPanel Toolbar() => new(){Dock=DockStyle.Top,Height=58,Padding=new Padding(0,8,0,8),WrapContents=false};
    public static void Safe(Action action) {try {action();}catch(Exception ex){Theme.Error(ex.GetBaseException().Message);} }
}
