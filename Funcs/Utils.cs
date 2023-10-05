/************************************************************************
 *                                                                      *
 *                          Utils.cs                                    *
 *                                                                      *
 *      Kodlama:                                                        *
 *          Burak Akat (Nexor)                                          *
 *                                                                      *
 *      Tarih:                                                          *
 *          19.07.2023                                                  *
 *                                                                      *
 *      Amaç:                                                           *
 *          Bu sınıf video bağlantıları ile ilgili bilgileri alacak.    *
 *                                                                      *
 ************************************************************************/

using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Type
{
    Yok,
    Youtube,
    SoundCloud,
    Spotify
};

public class JsonData_YT_SC
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

public class JsonData_Spotify
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

public class Utils
{
    /* False dönerse hatalı dosya adı var. */
    public static bool IsValidFileName(string dosyaAdi)
    {
        Regex regex = new Regex("[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]");
        return !regex.IsMatch(dosyaAdi) && !string.IsNullOrWhiteSpace(dosyaAdi);
    }
    public static string RemoveInvalidCharacters(string input)
    {
        char[] invalidCharacters = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        string result = new string(input.Where(c => !invalidCharacters.Contains(c)).ToArray());
        return result.TrimEnd();
    }

    public static Type GetURLType(string URL)
    {
        if (URL.Contains("youtube.com") || URL.Contains("youtu.be"))
        {
            return Type.Youtube;
        }
        else if (URL.Contains("soundcloud.com"))
        {
            return Type.SoundCloud;
        }
        else if (URL.Contains("spotify.com"))
        {
            return Type.Spotify;
        }
        return Type.Yok;
    }

    public static string GetYoutubeID(string URL)
    {
        var regex = @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/ ]{11})";
        var match = Regex.Match(URL, regex);
        return match.Success ? match.Groups[1].Value : null;
    }

    public static string GetYoutubePlayListID(string URL)
    {
        // Eski ve yeni URL formatları için ayrı regex kullanıyoruz
        string oldUrlPattern = @"(?:\?|\&)list=([a-zA-Z0-9_-]+)";
        string newUrlPattern = @"(?:\/|%3D|%3d)([a-zA-Z0-9_-]+)(?:\&|\%26)";

        Regex oldRegex = new Regex(oldUrlPattern);
        Regex newRegex = new Regex(newUrlPattern);

        Match match = oldRegex.Match(URL);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        match = newRegex.Match(URL);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
    }

    public static async Task<Tuple<string, string>> GetURLData(string URL)
    {
        switch (GetURLType(URL))
        {
            case Type.Youtube:
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string embedURL = $"https://noembed.com/embed?url={URL}";
                        var data = JsonConvert.DeserializeObject<JsonData_YT_SC>(await client.GetStringAsync(embedURL));
                        return Tuple.Create(data.title, $"https://i.ytimg.com/vi/{GetYoutubeID(URL)}/mqdefault.jpg");
                    }
                }

            case Type.SoundCloud:
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string embedURL = $"https://noembed.com/embed?url={URL}";
                        var data = JsonConvert.DeserializeObject<JsonData_YT_SC>(await client.GetStringAsync(embedURL));
                        return Tuple.Create(data.title.Substring(0, data.title.LastIndexOf("by") - 1), data.thumbnail_url);
                    }
                }

            case Type.Spotify:
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");

                        string embedURL = $"https://open.spotify.com/oembed?url={URL}";

                        var data = JsonConvert.DeserializeObject<JsonData_Spotify>(await client.GetStringAsync(embedURL));
                        
                        return Tuple.Create(data.title, data.thumbnail_url);
                    }
                }

            default: break;
        }
        return Tuple.Create("", "");
    }
}