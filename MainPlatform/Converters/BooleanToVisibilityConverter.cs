using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MainPlatform.Converters
{
    /// <summary>
    /// bool 转换 Visibility
    /// true → Visible
    /// false → Collapsed
    /// ConverterParameter=Invert 开启反向
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool b)
                flag = b;

            //支持参数取反
            if (parameter != null && parameter.ToString().Equals("Invert", StringComparison.OrdinalIgnoreCase))
            {
                flag = !flag;
            }

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
            {
                bool result = vis == Visibility.Visible;
                if (parameter != null && parameter.ToString().Equals("Invert", StringComparison.OrdinalIgnoreCase))
                {
                    result = !result;
                }
                return result;
            }
            return false;
        }
    }
}
