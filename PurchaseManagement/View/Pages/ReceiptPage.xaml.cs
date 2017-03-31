using PurchaseManagement.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for ReceiptPage.xaml
    /// </summary>
    public partial class ReceiptPage : Page
    {
        private Models.Receipt Receipt { get; set;}
        public ReceiptPage()
        {
            InitializeComponent();
            if (Receipt == null)
            {
                Receipt = new Models.Receipt();
                Receipt.StaffID = MySqlConnector.Connector.StaffID;
            }
            receiptPanel.DataContext = Receipt;
        }
        public ReceiptPage(Models.Receipt receipt)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Receipt.OrderID < 1) Alert.show(Properties.Resources.CheckOrderIDTips);
            else
            {
                if (!MySqlConnector.doInsert(Receipt)) Alert.show(Properties.Resources.CheckInputMsg);
                else MySqlConnector.doUpdate(SQLScript.formatUpdate("purchaseorders", "PurchaseState = '" + Models.HandState.Completed + "'", "OrderID ='"+Receipt.OrderID+"'"));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            object selectedRow;
            var cmdString = SQLScript.formatSelect("purchaseorders", "*", "PurchaseState = '" + Models.HandState.Handing+"'");
            if ((new SearchWindow(cmdString)).getSelectedOne(out selectedRow) == true)
            {
                if (selectedRow != null)
                {
                    var rowView = selectedRow as System.Data.DataRowView;
                    if (rowView != null)
                    {
                        var idrow = rowView.Row["OrderID"].ToString();
                        var totalrow = rowView.Row["TotalPrice"].ToString();
                        int id;
                        double totalPrice;
                        if (int.TryParse(idrow, out id) && double.TryParse(totalrow, out totalPrice))
                        {
                            orderIDTBox.Text = idrow;
                            Receipt.OrderID = id;
                            realPayTBox.Text = totalrow;
                            Receipt.RealPay = totalPrice;
                        }
                    }
                }
            }
        }
    }
}
