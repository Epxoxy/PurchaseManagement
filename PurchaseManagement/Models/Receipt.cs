using System;
using PurchaseManagement.Extension;
using System.Collections.Generic;

namespace PurchaseManagement.Models
{
    [Table(Name = "receipt", LocalName = "收货")]
    public class Receipt : Interfaces.ISqlModel
    {
        [Column(Name = "ReceiptID", LocalName = "收货单ID", IsPrimaryKey = true)]
        public int ReceiptID { get; set; }

        [Column(Name = "StaffID", LocalName = "员工ID", ReadOnly = true)]
        public int StaffID { get; set; }

        [Column(Name = "OrderID", LocalName = "订单ID")]
        public int OrderID { get; set; }

        [Column(Name = "RealAmount", LocalName = "实际数量")]
        public double RealAmount { get; set; }

        [Column(Name = "RealPay", LocalName = "实际支付")]
        public double RealPay { get; set; }

        [Column(Name = "Date", LocalName = "日期", Type = typeof(DateTime), ReadOnly = true)]
        public DateTime Date { get; set; }

        public bool ToScript(out Dictionary<string, string> columnValues)
        {
            columnValues = new Dictionary<string, string>();
            if (ReceiptID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "ReceiptID"), ReceiptID.ToString());
            columnValues.Add(ModelHelper.getColumnName(this.GetType(), "StaffID"), MySqlConnector.Connector.StaffID.ToString());
            if (OrderID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "OrderID"), OrderID.ToString());

            if (RealAmount > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "RealAmount"), RealAmount.ToString());
            if (RealPay > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "RealPay"), RealPay.ToString());
            if (columnValues.Count > 0) return true;
            return false;
        }
    }
}
