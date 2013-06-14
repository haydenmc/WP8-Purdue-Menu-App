using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurdueMenuApp
{
    public class DiningCourt
    {
        public String name
        {
            get;
            set;
        }
        public String web_id
        {
            get;
            set;
        }
        public bool open
        {
            get;
            set;
        }
        public DateTimeOffset time_breakfast_open
        {
            get;
            set;
        }
        public DateTimeOffset time_breakfast_close
        {
            get;
            set;
        }
        public DateTimeOffset time_lunch_open
        {
            get;
            set;
        }
        public DateTimeOffset time_lunch_close
        {
            get;
            set;
        }
        public DateTimeOffset time_dinner_open
        {
            get;
            set;
        }
        public DateTimeOffset time_dinner_close
        {
            get;
            set;
        }
    }
}
