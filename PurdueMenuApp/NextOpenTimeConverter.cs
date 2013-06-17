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
    public class NextOpenTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<List<DateTime>> times = (List<List<DateTime>>)value;
            //Make sure we have times.
            if (times.Count() <= 0)
                return "not serving";

            //Check to see if the location is currently open
            foreach (List<DateTime> range in times)
            {
                DateTime range_start = range.First();
                DateTime range_end = range.Last();
                if (DateTime.Now.CompareTo(range_start) > 0 && DateTime.Now.CompareTo(range_end) < 0)
                {
                    TimeSpan timeleft = range_end.Subtract(DateTime.Now);
                    if (timeleft.TotalHours >= 1)
                        return "open for " + timeleft.ToString("%h") + " hours, " + timeleft.ToString("%m") + " minutes";
                    else if (timeleft.Minutes <= 0)
                        return "closing now...";
                    else
                        return "open for " + timeleft.ToString("%m") + " minutes";
                }
            }

            //No? Have we missed the last time?
            if (times.Last().Last().CompareTo(DateTime.Now) < 0)
            {
                return "closed";
            }

            //Not open now... so find the next closest open time.
            System.Diagnostics.Debug.WriteLine("FINDING NEXT OPEN TIME:");
            DateTime nextopentime = times.First().First();
            foreach (List<DateTime> range in times)
            {
                System.Diagnostics.Debug.WriteLine("Checking time " + range.First() + " against time " + nextopentime);
                TimeSpan timetonew = range.First().Subtract(DateTime.Now);
                TimeSpan timetoold = nextopentime.Subtract(DateTime.Now);
                System.Diagnostics.Debug.WriteLine("Time until " + range.First() + ": " + timetonew + ", time until " + nextopentime + ": " + timetoold);
                if (timetonew.CompareTo(TimeSpan.Zero) < 0)
                    continue;
                if (timetoold.CompareTo(TimeSpan.Zero) < 0)
                {
                    System.Diagnostics.Debug.WriteLine("Selecting " + range.First());
                    nextopentime = range.First();
                    continue;
                }
                if (timetonew.CompareTo(timetoold) < 0) // < ?
                {
                    System.Diagnostics.Debug.WriteLine("Selecting " + range.First());
                    nextopentime = range.First();
                }
            }
            TimeSpan timeuntilopen = nextopentime.Subtract(DateTime.Now);
            if (timeuntilopen.TotalHours >= 1)
                return "opens in " + timeuntilopen.ToString("%h") + " hours, " + timeuntilopen.ToString("%m") + " minutes";
            else
                return "opens in " + timeuntilopen.ToString("%m") + " minutes";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
