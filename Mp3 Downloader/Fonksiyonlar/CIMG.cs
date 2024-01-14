/*
 * 			Mp3 Downloader
 * 
 * 	File:
 * 		CImg.cs
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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using Mp3_Downloader.Properties;

public class CIMG
{
    public static async Task<Image> Get(string baglanti, int index = 0, bool spotifyMode = false)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(CURL.User_Agents[index]);

            if (spotifyMode)
            {
                // Referer'ı ayarla
                httpClient.DefaultRequestHeaders.Referrer = new Uri("https://open.spotify.com/");
            }

            HttpResponseMessage response = await httpClient.GetAsync(baglanti);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                byte[] imageData = await response.Content.ReadAsByteArrayAsync();
                using (var stream = new System.IO.MemoryStream(imageData))
                {
                    return Image.FromStream(stream);
                }
            }
        }
        return Resources.uyari;
    }
}