/************************************************************************
*                                                                      *
*                          CBrowser.cs                                 *
*                                                                      *
*      Kodlama:                                                        *
*          Burak Akat (Nexor)                                          *
*                                                                      *
*      Tarih:                                                          *
*          10.01.2024                                                  *
*                                                                      *
*      İşlev:                                                          *
*          Bağlantıları mp3 formatına dönüştürüp belirtilen            *
*          klasöre indirme işlemi yapar.                               *
*                                                                      *
************************************************************************/

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CBrowser
{
    public static bool Indiriliyor = false;
    public static bool IslemBaslatildi = false;
    public static bool Ilk_Kullanim = false;
    public static IWebDriver xir = null;

    public static bool Check_GoogleChrome()
    {
        if(Environment.Is64BitOperatingSystem)
        {
            return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google\\Chrome\\Application\\chrome.exe"));
        }
        return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google\\Chrome\\Application\\chrome.exe"));
    }

    public static IWebElement IsValidElement(int sure, By by)
    {
        // https://stackoverflow.com/questions/41309611/try-catch-webdrivertimeoutexception-does-not-work
        WebDriverWait wait;
        IWebElement elementfound = null;

        try
        {
            wait = new WebDriverWait(xir, TimeSpan.FromSeconds(sure));
            elementfound = wait.Until<IWebElement>(d =>
            {
                try
                {
                    elementfound = xir.FindElement(by);
                }
                catch (NoSuchElementException)
                {
                }
                return elementfound;
            });
        }
        catch (WebDriverTimeoutException)
        {

        }
        return elementfound;
    }

    public static void Baslat()
    {
        if(Ilk_Kullanim == false)
        {
            Ilk_Kullanim = true;

            Task task2 = new Task(() =>
            {
                IslemBaslatildi = true;

                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                ChromeOptions options = new ChromeOptions();
                options.AddExtensions(Path.Combine(Application.StartupPath, "AdGuard.crx"));
                options.AddArgument("--headless");

                xir = new ChromeDriver(service, options);

                //Thread.Sleep(1500);

                //xir.Close();
                //xir.SwitchTo().Window(xir.WindowHandles[0]);
                Callback_Browser();
            });
            task2.Start();
        }
        else
        {
            IslemBaslatildi = true;
            Task task = new Task(() => Callback_Browser());
            task.Start();
        }
    }

    private static async void Callback_Browser()
    {
        while(IslemBaslatildi)
        {
            // Boş slot ara
            int index = await CList.GetIndexAsync();

            if (index == -1)
            {
                IslemBaslatildi = false;

                Label x4 = CHook.Hook_Label("label6");
                Label x5 = CHook.Hook_Label("label7");
                Label x6 = CHook.Hook_Label("label8");

                x4.Invoke((MethodInvoker)delegate () { x4.Visible = false; });
                x5.Invoke((MethodInvoker)delegate () { x5.Visible = false; });
                x6.Invoke((MethodInvoker)delegate () { x6.Visible = false; });

                MessageBox.Show("Tüm dosyalar indirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var cache = CList.Pool[index];

            Label x1 = CHook.Hook_Label("label7");
            Label x2 = CHook.Hook_Label("label8");

            CHook.Label(x2, $"{CList.Pool[index].Baslik.Text} (%0)");
            CHook.Label(x1, $"{index + 1} / {CList.Pool.Count}");

            if (CURL.CheckURL(cache.cached_URL) == URLType.Youtube)
            {
                CHook.Label(cache.Durum, "Hizmetler başlatılıyor...");

                // ytmp3.nu
                if(CAyarlar.Servis_Youtube == 0)
                {
                    WebDriverWait wait = new WebDriverWait(xir, TimeSpan.FromSeconds(10));
                    xir.Navigate().GoToUrl("https://ytmp3.nu/");

                    IWebElement kutu = IsValidElement(1, By.XPath("/html/body/form/div[2]/input[1]"));
                    IWebElement donustur = IsValidElement(1, By.XPath("/html/body/form/div[2]/input[2]"));
                    if (kutu != null && donustur != null)
                    {
                        CHook.Label(cache.Durum, "Bağlantı MP3 formatına dönüştürülüyor...");
                        kutu.SendKeys(cache.cached_URL);
                        donustur.Click();

                        IWebElement baglanti = IsValidElement(60, By.XPath("/html/body/form/div[2]/a[1]"));
                        if (baglanti != null)
                        {
                            await Dosya_Indir(index, baglanti.GetAttribute("href"));
                        }
                    }
                }

                // mp3indirdur.live
                if (CAyarlar.Servis_Youtube == 1)
                {
                    WebDriverWait wait = new WebDriverWait(xir, TimeSpan.FromSeconds(10));
                    xir.Navigate().GoToUrl("https://mp3indirdur.live/");

                    IWebElement kutu = IsValidElement(2, By.XPath("/html/body/div/div/main/section[1]/div/form/input"));
                    IWebElement donustur = IsValidElement(2, By.XPath("/html/body/div/div/main/section[1]/div/form/div[1]/div/button"));
                    if (kutu != null && donustur != null)
                    {
                        CHook.Label(cache.Durum, "Bağlantı MP3 formatına dönüştürülüyor...");
                        kutu.SendKeys(cache.cached_URL);
                        donustur.Click();

                        IWebElement baglanti = IsValidElement(60, By.XPath("/html/body/div/div/main/div/div/div[1]/div/a[1]"));
                        if (baglanti != null)
                        {
                            await Dosya_Indir(index, baglanti.GetAttribute("href"));
                        }
                    }
                }
            }

            if (CURL.CheckURL(cache.cached_URL) == URLType.SoundCloud)
            {
                CHook.Label(cache.Durum, "Hizmetler başlatılıyor...");
                WebDriverWait wait = new WebDriverWait(xir, TimeSpan.FromSeconds(10));
                xir.Navigate().GoToUrl("https://sclouddownloader.net/");

                IWebElement kutu = IsValidElement(1, By.XPath("/html/body/div[1]/div/div[4]/form/div[2]/input"));
                IWebElement donustur = IsValidElement(1, By.XPath("/html/body/div[1]/div/div[4]/button"));
                if (kutu != null && donustur != null)
                {
                    CHook.Label(cache.Durum, "Bağlantı MP3 formatına dönüştürülüyor...");
                    kutu.SendKeys(cache.cached_URL);
                    donustur.Click();

                    IWebElement baglanti = IsValidElement(60, By.XPath("/html/body/div[2]/div/div/div/div/div/div/div/div/div[2]/a[1]"));
                    if (baglanti != null)
                    {
                        await Dosya_Indir(index, baglanti.GetAttribute("href"));
                    }
                }
            }

            if (CURL.CheckURL(cache.cached_URL) == URLType.Spotify)
            {
                CHook.Label(cache.Durum, "Hizmetler başlatılıyor...");
                WebDriverWait wait = new WebDriverWait(xir, TimeSpan.FromSeconds(10));
                xir.Navigate().GoToUrl("https://spotifymate.com/");

                IWebElement kutu = IsValidElement(1, By.XPath("/html/body/main/div[1]/div/div/div/div/form/input[1]"));
                IWebElement donustur = IsValidElement(1, By.XPath("/html/body/main/div[1]/div/div/div/div/form/button"));
                if (kutu != null && donustur != null)
                {
                    CHook.Label(cache.Durum, "Bağlantı MP3 formatına dönüştürülüyor...");
                    kutu.SendKeys(cache.cached_URL);
                    donustur.Click();

                    IWebElement baglanti = IsValidElement(60, By.XPath("/html/body/main/div[2]/div/div/div/div/div/div[1]/div[3]/div[1]/a"));
                    if (baglanti != null)
                    {
                        await Dosya_Indir(index, baglanti.GetAttribute("href"));
                    }
                }
            }
        }
    }

    private static async Task Dosya_Indir(int index, string URL)
    {
        Indiriliyor = true;
        var cache = CList.Pool[index];
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(URL, HttpCompletionOption.ResponseHeadersRead);

                string dosya = Path.Combine(CAyarlar.Indirme_Konumu, (RemoveInvalidCharacters(cache.Baslik.Text + ".mp3")));

                if (response.IsSuccessStatusCode)
                {
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new System.IO.FileStream(dosya, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[8192];
                        long totalBytesRead = 0;
                        long totalBytes = response.Content.Headers.ContentLength ?? 0;

                        while (true)
                        {
                            int bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                            if (bytesRead == 0)
                            {
                                break;
                            }

                            await fileStream.WriteAsync(buffer, 0, bytesRead);

                            totalBytesRead += bytesRead;

                            if (totalBytes > 0)
                            {
                                int percentage = (int)((totalBytesRead * 100) / totalBytes);
                                CHook.Label(cache.Durum, $"İndiriliyor (%{percentage})");

                                Label x2 = CHook.Hook_Label("label8");
                                CHook.Label(x2, $"{CList.Pool[index].Baslik.Text} (%{percentage})");
                            }
                        }
                    }

                    CList.Pool[index].Indirildi = true;
                    CHook.Label(cache.Durum, "İndirme işlemi tamamlandı.");
                }
                else
                {
                    CList.Pool[index].Indirildi = false;
                    CHook.Label(cache.Durum, $"HTTP Hatası: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                CList.Pool[index].Indirildi = false;
                CHook.Label(cache.Durum, $"Hata: {ex.Message}");
            }
        }

        using (var cmd = SQL.SQL_BAGLANTILAR.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO indirilenler (isim, url) VALUES (@isim, @url);";
            cmd.Parameters.AddWithValue("@isim", cache.Baslik.Text);
            cmd.Parameters.AddWithValue("@url", cache.cached_URL);
            cmd.ExecuteNonQuery();
        }

        Indiriliyor = false;
    }

    private static string RemoveInvalidCharacters(string input)
    {
        char[] invalidCharacters = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        string result = new string(input.Where(c => !invalidCharacters.Contains(c)).ToArray());
        return result.TrimEnd();
    }
}