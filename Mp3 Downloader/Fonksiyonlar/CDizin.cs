/*
 * 			Mp3 Downloader
 * 
 * 	File:
 * 		Funcs_Dizin.cs
 * 
 * 	Created Date:
 * 		05.01.2024, 19:16
 * 
 * 	Author:
 * 		Burak (Nexor)
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class CDizin
{
    private static string[] klasorler =
    {
        "İndirilenler",
        "Veritabanı"
    };

    // Bu fonksiyon klasörlerin bir ön yüklemesini oluşturacak
    public static void OnYukleme()
    {
        foreach(var dir in klasorler)
        {
            if(!Directory.Exists(Path.Combine(Application.StartupPath, dir)))
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, dir));
            }
        }
    }

    // Bu fonksiyon bir belirtilen klasörün mevcut olup olmadığını kontrol eder.
    public static bool Kontrol(string konum)
    {
        return Directory.Exists(konum);
    }

    // Bu fonksiyon belirtilen konuma bir klasör oluşturur.
    public static void Olustur(string konum)
    {
        Directory.CreateDirectory(konum);
    }

    // Bu fonksiyon belirtilen konumdaki tüm dosyaları siler.
    public static bool Sil(string konum)
    {
        try
        {
            // Klasörü ve altındaki öğeleri sil
            if (Directory.Exists(konum))
            {
                Directory.Delete(konum, true);
                return true;
            }
        }
        catch
        {
            return false;
        }
        return false;
    }
}