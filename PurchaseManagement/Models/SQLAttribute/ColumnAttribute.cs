using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Models
{
    /// <summary>
    /// Column Attribute for DataBase Model
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public Type Type { get; set; }
        public bool ReadOnly { get; set; }
        public string LocalName { get; set; }
    }
}
