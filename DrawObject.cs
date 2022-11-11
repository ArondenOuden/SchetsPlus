using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

//De klasse waarin alle objecten getekent worden
public abstract class DrawObject
{
    public Color color = Color.Black;
    public int width = 10;
    public const int delta = 2;

    public DrawObject()
    {
    }
    
    //Maakt de gevraagde brush
    public Brush MakeBrush()
    {
        return new SolidBrush(color);
    }

    //Maakt de gevraagde pen
    public Pen MakePen()
    {
        Pen pen = new Pen(MakeBrush(), width);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }

    //Kijkt of er op het bepaald object geklikt is
    public virtual bool Clicked(SchetsControl s, Point p)
    {
        return false;
    }

    public abstract void Draw(Graphics g);

    //Draait elk punt om het midden heen
    public static Point PointRotation(Point p, Size size)
    {
        Point point = new Point(size.Width / 2, size.Height / 2);
        p = new Point(p.X - point.X, p.Y - point.Y);

        double cos = Math.Cos(-0.5 * Math.PI);
        double sin = Math.Sin(-0.5 * Math.PI);
        p = new Point((int)(p.X * cos - p.Y * sin), (int)(p.X * sin + p.Y * cos));

        return new Point(p.X + point.X, p.Y + point.Y);
    }
    public abstract void Rotate(Size size);
}

//Het pen object
public class PenObject : DrawObject
{
    //Maakt een lijst van allemaal kleine lijnen waaruit de pen bestaat
    public List<LineObject> lines = new List<LineObject>();

    public override void Draw(Graphics g)
    {
        foreach(LineObject line in lines)
        { 
            line.Draw(g);
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

    //Staat in elke subklasse, converteert het gegeven object naar een string format
    public override string ToString()
    {
        string s = "";
        foreach(LineObject line in lines)
        {
            s += "LineObject " + line.startPoint.X + " " + line.startPoint.Y + " " + line.endPoint.X + " " + line.endPoint.Y + " " + color.R + " " + color.G + " " + color.B + "\n";
        }
        s.TrimEnd('\n', '\r');
        return s;
    }

    public override void Rotate(Size size)
    {
        foreach(LineObject line in lines)
            line.Rotate(size);
    }
}

public abstract class StartPointObject : DrawObject
{
    public Point startPoint;

    public override void Rotate(Size size)
    {
        startPoint = DrawObject.PointRotation(startPoint, size);
    }
}
public class ImageObject : StartPointObject
{
    //maakt een array van bytes die het plaatje voorstelt
    public byte[] data;

    public override void Draw(Graphics g)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            Image image = Image.FromStream(ms);
            g.DrawImage(image, startPoint);
        }
    }

    public override string ToString()
    {
        return null;
    }
}
public class TextObject : StartPointObject
{
    public Font font = new Font("Comic Sans MS", 40.0f);

    public string text;

    public TextObject(string[] v)
    {
        startPoint = new Point(int.Parse(v[1]), int.Parse(v[2]));
        color = Color.FromArgb(int.Parse(v[v.Length - 3]), int.Parse(v[v.Length - 2]), int.Parse(v[v.Length - 1]));
        for (int i = 3; i < v.Length -3; i++)
        {
            text += v[i] + " ";
        }
            
    }
    public TextObject() { }

    public override void Draw(Graphics g)
    {
        g.DrawString(text, font, MakeBrush(), startPoint, StringFormat.GenericTypographic);
    }

    public override string ToString()
    {
        return "TextObject" + " " + startPoint.X + " " + startPoint.Y + " " + text + " " + color.R + " " + color.G + " " + color.B;
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

    public override void Rotate(Size size)
    {
        base.Rotate(size);
        endPoint = DrawObject.PointRotation(endPoint, size);
    }
}

public class LineObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        g.DrawLine(MakePen(), startPoint, endPoint);
    }

    public LineObject() { }

    public LineObject(string[] v)
    {
        startPoint = new Point(int.Parse(v[1]), int.Parse(v[2]));
        endPoint = new Point(int.Parse(v[3]), int.Parse(v[4]));
        color = Color.FromArgb(int.Parse(v[v.Length - 3]), int.Parse(v[v.Length - 2]), int.Parse(v[v.Length - 1]));
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
    public override string ToString()
    {
        return "LineObject" + " " + startPoint.X + " " + startPoint.Y + " " + endPoint.X + " " + endPoint.Y + " " + color.R + " " + color.G + " " + color.B;
    }


}
public class RectangleObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        g.DrawRectangle(MakePen(), Rectangle);
    }
    public override bool Clicked(SchetsControl s, Point p)
    {
        if (!base.Clicked(s, p))
            return false;
        return !ClickedInRectangle(InnerRectangle, p);
    }
    public override string ToString()
    {
        return "RectangleObject" + " " + startPoint.X + " " + startPoint.Y + " " + endPoint.X + " " + endPoint.Y + " " + color.R + " " + color.G + " " + color.B;
    }
    public RectangleObject() { }

    public RectangleObject(string[] v)
    {
        startPoint = new Point(int.Parse(v[1]), int.Parse(v[2]));
        endPoint = new Point(int.Parse(v[3]), int.Parse(v[4]));
        color = Color.FromArgb(int.Parse(v[v.Length - 3]), int.Parse(v[v.Length - 2]), int.Parse(v[v.Length - 1]));
    }
}

public class FilledRectangleObject : TwoPointObject
{
    
    public override void Draw(Graphics g)
    {
        g.FillRectangle(MakeBrush(), Rectangle);
    }
    public override string ToString()
    {
        return "FilledRectangleObject" + " " + startPoint.X + " " + startPoint.Y + " " + endPoint.X + " " + endPoint.Y + " " + color.R + " " + color.G + " " + color.B;
    }
    public FilledRectangleObject() { }

    public FilledRectangleObject(string[] v)
    {
        startPoint = new Point(int.Parse(v[1]), int.Parse(v[2]));
        endPoint = new Point(int.Parse(v[3]), int.Parse(v[4]));
        color = Color.FromArgb(int.Parse(v[v.Length - 3]), int.Parse(v[v.Length - 2]), int.Parse(v[v.Length - 1]));
    }
}

public class EllipseObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {

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
    public override string ToString()
    {
        return "EllipseObject" + " " + startPoint.X + " " + startPoint.Y + " " + endPoint.X + " " + endPoint.Y + " " + color.R + " " + color.G + " " + color.B;
    }
    public EllipseObject() { }

    public EllipseObject(string[] v)
    {
        startPoint = new Point(int.Parse(v[1]), int.Parse(v[2]));
        endPoint = new Point(int.Parse(v[3]), int.Parse(v[4]));
        color = Color.FromArgb(int.Parse(v[v.Length - 3]), int.Parse(v[v.Length - 2]), int.Parse(v[v.Length - 1]));
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
    public override string ToString()
    {
        return "FilledEllipseObject" + " " + startPoint.X + " " + startPoint.Y + " " + endPoint.X + " " + endPoint.Y + " " + color.R + " " + color.G + " " + color.B;
    }
    public FilledEllipseObject() { }

    public FilledEllipseObject(string[] v)
    {
        startPoint = new Point(int.Parse(v[1]), int.Parse(v[2]));
        endPoint = new Point(int.Parse(v[3]), int.Parse(v[4]));
        color = Color.FromArgb(int.Parse(v[v.Length - 3]), int.Parse(v[v.Length - 2]), int.Parse(v[v.Length - 1]));
    }
}

