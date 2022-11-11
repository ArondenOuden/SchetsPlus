using System;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p, MouseButtons b);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);
}

public abstract class SketchTool : ISchetsTool
{
    public const int NoObject = -1;
    protected DrawObject obj = null;

    public static int ClickedObject(SchetsControl s, Point p)
    {
        for (int i = s.Schets.objects.Count -1; i >= 0; i--)
        {
            if (s.Schets.objects[i].Clicked(s, p))
            {
                return i;
            }
        }
        return NoObject;
    }

    public virtual void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj.color = s.PenKleur;
        s.Schets.objects.Add(obj);
    }

    public virtual void MuisDrag(SchetsControl s, Point p)
    {
        s.Refresh();
    }

    public virtual void MuisLos(SchetsControl s, Point p)
    {
        s.Refresh();
    }

    public virtual void Letter(SchetsControl s, char c)
    {
        s.Refresh();
    }
}


public abstract class StartpuntTool : SketchTool
{
    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        ((StartPointObject)obj).startPoint = p;
        base.MuisVast(s, p, b);
    }
    /*protected Point startpunt;
    protected Brush kwast;

    public virtual void MuisVast(SchetsControl s, Point p)
    {   
        startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {   
        kwast = new SolidBrush(s.PenKleur);
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);*/
}

public class TekstTool : StartpuntTool
{
    public override string ToString() 
    { 
        return "tekst"; 
    }

    private static string backspace(string s)
    {
        if (s.Length == 0)
            return s;
        return s.Remove(s.Length - 1);
    }
    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new TextObject
        {
            font = new Font("Comic Sans MS", 40.0f)
    };
        base.MuisVast(s, p, b);
    }

    //public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c)
    {
        TextObject texto = (TextObject)this.obj;
        if(!Char.IsControl(c))
            texto.text += c.ToString();
        else if(c == '\b') 
            texto.text = backspace(texto.text);
        base.Letter(s, c);
        /*if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Tahoma", 40);
            string tekst = c.ToString();
            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            gr.DrawString   (tekst, font, kwast, 
                                            this.startpunt, StringFormat.GenericTypographic);
            // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
            startpunt.X += (int)sz.Width;
            s.Invalidate();
        }*/
    }
}

public abstract class TweepuntTool : StartpuntTool
{
    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        ((TwoPointObject)obj).endPoint = p;
        base.MuisVast(s, p, b);
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {
        ((TwoPointObject)obj).endPoint = p;
        base.MuisDrag(s, p);
    }
    /*public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {   
        return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                            , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                            );
    }
    public static Pen MaakPen(Brush b, int dikte)
    {   
        Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }
    public override void MuisVast(SchetsControl s, Point p)
    {  
        base.MuisVast(s, p);
        kwast = new SolidBrush(s.PenKleur);
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {   
        s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {   
        base.MuisLos(s, p);
        this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
        s.Invalidate();
    }
    public override void Letter(SchetsControl s, char c)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2);
        
    public virtual void Compleet(Graphics g, Point p1, Point p2)
    {   
        this.Bezig(g, p1, p2);
    }*/
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() 
    {
        return "rkader"; 
    }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new RectangleObject();
        
        base.MuisVast(s, p, b);
    }
    /*public override void Bezig(Graphics g, Point p1, Point p2)
    {   
        g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }
    public override void MuisLos(SchetsControl s, Point p)
    {
        base.MuisLos(s, p);
        //s.Schets.DrawObjects.Add(Type.Rectangle);
    }*/
}
    
public class VolRechthoekTool : TweepuntTool
{
    public override string ToString() 
    { 
        return "rvlak"; 
    }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new FilledRectangleObject();
        base.MuisVast(s, p, b);
    }
    /*public override void Compleet(Graphics g, Point p1, Point p2)
    {   
        g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }*/
}

public class OvaalTool : TweepuntTool
{
    public override string ToString() { return "okader"; }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new EllipseObject();
        base.MuisVast(s, p, b);
    }

    /*public override void Bezig(Graphics g, Point p1, Point p2)
    {   
        g.DrawEllipse(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }*/
}
    
public class VolOvaalTool : TweepuntTool
{
    public override string ToString() { return "ovlak"; }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new FilledEllipseObject();
        base.MuisVast(s, p, b);
    }

    /*public override void Compleet(Graphics g, Point p1, Point p2)
    {   
        g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }*/
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new LineObject();
        base.MuisVast(s, p, b);
    }


    /*public override void Bezig(Graphics g, Point p1, Point p2)
    {   
        g.DrawLine(MaakPen(this.kwast,3), p1, p2);
    }*/
}

public class PenTool : SketchTool
{
    private Point startPoint;

    public override string ToString() 
    { 
        return "pen"; 
    }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        startPoint = p;
        obj = new PenObject();
        base.MuisVast(s, p, b);
    }

    public override void MuisDrag(SchetsControl s, Point p)
    {
        PenObject obj = (PenObject)this.obj;
        obj.lines.Add(new LineObject { color = obj.color, startPoint = this.startPoint, endPoint = p });
        startPoint = p;
        base.MuisDrag(s, p);
    }

    /*public override void MuisDrag(SchetsControl s, Point p)
    {   
        this.MuisLos(s, p);
        this.MuisVast(s, p);
    }*/
}
    
public class GumTool : ISchetsTool
{
    public override string ToString() 
    { 
        return "gum";
    }
    public void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        int i = SketchTool.ClickedObject(s, p);
        if(i != SketchTool.NoObject)
        {
            s.Schets.objects.RemoveAt(i);
            s.Refresh();
        }
    }
    public void MuisDrag(SchetsControl s, Point p)
    {
    }

    public void MuisLos(SchetsControl s, Point p)
    {
    }

    public void Letter(SchetsControl s, char c)
    {
    }

    /*public override void Bezig(Graphics g, Point p1, Point p2)
    {   
        g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
    }*/
}

public class ImageTool : StartpuntTool
{
    private byte[] ImageToByteArray(Image image)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }

    public override string ToString()
    {
        return "Plaatje";
    }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        if(dlg.ShowDialog() == DialogResult.OK)
        {   try
            {
                obj = new ImageObject
                {
                    data = ImageToByteArray(Image.FromFile(dlg.FileName))
                };
                base.MuisVast(s, p, b);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "FOUT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
    }

}