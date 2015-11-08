using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClearSight.Core.Window
{
    /// <summary>
    /// A simple, empty WPF window without XAML.
    /// </summary>
    public class WPFWindow : System.Windows.Window, IWindow
    {
        private bool closed = false;

        bool IWindow.Closed
        {
            get
            {
                return closed;
            }
        }

        public WPFWindow(int width, int height)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            Background = Brushes.Black;
            Topmost = true;

            Width = width;
            Height = height;

            Closed += (sender, e) => closed = true;

            Show();
        }
    }
}