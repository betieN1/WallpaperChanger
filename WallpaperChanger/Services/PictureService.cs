using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using WallpaperChanger.Common;
using WallpaperChanger.Data;

namespace WallpaperChanger.Services
{
    public class PictureService
    {
        private const string DefaultScreenSize = "16-9";

        public static void UploadToCache()
        {
            while (true)
            {
                try
                {
                    foreach (WallpaperContentType wct in Enum.GetValues(typeof(WallpaperContentType)))
                    {
                        try
                        {
                            UploadPicture(wct);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromHours(1));
                }
            }
        }

        public static string GetScreenView()
        {
            var screenSize = Screen.PrimaryScreen.Bounds.Size.Width + "x" + Screen.PrimaryScreen.Bounds.Size.Height;
            return ConfigurationManager.AppSettings[screenSize] ?? DefaultScreenSize;
        }

        public static string GetPicturePath(WallpaperContentType contentType)
        {
            var fn = GetFileName();
            var filePath = GetFilePath(contentType, fn);
            if (File.Exists(filePath))
            {
                return filePath;
            }

            return UploadPicture(contentType);
        }

        private static string UploadPicture(WallpaperContentType contentType)
        {
            var fn = GetFileName();
            var filePath = GetFilePath(contentType, fn);

            var url = $"{ConfigurationManager.AppSettings["uploadUrl"]}/{contentType:D}/{fn}";

            using (var request = new WebClient())
            {
                var fileData = request.DownloadData(url);
                lock (SharedLocker.LockObj)
                {
                    using (var fs = File.Create(filePath))
                    {
                        fs.Write(fileData, 0, fileData.Length);
                        fs.Close();
                    }
                }
            }

            return filePath;
        }

        private static string GetFileName()
        {
            var screenFolderName = GetScreenView();
            return $"{screenFolderName}_{DateTime.Now.Month}.{ConfigurationManager.AppSettings["fileExt"]}";
        }

        private static string GetFilePath(WallpaperContentType contentType, string fn)
        {
            var userDirectory = Environment.GetEnvironmentVariable("USERPROFILE") ?? Path.GetTempPath();
            var ledelDirectory = Path.Combine(userDirectory, Path.Combine("LEDEL Cache", contentType.ToString()));
            Directory.CreateDirectory(ledelDirectory);
            var filePath = Path.Combine(ledelDirectory, fn);
            return filePath;
        }
    }
}
