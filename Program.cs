using System;
using System.Windows.Forms;

static class Program
{
    [STAThreadAttribute]
    static void Main()
    {
        Application.Run(new SchetsEditor());
    }
}