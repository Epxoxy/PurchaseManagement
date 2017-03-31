using System;
using PurchaseManagement.Extension;
using System.Collections.Generic;

namespace PurchaseManagement.Models
{
    [Table(Name = "warehousing", LocalName = "入库")]
    public class WareHousing : Interfaces.ISqlModel
    {
        [Column(Name = "WarehousingID", LocalName = "入库单ID", IsPrimaryKey = true)]
        public int WarehousingID { get; set; }

        [Column(Name = "ReceiptID", LocalName = "收货单ID")]
        public int ReceiptID { get; set; }

        [Column(Name = "WareroomID", LocalName = "库房ID")]
        public int WareroomID { get; set; }

        [Column(Name = "WarehousingState", LocalName = "入库状态")]
        public string WarehousingState { get; set; }

        [Column(Name = "StaffID", LocalName = "员工ID", ReadOnly = true)]
        public int StaffID { get; set; }

        [Column(Name = "Date", LocalName = "日期", Type = typeof(DateTime), ReadOnly = true)]
        public DateTime Date { get; set; }

        public bool ToScript(out Dictionary<string, string> columnValues)
        {
            columnValues = new Dictionary<string, string>();
            if (WarehousingID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "WarehousingID"), WarehousingID.ToString());
            if (ReceiptID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "ReceiptID"), ReceiptID.ToString());
            if (WareroomID >= 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "WareroomID"), WareroomID.ToString());

            columnValues.Add(ModelHelper.getColumnName(this.GetType(), "StaffID"), MySqlConnector.Connector.StaffID.ToString());
            if (!string.IsNullOrEmpty(WarehousingState)) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "WarehousingState"), WarehousingState);
            if (columnValues.Count > 0) return true;
            return false;
        }
    }
}
