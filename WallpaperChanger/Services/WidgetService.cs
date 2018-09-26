using System.Net;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using WallpaperChanger.Data;
using WallpaperChanger.Widget;

namespace WallpaperChanger.Services
{
    public static class WidgetService
    {
        public static WidgetNewItem GetNew()
        {
            const string url = "https://ledel.ru/bitrix/php_interface/utils/app_wallpaper.php?action=get_news";
            try
            {
                using (var request = new WebClient())
                {
                    var json = request.DownloadString(url);
                    return JsonConvert.DeserializeObject<WidgetNewItem>(json);
                }
            }
            catch
            {
                return new WidgetNewItem();
            }
        }

        public static void ChangeWidget(MainWindow mainWin, WallpaperContentType contentType, bool changeWithContent = true)
        {
            var coord = GetCoordinate(mainWin, contentType);
            mainWin.SetPosition(coord.Y, coord.X);
            if (changeWithContent)
            {
                SetWidgetContent(mainWin);
            }
        }

        private static void SetWidgetContent(MainWindow mainWin)
        {
            var lastNew = GetNew();
            mainWin.SetContent(lastNew.Description, lastNew.Header, lastNew.Link);
        }

        private static Point GetCoordinate(MainWindow mainWin, WallpaperContentType contentType)
        {
            switch (contentType)
            {
                case WallpaperContentType.Adult:
                    return new Point((Screen.PrimaryScreen.Bounds.Size.Width - mainWin.ActualWidth) / 2, Screen.PrimaryScreen.Bounds.Size.Height - mainWin.ActualHeight - 30);
                case WallpaperContentType.Nature:
                    return new Point(Screen.PrimaryScreen.Bounds.Size.Width - mainWin.ActualWidth, (Screen.PrimaryScreen.Bounds.Size.Height - mainWin.ActualHeight) / 2);
                case WallpaperContentType.Technology:
                    return new Point((Screen.PrimaryScreen.Bounds.Size.Width - mainWin.ActualWidth) / 2, 1);
                default:
                    return new Point(-mainWin.ActualWidth, -mainWin.ActualHeight);
            }
        }

        public class WidgetNewItem
        {
            public string Description { get; set; }

            public string Header { get; set; }

            public string Link { get; set; }
        }
    }
}
