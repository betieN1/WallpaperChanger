using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using WallpaperChanger.Data;
using WallpaperChanger.Services;
using MainWindow = WallpaperChanger.Widget.MainWindow;

namespace WallpaperChanger
{
    public partial class MainForm : Form
    {
        private readonly NotifyIcon _trayIcon;

        private readonly Settings _settings;
        private MainWindow _mainWin;
        private static readonly Mutex Mutex = new Mutex(true, "WallpaperChanger");

        public MainForm()
        {
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                _settings = SettingsService.Load();
                _trayIcon = new NotifyIcon
                {
                    Text = "LEDEL Wallpaper",
                    Icon = new Icon("icon\\tray.ico"),
                    ContextMenu = BuildContextMenu(),
                    Visible = true
                };

                InitializeComponent();
                Mutex.ReleaseMutex();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void ShowInfo(object sender, EventArgs e)
        {
            var infoForm = new Info();
            infoForm.Show();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            _mainWin = new MainWindow();
            _mainWin.Show();

            WidgetService.LoadTexts();

            new Thread(() => WallpaperServices.Check(_mainWin))
            {
                IsBackground = true
            }.Start();

            new Thread(PictureService.UploadToCache)
            {
                IsBackground = true
            }.Start();

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ChangeContentType(object sender, EventArgs e)
        {
            _settings.SettingsType = (SettingsType)(((MenuItem)sender).Index + 1);
            _trayIcon.ContextMenu = BuildContextMenu();

            var loadingWindow = new LoadingWindow();
            loadingWindow.Show();

            new Thread(() =>
            {
                SettingsService.Save(_settings);
                WallpaperServices.ChangeWallpaper(_settings.SettingsType, _mainWin);
                loadingWindow.DoClose();
            })
            {
                IsBackground = true
            }.Start();
        }

        private ContextMenu BuildContextMenu()
        {
            var trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Настройки", new[]
            {
                new MenuItem("Технологичность", ChangeContentType) { Checked = _settings.SettingsType == SettingsType.Technology },
                new MenuItem("Версия 18+ (календарь LEDEL)", ChangeContentType) { Checked = _settings.SettingsType == SettingsType.Adult },
                new MenuItem("Природа", ChangeContentType) { Checked = _settings.SettingsType == SettingsType.Nature },
                new MenuItem("Смешанная", ChangeContentType) { Checked = _settings.SettingsType == SettingsType.Mixed }
            });
            trayMenu.MenuItems.Add("О программе", ShowInfo);
            trayMenu.MenuItems.Add("Выход", OnExit);

            return trayMenu;
        }
    }
}
