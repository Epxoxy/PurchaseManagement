using PurchaseManagement.Extension;
using System.Collections.Generic;

namespace PurchaseManagement.Models
{
    [Table(Name = "staff", LocalName = "员工")]
    public class Staff : Interfaces.ISqlModel
    {
        [Column(Name = "StaffID", IsPrimaryKey = true)]
        public int StaffID { get; set; }

        [Column(Name = "DeptID")]
        public int DeptID { get; set; }

        [Column(Name = "Name")]
        public string Name { get; set; }

        [Column(Name = "RoleID")]
        public int RoleID { get; set; }

        [Column(Name = "Age")]
        public int Age { get; set; }

        [Column(Name = "Sex")]
        public string Sex { get; set; }

        public bool ToScript(out Dictionary<string, string> columnValues)
        {
            columnValues = new Dictionary<string, string>();
            if (StaffID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "StaffID"), StaffID.ToString());
            if (DeptID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "DeptID"), DeptID.ToString());
            if (!string.IsNullOrEmpty(Name)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "Name"), Name);
            if (RoleID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "RoleID"), RoleID.ToString());
            if (Age > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "Age"), Age.ToString());
            if (!string.IsNullOrEmpty(Sex)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "Sex"), Sex);
            if (columnValues.Count > 0) return true;
            return false;
        }
    }
}
