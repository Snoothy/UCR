using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;

namespace HidWizards.UCR.Settings
{
    public interface ISettingsProvider
    {
        // Check for start the app minimized
        bool StartMinimized { get; set; }

        // MainWindow coordinates
        Point WindowLocation { get; set; }

        // MainWindow size
        Size WindowSize { get; set; }
    }

    public class SettingsProvider : ISettingsProvider
    {
        private readonly ApplicationSettingsBase _settings;

        private readonly string _startMinimizedProp;
        private readonly string _windowLocationProp;
        private readonly string _windowSizeProp;

        public SettingsProvider(
            ApplicationSettingsBase settings,
                string startMinimizedProp,
                string windowLocationProp,
                string windowSizeProp)
        {
            _settings = settings;

            _startMinimizedProp = startMinimizedProp;
            _windowLocationProp = windowLocationProp;
            _windowSizeProp = windowSizeProp;
        }

        protected T GetValue<T>(string propName)
        {
            return (T)_settings[propName];
        }

        protected void SetValue(string propName, object value)
        {
            _settings[propName] = value;
            _settings.Save();
        }

        public bool StartMinimized
        {
            get { return GetValue<bool>(_startMinimizedProp); }
            set { SetValue(_startMinimizedProp, value); }
        }

        public Point WindowLocation
        {
            get { return GetValue<Point>(_windowLocationProp); }
            set { SetValue(_windowLocationProp, value); }
        }

        public Size WindowSize
        {
            get { return GetValue<Size>(_windowSizeProp); }
            set { SetValue(_windowSizeProp, value); }
        }
    }
}