using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class DrawObject
{
    public Color color = Color.Black;
    public int width = 2;

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

public class TextObject : StartPointObject
{
    public Font font;

    public string text;

    public override void Draw(Graphics g)
    {
        g.DrawString(text, font, MakeBrush(), startPoint, StringFormat.GenericTypographic);
    }
}

public abstract class TwoPointObject : StartPointObject
{
    public Point endPoint;
    public Rectangle rectangle
    {
        get
        {
            return new Rectangle(new Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.X, endPoint.X)),
                                new Size(Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y)));
        }
    }
}

public class LineObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        Console.WriteLine("Lijntje Tekenen");
        g.DrawLine(MakePen(), startPoint, endPoint);
    }
}
public class RectangleObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        Console.WriteLine("lege rechthoek Tekenen");
        g.DrawRectangle(MakePen(), rectangle);
    }
}

public class FilledRectangleObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        Console.WriteLine("volle rechthoek Tekenen");
        g.FillRectangle(MakeBrush(), rectangle);
    }
}

public class EllipseObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        Console.WriteLine("lege ellipse Tekenen");
        g.DrawEllipse(MakePen(), rectangle);
    }
}

public class FilledEllipseObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        Console.WriteLine("volle ellipse Tekenen");
        g.FillEllipse(MakeBrush(), rectangle);
    }
}

