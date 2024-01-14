using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

public class SQL
{
    public enum SQL_DB
    {
        AYARLAR,
        BAGLANTILAR
    }

    // Bunlar geçerli SQL veritabanları
    public static SqliteConnection SQL_AYARLAR;
    public static SqliteConnection SQL_BAGLANTILAR;

    public static void Tablolari_Hazirla()
    {
        // Dizinler
        string dizin_ayarlar = Path.Combine(Application.StartupPath, "Veritabanı", "AYARLAR.db");
        string dizin_baglantilar = Path.Combine(Application.StartupPath, "Veritabanı", "BAGLANTILAR.db");

        // Ayarlar veritabanını oluştur
        SQL_AYARLAR = new SqliteConnection($"Data Source = {dizin_ayarlar}");
        SQL_AYARLAR.Open();

        // Bağlantı veritabanını oluştur
        SQL_BAGLANTILAR = new SqliteConnection($"Data Source = {dizin_baglantilar}");
        SQL_BAGLANTILAR.Open();

        // Ayarlar tablosunu oluştur
        var cmd = SQL_AYARLAR.CreateCommand();
        cmd.CommandText = $@"
        CREATE TABLE IF NOT EXISTS `ayarlar` (
            `konum_indirme` TEXT, 
            `youtube` INT, 
            `spotify` INT, 
            `soundcloud` INT
        );";
        cmd.ExecuteNonQuery();

        // Bağlantı tablosunu oluştur
        var cmd2 = SQL_BAGLANTILAR.CreateCommand();
        cmd2.CommandText = $@"
        CREATE TABLE IF NOT EXISTS `indirilenler` (
            `id` INTEGER, 
            `isim` TEXT, 
            `url` TEXT, 
            PRIMARY KEY(`id` AUTOINCREMENT)
        );";
        cmd2.ExecuteNonQuery();
    }

    public static void Ayarlari_Yukle()
    {
        string default_dizin = Path.Combine(Application.StartupPath, "İndirilenler");
        using (var komut = SQL_AYARLAR.CreateCommand())
        {
            komut.CommandText = "SELECT * FROM `ayarlar`;";
            using (var rows = komut.ExecuteReader())
            {
                if (rows.HasRows == true)
                {
                    rows.Read();
                    CAyarlar.Indirme_Konumu = rows["konum_indirme"].ToString();
                    CAyarlar.Servis_Youtube = Convert.ToInt32(rows["youtube"]);
                    CAyarlar.Servis_Spotify = Convert.ToInt32(rows["spotify"]);
                    CAyarlar.Servis_Soundcloud = Convert.ToInt32(rows["soundcloud"]);
                }
                else
                {
                    CAyarlar.Indirme_Konumu = default_dizin;
                    using (var cmd2 = SQL_AYARLAR.CreateCommand())
                    {
                        cmd2.CommandText = 
                        $@"
                            INSERT INTO ayarlar (konum_indirme, youtube, spotify, soundcloud) VALUES (@konum_indirme, @youtube, @spotify, @soundcloud);
                        ";
                        cmd2.Parameters.AddWithValue("@konum_indirme", default_dizin);
                        cmd2.Parameters.AddWithValue("@youtube", 0);
                        cmd2.Parameters.AddWithValue("@spotify", 0);
                        cmd2.Parameters.AddWithValue("@soundcloud", 0);
                        cmd2.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    public static void Baglantilari_Sonlandir()
    {
        SQL_AYARLAR.Close();
        SQL_BAGLANTILAR.Close();
    }

    public static void Sorgu(SQL_DB Sql_Tip, string sorgu)
    {
        using (var komut = (Sql_Tip == SQL_DB.AYARLAR) ? SQL_AYARLAR.CreateCommand() : SQL_BAGLANTILAR.CreateCommand())
        {
            komut.CommandText = sorgu;
            komut.ExecuteNonQuery();
        }
    }

















    /*
    public static void OnYukleme()
    {
        // Ayarlar
        string db_ayarlar = Path.Combine(Application.StartupPath, "Veritabanı", "AYARLAR.db");
        SQL_AYARLAR = new SqliteConnection($"Data Source = {db_ayarlar}");
        SQL_AYARLAR.Open();

        // Ayarlar tablosunu oluştur
        var cmd = SQL_AYARLAR.CreateCommand();
        cmd.CommandText = $@"
        CREATE TABLE IF NOT EXISTS `ayarlar` (
            `id` INTEGER, 
            `konum_indirme` TEXT, 
            `youtube` INT, 
            `spotify` INT, 
            `soundcloud` INT, 
            PRIMARY KEY(`id` AUTOINCREMENT)
        );";
        cmd.ExecuteNonQuery();

        // #######################################################################################################

        // Bağlantılar
        string db_baglantilar = Path.Combine(Application.StartupPath, "Veritabanı", "BAGLANTILAR.db");
        SQL_BAGLANTILAR = new SqliteConnection($"Data Source = {db_baglantilar}");
        SQL_BAGLANTILAR.Open();

        var cmd2 = SQL_BAGLANTILAR.CreateCommand();
        cmd2.CommandText = $"CREATE TABLE IF NOT EXISTS `indirilenler` (`id` INTEGER, `isim` TEXT, `url` TEXT, PRIMARY KEY(`id` AUTOINCREMENT));";
        cmd2.ExecuteNonQuery();
    }

    public static string GetString(SQL_DB baglanti, string parametre, string sorgu)
    {
        var command = (baglanti == SQL_DB.AYARLAR) ? SQL_AYARLAR.CreateCommand() : SQL_BAGLANTILAR.CreateCommand();
        command.CommandText = "SELECT konum_indirme FROM ayarlar;";
        using (var reader = command.ExecuteReader())
        {
            if (reader.HasRows == true)
            {
                reader.Read();
                return reader[parametre].ToString();
            }
        }
        return null;
    }
    */
}