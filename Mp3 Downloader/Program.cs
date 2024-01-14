using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mp3_Downloader
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Dizinleri oluştur veya kontrol et
            CDizin.OnYukleme();

            // SQL veritabanlarını hazırla ve verileri yükle
            SQL.Tablolari_Hazirla();
            SQL.Ayarlari_Yukle();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Arayuz.AnaMenu());
        }
    }
}
