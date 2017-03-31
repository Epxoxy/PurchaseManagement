using PurchaseManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Models
{
    /// <summary>
    /// Table attribute for Database Model
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public string LocalName { get; set; }
    }

    public enum Tables
    {
        PurchaseOrder,
        OrderProject,
        Receipt,
        WareHousing,
        Supplier,
        Goods,
        Staff
    }
}
