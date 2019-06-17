using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace HidWizards.UCR.Views.Controls
{
    public partial class WindowBar : UserControl
    {
        private Window Window => Window.GetWindow(this);
        private bool RestoreForDragMove { get; set; }


        public WindowBar()
        {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }

        private void Bar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) return;

            RestoreForDragMove = Window.WindowState == WindowState.Maximized;
            Window.DragMove();
        }

        private void Bar_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ResizeWindow();
            }
        }

        private void Bar_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RestoreForDragMove = false;
        }

        private void Resize_OnClick(object sender, RoutedEventArgs e)
        {
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            if (Window.WindowState == WindowState.Maximized)
            {
                Window.BorderThickness = new Thickness(1.0);
                Window.WindowState = WindowState.Normal;
            }
            else
            {
                Window.BorderThickness = new Thickness(8.0);
                Window.WindowState = WindowState.Maximized;
            }
        }
        
        private void Minimize_OnClick(object sender, RoutedEventArgs e)
        {
            Window.WindowState = WindowState.Minimized;
        }

        private void Bar_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!RestoreForDragMove) return;
            RestoreForDragMove = false;

            var point = PointToScreen(e.MouseDevice.GetPosition(this));
            Window.Left = point.X - (Window.RestoreBounds.Width * 0.5);
            Window.Top = point.Y;
            ResizeWindow();
            Window.DragMove();
        }
    }
}
