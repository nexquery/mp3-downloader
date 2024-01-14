using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mp3_Downloader.Arayuz
{
    public partial class URLEkle : Form
    {
        public URLEkle()
        {
            InitializeComponent();
        }

        private void URLEkle_Load(object sender, EventArgs e)
        {
            //FormMover.Init(this, panel1);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dosya = new OpenFileDialog())
            {
                dosya.Filter = "Metin Dosyaları|*.txt|Tüm Dosyalar|*.*";
                dosya.Title = "Metin Dosyası Seç";

                if (dosya.ShowDialog() == DialogResult.OK)
                {
                    int total = 0;
                    try
                    {
                        var satirlar = File.ReadAllLines(dosya.FileName);

                        if(satirlar.Length > 0)
                        {
                            // Paralel işlemler için Task.WhenAll kullanma
                            var tasks = satirlar.Select(async satir =>
                            {
                                if (CURL.CheckURL(satir) != URLType.Yok)
                                {
                                    var x = CList.Pool.Find(j => j.cached_URL == satir);
                                    if (x == null)
                                    {
                                        await CList.Ekle(satir);
                                        total++;
                                    }
                                }
                            });

                            await Task.WhenAll(tasks);
                        }
                    }
                    catch(Exception hata)
                    {
                        MessageBox.Show("Dosya okunurken hata oluştu:\n\n" + hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if(total > 0)
                        {
                            MessageBox.Show($"Bağlantılar eklendi, toplam eklenen bağlantı sayısı: {total}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Bağlantılar eklenemedi.\nDosya içeriği boş veya bağlantılar hatalı/aynı olabilir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void URLEkle_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Bağlantı adresini girmediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(CURL.CheckURL(textBox1.Text) == URLType.Yok)
            {
                MessageBox.Show("Bağlantı adresi geçerli değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var x = CList.Pool.Find(j => j.cached_URL == textBox1.Text);
            if (x != null)
            {
                textBox1.Text = string.Empty;
                MessageBox.Show("Bu bağlantıyı zaten listeye eklemişsiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await CList.Ekle(textBox1.Text);

            textBox1.Text = string.Empty;
        }
    }
}
