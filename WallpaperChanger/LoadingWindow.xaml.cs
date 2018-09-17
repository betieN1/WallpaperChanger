using System;
using System.Windows;

namespace WallpaperChanger
{
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
        }

        public void DoClose()
        {
            Dispatcher.Invoke(new Action(Close));
        }
    }
}
