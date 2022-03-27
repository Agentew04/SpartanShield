using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SpartanShield.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    internal class BooleanToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Visibility.Collapsed;
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            Visibility visibility = (Visibility)value;
            if (visibility==Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
