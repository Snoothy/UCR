using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ConfigurableWindowDemo
{
    /// <summary>
    /// An abstract Window descendant that provides simple 
    /// user settings persistence. The concrete backing 
    /// store is exposed by subclasses via overriding the 
    /// CreateSettings method.
    /// </summary>
    public abstract class ConfigurableWindow : Window
    {
        #region Data

        bool _isLoaded;
        readonly IConfigurableWindowSettings _settings;

        #endregion // Data

        #region Constructor

        protected ConfigurableWindow()
        {
            _settings = this.CreateSettings();

            if (_settings == null)
                throw new Exception("Cannot return null.");

            this.Loaded += delegate { _isLoaded = true; };

            this.ApplySettings();
        }

        #endregion // Constructor

        #region CreateSettings

        /// <summary>
        /// Derived classes must return the object which exposes 
        /// persisted window settings. This method is only invoked 
        /// once per Window, during construction.
        /// </summary>
        protected abstract IConfigurableWindowSettings CreateSettings();

        #endregion // CreateSettings

        #region Base Class Overrides

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            // We need to delay this call because we are 
            // notified of a location change before a 
            // window state change.  That causes a problem 
            // when maximizing the window because we record 
            // the maximized window's location, which is not 
            // something worth saving.
            base.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new ThreadStart(delegate
                {
                    if (_isLoaded && base.WindowState == WindowState.Normal)
                    {
                        Point loc = new Point(base.Left, base.Top);
                        _settings.WindowLocation = loc;
                    }
                }));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo info)
        {
            base.OnRenderSizeChanged(info);

            if (_isLoaded && base.WindowState == WindowState.Normal)
            {
                _settings.WindowSize = base.RenderSize;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (_isLoaded)
            {
                // We don't want the Window to open in the 
                // minimized state, so ignore that value.
                if (base.WindowState != WindowState.Minimized)
                    _settings.WindowState = base.WindowState;
                else
                    _settings.WindowState = WindowState.Normal;
            }
        }

        #endregion // Base Class Overrides

        #region Private Helpers

        void ApplySettings()
        {
            Size sz = _settings.WindowSize;
            base.Width = sz.Width;
            base.Height = sz.Height;

            Point loc = _settings.WindowLocation;

            // If the user's machine had two monitors but now only
            // has one, and the Window was previously on the other
            // monitor, we need to move the Window into view.
            bool outOfBounds =
                loc.X <= -sz.Width ||
                loc.Y <= -sz.Height ||
                SystemParameters.VirtualScreenWidth <= loc.X ||
                SystemParameters.VirtualScreenHeight <= loc.Y;

            if (_settings.IsFirstRun || outOfBounds)
            {
                base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                base.WindowStartupLocation = WindowStartupLocation.Manual;

                base.Left = loc.X;
                base.Top = loc.Y;

                // We need to wait until the HWND window is initialized before
                // setting the state, to ensure that this works correctly on
                // a multi-monitor system.  Thanks to Andrew Smith for this fix.
                this.SourceInitialized += delegate
                {
                    base.WindowState = _settings.WindowState;
                };
            }
        }

        #endregion // Private Helpers
    }
}