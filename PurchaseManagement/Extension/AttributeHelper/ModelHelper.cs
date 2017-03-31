using PurchaseManagement.Models;
using System;
using System.Collections.Generic;

namespace PurchaseManagement.Extension
{
    public class ModelHelper
    {
        #region Table Helper

        public static bool tableHasColumn(Type tableType, Type columnType, out ColumnAttribute columnAttribute)
        {
            var columnPairs = getColumns(tableType);
            foreach (var columnPair in columnPairs)
            {
                if (columnPair.Value.Type == columnType)
                {
                    columnAttribute = columnPair.Value;
                    return true;
                }
            }
            columnAttribute = null;
            return false;
        }
        
        public static string getTableName(Type type)
        {
            return ((TableAttribute)type.GetCustomAttributeValue<TableAttribute>()).Name;
        }
        public static string getTableLocalName(Type type)
        {
            var tableAttribute = ((TableAttribute)type.GetCustomAttributeValue<TableAttribute>());
            if (Properties.Settings.Default.UsingLang.Equals("zh-CN"))
            {
                return tableAttribute.LocalName;
            }
            return tableAttribute.Name;
        }

        public static Type getTableType(Tables table)
        {
            switch (table)
            {
                case Tables.PurchaseOrder: { return typeof(PurchaseOrder); }
                case Tables.OrderProject: { return typeof(OrderProject); }
                case Tables.Receipt: { return typeof(Receipt); }
                case Tables.WareHousing: { return typeof(WareHousing); }
                case Tables.Supplier: { return typeof(Supplier); }
                case Tables.Goods: { return typeof(Goods); }
                case Tables.Staff: { return typeof(Staff); }
                default: return null;
            }
        }

        #endregion

        #region Column Helper

        public static List<Pair<string, ColumnAttribute>> getColumns(Type type)
        {
            //Get and check table name
            string tableName = ModelHelper.getTableName(type);
            if (string.IsNullOrEmpty(tableName)) return null;
            //Create if not exist
            bool useLocal = Properties.Settings.Default.UsingLang.Equals("zh-CN");
            if (!tableColumns.ContainsKey(tableName))
            {
                var props = type.GetProperties();
                if (props.Length > 0)
                {
                    List<Pair<string, ColumnAttribute>> columnsCache = new List<Pair<string, ColumnAttribute>>();
                    for (int i = 0; i < props.Length; ++i)
                    {
                        var columnAttribute = ModelHelper.getColumnAttribute(type, props[i].Name);
                        if (columnAttribute != null)
                        {
                            if(useLocal)
                                columnsCache.Add(new Pair<string, ColumnAttribute>(columnAttribute.LocalName, columnAttribute));
                            else
                                columnsCache.Add(new Pair<string, ColumnAttribute>(columnAttribute.Name, columnAttribute));
                        }
                    }
                    tableColumns.Add(tableName, columnsCache);
                }
                else
                {
                    tableColumns.Add(tableName, null);
                }
            }
            return tableColumns[tableName];
        }

        public static ColumnAttribute getColumnAttribute(Type type, string property)
        {
            if (string.IsNullOrEmpty(property))
                return ((ColumnAttribute)type.GetCustomAttributeValue<ColumnAttribute>());
            return ((ColumnAttribute)type.GetCustomAttributeValue<ColumnAttribute>(property));
        }

        public static string getColumnName(Type type, string property = "")
        {
            if (string.IsNullOrEmpty(property))
                return ((ColumnAttribute)type.GetCustomAttributeValue<ColumnAttribute>()).Name;
            return ((ColumnAttribute)type.GetCustomAttributeValue<ColumnAttribute>(property)).Name;
        }

        public static bool isPrimaryKey(Type type, string property = "")
        {
            if (string.IsNullOrEmpty(property))
                return ((ColumnAttribute)type.GetCustomAttributeValue<ColumnAttribute>()).IsPrimaryKey;
            return ((ColumnAttribute)type.GetCustomAttributeValue<ColumnAttribute>(property)).IsPrimaryKey;
        }
        
        private static void initReadOnly(Type type)
        {
            var tableName = ModelHelper.getTableName(type);
            if (string.IsNullOrEmpty(tableName)) return;
            var attributes = ModelHelper.getColumns(type);
            if (attributes == null) return;
            List<string> readonlyList = new List<string>();
            foreach (var attribute in attributes)
            {
                if (attribute.Value.ReadOnly)
                {
                    if (!readonlyList.Contains(attribute.Value.Name))
                        readonlyList.Add(attribute.Value.Name);
                }
            }
            if (readonlyDictionary.ContainsKey(tableName))
                readonlyDictionary.Add(tableName, readonlyList);
            else readonlyDictionary[tableName] = readonlyList;
        }

        public static bool isReadOnly(string table, string column)
        {
            if (ReadonlyDictionary.ContainsKey(table)) return ReadonlyDictionary[table].Contains(column);
            return false;
        }

        private static Dictionary<string, List<string>> readonlyDictionary;
        public static Dictionary<string, List<string>> ReadonlyDictionary
        {
            get
            {
                if (readonlyDictionary == null)
                {
                    readonlyDictionary = new Dictionary<string, List<string>>();
                    initReadOnly(typeof(Goods));
                    initReadOnly(typeof(PurchaseOrder));
                    initReadOnly(typeof(OrderProject));
                    initReadOnly(typeof(Receipt));
                    initReadOnly(typeof(Supplier));
                    initReadOnly(typeof(Staff));
                    initReadOnly(typeof(WareHousing));
                }
                return readonlyDictionary;
            }
        }
        private static Dictionary<string, List<Pair<string, ColumnAttribute>>> tableColumns = new Dictionary<string, List<Pair<string, ColumnAttribute>>>();
        #endregion
    }
}
