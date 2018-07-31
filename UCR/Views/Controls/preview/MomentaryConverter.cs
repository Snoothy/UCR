using System;
using System.Globalization;

using System.Windows.Data;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Views.Controls.preview
{
    class MomentaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pressed = (long) value;
            return 100.0 * pressed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
