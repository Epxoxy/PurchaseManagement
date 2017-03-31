using PurchaseManagement.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Models
{
    public class HandState
    {
        public const string Pending = "Pending";
        public const string Handing = "Handing";
        public const string Completed = "Completed";
        public const string Canc = "Canceled";
    }

    public class OrderType
    {
        public const string Buy = "Buy";
        public const string Transform = "Transform";
    }

    public class GoodsType
    {
        public const string Food = "Food";
        public const string Daliy = "Daliy";
        public const string Other = "Other";
    }

    public class ColumnSelections
    {
        private static Dictionary<string, Pair<string, string>[]> selectionDictionary;
        public const string purchaseStateKeyword = "PurchaseState", orderTypeKeyword = "OrderType", wareHousingStateKeyword = "WarehousingState",goodsTypeKeyword = "GoodsType";

        private static Pair<string, string>[] purchaseStates;
        private static Pair<string, string>[] orderType;
        private static Pair<string, string>[] warehousingState;
        private static Pair<string, string>[] goodsType;

        public static Pair<string, string>[] PurchaseStates
        {
            get
            {
                if(purchaseStates == null)
                {
                    
                    purchaseStates = new Pair<string, string>[]
                    {
                        new Pair<string, string>(Properties.Resources.Pending, HandState.Pending),
                        new Pair<string, string>(Properties.Resources.Handing, HandState.Handing),
                        new Pair<string, string>(Properties.Resources.Completed, HandState.Completed),
                        new Pair<string, string>(Properties.Resources.Cancel, HandState.Canc)
                    };
                }
                return purchaseStates;
            }
        }
        public static Pair<string, string>[] OrderTypes
        {
            get
            {
                if (orderType == null)
                {
                    orderType = new Pair<string, string>[]
                    {
                        new Pair<string, string>(Properties.Resources.Buy, OrderType.Buy),
                        new Pair<string, string>(Properties.Resources.Transform, OrderType.Transform)
                    };
                }
                return orderType;
            }
        }
        public static Pair<string, string>[] WarehousingStates
        {
            get
            {
                if (warehousingState == null)
                {
                    warehousingState = new Pair<string, string>[]
                    {
                        new Pair<string, string>(Properties.Resources.Pending, HandState.Pending),
                        new Pair<string, string>(Properties.Resources.Handing, HandState.Handing),
                        new Pair<string, string>(Properties.Resources.Completed, HandState.Completed),
                        new Pair<string, string>(Properties.Resources.Cancel, HandState.Canc)
                    };
                }
                return warehousingState;
            }
        }
        public static Pair<string, string>[] GoodsTypes
        {
            get
            {
                if (goodsType == null)
                {
                    goodsType = new Pair<string, string>[]
                    {
                        new Pair<string, string>(Properties.Resources.Food, GoodsType.Food),
                        new Pair<string, string>(Properties.Resources.Daliy, GoodsType.Daliy),
                        new Pair<string, string>(Properties.Resources.Other, GoodsType.Other)
                    };
                }
                return goodsType;
            }
        }

        private static void initDictionary()
        {
            selectionDictionary = new Dictionary<string, Pair<string, string>[]>();
            selectionDictionary.Add(purchaseStateKeyword, PurchaseStates);
            selectionDictionary.Add(orderTypeKeyword, OrderTypes);
            selectionDictionary.Add(wareHousingStateKeyword, WarehousingStates);
            selectionDictionary.Add(goodsTypeKeyword, GoodsTypes);
        }

        public static Pair<string, string>[] getSelection(string selectionKeyword)
        {
            if (selectionDictionary == null) initDictionary();
            if (selectionDictionary.ContainsKey(selectionKeyword)) return selectionDictionary[selectionKeyword];
            return null;
        }

        public static bool hasSelection(string columnName)
        {
            if (selectionDictionary == null) initDictionary();
            return selectionDictionary.ContainsKey(columnName);
        }
    }
}
