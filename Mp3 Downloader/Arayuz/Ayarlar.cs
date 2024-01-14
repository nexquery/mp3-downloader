using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mp3_Downloader.Arayuz
{
    public partial class Ayarlar : Form
    {
        public Ayarlar()
        {
            InitializeComponent();
        }

        private void Ayarlar_Load(object sender, EventArgs e)
        {
            textBox1.Text = CAyarlar.Indirme_Konumu;

            comboBox1.SelectedIndex = CAyarlar.Servis_Youtube;
            comboBox2.SelectedIndex = CAyarlar.Servis_Spotify;
            comboBox3.SelectedIndex = CAyarlar.Servis_Soundcloud;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog klasor = new FolderBrowserDialog())
            {
                klasor.ShowNewFolderButton = true;
                if (klasor.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = klasor.SelectedPath;
                    CAyarlar.Indirme_Konumu = klasor.SelectedPath;
                    SQL.Sorgu(SQL.SQL_DB.AYARLAR, $"UPDATE ayarlar SET konum_indirme = '{klasor.SelectedPath}';");
                    MessageBox.Show($"İndirme konumu \"{klasor.SelectedPath}\" olarak seçildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Tüm bağlantı bilgilerini veritabanından silmek istediğinize emin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(cevap == DialogResult.Yes)
            {
                SQL.Sorgu(SQL.SQL_DB.BAGLANTILAR, "DELETE FROM indirilenler; UPDATE SQLITE_SEQUENCE SET SEQ = 0 WHERE NAME = 'indirilenler';");
                MessageBox.Show("Bağlantı bilgileri veritabanından başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(CAyarlar.Servis_Youtube != comboBox1.SelectedIndex)
            {
                CAyarlar.Servis_Youtube = comboBox1.SelectedIndex;
                SQL.Sorgu(SQL.SQL_DB.AYARLAR, $"UPDATE ayarlar SET youtube = '{CAyarlar.Servis_Youtube}';");
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CAyarlar.Servis_Spotify != comboBox2.SelectedIndex)
            {
                CAyarlar.Servis_Spotify = comboBox2.SelectedIndex;
                SQL.Sorgu(SQL.SQL_DB.AYARLAR, $"UPDATE ayarlar SET spotify = '{CAyarlar.Servis_Spotify}';");
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CAyarlar.Servis_Soundcloud != comboBox3.SelectedIndex)
            {
                CAyarlar.Servis_Soundcloud = comboBox3.SelectedIndex;
                SQL.Sorgu(SQL.SQL_DB.AYARLAR, $"UPDATE ayarlar SET soundcloud = '{CAyarlar.Servis_Soundcloud}';");
            }
        }
    }
}
