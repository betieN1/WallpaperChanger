using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace WallpaperChanger.Widget
{
    public partial class MainWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private string _content;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) => Diactivate();
        }

        private void Diactivate()
        {
            var helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        public void SetPosition(double top, double left)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Top = top;
                Left = left;
            }));
        }

        public void SetContent(string header, string content)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(header))
                {
                    gdMain.Visibility = Visibility.Hidden;
                    return;
                }

                _content = content;
                lbHeaderTxt.Text = header;
                lbDateTxt.Content = DateTime.Today.ToString("dd MMMM", new CultureInfo("ru-Ru"));

                gdMain.Visibility = Visibility.Visible;
            }));
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var xCoord = Screen.PrimaryScreen.Bounds.Size.Width / 2 - Width;
            var yCoord = Screen.PrimaryScreen.Bounds.Size.Height / 2 - Height;

            var contentWindow = new ContentWindow();
            contentWindow.Left = xCoord;
            contentWindow.Top = yCoord;
            contentWindow.tbContentTxt.Text = _content;
            contentWindow.ShowDialog();
        }
    }
}
