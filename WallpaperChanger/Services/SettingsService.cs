using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using WallpaperChanger.Data;

namespace WallpaperChanger.Services
{
    public static class SettingsService
    {
        private static readonly ReaderWriterLock Rwl = new ReaderWriterLock();

        private static readonly string SettingsFileName = $@"{Environment.GetEnvironmentVariable("USERPROFILE")}\LEDEL Wallpaper\settings.ini";
        private static readonly FileIniDataParser FileIniDataParser = new FileIniDataParser();

        public static void Save(Settings settings)
        {
            Rwl.AcquireWriterLock(TimeSpan.FromSeconds(1));
            try
            {
                var iniData = new IniData();
                iniData["MAIN"]["SettingsType"] = settings.SettingsType.ToString("D");
                iniData["MAIN"]["ContentType"] = settings.ContentType.ToString("D");
                iniData["MAIN"]["CurrentImageMonth"] = settings.CurrentImageMonth.ToString();
                FileIniDataParser.WriteFile(SettingsFileName, iniData);
            }
            finally
            {
                Rwl.ReleaseWriterLock();
            }
        }

        public static Settings Load()
        {
            if (!File.Exists(SettingsFileName))
            {
                return new Settings();
            }

            Rwl.AcquireReaderLock(TimeSpan.FromSeconds(1));
            try
            {
                var iniData = FileIniDataParser.ReadFile(SettingsFileName);
                return new Settings
                {
                    SettingsType = (SettingsType)int.Parse(iniData["MAIN"]["SettingsType"]),
                    ContentType = (WallpaperContentType)int.Parse(iniData["MAIN"]["ContentType"]),
                    CurrentImageMonth = int.Parse(iniData["MAIN"]["CurrentImageMonth"])
                };
            }
            catch
            {
                return new Settings();
            }
            finally
            {
                Rwl.ReleaseReaderLock();
            }
        }
    }
}
