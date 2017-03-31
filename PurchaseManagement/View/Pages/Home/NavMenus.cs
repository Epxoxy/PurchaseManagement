using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.View
{
    public class NavMenus
    {
        private string header;
        private string group;
        private NavMenuItem[] subMenus;

        public string Header
        {
            get { return header; }
            set { header = value; }
        }
        public string Group
        {
            get { return group; }
            set { group = value; }
        }
        public NavMenuItem[] SubMenus
        {
            get { return subMenus; }
            set { subMenus = value; }
        }

        public NavMenus() { }
        public NavMenus(NavMenuItem[] subMenus) : this("Header", subMenus, string.Empty) { }
        public NavMenus(NavMenuItem[] subMenus, string group) : this("Header", subMenus, group) { }
        public NavMenus(string header, NavMenuItem[] subMenus) : this(header, subMenus, string.Empty) { }
        public NavMenus(string header, NavMenuItem[] subMenus, string group)
        {
            this.Header = header;
            this.SubMenus = subMenus;
            this.Group = group;
        }

    }
}
