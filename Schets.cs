using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Schets
{
    public List<DrawObject> objects = new List<DrawObject>();
    

    private Bitmap bitmap;   
    public Schets()
    {
        bitmap = new Bitmap(1, 1);
    }
    public Graphics BitmapGraphics
    {
        get 
        { 
            return Graphics.FromImage(bitmap); 
        }
    }
    public void VeranderAfmeting(Size sz)
    {
        if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
        {
            Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                     , Math.Max(sz.Height, bitmap.Size.Height)
                                     );
            Graphics gr = Graphics.FromImage(nieuw);
            gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
            gr.DrawImage(bitmap, 0, 0);
            bitmap = nieuw;
        }
    }
    public void Teken(Graphics gr)
    {
        BitmapGraphics.Clear(Color.White);
        foreach(DrawObject d in objects)
        {
            d.Draw(BitmapGraphics);
        }
        gr.DrawImage(bitmap, 0, 0);
    }
    public void Schoon()
    {
        objects.Clear();
    }
    public void Roteer()
    {
        foreach (DrawObject d in objects)
            d.Rotate(bitmap.Size);
    }

    //Exporteert een plaatje van het bestand
    public void Exporteren()
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "Png|*.png|Jpg|*.jpg|Bmp|*.bmp|Txt|*.txt";
        dialog.DefaultExt = "png";
        dialog.AddExtension = true;
        if (dialog.ShowDialog()==DialogResult.OK)
        {
            string Filename = dialog.FileName;
            FileStream Filestream = new FileStream(Filename, FileMode.CreateNew);
            bitmap.Save(Filestream, System.Drawing.Imaging.ImageFormat.Jpeg);
            FileInfo fi = new FileInfo(Filename);
            string naam = fi.Name;
            MessageBox.Show ( $"De bitmap is ge?xporteerd als {naam}"
                        , "Ge?xporteerd"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Information
                        );
        }
    }

    //Slaat het bestand op met de beste exstentie ooit
    public void Opslaan()
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "FokkerLover|.FokkerLover";
        dialog.DefaultExt = "FokkerLover";
        dialog.AddExtension = true;
        if (dialog.ShowDialog()==DialogResult.OK)
        {
            string s = dialog.FileName;
            TextWriter sw = new StreamWriter(s);
            foreach (DrawObject drawobject in objects)
            {
                string str = drawobject.ToString();
                sw.WriteLine(str);
            }
            sw.Close();
        }
        
    }

    //Leest het bestand met de beste exstentie ooit
    public void Inlezen()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            string s = dialog.FileName;
            TextReader tw = new StreamReader(s);
            string str;
            while ((str = tw.ReadLine()) != null)
            {
                string[] v;
                v = str.Split(" ");

                switch (v[0])
                {
                    case "TextObject":
                        objects.Add(new TextObject(v));
                        break;
                    case "LineObject":
                        objects.Add(new LineObject(v));
                        break;
                    case "RectangleObject":
                        objects.Add(new RectangleObject(v));
                        break;
                    case "FilledRectangleObject":
                        objects.Add(new FilledRectangleObject(v));
                        break;
                    case "EllipseObject":
                        objects.Add(new EllipseObject(v));
                        break;
                    case "FilledEllipseObject":
                        objects.Add(new FilledEllipseObject(v));
                        break;
                }
            }
        }
    }
}
