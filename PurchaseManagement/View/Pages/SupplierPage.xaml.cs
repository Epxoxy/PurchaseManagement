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
    /// Interaction logic for NewSupplierPage.xaml
    /// </summary>
    public partial class SupplierPage : Page
    {
        private Models.Supplier supplier;
        private Models.Supplier Supplier
        {
            get { if (supplier == null) supplier = new Models.Supplier();return supplier; }
            set { supplier = value; }
        }

        public SupplierPage()
        {
            InitializeComponent();
            SupplierPanel.DataContext = Supplier;
        }
        public SupplierPage(Models.Supplier supplier)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(!Extension.MySqlConnector.doInsert(Supplier)) Extension.Alert.show("Check input message");
        }
    }
}
