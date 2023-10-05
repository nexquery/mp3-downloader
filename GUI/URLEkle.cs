using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MP3_Downloader.GUI
{
    public partial class URLEkle : Form
    {
        public URLEkle()
        {
            InitializeComponent();
        }

        private void URLEkle_Load(object sender, EventArgs e)
        {
            FormMover.Init(this, this);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("URL adresini girmediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Utils.GetURLType(textBox1.Text) == Type.Yok)
            {
                MessageBox.Show("URL adresi geçerli değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(Utils.GetYoutubePlayListID(textBox1.Text) == null)
            {
                CList.List_Ekle(textBox1.Text);
            }
            else
            {
                string url = $"https://cable.ayra.ch/ytdl/playlist.php?url=https://www.youtube.com/playlist?list={Utils.GetYoutubePlayListID(textBox1.Text)}&API=1";
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

                            using (StringReader reader = new StringReader(result))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
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

            textBox1.Text = "";
        }

        private void URLEkle_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void URLEkle_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void URLEkle_MouseMove(object sender, MouseEventArgs e)
        {
            
        }
    }
}
