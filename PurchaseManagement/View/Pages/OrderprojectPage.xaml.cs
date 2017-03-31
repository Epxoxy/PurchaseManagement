using PurchaseManagement.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for NewOrderprojectPage.xaml
    /// </summary>
    public partial class OrderprojectPage : Page
    {
        BindingList<Models.OrderProject> orderProject = new BindingList<Models.OrderProject>();
        public int PurchaseID { get; set; }
        public OrderprojectPage()
        {
            InitializeComponent();
            dataGrid.ItemsSource = orderProject;
            rootGrid.DataContext = this;
        }
        public OrderprojectPage(Models.OrderProject orderProj)
        {
        }

        private void saveOrderProject()
        {
            if (Validation.GetHasError(orderidTb))
            {
                Alert.show(Properties.Resources.InputRightIDTips);
                return;
            }
            if (orderProject.Count < 1) return;
            int emptyAmountCount = 0;
            foreach (var item in orderProject)
            {
                if(item.Amount < 1)
                {
                    ++emptyAmountCount;
                }
                if (item.OrderID != PurchaseID) item.OrderID = PurchaseID;
            }
            if(emptyAmountCount > 0)
            {
                var tips = string.Format(Properties.Resources.EmptyTipsFormat, emptyAmountCount);
                var result = Alert.show(tips, System.Windows.Forms.MessageBoxButtons.OKCancel);
                if (result == System.Windows.Forms.DialogResult.Cancel) return;
            }
            MySqlConnector.doInsert(orderProject.Where(o => o.Amount > 0).ToArray());
        }

        private void saveBtnClick(object sender, RoutedEventArgs e)
        {
            saveOrderProject();
        }

        private void addBtnClick(object sender, RoutedEventArgs e)
        {
            dataGrid.CommitEdit();
            IList selectedItems;
            var searchWin = new SearchWindow(SQLScript.formatSelect("goods", "*"));
            if (searchWin.getSelection(out selectedItems) == true)
            {
                if (selectedItems.Count > 0)
                {
                    int[] addIndex = new int[2];
                    var table = (selectedItems[0] as System.Data.DataRowView).Row.Table;
                    for (int i = 0; i < table.Columns.Count; ++i)
                    {
                        if (table.Columns[i].ColumnName.Equals("GoodsID")) addIndex[0] = i;
                        if (table.Columns[i].ColumnName.Equals("UnitPrice")) addIndex[1] = i;
                    }
                    var list = orderProject.ToList();
                    foreach (System.Data.DataRowView row in selectedItems)
                    {
                        int goodsID, unitPrice;
                        if (int.TryParse(row[addIndex[0]].ToString(), out goodsID) && int.TryParse(row[addIndex[1]].ToString(), out unitPrice))
                        {
                            if (list.FindIndex(o => o.GoodsID == goodsID) < 0)
                                orderProject.Add(new Models.OrderProject() { GoodsID = goodsID, UnitPrice = unitPrice });
                        }
                    }
                }
            }
        }
    }
}
