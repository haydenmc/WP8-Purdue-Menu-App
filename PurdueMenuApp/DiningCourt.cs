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
        public List<List<DateTime>> open_times
        {
            get;
            set;
        }
    }
}
