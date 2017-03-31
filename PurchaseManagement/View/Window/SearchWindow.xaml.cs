using PurchaseManagement.Extension;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchWindow(string cmdString)
        {
            InitializeComponent();
            resultGrid.queryDBUpdateUse(cmdString);
            this.Owner = Application.Current.MainWindow;
            this.MaxHeight = Application.Current.MainWindow.ActualHeight;
            this.MaxWidth = Application.Current.MainWindow.ActualWidth;
        }

        public bool? getSelection(out IList selectedItems)
        {
            this.ShowDialog();
            selectedItems = resultGrid.SelectedItems;
            return DialogResult;
        }

        public bool? getSelectedOne(out object selectedItem)
        {
            this.ShowDialog();
            resultGrid.SelectionMode = DataGridSelectionMode.Single;
            selectedItem = resultGrid.SelectedItem;
            return DialogResult;
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
