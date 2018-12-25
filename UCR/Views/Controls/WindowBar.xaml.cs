using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HidWizards.UCR.Views.Controls
{
    public partial class WindowBar : UserControl
    {
        private Window Window => Window.GetWindow(this);

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
            Window.DragMove();
        }

        private void Bar_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ResizeWindow();
            }
        }

        private void Resize_OnClick(object sender, RoutedEventArgs e)
        {
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            Window.WindowState = Window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void Minimize_OnClick(object sender, RoutedEventArgs e)
        {
            Window.WindowState = WindowState.Minimized;
        }
    }
}
