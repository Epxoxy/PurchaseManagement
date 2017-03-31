using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Extension
{
    public class Pair<T1, T2>
    {
        public T1 Display { get; set; }
        public T2 Value { get; set; }

        public Pair(T1 display, T2 value)
        {
            Display = display;
            Value = value;
        }
    }
}
