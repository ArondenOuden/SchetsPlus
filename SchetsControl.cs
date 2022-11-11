using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class SchetsControl : UserControl
{   
    private Schets schets;
    private Color penkleur;
    public ColorDialog colorDialog = new ColorDialog();
    public bool Changes;

    public Color PenKleur
    { 
        get 
        { 
            return penkleur; 
        }
    }

    public Schets Schets
    { 
        get 
        { 
            return schets;
        }
    }
    public SchetsControl()
    {   
        this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets();
        this.Paint += this.teken;
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }
    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }
    private void teken(object o, PaintEventArgs pea)
    {   
        schets.Teken(pea.Graphics);
    }
    private void veranderAfmeting(object o, EventArgs ea)
    {   
        schets.VeranderAfmeting(this.ClientSize);
        this.Refresh();
    }
    public Graphics MaakBitmapGraphics()
    {  
        Graphics g = schets.BitmapGraphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        return g;
    }
    public void Schoon(object o, EventArgs ea)
    {   
        schets.Schoon();
        this.Refresh();
        Changes = false;
    }
    public void Roteer(object o, EventArgs ea)
    {   
        schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
        schets.Roteer();
        this.Refresh();
    }
    public void VeranderKleur(object obj, EventArgs ea)
    {   
        string kleurNaam = ((ComboBox)obj).Text;
        if(kleurNaam != "Custom")
        {
            penkleur = Color.FromName(kleurNaam);
        }
        else
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                penkleur = colorDialog.Color;
            }
        }
    }
    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {   
        string kleurNaam = ((ToolStripMenuItem)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }

    public void Exporteren(object o, EventArgs ea)
    {   
        schets.Exporteren();
    }
    public void Opslaan(object o, EventArgs ea)
    {
        schets.Opslaan();
        Changes = false;
    }
    public void Inlezen(object o, EventArgs ea)
    {
        schets.Inlezen();
        this.Refresh();
    }
}