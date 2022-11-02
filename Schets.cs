using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Schets
{
    private Bitmap bitmap;
        
    public Schets()
    {
        
        bitmap = new Bitmap(1, 1);
    }
    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
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
        gr.DrawImage(bitmap, 0, 0);
    }
    public void Schoon()
    {
        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
    }
    public void Roteer()
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }
    public void Exporteren()
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "Png|*.png|Jpg|*.jpg|Txt|*.txt|Bmp|*.bmp";
        dialog.DefaultExt = "png";
        dialog.AddExtension = true;
        if (dialog.ShowDialog()==DialogResult.OK)
        {
            string Filename = dialog.FileName;
            FileStream Filestream = new FileStream(Filename, FileMode.CreateNew);
            bitmap.Save(Filestream, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}