using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PurdueMenuApp
{
    public class IsServingTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<List<DateTime>> times = (List<List<DateTime>>)value;
            //return Visibility.Visible;
            if (parameter != null && ((String)parameter).Equals("Inverted"))
            {
                return (times.Count() > 0) ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return (times.Count() > 0) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
