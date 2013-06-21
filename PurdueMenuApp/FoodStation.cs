using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurdueMenuApp
{
    class FoodStation<MenuItem> : List<MenuItem>
    {
        public string title
        {
            get;
            set;
        }

        public FoodStation(String name, List<MenuItem> items) : base (items)
        {
            this.title = name;
        }
    }
}
