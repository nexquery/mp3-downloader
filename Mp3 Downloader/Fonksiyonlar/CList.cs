/*
 * 			Mp3 Downloader
 * 
 * 	File:
 * 		CList.cs
 * 
 * 	Created Date:
 * 		06.01.2024, 18:27
 * 
 * 	Author:
 * 		Burak (Nexor)
 *
 */

using Mp3_Downloader.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CList
{
    public class Cache
    {
        public Panel borderPanel;
        public Panel normalPanel;
        public PictureBox KapakResmi;
        public Label Baslik;
        public Label URL;
        public Label Durum;
        public PictureBox Kaldir;
        public string cached_URL;
        public bool Indirildi;
    }

    public static List<Cache> Pool = new List<Cache>();

    //
    //
    //
    //
    //

    public static async Task Ekle(string baglanti)
    {
        CHook.Hooked_SuspendAndResumeLayout(false);

        Tuple<string, string> data = await CURL.GetStreamInfo(baglanti, (CURL.CheckURL(baglanti) == URLType.Spotify));

        // Border Panel
        Panel border = new Panel
        {
            BackColor = ColorTranslator.FromHtml("#804000"),
            Size = new Size(649, 100),
            Location = new Point(3, 3),
            Margin = new Padding(3, 3, 3, 3),
            Dock = DockStyle.None,
            AutoSize = false,
        };

        // Normal Panel
        Panel normalPanel = new Panel
        {
            BackColor = ColorTranslator.FromHtml("#191919"),
            Size = new Size(643, 94),
            Location = new Point(3, 3),
            Margin = new Padding(3, 3, 3, 3)
        };
        border.Controls.Add(normalPanel);

        // Kapak Resmi
        PictureBox kapak = new PictureBox
        {
            BackColor = ColorTranslator.FromHtml("#191919"),
            SizeMode = PictureBoxSizeMode.Zoom,
            Size = new Size(145, 83),
            Location = new Point(5, 6),
            Image = await CIMG.Get(data.Item2, 0, (CURL.CheckURL(baglanti) == URLType.Spotify))
        };
        normalPanel.Controls.Add(kapak);

        // Başlık
        Label baslik = new Label
        {
            BackColor = ColorTranslator.FromHtml("#191919"),
            ForeColor = Color.Silver,
            AutoSize = true,
            Location = new Point(166, 11),
            Margin = new Padding(3, 0, 3, 0),
            Size = new Size(87, 15),
            Text = data.Item1
        };
        normalPanel.Controls.Add(baslik);

        // URL
        Label urlAdres = new Label
        {
            BackColor = ColorTranslator.FromHtml("#191919"),
            ForeColor = Color.DarkGray,
            AutoSize = true,
            Location = new Point(166, 36),
            Margin = new Padding(3, 0, 3, 0),
            Size = new Size(87, 15),
            Text = (baglanti.Length < 70) ? (baglanti) : (baglanti.Substring(0, 70).Insert(70, "..."))
        };
        normalPanel.Controls.Add(urlAdres);

        // Durum
        Label durum = new Label
        {
            BackColor = ColorTranslator.FromHtml("#191919"),
            ForeColor = Color.DimGray,
            AutoSize = true,
            Location = new Point(166, 61),
            Margin = new Padding(3, 0, 3, 0),
            Size = new Size(74, 15),
            Text = "Bekleniyor..."
        };
        normalPanel.Controls.Add(durum);

        // Kaldır
        PictureBox kaldir = new PictureBox
        {
            BackColor = ColorTranslator.FromHtml("#191919"),
            SizeMode = PictureBoxSizeMode.Zoom,
            Size = new Size(33, 40),
            Location = new Point(604, 49),
            Cursor = Cursors.Hand,
            Image = Resources.kaldir,
        };
        kaldir.Click += Callback_ListKaldir;
        normalPanel.Controls.Add(kaldir);

        // Border Panel
        Pool.Add(new Cache()
        {
            borderPanel = border,
            normalPanel = normalPanel,
            KapakResmi = kapak,
            Baslik = baslik,
            URL = urlAdres,
            Durum = durum,
            Kaldir = kaldir,
            cached_URL = baglanti,
            Indirildi = false
        });

        // Hook
        CHook.Hooked(border);
        CHook.Hooked_SuspendAndResumeLayout(true);
    }

    private static void Callback_ListKaldir(object sender, EventArgs e)
    {
        PictureBox pId = (PictureBox)sender;
        foreach (var c in Pool)
        {
            if (c.Kaldir == pId)
            {
                c.Kaldir.Dispose();
                c.Durum.Dispose();
                c.URL.Dispose();
                c.Baslik.Dispose();
                c.KapakResmi.Dispose();
                c.normalPanel.Dispose();

                c.borderPanel.Parent.Controls.Remove(c.borderPanel);

                c.borderPanel.Dispose();

                Pool.Remove(c);
                break;
            }
        }
    }

    public static bool IndirmeKontrol()
    {
        int total = 0;
        for (int i = 0; i < CList.Pool.Count; i++)
        {
            if (CList.Pool[i].Indirildi == true)
            {
                total++;
            }
        }
        return total == CList.Pool.Count;
    }

    public static int GetIndex()
    {
        for (int i = 0; i < CList.Pool.Count; i++)
        {
            if (CList.Pool[i].Indirildi == false)
            {
                return i;
            }
        }
        return -1;
    }

    public static async Task<int> GetIndexAsync()
    {
        return await Task.Run(() =>
        {
            for (int i = 0; i < CList.Pool.Count; i++)
            {
                if (CList.Pool[i].Indirildi == false)
                {
                    return i;
                }
            }
            return -1;
        });
    }
}