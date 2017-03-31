using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PurchaseManagement.View
{
    public class NavMenuItem
    {
        public string ItemName { get; set; }
        public string GroupName { get; set; }
        public object ExtraData { get; set; }
        public string CaseString { get; set; }

        public NavMenuItem() { }
        public NavMenuItem(string itemName) : this(itemName, null) { }

        public NavMenuItem(string itemName, string caseString) : this(itemName, caseString, null) { }

        public NavMenuItem(string itemName, string caseString, object extradata)
        {
            ItemName = itemName;
            CaseString = caseString;
            ExtraData = extradata;
        }
    }
}
