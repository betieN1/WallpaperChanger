using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using WallpaperChanger.Data;
using WallpaperChanger.Widget;

namespace WallpaperChanger.Services
{
    public static class WidgetService
    {
        private static WidgetTextItem[] _texts;

        public static void LoadTexts()
        {
            var texts = File.ReadAllText("Content\\texts.js");
            _texts = JsonConvert.DeserializeObject<WidgetTextItem[]>(texts);
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
            if (_texts != null && _texts.Any())
            {
                var random = new Random();
                var index = random.Next(0, _texts.Length - 1);
                var content = _texts[index];
                mainWin.SetContent(content.Text, content.Author);
            }
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

        public class WidgetTextItem
        {
            public string Text { get; set; }

            public string Author { get; set; }
        }
    }
}
