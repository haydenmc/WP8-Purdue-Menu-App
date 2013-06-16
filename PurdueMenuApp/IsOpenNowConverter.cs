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
    public class IsOpenNowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<List<DateTime>> times = (List<List<DateTime>>)value;
            foreach (List<DateTime> range in times)
            {
                DateTime range_start = range.First();
                DateTime range_end = range.Last();
                if (DateTime.Now.CompareTo(range_start) > 0 && DateTime.Now.CompareTo(range_end) < 0)
                    if (parameter != null && ((String)parameter).Equals("Inverted"))
                    {
                        return Visibility.Collapsed;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
            }
            if (parameter != null && ((String)parameter).Equals("Inverted"))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
            
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
