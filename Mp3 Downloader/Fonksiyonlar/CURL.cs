/*
 * 			Mp3 Downloader
 * 
 * 	File:
 * 		CURL.cs
 * 
 * 	Created Date:
 * 		06.01.2024
 * 
 * 	Author:
 * 		Burak (Nexor)
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Data_YT_SC
{
    public string version { get; set; }
    public string thumbnail_height { get; set; }
    public string url { get; set; }
    public string html { get; set; }
    public string height { get; set; }
    public string type { get; set; }
    public string provider_url { get; set; }
    public string title { get; set; }
    public string author_name { get; set; }
    public string author_url { get; set; }
    public string thumbnail_url { get; set; }
    public string provider_name { get; set; }
    public string thumbnail_width { get; set; }
    public string width { get; set; }

}

public class Data_Spotify
{
    public string html { get; set; }
    public string width { get; set; }
    public string height { get; set; }
    public string version { get; set; }
    public string provider_name { get; set; }
    public string provider_url { get; set; }
    public string type { get; set; }
    public string title { get; set; }
    public string thumbnail_url { get; set; }
    public string thumbnail_width { get; set; }
    public string thumbnail_height { get; set; }
}

public class CURL
{
    // User agent
    public static string[] User_Agents =
    {
        // Spotify
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
    };

    // Bu fonskiyon bağlantının hangi tip olduğunu belirtir.
    public static URLType CheckURL(string baglanti)
    {
        if (baglanti.Contains("youtube.com") || baglanti.Contains("youtu.be"))
        {
            return URLType.Youtube;
        }
        else if (baglanti.Contains("soundcloud.com"))
        {
            return URLType.SoundCloud;
        }
        else if (baglanti.Contains("spotify.com"))
        {
            return URLType.Spotify;
        }
        return URLType.Yok;
    }
    
    // Bu fonksiyon Youtube, Spotify ve Soundcloud bağlantılarındaki
    // akışların bilgilerini alır
    public static async Task<Tuple<string, string>> GetStreamInfo(string baglanti, bool spotifyMode = false)
    {
        URLType baglantiTipi = CheckURL(baglanti);
        if(baglantiTipi != URLType.Yok)
        {
            using (HttpClient client = new HttpClient())
            {
                // Değişkenler
                string URL = string.Empty, VideoID = GetYoutubeVideoID(baglanti);

                // User Agent
                client.DefaultRequestHeaders.UserAgent.ParseAdd(User_Agents[0]);

                // Spotify dan mı veri çekilecek ? 
                if (spotifyMode == true) {
                    client.DefaultRequestHeaders.Referrer = new Uri("https://open.spotify.com/");
                }

                // URL Adresini yapılandır
                if (baglantiTipi == URLType.Youtube || baglantiTipi == URLType.SoundCloud) {
                    URL = $"https://noembed.com/embed?url={baglanti}";
                } else {
                    URL = $"https://open.spotify.com/oembed?url={baglanti}";
                }

                // Cevap bekle
                HttpResponseMessage cevap = await client.GetAsync(URL);

                // Cevap sonucu geçerli mi ?
                if (cevap.StatusCode == HttpStatusCode.OK)
                {
                    // Youtube
                    if (baglantiTipi == URLType.Youtube)
                    {
                        var data = JsonConvert.DeserializeObject<Data_YT_SC>(await cevap.Content.ReadAsStringAsync());
                        return Tuple.Create(data.title, $"https://i.ytimg.com/vi/{VideoID}/mqdefault.jpg");
                    }

                    // Soundcloud
                    if(baglantiTipi == URLType.SoundCloud)
                    {
                        var data = JsonConvert.DeserializeObject<Data_YT_SC>(await cevap.Content.ReadAsStringAsync());
                        return Tuple.Create(data.title.Substring(0, data.title.LastIndexOf("by") - 1), data.thumbnail_url);
                    }

                    // Spotify
                    if (baglantiTipi == URLType.Spotify)
                    {
                        var data = JsonConvert.DeserializeObject<Data_Spotify>(await cevap.Content.ReadAsStringAsync());
                        return Tuple.Create(data.title, data.thumbnail_url);
                    }
                }
            }
        }
        return Tuple.Create("Hata", "N/A");
    }

    // Bu fonksiyon youtube video id kısmını almaya yara
    public static string GetYoutubeVideoID(string baglanti)
    {
        Regex regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
        Match match = regex.Match(baglanti);
        return match.Success ? match.Groups[1].Value : null;
    }
}