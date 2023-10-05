using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MP3_Downloader.GUI
{
    public partial class AnaMenu : Form
    {
        public AnaMenu()
        {
            InitializeComponent();
        }

        private void AnaMenu_Load(object sender, EventArgs e)
        {
            FormMover.Init(this, panel1);
            label8.Hide();
            //pictureBox1.LoadAsync("https://i3.ytimg.com/vi/oIEmHSvscQI/maxresdefault.jpg");
        }

        private void imgKucult_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void imgKapat_Click(object sender, EventArgs e)
        {
            CBrowser.Kapat();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog klasor = new FolderBrowserDialog())
            {
                klasor.ShowNewFolderButton = true;
                if (klasor.ShowDialog() == DialogResult.OK)
                {
                    Ayarlar.Indirme_Konumu = klasor.SelectedPath + "\\";
                    MessageBox.Show($"İndirme konumu \"{Ayarlar.Indirme_Konumu}\" olarak seçildi.", "İndirme Konumu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(Ayarlar.Indirme_Konumu == null) 
            {
                MessageBox.Show("İndirme konumu seçili değil!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(CList.ListPool.Count == 0)
            {
                MessageBox.Show("Ekli bir URL Adresi yok.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for(int i = 1; i <  CList.ListPool.Count; i++)
            {
                CList.ListPool[i].DurumBilgi.Text = "Kuyrukta...";
            }

            foreach (Control control in this.Controls)
            {
                if (control is System.Windows.Forms.Button button)
                {
                    button.Enabled = false;
                }
            }

            CBrowser.Baslat();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GUI.URLEkle c = new GUI.URLEkle();
            c.ShowDialog();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dosya = new OpenFileDialog())
            {
                dosya.Filter = "Metin Dosyaları|*.txt|Tüm Dosyalar|*.*";
                dosya.Title = "Metin Dosyası Seç";

                if (dosya.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(dosya.FileName))
                        {
                            int count = 0, eCount = 0;

                            string satir;
                            string hatali_url = "";
                            
                            while ((satir = reader.ReadLine()) != null)
                            {
                                if(Utils.GetURLType(satir) != Type.Yok)
                                {
                                    if(Utils.GetYoutubePlayListID(satir) == null)
                                    {
                                        count++;
                                        CList.List_Ekle(satir);
                                    }
                                    else
                                    {
                                        string url = $"https://cable.ayra.ch/ytdl/playlist.php?url=https://www.youtube.com/playlist?list={Utils.GetYoutubePlayListID(satir)}&API=1";
                                        using (HttpClient httpClient = new HttpClient())
                                        {
                                            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
                                            try
                                            {
                                                HttpResponseMessage response = await httpClient.GetAsync(url);
                                                response.EnsureSuccessStatusCode();

                                                using (HttpContent content = response.Content)
                                                {
                                                    string result = await content.ReadAsStringAsync();

                                                    using (StringReader buffer = new StringReader(result))
                                                    {
                                                        string line;
                                                        while ((line = buffer.ReadLine()) != null)
                                                        {
                                                            count++;
                                                            CList.List_Ekle(line);
                                                        }
                                                    }
                                                }
                                            }
                                            catch (HttpRequestException hata)
                                            {
                                                MessageBox.Show("Play List almada hata oluştu.\n\n" + hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    eCount++;
                                    hatali_url += satir + "\n";
                                }
                            }

                            if(count != 0 && eCount != 0)
                            {
                                hatali_url.Insert(0, "Aşağıdaki URL Adresleri hatalı olduğu için eklenmedi.\n");
                                MessageBox.Show($"URL Adresleri eklendi. Eklenen: {count}, Eklenmeyen: {eCount}\n\n{hatali_url}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            
                            if (count == 0 && eCount != 0)
                            {
                                hatali_url.Insert(0, "Aşağıdaki URL Adresleri hatalı olduğu için eklenmedi.\n");
                                MessageBox.Show(hatali_url, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            if (count != 0 && eCount == 0)
                            {
                                MessageBox.Show($"URL Adresleri eklendi. Eklenen: {count}, Eklenmeyen: {eCount}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            if (count == 0 && eCount == 0)
                            {
                                MessageBox.Show("Dosya içeriği boş.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dosya okunurken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach(var c in CList.ListPool)
            {
                c.IndirmeBari.Dispose();
                c.DurumBilgi.Dispose();
                c.Durum.Dispose();
                c.ParcaAdi.Dispose();
                c.ParcaKaldir.Dispose();
                c.KapakResim.Dispose();
                c.Panel.Dispose();
            }

            CList.ListPool.Clear();

            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Refresh();

            label8.Hide();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void AnaMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            CBrowser.Kapat();
            Application.Exit();
        }
    }
}