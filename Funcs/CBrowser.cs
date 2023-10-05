/************************************************************************
 *                                                                      *
 *                          CBrowser.cs                                 *
 *                                                                      *
 *      Kodlama:                                                        *
 *          Burak Akat (Nexor)                                          *
 *                                                                      *
 *      Tarih:                                                          *
 *          19.07.2023                                                  *
 *                                                                      *
 *      İşlev:                                                          *
 *          Bağlantıları mp3 formatına dönüştürüp indirme               *
 *          adreslerini almasını sağlayan çekirdek sınıfı.              *
 *                                                                      *
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Windows.Forms;
using System.Reflection.Emit;
using System.IO;
using System.Net.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using OpenQA.Selenium.DevTools.V112.Profiler;
using SeleniumExtras.WaitHelpers;
using System.Runtime.InteropServices;
using MP3_Downloader.GUI;
using System.Security.Cryptography;

public class CBrowser
{
    private static int Index = 0;
    public static IWebDriver Chrome = null;
    public static bool TaskDurdur = false;

    public static void Baslat()
    {
        if (Chrome == null)
        {
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");

            // Arka Plan işlemlerini başlat
            Task.Run(() =>
            {
                Chrome = new ChromeDriver(service, options);

                Callback_Browser(Index);

            }, CTask.Token_Olustur());
        }
        else
        {
            foreach (var c in CList.ListPool)
            {
                if (c.IndirmeBari.Value >= 98)
                {
                    c.Panel.Parent.Controls.Remove(c.Panel);
                    CList.ListPool.Remove(c);
                }
            }

            /* Hook */
            AnaMenu hook = Application.OpenForms.OfType<AnaMenu>().Single();
            if (hook != null)
            {
                FlowLayoutPanel flp = (FlowLayoutPanel)hook.Controls.Find("flowLayoutPanel1", true).First();
                if (flp != null)
                {
                    flp.Refresh();
                }
            }

            Task.Run(() => Callback_Browser(Index = 0), CTask.Token_Olustur());
        }
    }

    public static void Kapat()
    {
        if (Chrome != null)
        {
            // Önce Task daki görevi sonlandır.
            CTask.Durdur();

            // Tarayıcı kapat
            Chrome.Quit();
            Chrome = null;
        }
    }

    private static async void Callback_Browser(int PoolIndex)
    {
        try
        {
            /* Hook */
            AnaMenu hook = Application.OpenForms.OfType<AnaMenu>().Single();
            if (hook != null)
            {
                System.Windows.Forms.Label flp = (System.Windows.Forms.Label)hook.Controls.Find("label8", true).First();
                if (flp != null)
                {
                    CInvoke.Update_Label(flp, $"İndirme Sayacı\n[{PoolIndex+1} / {CList.ListPool.Count}]");
                }
            }

            var c = CList.ListPool[PoolIndex];

            if (c.Tip == Type.Youtube)
            {
                CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi bekleniyor.");
                Chrome.Navigate().GoToUrl("https://ytmp3.nu/");

                IWebElement kutu = IsValidElement(10, By.XPath("/html/body/form/div[2]/input[1]"));
                if (kutu != null)
                {
                    kutu.SendKeys(c.URL);

                    IWebElement convert = IsValidElement(10, By.XPath("/html/body/form/div[2]/input[3]"));
                    if (convert != null)
                    {
                        CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi başlatıldı...");
                        convert.Click();

                        IWebElement mp3Link = IsValidElement(60, By.XPath("/html/body/form/div[2]/a[1]"));
                        if (mp3Link != null)
                        {
                            CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi tamamlandı, indirme işlemi başlıyor...");

                            await Task.Delay(500, CTask.Token_Al());

                            try
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    using (var response = await httpClient.GetAsync(mp3Link.GetAttribute("href"), HttpCompletionOption.ResponseHeadersRead))
                                    using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                                    {
                                        using (var streamToWriteTo = new FileStream(Path.Combine(Ayarlar.Indirme_Konumu, Utils.RemoveInvalidCharacters(c.ParcaAdi.Text) + ".mp3"), FileMode.Create))
                                        {
                                            var totalRead = 0L;
                                            var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                                            var buffer = new byte[4096];
                                            var bytesRead = 0;

                                            while ((bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                            {
                                                await streamToWriteTo.WriteAsync(buffer, 0, bytesRead);
                                                totalRead += bytesRead;
                                                var progressPercentage = totalBytes <= 0 ? 0 : (int)(totalRead * 100 / totalBytes);

                                                CInvoke.Update_Label(c.DurumBilgi, "İndiriliyor: %{0}", progressPercentage);
                                                CInvoke.Update_ProgressBar(c.IndirmeBari, progressPercentage);
                                            }
                                        }
                                    }
                                }

                                CInvoke.Update_Label(c.DurumBilgi, "İndirme işlemi tamamlandı.");
                            }
                            catch (Exception ex)
                            {
                                CInvoke.Update_Label(c.DurumBilgi, "Hata: {0}", ex.Message);
                            }
                        }
                        else
                        {
                            if (Chrome.PageSource.Contains("An backend error occurred. Error code (p:3 / e:0)."))
                            {
                                CInvoke.Update_Label(c.DurumBilgi, "Bu URL MP3 Dönüştürmeye karşı korumalı.");
                            }
                            else
                            {
                                CInvoke.Update_Label(c.DurumBilgi, "MP3 Dönüştürme zaman aşımına uğradı.");
                            }
                        }
                    }
                }
                else
                {
                    CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi iptal edildi (zaman aşımına uğradı).");
                }
            }

            if (c.Tip == Type.SoundCloud)
            {
                CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi bekleniyor.");
                Chrome.Navigate().GoToUrl("https://sctomp3.net/");

                IWebElement kutu = IsValidElement(10, By.XPath("/html/body/div/center/div/div[1]/div[1]/form/div/input"));
                if (kutu != null)
                {
                    kutu.SendKeys(c.URL);

                    IWebElement convert = IsValidElement(10, By.XPath("/html/body/div/center/div/div[1]/div[1]/form/button"));
                    if (convert != null)
                    {
                        CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi başlatıldı...");
                        convert.Click();

                        IWebElement mp3Link = IsValidElement(10, By.XPath("/html/body/div/div[2]/div/div/div[2]/div/a"));
                        if (mp3Link != null)
                        {
                            CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi tamamlandı, indirme işlemi başlıyor...");

                            await Task.Delay(250, CTask.Token_Al());

                            try
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    using (var response = await httpClient.GetAsync(mp3Link.GetAttribute("href"), HttpCompletionOption.ResponseHeadersRead))
                                    using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                                    {
                                        using (var streamToWriteTo = new FileStream(Path.Combine(Ayarlar.Indirme_Konumu, Utils.RemoveInvalidCharacters(c.ParcaAdi.Text) + ".mp3"), FileMode.Create))
                                        {
                                            var totalRead = 0L;
                                            var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                                            var buffer = new byte[4096];
                                            var bytesRead = 0;

                                            while ((bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                            {
                                                await streamToWriteTo.WriteAsync(buffer, 0, bytesRead);
                                                totalRead += bytesRead;
                                                var progressPercentage = totalBytes <= 0 ? 0 : (int)(totalRead * 100 / totalBytes);

                                                CInvoke.Update_Label(c.DurumBilgi, "İndiriliyor: %{0}", progressPercentage);
                                                CInvoke.Update_ProgressBar(c.IndirmeBari, progressPercentage);
                                            }
                                        }
                                    }
                                }

                                CInvoke.Update_Label(c.DurumBilgi, "İndirme işlemi tamamlandı.");
                            }
                            catch (Exception ex)
                            {
                                CInvoke.Update_Label(c.DurumBilgi, "Hata: {0}", ex.Message);
                            }
                        }
                        else
                        {
                            CInvoke.Update_Label(c.DurumBilgi, "MP3 Dönüştürme zaman aşımına uğradı.");
                        }
                    }
                }
                else
                {
                    CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi iptal edildi (zaman aşımına uğradı).");
                }
            }

            if (c.Tip == Type.Spotify)
            {
                CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi bekleniyor.");
                Chrome.Navigate().GoToUrl("https://spotifymate.com/");

                IWebElement kutu = IsValidElement(10, By.XPath("/html/body/main/div[1]/div/div/div/div/form/input[1]"));
                if (kutu != null)
                {
                    kutu.SendKeys(c.URL);

                    IWebElement convert = IsValidElement(10, By.XPath("/html/body/main/div[1]/div/div/div/div/form/button"));
                    if (convert != null)
                    {
                        convert.Click();

                        IWebElement mp3Link = IsValidElement(10, By.XPath("/html/body/main/div[2]/div/div/div/div/div/div[1]/div[3]/div[1]/a"));
                        if (mp3Link != null)
                        {
                            CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi tamamlandı, indirme işlemi başlıyor...");

                            await Task.Delay(250, CTask.Token_Al());

                            try
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    using (var response = await httpClient.GetAsync(mp3Link.GetAttribute("href"), HttpCompletionOption.ResponseHeadersRead))
                                    using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                                    {
                                        using (var streamToWriteTo = new FileStream(Path.Combine(Ayarlar.Indirme_Konumu, Utils.RemoveInvalidCharacters(c.ParcaAdi.Text) + ".mp3"), FileMode.Create))
                                        {
                                            var totalRead = 0L;
                                            var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                                            var buffer = new byte[4096];
                                            var bytesRead = 0;

                                            while ((bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                            {
                                                await streamToWriteTo.WriteAsync(buffer, 0, bytesRead);
                                                totalRead += bytesRead;
                                                var progressPercentage = totalBytes <= 0 ? 0 : (int)(totalRead * 100 / totalBytes);

                                                CInvoke.Update_Label(c.DurumBilgi, "İndiriliyor: %{0}", progressPercentage);
                                                CInvoke.Update_ProgressBar(c.IndirmeBari, progressPercentage);
                                            }
                                        }
                                    }
                                }

                                CInvoke.Update_Label(c.DurumBilgi, "İndirme işlemi tamamlandı.");
                            }
                            catch (Exception ex)
                            {
                                CInvoke.Update_Label(c.DurumBilgi, "Hata: {0}", ex.Message);
                            }
                        }
                        else
                        {
                            CInvoke.Update_Label(c.DurumBilgi, "MP3 Dönüştürme zaman aşımına uğradı.");
                        }
                    }
                }
                else
                {
                    CInvoke.Update_Label(c.DurumBilgi, "Dönüştürme işlemi iptal edildi (zaman aşımına uğradı).");
                }
            }
        }
        catch (OperationCanceledException)
        {
            //MessageBox.Show("İndirme işlemi durduruldu.");
        }

        if( (PoolIndex + 1) == CList.ListPool.Count)
        {
            MessageBox.Show("Tüm URL'ler MP3 formatına dönüştürüldü.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            Callback_Browser(PoolIndex + 1);
        }
    }

    private static IWebElement IsValidElement(int sure, By by)
    {
        IWebElement element = null;
        if (Chrome != null)
        {
            WebDriverWait wait = new WebDriverWait(Chrome, TimeSpan.FromSeconds(sure));
            try
            {
                element = wait.Until(ExpectedConditions.ElementExists(by));
            }
            catch (NoSuchElementException)
            {
                element = null;
            }
            catch (WebDriverTimeoutException)
            {
                element = null;
            }
            catch (TimeoutException)
            {
                element = null;
            }
            return element;
        }
        return null;
    }
}