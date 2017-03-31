using PurchaseManagement.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PurchaseManagement.Models;

namespace PurchaseManagement.View
{
    public class NavModuleBuilder
    {
        private static NavModuleBuilder builder;
        private static NavModuleBuilder Builder
        {
            get
            {
                if (builder == null)
                    builder = new NavModuleBuilder();
                return builder;
            }
        }

        private NavModuleBuilder()
        {

        }

        #region All Build Menu

        private NavMenuItem[] purchaseBuild(string groupName, bool viewOnly)
        {
            var allPurchase = new NavMenuItem(Properties.Resources.All, PageUrls.search, SQLScript.formatSelect("purchaseorders", "*"));
            var pendingPurchase = new NavMenuItem(Properties.Resources.Pending, PageUrls.search, SQLScript.formatSelect("purchaseorders ", "*", "PurchaseState = '" + HandState.Pending + "'"));
            var handingPurchase = new NavMenuItem(Properties.Resources.Handing, PageUrls.search, SQLScript.formatSelect("purchaseorders", "*", "PurchaseState = '" + HandState.Handing + "'"));
            var completePurchase = new NavMenuItem(Properties.Resources.Completed, PageUrls.search, SQLScript.formatSelect("purchaseorders ", "*", "PurchaseState = '" + HandState.Completed + "'"));
            var cancelPurchase = new NavMenuItem(Properties.Resources.Canceled, PageUrls.search, SQLScript.formatSelect("purchaseorders ", "*", "PurchaseState = '" + HandState.Canc + "'"));
            var newPurchase = new NavMenuItem(Properties.Resources.New, PageUrls.newpurchaseorder);
            //Define function list
            if (viewOnly) return groupNavMenuItem(groupName, new NavMenuItem[] { allPurchase, pendingPurchase, handingPurchase, completePurchase, cancelPurchase });
            else return groupNavMenuItem(groupName, new NavMenuItem[] { newPurchase, allPurchase, pendingPurchase, handingPurchase, completePurchase, cancelPurchase });
        }

        private NavMenuItem[] orderProjectBuild(string groupName, bool viewOnly)
        {
            var viewOrderProject = new NavMenuItem(Properties.Resources.View, PageUrls.search, SQLScript.formatSelect("orderproject", "*"));
            var newOrderProject = new NavMenuItem(Properties.Resources.New, PageUrls.neworderproject);
            //Define function list
            if(viewOnly) return groupNavMenuItem(groupName, new NavMenuItem[] { viewOrderProject });
            else return groupNavMenuItem(groupName, new NavMenuItem[] { newOrderProject, viewOrderProject });
        }

        private NavMenuItem[] warehousingBuild(string groupName, bool viewOnly)
        {
            var allWareHousing = new NavMenuItem(Properties.Resources.All, PageUrls.search, SQLScript.formatSelect("warehousing", "*"));
            var pendingWareHousing = new NavMenuItem(Properties.Resources.Pending, PageUrls.search, SQLScript.formatSelect("warehousing", "*", "WarehousingState = '"+ HandState.Pending +"'"));
            var handingWareHousing = new NavMenuItem(Properties.Resources.Handing, PageUrls.search, SQLScript.formatSelect("warehousing", "*", "WarehousingState = '" + HandState.Handing + "'"));
            var completedWareHousing = new NavMenuItem(Properties.Resources.Completed, PageUrls.search, SQLScript.formatSelect("warehousing", "*", "WarehousingState = '" + HandState.Completed + "'"));
            var cancdWareHousing = new NavMenuItem(Properties.Resources.Canceled, PageUrls.search, SQLScript.formatSelect("warehousing", "*", "WarehousingState = '" + HandState.Canc + "'"));
            var newWareHousing = new NavMenuItem(Properties.Resources.New, PageUrls.newwarehousing);
            //Define function list
            if (viewOnly) return groupNavMenuItem(groupName, new NavMenuItem[] { allWareHousing, pendingWareHousing, handingWareHousing, completedWareHousing, cancdWareHousing });
            else return groupNavMenuItem(groupName, new NavMenuItem[] { newWareHousing, allWareHousing, pendingWareHousing, handingWareHousing, completedWareHousing, cancdWareHousing });
        }

        private NavMenuItem[] receiptBuild(string groupName, bool viewOnly)
        {
            var allReceipt = new NavMenuItem(Properties.Resources.All, PageUrls.search, SQLScript.formatSelect("receipt", "*"));
            var newReceipt = new NavMenuItem(Properties.Resources.New, PageUrls.newreceipt);
            //Define function list
            if (viewOnly) return groupNavMenuItem(groupName, new NavMenuItem[] { allReceipt });
            else return groupNavMenuItem(groupName, new NavMenuItem[] { newReceipt, allReceipt });
        }

        private NavMenuItem[] supplierBuild(string groupName, bool viewOnly)
        {
            var allSupplier = new NavMenuItem(Properties.Resources.All, PageUrls.search, SQLScript.formatSelect("supplier", "*"));
            var newSupplier = new NavMenuItem(Properties.Resources.New, PageUrls.newsupplier);
            //Define function list
            if (viewOnly) return groupNavMenuItem(groupName, new NavMenuItem[] { allSupplier });
            else return groupNavMenuItem(groupName, new NavMenuItem[] { newSupplier, allSupplier });
        }

        private NavMenuItem[] goodsBuild(string groupName, bool viewOnly)
        {
            var allGoods = new NavMenuItem(Properties.Resources.All, PageUrls.search, SQLScript.formatSelect("goods", "*"));
            var newGoods = new NavMenuItem(Properties.Resources.New, PageUrls.newgoods);
            //Define function list
            if (viewOnly) return groupNavMenuItem(groupName, new NavMenuItem[] { allGoods });
            else return groupNavMenuItem(groupName, new NavMenuItem[] { newGoods, allGoods });
        }

        #endregion

        public static void generateNavCmdItems(out List<NavMenus> navMenus)
        {
            navMenus = null;
            if (MySqlConnector.Connector.CurrentRole == MySqlConnector.Role.Debug) return;
            PageUrls.regDefBlockPage();

            navMenus = new List<NavMenus>();
            string purchaseGroup = "purchaseList", infoGroup = "infoList";
            long viewAuth = (long)Auth.View;
            foreach (var item in MySqlConnector.Connector.AccessList)
            {
                bool isViewOnly = item.AuthCode == viewAuth;
                switch (item.Table)
                {
                    case Tables.PurchaseOrder: { navMenus.Add(new NavMenus(Builder.purchaseBuild(Properties.Resources.PurchaseOrder, isViewOnly), purchaseGroup)); } break;
                    case Tables.OrderProject: { navMenus.Add(new NavMenus(Builder.orderProjectBuild(Properties.Resources.OrderProject, isViewOnly), purchaseGroup)); } break;
                    case Tables.Receipt: { navMenus.Add(new NavMenus(Builder.receiptBuild(Properties.Resources.Receipt, isViewOnly), purchaseGroup)); } break;
                    case Tables.WareHousing: { navMenus.Add(new NavMenus(Builder.warehousingBuild(Properties.Resources.WareHousing, isViewOnly), purchaseGroup)); } break;
                    case Tables.Supplier: { navMenus.Add(new NavMenus(Builder.supplierBuild(Properties.Resources.Supplier, isViewOnly), infoGroup)); } break;
                    case Tables.Goods: { navMenus.Add(new NavMenus(Builder.goodsBuild(Properties.Resources.Goods, isViewOnly), infoGroup)); } break;
                    default: break;
                }
            }
        }

        private static ICollectionView arrayToICollectionView(object source)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(source);
            view.GroupDescriptions.Add(new PropertyGroupDescription() { Converter = Converters.GroupConverter });
            return view;
        }

        private static NavMenuItem[] groupNavMenuItem(string groupName, NavMenuItem[] navMenuItems)
        {
            for(int i = 0; i < navMenuItems.Length; ++i)
            {
                navMenuItems[i].GroupName = groupName;
            }
            return navMenuItems;
        }
    }
}
