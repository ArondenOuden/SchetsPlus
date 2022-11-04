using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class DrawObjects
{
    public Color color = Color.Black;
    public int width = 2;

    public DrawObjects()
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
public abstract class StartPointObject : DrawObjects
{
    public Point startPoint;

    public override void Move(int dx, int dy)
    {
        startPoint.X += dx;
        startPoint.Y += dy;
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
        g.DrawLine(MakePen(), startPoint, endPoint);
    }
}
public class RectangleObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        g.DrawRectangle(MakePen(), rectangle);
    }
}

public class FilledRectangleObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        g.FillRectangle(MakeBrush(), rectangle);
    }
}

public class EllipseObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        g.DrawEllipse(MakePen(), rectangle);
    }
}

public class FilledEllipseObject : TwoPointObject
{
    public override void Draw(Graphics g)
    {
        g.FillEllipse(MakeBrush(), rectangle);
    }
}

