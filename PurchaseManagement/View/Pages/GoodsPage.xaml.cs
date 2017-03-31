using System;
using System.Windows.Controls;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for NewGoodsPage.xaml
    /// </summary>
    public partial class GoodsPage : Page, Interfaces.ITransitive
    {
        private Models.Goods Goods { get; set; }
        public GoodsPage()
        {
            InitializeComponent();
            Goods = new Models.Goods();
            rootHeader.DataContext = Goods;
        }
        public GoodsPage(Models.Goods goods)
        {
            Goods = goods;
        }

        public void Transmit(object obj)
        {
            var goods = obj as Models.Goods;
            if (goods != null)
            {
                Goods = goods;
                rootHeader.DataContext = Goods;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!Extension.MySqlConnector.doInsert(Goods)) Extension.Alert.show("Check input message");
        }

        public object Submit()
        {
            throw new NotImplementedException();
        }
    }
}
