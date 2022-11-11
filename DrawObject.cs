using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class DrawObject
{
    public Color color = Color.Black;
    public int width = 10;
    public const int delta = 2;

    public DrawObject()
    {

    }
    
    public Brush MakeBrush()
    {
        return new SolidBrush(color);
    }

    public Pen MakePen()
    {
        Pen pen = new Pen(MakeBrush(), width);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }

    public virtual bool Clicked(SchetsControl s, Point p)
    {
        return false;
    }

    public abstract void Draw(Graphics g);
    public abstract void Move(int dx, int dy);
}

public class PenObject : DrawObject
{
    public List<LineObject> lines = new List<LineObject>();

    public override void Draw(Graphics g)
    {
        foreach(LineObject line in lines)
        {
            line.Draw(g);
        }
    }
    public override void Move(int dx, int dy)
    {
        foreach (LineObject line in lines)
        {
            line.Move(dx, dy);
        }
    }
    public override bool Clicked(SchetsControl s, Point p)
    {
        foreach(LineObject line in lines)
        {
            if (line.Clicked(s, p))
                return true;
        }
        return false;
    }
}

public abstract class StartPointObject : DrawObject
{
    public Point startPoint;

    public override void Move(int dx, int dy)
    {
        startPoint.X += dx;
        startPoint.Y += dy;
    }
}
public class ImageObject : StartPointObject
{
    public byte[] data;

    public override void Draw(Graphics g)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            Image image = Image.FromStream(ms);
            g.DrawImage(image, startPoint);
        }
    }
}

public class TextObject : StartPointObject
{
    public Font font = new Font("Times New Roman", 12.0f);
    

    public string text;

    public override void Draw(Graphics g)
    {
        g.DrawString(text, font, MakeBrush(), startPoint, StringFormat.GenericTypographic);
    }
}

public abstract class TwoPointObject : StartPointObject
{
    public Point endPoint;
    public Rectangle Rectangle
    {
        get
        {
            return new Rectangle(new Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y)),
                                new Size(Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y)));
        }
    }

    public Rectangle OuterRectangle
    {
        get
        {
            Rectangle r = Rectangle;
            if(width > 0)
            {
                width += DrawObject.delta * 2;
                r.X -= width / 2;
                r.Y -= width / 2;
                r.Width += width;
                r.Height += width;
                width -= DrawObject.delta * 2;
            }
            return r;
        }
    }

    public Rectangle InnerRectangle
    {
        get
        {
            Rectangle r = Rectangle;
            if (width > 0)
            {
                width += DrawObject.delta * 2;
                r.X += width / 2;
                r.Y += width / 2;
                r.Width -= width;
                r.Height -= width;
                width -= DrawObject.delta * 2;
            }
            return r;
        }
    }

    public static bool ClickedInRectangle(Rectangle r, Point p)
    {
        return (p.X >= r.Left && p.X <= r.Right && p.Y >= r.Top && p.Y <= r.Bottom);
    }

    public override bool Clicked(SchetsControl s, Point p)
    {
        return ClickedInRectangle(OuterRectangle, p);
    }
}

public class LineObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        Console.WriteLine("Lijntje Tekenen");
        g.DrawLine(MakePen(), startPoint, endPoint);
    }

    public override bool Clicked(SchetsControl s, Point p)
    {
        if (!base.Clicked(s, p))
            return false;
        return DistanceToLine(p) < DrawObject.delta;
    }

    public double DistanceToLine(Point p)
    {
        double x0 = p.X;
        double y0 = p.Y;
        double x1 = startPoint.X;
        double y1 = startPoint.Y;
        double x2 = endPoint.X;
        double y2 = endPoint.Y;
        double dx = x2 - x1;
        double dy = y2 - y1;
        //Aqcuired formula from http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Line_defined_by_two_points
        return Math.Abs(dy * x0 - dx * y0 - x1 * y2 + x2 * y1) / Math.Sqrt(dx * dx + dy * dy) - width / 2;
    }
}
public class RectangleObject : TwoPointObject
{
    Rectangle rec = Rectangle;
    public override void Draw(Graphics g)
    {
        Console.WriteLine("lege rechthoek Tekenen");
        g.DrawRectangle(MakePen(), Rectangle);
    }
    public override bool Clicked(SchetsControl s, Point p)
    {
        if (!base.Clicked(s, p))
            return false;
        return !ClickedInRectangle(InnerRectangle, p);
    }
}

public class FilledRectangleObject : TwoPointObject
{
    
    public override void Draw(Graphics g)
    {
        Console.WriteLine("volle rechthoek Tekenen");
        g.FillRectangle(MakeBrush(), Rectangle);
    }
    
}

public class EllipseObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        Console.WriteLine("lege ellipse Tekenen");
        g.DrawEllipse(MakePen(), Rectangle);
    }
    public override bool Clicked(SchetsControl s, Point p)
    {
        if (!base.Clicked(s, p))
            return false;
        Rectangle r = InnerRectangle;
        Size size = new Size(r.Width / 2, r.Height / 2);
        p = new Point(p.X - r.X - size.Width, p.Y - r.Y - size.Height);
        return !(FilledEllipseObject.EllipseValidity(p, size) <= 1);
    }
}

public class FilledEllipseObject : TwoPointObject
{
    public static double EllipseValidity(Point p, Size s)
    {
        return Math.Pow(p.X, 2) / Math.Pow(s.Width, 2) + Math.Pow(p.Y, 2) / Math.Pow(s.Height, 2);
    }
    public override void Draw(Graphics g)
    {
        Console.WriteLine("volle ellipse Tekenen");
        g.FillEllipse(MakeBrush(), Rectangle);
    }
    public override bool Clicked(SchetsControl s, Point p)
    {
        if (!base.Clicked(s, p))
            return false;
        Rectangle r = OuterRectangle;
        Size size = new Size(r.Width / 2, r.Height / 2);
        p = new Point(p.X - r.X - size.Width, p.Y - r.Y - size.Height);
        return EllipseValidity(p, size) <= 1;
    }
}

