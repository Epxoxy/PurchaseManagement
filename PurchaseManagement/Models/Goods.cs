using System.Collections.Generic;
using PurchaseManagement.Extension;

namespace PurchaseManagement.Models
{
    [Table(Name = "goods", LocalName = "商品")]
    public class Goods : Interfaces.ISqlModel
    {
        [Column(Name = "GoodsID", LocalName = "商品ID", IsPrimaryKey = true)]
        public int GoodsID { get; set; }

        [Column(Name = "Name", LocalName = "名称")]
        public string Name { get; set; }

        [Column(Name = "GoodsType", LocalName = "商品类型")]
        public string Type { get; set; }

        [Column(Name = "Specifications", LocalName = "规格")]
        public string Specifications { get; set; }

        [Column(Name = "UnitPrice", LocalName = "单价")]
        public double UnitPrice { get; set; }

        [Column(Name = "StockAmount", LocalName = "库存")]
        public double StockAmount { get; set; } = 0;

        [Column(Name = "WareroomID", LocalName = "库房ID")]
        public int WareroomID { get; set; }

        [Column(Name = "MaxAmount", LocalName = "最大量")]
        public double MaxAmount { get; set; } = 0;

        public bool ToScript(out Dictionary<string, string> columnValues)
        {
            columnValues = new Dictionary<string, string>();
            if (GoodsID > 0) columnValues.Add(ModelHelper.getColumnName(typeof(Goods), "GoodsID"), GoodsID.ToString());
            if (!string.IsNullOrEmpty(Name)) columnValues.Add(ModelHelper.getColumnName(typeof(Goods), "Name"), Name);
            if (!string.IsNullOrEmpty(Type)) columnValues.Add(ModelHelper.getColumnName(typeof(Goods), "Type"), Type);
            if (!string.IsNullOrEmpty(Specifications)) columnValues.Add(ModelHelper.getColumnName(typeof(Goods), "Specifications"), Specifications);
            if (UnitPrice > 0) columnValues.Add(ModelHelper.getColumnName(typeof(Goods), "UnitPrice"), UnitPrice.ToString());
            if (StockAmount > 0) columnValues.Add(ModelHelper.getColumnName(typeof(Goods), "StockAmount"), StockAmount.ToString());
            if (WareroomID > 0) columnValues.Add(ModelHelper.getColumnName(typeof(Goods), "WareroomID"), WareroomID.ToString());
            if (MaxAmount > 0) columnValues.Add(ModelHelper.getColumnName(typeof(Goods), "MaxAmount"), MaxAmount.ToString());
            if (columnValues.Count > 0) return true;
            return false;
        }
    }
}
