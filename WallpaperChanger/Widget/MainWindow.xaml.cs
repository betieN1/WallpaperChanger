﻿using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

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

        public void SetContent(string content, string author)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                lbContentTxt.Text = content;
                lbAuthorTxt.Content = author;
                lbDateTxt.Content = DateTime.Today.ToString("dd MMMM", new CultureInfo("ru-Ru"));
            }));
        }
    }
}