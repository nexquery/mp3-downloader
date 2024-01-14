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
    public partial class AnaMenu : Form
    {
        public AnaMenu()
        {
            InitializeComponent();
        }

        private void AnaMenu_Load(object sender, EventArgs e)
        {
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;

            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            panel5.Dispose();

            FormMover.Init(this, panel1);
        }

        private void AnaMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CBrowser.Indiriliyor == true)
            {
                e.Cancel = true;
                MessageBox.Show($"Uygulamayı kapatmak için indirme işlemini durdurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CBrowser.IslemBaslatildi == true)
            {
                e.Cancel = true;
                MessageBox.Show($"Lütfen devam eden indirme tamamlandığında uygulamayı kapatın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SQL.Baglantilari_Sonlandir();

            if (CBrowser.xir != null)
            {
                CBrowser.xir.Quit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CBrowser.Indiriliyor == true)
            {
                MessageBox.Show($"Uygulamayı kapatmak için indirme işlemini durdurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Arayuz.URLEkle m_URLPanel = new URLEkle();
            m_URLPanel.ShowDialog();

            //await CList.Ekle("https://www.youtube.com/watch?v=y1KJfy0sVj0");
            //CHook.UpdateSayac(true);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (CBrowser.Indiriliyor == true || CBrowser.IslemBaslatildi == true)
            {
                MessageBox.Show("İndirme işlemi yapılırken listeyi temizleyemezsiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CList.Pool.Count == 0)
            {
                MessageBox.Show("Liste zaten boş.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for(int i = CList.Pool.Count - 1; i >= 0; i--)
            {
                var c = CList.Pool[i];

                c.Kaldir.Dispose();
                c.Durum.Dispose();
                c.URL.Dispose();
                c.Baslik.Dispose();
                c.KapakResmi.Dispose();
                c.normalPanel.Dispose();

                c.borderPanel.Parent.Controls.Remove(c.borderPanel);

                c.borderPanel.Dispose();

                CList.Pool.Remove(c);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(CList.Pool.Count == 0)
            {
                MessageBox.Show($"Listede ekli bağlantı bulunmuyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Metin Dosyaları|*.txt|Tüm Dosyalar|*.*";
                dialog.Title = "Metin Dosyasını Kaydet";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        int total = 0;

                        using (StreamWriter writer = new StreamWriter(dialog.FileName))
                        {
                            foreach(var x in CList.Pool)
                            {
                                total++;
                                writer.WriteLine(x.cached_URL);
                            }
                        }

                        MessageBox.Show($"URL Adresleri kayıt edildi. Toplam Adres Sayısı: {total}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"URL Adresleri kayıt edilirken hata oluştu:\n\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (CBrowser.Indiriliyor == true || CBrowser.IslemBaslatildi == true)
            {
                MessageBox.Show("İndirme işlemi yapılırken ayarları değiştiremezsiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Ayarlar AyarlarPanel = new Ayarlar();
            AyarlarPanel.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!CBrowser.Check_GoogleChrome())
            {
                MessageBox.Show("Mp3 İndiriciyi kullanabilmek için bilgisayarınızda\nGoogle Chrome yüklü olması gerekmektedir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(CAyarlar.Indirme_Konumu == null)
            {
                MessageBox.Show("Ayarlardan indirme konumunu ayarlayın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CList.Pool.Count == 0)
            {
                MessageBox.Show("İndirme işlemini başlatmak için URL Ekleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CBrowser.IslemBaslatildi == true)
            {
                MessageBox.Show("İndirme işlemini zaten başlattınız.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CBrowser.Indiriliyor == true)
            {
                MessageBox.Show("İndirilmeye devam eden bir işlem var, tamamlanmasını bekleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CList.IndirmeKontrol())
            {
                MessageBox.Show("Tüm dosyalar zaten indirilmiş.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int index = CList.GetIndex();

            CBrowser.Baslat();

            // İndiriliyor
            label6.Text = "İndiriliyor:";
            label6.Visible = true;

            // Sayaç
            label7.Text = $"{index + 1} / {CList.Pool.Count}";
            label7.Visible = true;

            // Dosya adı
            label8.Text = CList.Pool[index].Baslik.Text + " (%0)";
            label8.Visible = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (CBrowser.IslemBaslatildi == false)
            {
                MessageBox.Show("Önce indirme işlemini başlatın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CBrowser.IslemBaslatildi = false;
            MessageBox.Show("İndirme işlemi durduruldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
