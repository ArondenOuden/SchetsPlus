using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class SchetsWin : Form
{   
    MenuStrip menuStrip;
    SchetsControl schetscontrol;
    ISchetsTool huidigeTool;
    Panel paneel;
    bool vast;

    private void veranderAfmeting(object o, EventArgs ea)
    {
        schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                      , this.ClientSize.Height - 50);
        paneel.Location = new Point(64, this.ClientSize.Height - 30);
    }

    private void klikToolMenu(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
    }

    private void klikToolButton(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
    }

    private void afsluiten(object obj, EventArgs ea)
    {
        this.Close();
    }

    public SchetsWin()
    {
        DoubleBuffered = true;
        ISchetsTool[] deTools = { new PenTool()         
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new OvaalTool()
                                , new VolOvaalTool()
                                , new TekstTool()
                                , new GumTool()
                                , new ImageTool()
                                };
        String[] deKleuren = {"Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan", "Custom"};

        this.ClientSize = new Size(700, 500);
        huidigeTool = deTools[0];

        schetscontrol = new SchetsControl();
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
        {   
            vast=true;  
            huidigeTool.MuisVast(schetscontrol, mea.Location, mea.Button); 
        };
        schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
        {   
            if (vast)
            huidigeTool.MuisDrag(schetscontrol, mea.Location); 
        };
        schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
        {   
            if (vast)
            huidigeTool.MuisLos (schetscontrol, mea.Location);
            vast = false; 
        };
        schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
        {   
            huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
        };
        this.Controls.Add(schetscontrol);

        menuStrip = new MenuStrip();
        menuStrip.Visible = false;
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakToolMenu(deTools);
        this.maakActieMenu(deKleuren);
        this.maakToolButtons(deTools);
        this.maakActieButtons(deKleuren);
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
        this.FormClosing += (object o, FormClosingEventArgs e) =>
        {
            if (schetscontrol.Changes)
            {
                DialogResult dr = MessageBox.Show("Er zijn veranderingen die nog niet zijn opgeslagen, weet u zeker dat u door wilt gaan?", "Niet opgeslagen", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No)
                    e.Cancel = true;
            }
        };
    }

    private void maakFileMenu()
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("File");
        menu.MergeAction = MergeAction.MatchOnly;
        menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
        menuStrip.Items.Add(menu);
    }

    private void maakToolMenu(ICollection<ISchetsTool> tools)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
        foreach (ISchetsTool tool in tools)
        {   ToolStripItem item = new ToolStripMenuItem();
            item.Tag = tool;
            item.Text = tool.ToString();
            item.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            item.Click += this.klikToolMenu;
            menu.DropDownItems.Add(item);
        }
        menuStrip.Items.Add(menu);
    }

    private void maakActieMenu(String[] kleuren)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
        menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
        menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
        ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
        foreach (string k in kleuren)
            submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
        menu.DropDownItems.Add(submenu);
        menuStrip.Items.Add(menu);
    }

    private void maakToolButtons(ICollection<ISchetsTool> tools)
    {
        Panel p = new Panel();
        p.AutoScroll = true;
        p.Location = new Point(10, 10);
        p.Size = new Size(65, this.ClientSize.Height - p.Location.Y);
        this.Controls.Add(p);

        int t = 0;
        foreach (ISchetsTool tool in tools)
        {
            RadioButton b = new RadioButton();
            b.Appearance = Appearance.Button;
            b.Size = new Size(50, 62);
            
            b.Location = new Point(10, 10 + t * 62);
            b.Tag = tool;
            b.Text = tool.ToString();
            b.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            b.TextAlign = ContentAlignment.TopCenter;
            b.ImageAlign = ContentAlignment.BottomCenter;
            b.Click += this.klikToolButton;
            p.Controls.Add(b);
            if (t == 0) 
                b.Select();
            t++;
        }
    }

    private void maakActieButtons(String[] kleuren)
    {   
        paneel = new Panel(); this.Controls.Add(paneel);
        paneel.Size = new Size(600, 24);
            
        Button clear = new Button(); paneel.Controls.Add(clear);
        clear.Text = "Clear";  
        clear.Location = new Point(  0, 0); 
        clear.Click += schetscontrol.Schoon;        
            
        Button rotate = new Button(); paneel.Controls.Add(rotate);
        rotate.Text = "Rotate"; 
        rotate.Location = new Point( 80, 0); 
        rotate.Click += schetscontrol.Roteer; 
           
        Label penkleur = new Label(); paneel.Controls.Add(penkleur);
        penkleur.Text = "Penkleur:"; 
        penkleur.Location = new Point(160, 3); 
        penkleur.AutoSize = true;
            
        ComboBox cbb = new ComboBox(); paneel.Controls.Add(cbb);
        cbb.Location = new Point(220, 0); 
        cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
        cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
        foreach (string k in kleuren)
            cbb.Items.Add(k);
        cbb.SelectedIndex = 0;
                
        Button exporteren = new Button(); paneel.Controls.Add(exporteren);
        exporteren.Text = "Exporteren"; 
        exporteren.Location = new Point(530, 0); 
        exporteren.Click += schetscontrol.Exporteren;

        Button opslaan = new Button(); paneel.Controls.Add(opslaan);
        opslaan.Text = "Opslaan";
        opslaan.Location = new Point(410, 0);
        opslaan.Size = new Size(60, 25);
        opslaan.Click += schetscontrol.Opslaan;

        Button inlezen = new Button(); paneel.Controls.Add(inlezen);
        inlezen.Text = "Inlezen";
        inlezen.Location = new Point(350, 0);
        inlezen.Size = new Size(60, 25);
        inlezen.Click += schetscontrol.Inlezen;

        Button undo = new Button(); paneel.Controls.Add(undo);
        undo.Text = "Undo";
        undo.Location = new Point(470, 0);
        undo.Size = new Size(60, 25);
        undo.Click += schetscontrol.Undo;
    }
}