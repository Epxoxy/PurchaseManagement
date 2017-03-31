using System;

namespace PurchaseManagement.View
{
    public class PageUrls
    {
        public const string search = "search";
        public const string newpurchaseorder = "newpurchaseorder";
        public const string neworderproject = "neworderproject";
        public const string newwarehousing = "newwarehousing";
        public const string newreceipt = "newreceipt";
        public const string newsupplier = "newsupplier";
        public const string newgoods = "newgoods";
        public const string statistics = "statistics";

        public static void regDefBlockPage()
        {
            NavigateCase.register(search, new Uri("View/Pages/Statistics/SearchPage.xaml", UriKind.Relative));
            NavigateCase.register(newpurchaseorder, new Uri("View/Pages/PurchaseOrderPage.xaml", UriKind.Relative));
            NavigateCase.register(neworderproject, new Uri("View/Pages/OrderprojectPage.xaml", UriKind.Relative));
            NavigateCase.register(newwarehousing, new Uri("View/Pages/WarehousingPage.xaml", UriKind.Relative));
            NavigateCase.register(newreceipt, new Uri("View/Pages/ReceiptPage.xaml", UriKind.Relative));
            NavigateCase.register(newsupplier, new Uri("View/Pages/SupplierPage.xaml", UriKind.Relative));
            NavigateCase.register(newgoods, new Uri("View/Pages/GoodsPage.xaml", UriKind.Relative));
            NavigateCase.register(statistics, new Uri("View/Pages/Statistics/StatsPage.xaml", UriKind.Relative));
        }
    }
}
