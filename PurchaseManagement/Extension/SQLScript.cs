using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Extension
{
    public enum StatisticType
    {
        Count,
        Avg,
        Sum,
    }
    public enum SplitTimeType
    {
        Day,
        Week,
        Month,
        Year
    }
    public class SQLScript
    {
        public const string fullTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public const string quote = "'";
        public const string comma = ",";
        public const string connectFormat = "server={0};user id={1};password={2};database={3};convert zero datetime=True;";//serverIP, userID, password,databaseName
        public const string updatePassword = "set password = password('{0}');";
        public const string getRoleStaffIDFormat = "select StaffID, Name, RoleID from {0}.staff where LoginID = '{1}';";
        public const string showCurrentGrants = "show grants;";
        public const string revokeAllGrantsFormat = "revoke all privileges, grant option from '{0}'@'{1}';";

        //CRUD Script
        public const string selectFromat = "select {0} from {1}.{2} {3};";
        public const string insertFromat = "insert into {0}.{1} ({2}) values({3});";
        public const string updateFromat = "update {0}.{1} set {2} where {3};";
        public const string deleteAllFromat = "delete * from {0}.{1};";
        public const string deleteFromat = "delete from {0}.{1} where {2};";

        //Statistics
        //Format all statistics
        public const string statsFormat = "select {0}({1}) from {2}.{3} {4};";
        //By Time
        public const string selectTodayFormat = "select * from {0}.{1} where date({2}) = date(now());";
        public const string selectThisWeekFormat = "select * from {0}.{1} where week({2}) = week(now());";
        public const string selectThisMonthFormat = "select * from {0}.{1} where month({2}) = month(curdate())and year({2})=year(curdate());";
        public const string selectThisYearFormat = "select * from {0}.{1} where dayofyear({2}) = dayofyear(now());";

        //Format case..when.. sentences
        public const string caseWhenFormat = "{0}(case when {1}='{2}' then 1 else 0 end ) as {2},";
        //Format for statistics by split time
        public const string statsByTimeFormat = "select DATE_FORMAT({0},'{1}') as `{0} by {2}` {3} from {4}.{5} {6} group by `{0} by {2}`;";
        public const string statsGroupFormat = "select {0} {1} from {2}.{3} {4} group by {0};";

        private static string connectDBName { get; } = MySqlConnector.connectDBName;


        #region Format

        #region Basic Format

        public static string formatDelete(string table, string conditions = "")
        {
            if (conditions == "*")
            {
                return string.Format(deleteAllFromat, connectDBName, table);
            }
            return string.Format(deleteFromat, connectDBName, table, conditions);
        }

        public static string formatSelect(string table, string columns, string conditions = "", bool autoWhere = true)
        {
            if (string.IsNullOrEmpty(conditions) || autoWhere == false)
            {
                return string.Format(selectFromat, columns, connectDBName, table, conditions);
            }
            return string.Format(selectFromat, columns, connectDBName, table, " where " + conditions);
        }

        public static string formatInsert(string table, string columns, string values)
        {
            return string.Format(insertFromat, connectDBName, table, columns, values);
        }

        public static string formatUpdate(string table, string columnsAndValues, string conditions)
        {
            return string.Format(updateFromat, connectDBName, table, columnsAndValues, conditions);
        }
       
        #endregion

        #region Stats Format

        public static string formatStatsSearch(StatisticType statisticsType, string talble, string column, string conditions = "")
        {
            string statsKeyword = statisticsType == StatisticType.Count ? "count" : (statisticsType == StatisticType.Sum ? "sum" : "avg");
            if (string.IsNullOrEmpty(conditions))
                return string.Format(statsFormat, statsKeyword,  column, connectDBName, talble, conditions);
            return string.Format(statsFormat, statsKeyword, column, connectDBName, talble, " where " + conditions);
        }
        
        public static string formatCaseWhen(System.Collections.IEnumerable value, string statisticsType, string column)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in value)
            {
                stringBuilder.Append(string.Format(caseWhenFormat, statisticsType, column, item));
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 1);
            return stringBuilder.ToString();
        }

        public static string formatStatsGroup(string table, string column, string condition, string statsCondition)
        {
            return string.Format(statsGroupFormat, column, statsCondition, connectDBName, table, condition);
        }

        public static string formatStatsTimeGroup(SplitTimeType splitType, string table, string column,string condition, string statsCondition)
        {
            string timeFormat = string.Empty;
            switch (splitType)
            {
                case SplitTimeType.Day: timeFormat = "%Y-%m-%d";break;
                case SplitTimeType.Week: timeFormat = "%Y-%u"; break;
                case SplitTimeType.Month: timeFormat = "%Y-%m"; break;
                case SplitTimeType.Year: timeFormat = "%Y"; break;
            }
            return string.Format(statsByTimeFormat, column, timeFormat, splitType.ToString(), statsCondition, connectDBName, table, condition);
        }

        #endregion

        #endregion

        public static string combineCondition(params string[] conditions)
        {
            var builder = new StringBuilder();
            bool hasAppend = false;
            foreach(var condition in conditions)
            {
                if (string.IsNullOrEmpty(condition)) continue;
                if (hasAppend) builder.Append(" and ");

                builder.Append(condition);
                if (!hasAppend) hasAppend = true;
            }
            string conditionString = builder.ToString();
            if (string.IsNullOrEmpty(conditionString)) return string.Empty;
            return string.Format(" where {0} ", conditionString);
        }

        private static System.Text.RegularExpressions.Regex tableRegex = new System.Text.RegularExpressions.Regex("from.+?\\.(.+?) ");
        public static string getSingelEffectTable(string cmdString)
        {
            var math = tableRegex.Match(cmdString);
            return math.Groups[1].Value;
        }

        //Build command string with dataTable which may has changes
        public static string buildCommandString(ref DataTable sourceTable)
        {
            string tableName = sourceTable.TableName;
            if (sourceTable.PrimaryKey.Length < 1) return string.Empty;

            var cmdbuilder = new StringBuilder();
            //Get table name && primarykey && column's name
            string[] columnNames = new string[sourceTable.Columns.Count];
            string primaryKey = sourceTable.PrimaryKey[0].ColumnName;
            int primaryKeyIndex = 0;
            for (int i = 0; i < sourceTable.Columns.Count; ++i)
            {
                if (sourceTable.Columns[i] == sourceTable.PrimaryKey[0]) primaryKeyIndex = i;
                columnNames[i] = sourceTable.Columns[i].ColumnName;
            }

            //Get delete command string
            var deletes = sourceTable.GetChanges(DataRowState.Deleted);
            if (deletes != null )
            {
                for (int i = 0; i < deletes.Rows.Count; ++i)
                {
                    cmdbuilder.Append(formatDelete(tableName, primaryKey + " = " + deletes.Rows[i][primaryKeyIndex, DataRowVersion.Original]));
                }
            }

            //Get update command string
            var modifieds = sourceTable.GetChanges(DataRowState.Modified);
            if (modifieds != null)
            {
                string columnValueFormat = ", {0} = '{1}'";
                for (int i = 0; i < modifieds.Rows.Count; ++i)
                {
                    var row = modifieds.Rows[i];
                    var columnValueBuilder = new StringBuilder();
                    for (int j = 0; j < modifieds.Columns.Count; ++j)
                    {
                        var obj = row[j, DataRowVersion.Current];
                        if (!obj.Equals(row[j, DataRowVersion.Original]))
                        {
                            columnValueBuilder.Append(string.Format(columnValueFormat, columnNames[j], obj));
                        }
                    }
                    var sentents = columnValueBuilder.ToString();
                    if (!string.IsNullOrEmpty(sentents))
                    {
                        sentents = sentents.Remove(0, 1);
                        cmdbuilder.Append(formatUpdate(tableName, sentents, primaryKey + " = '" + row[0, DataRowVersion.Original] + "'"));
                    }
                }
            }
            return cmdbuilder.ToString();
        }

        /*
        //getPriv = "select * from mysql.tables_priv where user='{0}'";//userName
        //addUserFormat = "create user '{0}'@'{1}' identified by '{2}';";//userName, hostName, password
        //addUserFormat2> insert into user (host,user,password) values ('{0}','{1}',password('{2}'));
        //deleteUser = "delete from mysql.user where user='{0}' and host='{1}';";
        //grantFormat = "grant {0} on {1}.{2} to '{3}'@'{4}' identified by '{5}'";//permisson, databaseName, tableName, userName, hostName, password(//mysql.tables_priv)
        //revokeUser = "revoke {0} on {1}.{2} from '{3}'@'{4}';";
        //updatePasswordFormat2> set password for '{0}'@'{1}' = password('{2}')
        //updatePasswordFormat3 = "update mysql.user set password=password('{0}') where user=\"{1}\" and host=\"{2}\";";

        */
    }
}
