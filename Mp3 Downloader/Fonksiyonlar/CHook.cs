/*
 * 			Mp3 Downloader
 * 
 * 	File:
 * 		CList.cs
 * 
 * 	Created Date:
 * 		06.01.2024, 20:18
 * 
 * 	Author:
 * 		Burak (Nexor)
 *
 */

using System;
using System.Linq;
using System.Windows.Forms;

public class CHook
{
    public static int SayacIndex = 0;
    public static bool IndirmeBasladi = false;

    public static void Hooked(dynamic nesne)
    {
        var hook = Application.OpenForms.OfType<Mp3_Downloader.Arayuz.AnaMenu>().FirstOrDefault();
        if (hook != null)
        {
            FlowLayoutPanel panel = (FlowLayoutPanel)hook.Controls.Find("flowLayoutPanel1", true).First();
            if (panel != null)
            {
                panel.Controls.Add(nesne);
            }
        }
    }

    public static void Hooked_SuspendAndResumeLayout(bool suspend)
    {
        var hook = Application.OpenForms.OfType<Mp3_Downloader.Arayuz.AnaMenu>().FirstOrDefault();
        if (hook != null)
        {
            FlowLayoutPanel panel = (FlowLayoutPanel)hook.Controls.Find("flowLayoutPanel1", true).First();
            if (panel != null)
            {
                if(suspend == false)
                {
                    panel.SuspendLayout();
                }
                else
                {
                    panel.ResumeLayout();
                }
            }
        }
    }

    public static void UpdateSayac(bool update = false)
    {
        var hook = Application.OpenForms.OfType<Mp3_Downloader.Arayuz.AnaMenu>().FirstOrDefault();
        if (hook != null)
        {
            // indiriliyor
            Label txt = (Label)hook.Controls.Find("label6", true).First();
            if (txt?.Visible == false)
            {
                txt.Visible = true;
            }

            // Parça adı
            txt = (Label)hook.Controls.Find("label8", true).First();
            if (txt?.Visible == false)
            {
                txt.Visible = true;
            }
            else
            {
                txt.Text = CList.Pool[SayacIndex].Baslik.Text;
            }

            // Sayaç
            txt = (Label)hook.Controls.Find("label7", true).First();
            if (txt?.Visible == false)
            {
                txt.Visible = true;
            }
            else
            {
                txt.Text = $"{SayacIndex + 1} / {CList.Pool.Count}";
            }

            // Update
            if(update == true)
            {
                // Parça adı
                txt = (Label)hook.Controls.Find("label8", true).First();
                txt.Text = CList.Pool[SayacIndex].Baslik.Text;

                // Sayaç
                txt = (Label)hook.Controls.Find("label7", true).First();
                txt.Text = $"{SayacIndex + 1} / {CList.Pool.Count}";
            }
        }
    }

    public static void Label(Label label, string icerik, params object[] format)
    {
        if (label.InvokeRequired)
        {
            label.Invoke(new Action(() =>
            {
                label.Text = string.Format(icerik, format);
            }));
        }
        else
        {
            label.Text = string.Format(icerik, format);
        }
    }

    public static Label Hook_Label(string isim)
    {
        var form = Application.OpenForms.OfType<Mp3_Downloader.Arayuz.AnaMenu>().FirstOrDefault();
        if (form != null)
        {
            Label label = (Label)form.Controls.Find(isim, true).First();
            return label;
        }
        return null;
    }
}