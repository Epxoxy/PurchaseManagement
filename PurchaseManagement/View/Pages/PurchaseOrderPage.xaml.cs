using PurchaseManagement.Extension;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for AddPage.xaml
    /// </summary>
    public partial class PurchaseOrderPage : Page
    {
        Models.PurchaseOrder PurchaseOrder { get; set; }
        public PurchaseOrderPage()
        {
            InitializeComponent();
            initPurchaseOrder();
        }

        private void initPurchaseOrder()
        {
            PurchaseOrder = new Models.PurchaseOrder();
            PurchaseOrder.StaffID = Extension.MySqlConnector.Connector.StaffID;
            orderRoot.DataContext = PurchaseOrder;
        }

        public PurchaseOrderPage(Models.PurchaseOrder purchaseOrder)
        {
        }
        
        private void addOrderBtnClick(object sender, RoutedEventArgs e)
        {
            if (PurchaseOrder.TotalPrice < 0) Alert.show(Properties.Resources.ProjEmptyTips);
            else
            {
                if (MySqlConnector.doInsert(PurchaseOrder))
                {
                    initPurchaseOrder();
                }
            }
        }

        //Search from datebase use the command string, and update the result to DataGrid
        public void searchDatabaseUse(DataGrid datagrid, string cmdString)
        {
            string tableName = SQLScript.getSingelEffectTable(cmdString);
        }

        private void searchProjBtnClick(object sender, RoutedEventArgs e)
        {
            object selectedRow;
            var groupString = "OrderProjectID as `"+Properties.Resources.OrderProjectID
                +"`,OrderID as `"+Properties.Resources.OrderID 
                +"`,count(GoodsID) as `"+Properties.Resources.Amount
                +"`,sum(Amount*UnitPrice) as `" +Properties.Resources.TotalPrices +"`";
            var cmdString = SQLScript.formatSelect("orderproject", groupString, " group by `"+Properties.Resources.OrderID + "`", false);
            if ((new SearchWindow(cmdString)).getSelectedOne(out selectedRow) == true)
            {
                if (selectedRow != null)
                {
                    var rowView = selectedRow as System.Data.DataRowView;
                    if (rowView != null)
                    {
                        var idrow = rowView.Row[1].ToString();
                        var totalRow = rowView.Row[3].ToString();
                        int id, totalPrice;
                        if (int.TryParse(idrow, out id) && int.TryParse(totalRow, out totalPrice))
                        {
                            PurchaseOrder.OrderID = id;
                            orderIDTBox.Text = idrow;
                            PurchaseOrder.TotalPrice = totalPrice;
                            totalPricesTBox.Text = totalRow;
                            
                            var queryString = SQLScript.formatSelect("orderproject", "*", "OrderID = '" + id + "'");
                            //Get OrderProject's data
                            var dataTable = new System.Data.DataTable();
                            MySqlConnector.doQuery(queryString, ref dataTable);
                            projDG.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
        }

        private void getSupplierBtnClick(object sender, RoutedEventArgs e)
        {
            object selectedRow;
            if ((new SearchWindow(SQLScript.formatSelect("supplier", "*"))).getSelectedOne(out selectedRow) == true)
            {
                if(selectedRow != null)
                {
                    var rowView = selectedRow as System.Data.DataRowView;
                    if(rowView != null)
                    {
                        var value = rowView.Row["SupplierID"].ToString();
                        int id;
                        if(int.TryParse(value, out id))
                        {
                            PurchaseOrder.SupplierID = id;
                            supplierTBox.Text = value;
                        }
                    }
                }
            }
        }
    }
}
