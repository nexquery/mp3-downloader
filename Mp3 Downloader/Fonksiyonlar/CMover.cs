/************************************************************************
 *                                                                      *
 *                          FormMover.cs                                *
 *                                                                      *
 *      Kodlama:                                                        *
 *          Burak Akat (Nexor)                                          *
 *                                                                      *
 *      Tarih:                                                          *
 *          19.07.2023                                                  *
 *                                                                      *
 *      Amaç:                                                           *
 *          Bu sınıf bir öğenin hareket ettirmesini sağlar.             *
 *                                                                      *
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class FormMover
{
    private static int x, y;
    private static bool hareket;
    public static void Init(Form f, dynamic arac)
    {
        arac.MouseUp += new MouseEventHandler(Callback_MouseUp);
        arac.MouseDown += new MouseEventHandler(Callback_MouseDown);
        arac.MouseMove += new MouseEventHandler((s, e) => Callback_MouseMove(f));
    }

    private static void Callback_MouseUp(object sender, MouseEventArgs e)
    {
        hareket = false;
    }

    private static void Callback_MouseDown(object sender, MouseEventArgs e)
    {
        x = e.X;
        y = e.Y;
        hareket = true;
    }

    private static void Callback_MouseMove(Form f)
    {
        if (hareket)
        {
            f.SetDesktopLocation(Control.MousePosition.X - x, Control.MousePosition.Y - y);
        }
    }
}