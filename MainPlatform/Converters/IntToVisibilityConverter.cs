using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MainPlatform.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                switch (intValue)
                {
                    case 0:
                        return System.Windows.Visibility.Collapsed;
                    case 1:
                        return System.Windows.Visibility.Visible;
                    case 2:
                        return System.Windows.Visibility.Visible;
                }
            }
            return System.Windows.Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Visibility.Visible.Equals(value) ? 1 : 0;
        }
    }
}
