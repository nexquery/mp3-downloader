using MP3_Downloader.GUI;
using MP3_Downloader.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CList
{
    public class Data
    {
        public String URL;
        public Type Tip;
        public Panel Panel;
        public PictureBox KapakResim;
        public PictureBox ParcaKaldir;
        public Label ParcaAdi;
        public Label Durum;
        public Label DurumBilgi;
        public ProgressBar IndirmeBari;
    }

    public static List<Data> ListPool = new List<Data>();

    public static async void List_Ekle(string URL)
    {
        /* [Parça Adı] :: [Kapak Resmi] */
        Tuple<string, string> data = await Utils.GetURLData(URL);

        /* Paneli oluştur */
        Panel panel = new Panel();
        panel.BackColor = ColorTranslator.FromHtml("#404040");
        panel.Location = new Point(3, 3);
        panel.Margin = new Padding(3, 3, 3, 3);
        panel.Size = new Size(473, 62);

        /* Kapak Resmini oluştur */
        PictureBox kapakResmi = new PictureBox();
        kapakResmi.BackColor = ColorTranslator.FromHtml("#404040");
        kapakResmi.SizeMode = PictureBoxSizeMode.StretchImage;
        kapakResmi.Dock = DockStyle.Left;
        kapakResmi.Margin = new Padding(3);
        kapakResmi.Size = new Size(93, 62);
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");

            HttpResponseMessage response = await httpClient.GetAsync(data.Item2);
            response.EnsureSuccessStatusCode();

            byte[] imageData = await response.Content.ReadAsByteArrayAsync();
            using (var stream = new System.IO.MemoryStream(imageData))
            {
                kapakResmi.Image = System.Drawing.Image.FromStream(stream);
            }
        }
        panel.Controls.Add(kapakResmi);

        /* Parçayı Kaldır */
        PictureBox parcaKaldir = new PictureBox();
        parcaKaldir.BackColor = ColorTranslator.FromHtml("#404040");
        parcaKaldir.SizeMode = PictureBoxSizeMode.CenterImage;
        parcaKaldir.Location = new Point(423, 15);
        parcaKaldir.Margin = new Padding(3);
        parcaKaldir.Cursor = Cursors.Hand;
        parcaKaldir.Size = new Size(38, 30);
        parcaKaldir.Image = Resources.kaldir;
        parcaKaldir.Click += Callback_ListKaldir;
        panel.Controls.Add(parcaKaldir);

        /* Parça Adı */
        Label parcaAdi = new Label();
        parcaAdi.BackColor = ColorTranslator.FromHtml("#404040");
        parcaAdi.ForeColor = ColorTranslator.FromHtml("#E0E0E0");
        parcaAdi.AutoSize = true;
        parcaAdi.Location = new Point(99, 12);
        parcaAdi.Margin = new Padding(3, 0, 3, 0);
        parcaAdi.Size = new Size(158, 13);
        parcaAdi.Text = data.Item1;
        panel.Controls.Add(parcaAdi);

        /* Durum */
        Label durum = new Label();
        durum.BackColor = ColorTranslator.FromHtml("#404040");
        durum.ForeColor = Color.LightSteelBlue;
        durum.AutoSize = true;
        durum.Location = new Point(99, 34);
        durum.Margin = new Padding(3, 0, 3, 0);
        durum.Size = new Size(44, 13);
        durum.Text = "Durum:";
        panel.Controls.Add(durum);

        /* Durum Bilgi */
        Label durumBilgi = new Label();
        durumBilgi.BackColor = ColorTranslator.FromHtml("#404040");
        durumBilgi.ForeColor = Color.Thistle;
        durumBilgi.AutoSize = true;
        durumBilgi.Location = new Point(140, 35);
        durumBilgi.Margin = new Padding(3, 0, 3, 0);
        durumBilgi.Size = new Size(161, 13);
        durumBilgi.Text = "İşlem bekleniyor.";
        panel.Controls.Add(durumBilgi);

        /* ProgressBar */
        ProgressBar bar = new ProgressBar();
        bar.Minimum = 0;
        bar.Maximum = 100;
        bar.Value = 0;
        bar.Margin = new Padding(3);
        bar.Location = new Point(94, 60);
        bar.Size = new Size(397, 2);
        panel.Controls.Add(bar);

        /* Veriyi kaydet */
        ListPool.Add(new Data()
        {
            URL = URL,
            Tip = Utils.GetURLType(URL),
            Panel = panel,
            KapakResim = kapakResmi,
            ParcaKaldir = parcaKaldir,
            ParcaAdi = parcaAdi,
            Durum = durum,
            DurumBilgi = durumBilgi,
            IndirmeBari = bar,
        });

        /* Hook */
        AnaMenu hook = Application.OpenForms.OfType<AnaMenu>().Single();
        if (hook != null)
        {
            FlowLayoutPanel flp = (FlowLayoutPanel)hook.Controls.Find("flowLayoutPanel1", true).First();
            if (flp != null)
            {
                flp.Controls.Add(panel);
            }

            System.Windows.Forms.Label pText = (System.Windows.Forms.Label)hook.Controls.Find("label8", true).First();
            if (pText != null)
            {
                pText.Text = $"İndirme Sayacı\n[0 / {CList.ListPool.Count}]";
                pText.Show();
            }
        }
    }

    private static void Callback_ListKaldir(object sender, EventArgs e)
    {
        PictureBox pId = (PictureBox)sender;
        foreach (var c in ListPool)
        {
            if(c.ParcaKaldir == pId)
            {
                c.Panel.Parent.Controls.Remove(c.Panel);
                ListPool.Remove(c);
                break;
            }
        }
    }
};