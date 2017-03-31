using PurchaseManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PurchaseManagement.Extension
{
    public static class DataTableExtension
    {
        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> enumerable, int chunkSize)
        {
            int itemsReturned = 0;
            var list = enumerable.ToList(); // Prevent multiple execution of IEnumerable.
            int count = list.Count;
            while (itemsReturned < count)
            {
                int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
                yield return list.GetRange(itemsReturned, currentChunkSize);
                itemsReturned += currentChunkSize;
            }
        }

        //Get columnTemplate by System.Date.DataColumn
        public static DataGridColumn getColumnTemplate(string tableName, System.Data.DataColumn dataColumn)
        {
            var columnName = dataColumn.ColumnName;
            var binding = new System.Windows.Data.Binding(columnName);

            if (ColumnSelections.hasSelection(columnName))
            {
                return new DataGridComboBoxColumn()
                {
                    Header = columnName,
                    ItemsSource = ColumnSelections.getSelection(columnName),
                    DisplayMemberPath = "Display",
                    SelectedValuePath = "Value",
                    SelectedValueBinding = binding,
                    EditingElementStyle = App.Current.FindResource("DataGridComboBoxColumnEditingElementStyle") as Style
                };

            }
            else
            {
                return new DataGridTextColumn()
                {
                    Header = columnName,
                    Binding = binding,
                    IsReadOnly = ModelHelper.isReadOnly(tableName, columnName)
                };
            }
        }


        //Search from datebase use the command string, and update the result to DataGrid
        public static void queryDBUpdateUse(this DataGrid datagrid, string cmdString)
        {
            if (datagrid.AutoGenerateColumns) datagrid.AutoGenerateColumns = false;
            var dataTable = new System.Data.DataTable();
            MySqlConnector.doQuery(cmdString, ref dataTable);
            datagrid.Columns.Clear();
            string tableName = dataTable.TableName;
            for (int i = 0; i < dataTable.Columns.Count; ++i)
            {
                datagrid.Columns.Add(getColumnTemplate(tableName, dataTable.Columns[i]));
            }
            datagrid.ItemsSource = dataTable.DefaultView;
        }
    }
}
