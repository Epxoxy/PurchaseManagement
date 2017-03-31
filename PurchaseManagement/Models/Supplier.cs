using PurchaseManagement.Extension;
using System.Collections.Generic;

namespace PurchaseManagement.Models
{
    [Table(Name = "supplier", LocalName = "供应商")]
    public class Supplier : Interfaces.ISqlModel
    {
        [Column(Name = "SupplierID", LocalName = "供应商ID", IsPrimaryKey = true)]
        public int SupplierID { get; set; }

        [Column(Name = "Name", LocalName = "名称")]
        public string Name { get; set; }

        [Column(Name = "Telephone", LocalName = "电话")]
        public string Telephone { get; set; }

        [Column(Name = "Address", LocalName = "地址")]
        public string Address { get; set; }


        public bool ToScript(out Dictionary<string, string> columnValues)
        {
            columnValues = new Dictionary<string, string>();
            if (SupplierID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "SupplierID"), SupplierID.ToString());
            if (!string.IsNullOrEmpty(Name)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "Name"), Name);
            if (!string.IsNullOrEmpty(Telephone)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "Telephone"), Telephone);

            if (!string.IsNullOrEmpty(Address)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "Address"), Address);
            if (columnValues.Count > 0) return true;
            return false;
        }
    }
}
