using System;
using System.Runtime.InteropServices;
using System.Threading;
using WallpaperChanger.Common;
using WallpaperChanger.Data;
using MainWindow = WallpaperChanger.Widget.MainWindow;

namespace WallpaperChanger.Services
{
    public static class WallpaperServices
    {
        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uiAction, int uiParam, string pvParam, uint fWinIni);

        public static void Check(MainWindow mainWin)
        {
            while (true)
            {
                try
                {
                    var settings = SettingsService.Load();

                    if (settings.SettingsType != SettingsType.Mixed)
                    {
                        var currentDate = DateTime.Now;

                        if (currentDate.Day == 1 || settings.CurrentImageMonth != currentDate.Month)
                        {
                            ChangeWallpaper(settings, mainWin);
                        }
                        else
                        {
                            WidgetService.ChangeWidget(mainWin, settings.ContentType);
                        }
                    }
                    else
                    {
                        ChangeWallpaper(settings, mainWin);
                    }
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromSeconds((DateTime.Today.AddDays(1) - DateTime.Now).TotalSeconds));
                }
            }
        }

        public static void ChangeWallpaper(SettingsType settingsType, MainWindow mainWindow)
        {
            var settings = SettingsService.Load();
            if (settingsType == SettingsType.Mixed)
            {
                settings.ContentType = Enum.IsDefined(typeof(WallpaperContentType), settings.ContentType + 1)
                    ? settings.ContentType + 1
                    : WallpaperContentType.Technology;
            }
            else
            {
                settings.ContentType = (WallpaperContentType) settingsType;
            }

            var picturePath = PictureService.GetPicturePath(settings.ContentType);

            EnsureChangeWallpaper(picturePath, settings, mainWindow, false);
        }

        private static void ChangeWallpaper(Settings settings, MainWindow mainWindow)
        {
            if (settings.SettingsType == SettingsType.Mixed)
            {
                settings.ContentType = Enum.IsDefined(typeof(WallpaperContentType), settings.ContentType + 1)
                    ? settings.ContentType + 1
                    : WallpaperContentType.Technology;
            }

            var picturePath = PictureService.GetPicturePath(settings.ContentType);
            EnsureChangeWallpaper(picturePath, settings, mainWindow);
        }

        private static void EnsureChangeWallpaper(string picturePath, Settings settings, MainWindow mainWindow, bool changeWithWidgetContent = true)
        {
            ChangeWallpaper(picturePath);
            settings.CurrentImageMonth = DateTime.Now.Month;
            SettingsService.Save(settings);
            WidgetService.ChangeWidget(mainWindow, settings.ContentType, changeWithWidgetContent);
        }

        private static void ChangeWallpaper(string path)
        {
            lock (SharedLocker.LockObj)
            {
                SystemParametersInfo(0x0014, 0, path, 0x0001);
            }
        }
    }
}
