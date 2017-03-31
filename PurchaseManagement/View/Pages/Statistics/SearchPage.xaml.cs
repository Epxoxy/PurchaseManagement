using PurchaseManagement.Extension;
using System;
using System.Windows;
using System.Windows.Controls;
using PurchaseManagement.Models;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page, Interfaces.ITransitive, Interfaces.ITablePrint
    {
        //Cache the command string everytime do search
        private string CommandCache { get; set; }

        #region Contructor

        public SearchPage()
        {
            InitializeComponent();
            changedSearchBarState(Visibility.Visible);
            searchBtn.Click += onSearchBtnClick;
            columnComboBox.SelectionChanged += onColumnComboBoxSelectionChanged;
        }

        private void onColumnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1) return;
            var pair = e.AddedItems[0] as Pair<string, ColumnAttribute>;
            if (ColumnSelections.hasSelection(pair.Value.Name))
            {
                intervalBox.UseComboBox = true;
                intervalBox.ItemsSource = ColumnSelections.getSelection(pair.Value.Name);
            }
            else
            {
                intervalBox.UseComboBox = false;
            }
        }

        #endregion

        #region Database Operation

        //Search from datebase use the command string, and update the result to DataGrid
        public void callQuery(DataGrid datagrid, string cmdString)
        {
            if (CommandCache != cmdString) CommandCache = cmdString;
            datagrid.queryDBUpdateUse(cmdString);
        }

        //Method of save changes to datebase and refresh the DataGrid's columns
        private int saveChangeAndRefresh(DataGrid datagrid, string cmdString)
        {
            if (string.IsNullOrEmpty(cmdString)) return 0;

            var datatable = ((System.Data.DataView)datagrid.ItemsSource).Table;
            if (datatable != null)
            {
                //Update change to database
                // MySqlConnector.doUpdate(cmdString, ref datatable);
                MySqlConnector.doUpdate(CommandCache,ref datatable);
                //Select newest result
                var dataTable = new System.Data.DataTable();
                int count = MySqlConnector.doQuery(cmdString, ref dataTable);
                //Refresh DataGrid.
                datagrid.ItemsSource = null;
                datagrid.ItemsSource = dataTable.DefaultView;
                return count;
            }
            return 0;
        }

        #endregion

        #region Implement interfaces

        //Implement ITransitive
        public void Transmit(object obj)
        {
            if (obj != null)
            {
                var cmdString = obj as string;
                if (string.IsNullOrEmpty(cmdString))
                {
                    changedSearchBarState(Visibility.Visible);
                }
                else
                {
                    changedSearchBarState(Visibility.Collapsed);
                    callQuery(resultGrid, cmdString);
                }
            }
        }

        public object Submit()
        {
            return null;
        }

        //Implement IPrintable
        public System.Data.DataTable GetPrintTable()
        {
            return ((System.Data.DataView)resultGrid.ItemsSource).Table;
        }


        #endregion
        
        //Table ComboBox SelectionChanged Event Handler
        //When selection changed, udpate the available columns of columnsComboBox
        private void tableComboBoxSeletionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tableComboBox.SelectedValue != null)
            {
                //Check selected value
                var type = tableComboBox.SelectedValue as Type;
                if (type == null) return;
                //Get Columns of current selected value(Table)
                columnComboBox.ItemsSource = ModelHelper.getColumns(type);
                //Clear result DataGrid's columns
                if (resultGrid.ItemsSource != null)
                {
                    resultGrid.ItemsSource = null;
                    resultGrid.Columns.Clear();
                }
            }
            else
            {
                //Clear Columns
                columnComboBox.ItemsSource = null;
            }
        }
        
        /// <summary>
        /// Update SearchBar State
        /// </summary>
        /// <param name="visibility">New Visibility</param>
        private void changedSearchBarState(Visibility visibility)
        {
            if (searchBar.Visibility != visibility) searchBar.Visibility = visibility;
            if (visibility == Visibility.Visible)
            {
                rootHeader.Header = Properties.Resources.Search;
                var source = MySqlConnector.Connector.AccessTable;
                tableComboBox.ItemsSource = source;
                tableComboBox.SelectionChanged += tableComboBoxSeletionChanged;
                if (source != null && source.Count > 0 && source.Count < 2)
                    tableComboBox.SelectedIndex = 0;
            }
            else
            {
                rootHeader.Header = Properties.Resources.Detail;
                tableComboBox.ItemsSource = null;
                tableComboBox.SelectionChanged -= tableComboBoxSeletionChanged;
            }
        }

        #region Button Click Event Handler

        //Reset Button Click Event Handler
        //When it happen, Clear all value of search condition
        private void resetBtnClick(object sender, RoutedEventArgs e)
        {
            tableComboBox.SelectedIndex = -1;
            intervalBox.emptyValue();
        }

        //Search Button Click Event Handler
        //Combine search command string, and call search method, then update result
        private void onSearchBtnClick(object sender, RoutedEventArgs e)
        {
            //Get and check selected value
            var type = tableComboBox.SelectedValue as Type;
            if (type == null ) return;

            //Get table name
            string tableName = ModelHelper.getTableName(type);
            if (string.IsNullOrEmpty(tableName)) return;

            //Get selected column attribute
            var columnAttribute = columnComboBox.SelectedItem as Pair<string, ColumnAttribute>;
            if (columnAttribute == null) return;
            string columnName = columnAttribute.Value.Name;

            //Get start/end value of search
            string condition = string.Empty;
            intervalBox.getCondition(columnName, out condition);
            callQuery(resultGrid, SQLScript.formatSelect(tableName, "*", condition));
        }

        //Save Button Click Event Handler
        //Get changes and udpate it to datebase, and udpate result
        private void onSaveBtnClick(object sender, RoutedEventArgs e)
        {
            resultGrid.CommitEdit();
            saveChangeAndRefresh(resultGrid, CommandCache);
        }

        #endregion
    }
}
