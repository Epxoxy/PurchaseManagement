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
    /// Interaction logic for NewWarehousingPage.xaml
    /// </summary>
    public partial class WarehousingPage : Page
    {
        Models.WareHousing WareHousing { get; set; }
        public WarehousingPage()
        {
            InitializeComponent();
            WareHousing = new Models.WareHousing();
            WareHousing.StaffID = Extension.MySqlConnector.Connector.StaffID;
            contentRoot.DataContext = WareHousing;
        }
        public WarehousingPage(Models.WareHousing wareHousing)
        {
            WareHousing = wareHousing;
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WareHousing.ReceiptID < 1) Alert.show(Properties.Resources.CheckInputMsg);
            else MySqlConnector.doInsert(WareHousing);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object selectedRow;
            var cmdString = SQLScript.formatSelect("receipt", "*");
            if ((new SearchWindow(cmdString)).getSelectedOne(out selectedRow) == true)
            {
                if (selectedRow != null)
                {
                    var rowView = selectedRow as System.Data.DataRowView;
                    if (rowView != null)
                    {
                        var idrow = rowView.Row["ReceiptID"].ToString();
                        int id;
                        if (int.TryParse(idrow, out id))
                        {
                            receiptIDTBox.Text = idrow;
                            WareHousing.ReceiptID = id;
                        }
                    }
                }
            }
        }
    }
}
