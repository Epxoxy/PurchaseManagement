using PurchaseManagement.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using PurchaseManagement.Models;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for StatsPage.xaml
    /// </summary>
    public partial class StatsPage : Page, Interfaces.ITablePrint
    {
        BindingList<string> StatsList = new BindingList<string>();
        Dictionary<string, string> statsStoreList = new Dictionary<string, string>();
        public StatsPage()
        {
            InitializeComponent();
            statsData.ItemsSource = StatsList;
            updateTableSource(MySqlConnector.Connector.AccessTable);
            tableComboBox.SelectionChanged += tableComboBoxSeletionChanged;
            statsBtn.Click += onStatsBtnClick;
            analysisComboBox.SelectionChanged += onColumnComboBoxSelectionChanged;
            conditionComboBox.SelectionChanged += onColumnComboBoxSelectionChanged;
        }

        private void onColumnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1) return;
            var pair = e.AddedItems[0] as Pair<string, ColumnAttribute>;
            IntervalBox iBox = sender == analysisComboBox ? analysisIBox : conditionIBox;
            if (ColumnSelections.hasSelection(pair.Value.Name))
            {
                iBox.UseComboBox = true;
                iBox.ItemsSource = ColumnSelections.getSelection(pair.Value.Name);
            }
            else
            {
                iBox.UseComboBox = false;
            }
        }

        #region Stats

        //Add/Remove Stats method
        private void addNewStats(string column, string stats)
        {
            string display = string.Format("{0}'s {1}", column, stats);
            if (statsStoreList.ContainsKey(display)) return;
            StatsList.Add(display);
            statsStoreList.Add(display, string.Format(",{0}({1}) as `{2}` ", stats, column, display));
        }
        private void removeStats(string item)
        {
            if (StatsList.Contains(item))
            {
                StatsList.Remove(item);
                statsStoreList.Remove(item);
            }
        }

        #endregion

        //Add new Stats to StatsList on addBtn click
        private void addBtnClick(object sender, RoutedEventArgs e)
        {
            if (statsColumnComboBox.SelectedValue == null) return;
             var columnAttribute = statsColumnComboBox.SelectedValue as ColumnAttribute;
            if (columnAttribute == null) return;

            string column = columnAttribute.Name;
            string stats = typeComboBox.SelectedValue.ToString();
            addNewStats(column, stats);
        }

        //Remove Stats from button of StatsItem is clicked
        private void onStatsItemBtnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if(btn != null && btn.Content != null)
            {
                removeStats(btn.Content.ToString());
            }
        }
        
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
                var columnSources = ModelHelper.getColumns(type);
                if(byTimeBtn.IsChecked == true)
                {
                    ColumnAttribute colAttribute;
                    if(ModelHelper.tableHasColumn(type, typeof(DateTime), out colAttribute))
                    {
                        var pair = new Pair<string, ColumnAttribute>(colAttribute.Name, colAttribute);
                        analysisComboBox.ItemsSource = new List<Pair<string, ColumnAttribute>>() { pair };
                        analysisComboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    analysisComboBox.ItemsSource = columnSources;
                }
                statsColumnComboBox.ItemsSource = columnSources;
                conditionComboBox.ItemsSource = columnSources;
                StatsList.Clear();
                statsStoreList.Clear();
                //Clear result DataGrid's columns
                if (dataGrid.ItemsSource != null)
                {
                    dataGrid.ItemsSource = null;
                    dataGrid.Columns.Clear();
                }
            }
            else
            {
                //Clear Columns
                analysisComboBox.ItemsSource = null;
                statsColumnComboBox.ItemsSource = null;
                conditionComboBox.ItemsSource = null;
                StatsList.Clear();
                statsStoreList.Clear();
            }
        }
        
        //Do statistics when StatsBtn click
        private void onStatsBtnClick(object sender, RoutedEventArgs e)
        {
            //Get selected table
            var tableType = tableComboBox.SelectedValue as Type;
            if (tableType == null) return;

            //Get table name
            string tableName = ModelHelper.getTableName(tableType);
            if (string.IsNullOrEmpty(tableName)) return;

            string columnName = string.Empty;
            ColumnAttribute columnAtti = null;
            if (byTimeBtn.IsChecked == true)
            {
                var columnPairs = ModelHelper.getColumns(tableType);
                foreach (var columnPair in columnPairs)
                {
                    if (columnPair.Value.Type == typeof(DateTime)){
                        columnName = columnPair.Value.Name;
                        columnAtti = columnPair.Value;
                        break;
                    }
                }
            }
            else
            {
                //Get selected column attribute
                var columnPair = analysisComboBox.SelectedItem as Pair<string, ColumnAttribute>;
                if (columnPair == null) return;
                columnAtti = columnPair.Value;
                columnName = columnPair.Value.Name;
            }
            if (string.IsNullOrEmpty(columnName)) return;

            //Get where condition
            string analyBoxString, condiBoxString = string.Empty, conditions;
            analysisIBox.getCondition(columnName, out analyBoxString);
            if(conditionComboBox.SelectedValue != null)
            {
                string conditionColumnName = (conditionComboBox.SelectedValue as ColumnAttribute).Name;
                conditionIBox.getCondition(conditionColumnName, out condiBoxString);
            }
            conditions = SQLScript.combineCondition(analyBoxString, condiBoxString);

            //Get stats condition
            var statsConditionBuilder = new StringBuilder();
            foreach(var item in statsStoreList)
            {
                statsConditionBuilder.Append(item.Value);
            }
            //Do query
            if (byTimeBtn.IsChecked == true)
            {
                var timeType = (SplitTimeType)timeTypeComboBox.SelectedItem;
                dataGrid.queryDBUpdateUse(SQLScript.formatStatsTimeGroup(timeType, tableName, columnName, conditions, statsConditionBuilder.ToString()));
            }
            else
            {
                dataGrid.queryDBUpdateUse(SQLScript.formatStatsGroup(tableName, columnName, conditions, statsConditionBuilder.ToString()));
            }
        }
        
        //Implement Interfaces.ITablePrint in order to print data
        public System.Data.DataTable GetPrintTable()
        {
            return ((System.Data.DataView)dataGrid.ItemsSource).Table;
        }

        //Reset input/select on resetBtn click
        private void resetBtnClick(object sender, RoutedEventArgs e)
        {
            tableComboBox.SelectedIndex = -1;
            analysisIBox.emptyValue();
            conditionIBox.emptyValue();
        }
        
        private void byTimeBtnClick(object sender, RoutedEventArgs e)
        {
            if (byTimeBtn.IsChecked == true)
            {
                updateTableSource(MySqlConnector.Connector.TimeSeriesTable);
            }
            else
            {
                updateTableSource(MySqlConnector.Connector.AccessTable);
            }
        }

        private void updateTableSource(List<Pair<string, Type>> source)
        {
            tableComboBox.ItemsSource = source;
            if (source == null) return;
            if (source.Count > 0 && source.Count < 2) tableComboBox.SelectedIndex = 0;
        }
    }
}
