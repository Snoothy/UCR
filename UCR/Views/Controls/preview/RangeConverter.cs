using System;
using System.Globalization;

using System.Windows.Data;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Views.Controls.preview
{
    class RangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var range = (long) value;
            return 50.0 + ((double)range / Constants.AxisMaxValue) * 50;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
