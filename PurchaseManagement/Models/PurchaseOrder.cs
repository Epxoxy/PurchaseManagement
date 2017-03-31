using System;
using PurchaseManagement.Extension;
using System.Collections.Generic;

namespace PurchaseManagement.Models
{
    [Table(Name = "purchaseorders", LocalName = "进货")]
    public class PurchaseOrder : Interfaces.ISqlModel
    {
        [Column(Name = "OrderID", LocalName = "订单ID", IsPrimaryKey = true)]
        public int OrderID { get; set; }

        [Column(Name = "OrderType", LocalName = "订单类型")]
        public string OrderType { get; set; }

        [Column(Name = "SupplierID", LocalName = "供应商ID")]
        public int SupplierID { get; set; }

        [Column(Name = "StaffID", LocalName = "员工ID", ReadOnly = true)]
        public int StaffID { get; set; }

        [Column(Name = "TotalPrice", LocalName = "总价")]
        public double TotalPrice { get; set; } = -1;
        
        [Column(Name = "MakeDate", LocalName = "生成时间", Type = typeof(DateTime), ReadOnly = true)]
        public DateTime MakeDate { get; set; }

        [Column(Name = "PurchaseState", LocalName = "进货状态")]
        public string PurchaseState { get; set; }

        [Column(Name = "Note", LocalName = "备注")]
        public string Note { get; set; }


        public bool ToScript(out Dictionary<string, string> columnValues)
        {
            columnValues = new Dictionary<string, string>();
            if (OrderID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "OrderID"), OrderID.ToString());
            if (!string.IsNullOrEmpty(OrderType)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "OrderType"), OrderType);
            if (SupplierID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "SupplierID"), SupplierID.ToString());

            columnValues.Add(ModelHelper.getColumnName(this.GetType(), "StaffID"), MySqlConnector.Connector.StaffID.ToString());

            if (TotalPrice > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "TotalPrice"), TotalPrice.ToString());
            if (!string.IsNullOrEmpty(PurchaseState)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "PurchaseState"), PurchaseState);
            if (!string.IsNullOrEmpty(Note)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "Note"), Note);
            if (columnValues.Count > 0) return true;
            return false;
        }
    }
}
