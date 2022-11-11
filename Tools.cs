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
        s.Changes = true;
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

    public override void Letter(SchetsControl s, char c)
    {
        TextObject texto = (TextObject)this.obj;
        if(!Char.IsControl(c))
            texto.text += c.ToString();
        else if(c == '\b') 
            texto.text = backspace(texto.text);
        base.Letter(s, c);
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
}

public class OvaalTool : TweepuntTool
{
    public override string ToString() { return "okader"; }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new EllipseObject();
        base.MuisVast(s, p, b);
    }
}
    
public class VolOvaalTool : TweepuntTool
{
    public override string ToString() { return "ovlak"; }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new FilledEllipseObject();
        base.MuisVast(s, p, b);
    }
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        obj = new LineObject();
        base.MuisVast(s, p, b);
    }
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
        return "plaatje";
    }

    public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        if(dlg.ShowDialog() == DialogResult.OK)
        {   
            try
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