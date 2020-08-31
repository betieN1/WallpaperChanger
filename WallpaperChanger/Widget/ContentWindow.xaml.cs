using System.Windows;
using System.Windows.Input;

namespace WallpaperChanger.Widget
{
    public partial class ContentWindow : Window
    {
        public ContentWindow()
        {
            InitializeComponent();
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
            Close();
        }
    }
}
