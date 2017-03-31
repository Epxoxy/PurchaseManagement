using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurchaseManagement.Extension;

namespace PurchaseManagement.Models
{
    [Table(Name = "orderproject", LocalName = "订货项目")]
    public class OrderProject : Interfaces.ISqlModel
    {
        [Column(Name = "OrderProjectID", LocalName = "订货项目ID", IsPrimaryKey = true)]
        public int OrderProjectID { get; set; }

        [Column(Name = "OrderID", LocalName = "订单ID")]
        public int OrderID { get; set; }

        [Column(Name = "GoodsID", LocalName = "商品ID")]
        public int GoodsID { get; set; }

        [Column(Name = "Amount", LocalName = "数量")]
        public double Amount { get; set; }

        [Column(Name = "UnitPrice", LocalName = "单价")]
        public double UnitPrice { get; set; }


        public bool ToScript(out Dictionary<string, string> columnValues)
        {
            columnValues = new Dictionary<string, string>();
            if (OrderID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "OrderID"), OrderID.ToString());
            if (GoodsID > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "GoodsID"), GoodsID.ToString());
            if (UnitPrice > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "UnitPrice"), UnitPrice.ToString());
            if (Amount > 0) columnValues.Add(ModelHelper.getColumnName(this.GetType(), "Amount"), Amount.ToString());
            if (columnValues.Count > 0) return true;
            return false;
        }
    }
}
