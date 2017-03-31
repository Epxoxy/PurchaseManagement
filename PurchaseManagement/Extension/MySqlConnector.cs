using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using PurchaseManagement.Models;

namespace PurchaseManagement.Extension
{
    public class AccessTable
    {
        public string Display { get; set; }
        public Type Type { get; set; }
        public long AuthCode { get; set; }
        public Tables Table { get; set; }

        public AccessTable(Tables table, Auth auth) : this(table, (long)auth) { }
        public AccessTable(Tables table, long authCode) : this(string.Empty, table, authCode) { }
        public AccessTable(string display, Tables table, long authCode)
        {
            Table = table;
            Type = ModelHelper.getTableType(table);
            AuthCode = authCode;
            if (string.IsNullOrEmpty(display)) Display = ModelHelper.getTableLocalName(Type);
            else Display = display;
        }
    }

    public enum Auth : long
    {
        View = 2,
        Update = 4,
        Delete = 8,
        All = 14
    }
    
    /// <summary>
    /// MySql Database connector
    /// </summary>
    public class MySqlConnector
    {
        #region Connector

        private static MySqlConnector connector = null;
        private static readonly object lockHelpr = new object();
        public static MySqlConnector Connector
        {
            get
            {
                lock (lockHelpr)
                {
                    if (connector == null) connector = new MySqlConnector();
                }
                return connector;
            }
        }

        #endregion

        #region Privileges

        public enum Privileges
        {
            None = 0,
            Usage = 2,
            Select = 4,
            Insert = 8,
            Update = 16,
            Delete = 32,
            Create = 64,
            Drop = 128,
            References = 256,
            Index = 512,
            Alter= 1024,
            CreateView = 2048,
            ShowView = 4096,
            Trigger = 8192
        }

        public class Grant
        {
            private string grantString;
            //Properity
            public string DBName { get; set; }
            public string Table { get; set; }
            public long Privileges { get; private set; }
            public string GrantString
            {
                get { return grantString; }
                set { grantString = value; initByGrantString(); }
            }
            //Const
            private const string regexGrant = "GRANT (.+?) ON (.+?)\\.(.+?) TO";
            private const string regexWord = "[A-Za-z]+";
            private static List<string> allPrivileges = new List<string>()
            {
                "NONE", "USAGE", "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE",
                "DROP", "REFERENCES", "INDEX", "ALTER", "CREATEVIEW", "SHOWVIEW", "TRIGGER"
            };
            //Constructor
            public Grant() { }
            public Grant(string grantString) { this.GrantString = grantString; }

            /// <summary>
            /// Initilize 'Table'&'Privileges' by grant string value
            /// </summary>
            /// <param name="grantString">Grant string value</param>
            /// <returns>If operation is successful</returns>
            private bool initByGrantString()
            {
                if (string.IsNullOrEmpty(grantString)) return false;
                var regex = new Regex(regexGrant);
                var matches = regex.Matches(grantString);
                for (int i = 0; i < matches.Count; ++i)
                {
                    var group = matches[i].Groups;
                    string privilegesString = group[1].Value;
                    string databaseName = group[2].Value;
                    string tableName = group[3].Value;
                    System.Diagnostics.Debug.WriteLine("Table : " + databaseName + ", privileges" + privilegesString);

                    if (databaseName == "**" && group[1].Value == "ALL PRIVILEGES")
                    {
                        //Current user = system manager
                        //Operate table = mysql.user
                    }
                    this.DBName = databaseName;
                    this.Table = tableName;
                    List<long> privileges;
                    if(regexPrivileges(privilegesString, out privileges))
                    {
                        this.Privileges = BinAuth.GenAuthCode(privileges.ToArray());
                        System.Diagnostics.Debug.WriteLine("Privileges, " + Privileges);
                    }
                }
                return true;
            }

            /// <summary>
            /// Regex for privileges
            /// </summary>
            /// <param name="inputString">Input string</param>
            /// <param name="permissionArray">Result string array</param>
            /// <returns>If operation is successful</returns>
            private bool regexPrivileges(string inputString, out List<long> privileges)
            {
                privileges = null;
                if (string.IsNullOrEmpty(inputString))
                {
                    return false;
                }
                inputString = inputString.Replace(" ", "").ToUpper();
                if (inputString == "ALLPRIVILEGES")
                {
                    //All permission on table
                    privileges = new List<long> { 0, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192};
                    return true;
                }
                else
                {
                    //Other
                    var wordRegex = new Regex(regexWord);
                    var wordMatches = wordRegex.Matches(inputString);
                    if (wordMatches.Count < 1) return false;

                    privileges = new List<long>();
                    for (int i = 0; i < wordMatches.Count; ++i)
                    {
                        int index = allPrivileges.IndexOf(wordMatches[i].Groups[0].Value);
                        System.Diagnostics.Debug.WriteLine(index);
                        if (index > 0) privileges.Add((long)Math.Pow(2, index));
                    }
                    return true;
                }
            }

            /// <summary>
            /// Get if has privilege
            /// </summary>
            /// <param name="privilege"></param>
            /// <returns></returns>
            public bool hasPrivilege(Privileges privilege)
            {
                return BinAuth.IsHasAuth(Privileges, (long)privilege);
            }
            public bool hasPrivileges(Privileges[] privileges)
            {
                foreach(var privilege in privileges)
                {
                    if(!BinAuth.IsHasAuth(Privileges, (long)privilege))
                    {
                        return false;
                    }
                }
                return true;
            }

            public override string ToString()
            {
                if(GrantString != null)
                {
                    return GrantString;
                }
                return base.ToString();
            }
        }

        private static bool checkGrant(out List<Grant> grantList)
        {
            grantList = null;
            var grantListCache = new List<Grant>();
            using (MySqlConnection connection = getMySqlConnection())
            {
                connection.Open();
                MySqlCommand cmdSel = new MySqlCommand(SQLScript.showCurrentGrants, connection);
                var reader = cmdSel.ExecuteReader();
                //Read result to list
                while (reader.Read())
                {
                    grantListCache.Add(new Grant(reader.GetString(0).Replace("`", "")));
                }
            }
            if (grantListCache.Count > 0)
            {
                grantList = grantListCache;
                return true;
            }
            return false;
        }
        #endregion

        #region Role

        public enum Role
        {
            Debug = 0,
            Purchaser = 1,
            Consignee = 2,
            WareHouseOperator = 3,
            EntryClerk = 4,
            Manager = 5
        }

        #endregion

        #region Login/Logout

        /// <summary>
        /// Login to database use account and password
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool loginWith(string account, string password)
        {
            setAccount(account, password);
            string errorMsg = string.Empty;

            try
            {
                checkAccountInfo();
                if(Connector.CurrentRole == Role.Debug)
                {
                    Alert.show("Debug login");
                }
                #region
                /*
                connection = MySqlConnector.getMySqlConnection();
                connection.Open();
                MySqlCommand verifyUserCmd = new MySqlCommand(countUserString, connection);
                int accountCount = int.Parse(verifyUserCmd.ExecuteScalar().ToString());
                System.Diagnostics.Debug.WriteLine(accountCount);
                */
                #endregion
            }
            catch (MySqlException ex)
            {
                errorMsg = handException(ref ex);
                return false;
            }
            if (string.IsNullOrEmpty(errorMsg)) return true;
            else { errorMsg = "Get RoleID fail, please check your permission."; return false; }

            #region
            /*
            using (MySqlConnection connection = new MySqlConnection(verifyUserString))
            {
                MySqlCommand verifyUserCmd = new MySqlCommand(verifyUserString, connection);
                connection.Open();
                Get select result
                using (var reader = verifyUserCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        value = reader.GetString(2).ToString();
                    }
                }
                if (string.IsNullOrEmpty(value))
                {
                    verifyUserCmd = new MySqlCommand(countUserString, connection);
                    int accountCount = int.Parse(verifyUserCmd.ExecuteScalar().ToString());
                    if (accountCount > 0) MessageBox.Show("Wrong password!");
                    else MessageBox.Show("Account not exits!");
                }
                Dispose connection
                verifyUserCmd.Dispose();
                connection.Close();
            }
            */
            #endregion
        }
        
        /// <summary>
        /// Logout and clean connection pools
        /// </summary>
        public static void logout()
        {
            MySqlConnection.ClearAllPools();
            Connector.connectString = string.Empty;
            connector = null;
        }

        #endregion
        
        #region Init Access Data
        
        private static void initAccessTable(Role role, out List<AccessTable> accessList, out  List<Pair<string, Type>> accessTable)
        {
            accessList = new List<AccessTable>();
            switch (role)
            {
                case Role.Purchaser: {
                        accessList.Add(new AccessTable(Tables.PurchaseOrder, Auth.All));
                        accessList.Add(new AccessTable(Tables.OrderProject, Auth.All));
                        accessList.Add(new AccessTable(Tables.Supplier, Auth.View));
                        accessList.Add(new AccessTable(Tables.Goods, Auth.View));
                    } break;
                case Role.Consignee:
                    {
                        accessList.Add(new AccessTable(Tables.Receipt, Auth.All));
                        accessList.Add(new AccessTable(Tables.Supplier, Auth.View));
                        accessList.Add(new AccessTable(Tables.Goods, Auth.View));
                    } break;
                case Role.WareHouseOperator:
                    {
                        accessList.Add(new AccessTable(Tables.WareHousing, Auth.All));
                        accessList.Add(new AccessTable(Tables.Supplier, Auth.View));
                        accessList.Add(new AccessTable(Tables.Goods, Auth.View));
                        ;
                    } break;
                case Role.EntryClerk:
                    {
                        accessList.Add(new AccessTable(Tables.Supplier, Auth.All));
                        accessList.Add(new AccessTable(Tables.Goods, Auth.All));
                    } break;
                case Role.Manager:
                    {
                        accessList.Add(new AccessTable(Tables.PurchaseOrder, Auth.All));
                        accessList.Add(new AccessTable(Tables.OrderProject, Auth.All));
                        accessList.Add(new AccessTable(Tables.Receipt, Auth.All));
                        accessList.Add(new AccessTable(Tables.WareHousing, Auth.All));
                        accessList.Add(new AccessTable(Tables.Supplier, Auth.All));
                        accessList.Add(new AccessTable(Tables.Goods, Auth.All));
                    } break;
                default:break;
            }
            accessTable = new List<Pair<string, Type>>();
            foreach (var access in accessList)
            {
                accessTable.Add(new Pair<string, Type>(access.Display, access.Type));
            }
        }

        #endregion

        #region Account Operation

        private static void checkAccountInfo()
        {
            string role = string.Empty, staffID = string.Empty, userName = string.Empty;
            using (MySqlConnection connection = getMySqlConnection())
            {
                connection.Open();
                string cmdString = string.Format(SQLScript.getRoleStaffIDFormat, connectDBName, Connector.userId);
                MySqlCommand cmdSel = new MySqlCommand(cmdString, connection);
                var reader = cmdSel.ExecuteReader();
                //Read result to list
                while (reader.Read())
                {
                    staffID = reader.GetString(0);
                    userName = reader.GetString(1);
                    role = reader.GetString(2);
                }
            }

            Connector.StaffID = int.Parse(staffID);
            Connector.UserName = userName;

            if (!string.IsNullOrEmpty(role))
            {
                int roleID;
                if (int.TryParse(role, out roleID) && Enum.IsDefined(typeof(Role), roleID))
                {
                    Connector.CurrentRole = (Role)roleID;
                    List<AccessTable> accessList;
                    List<Pair<string, Type>> accessTable;
                    initAccessTable(Connector.CurrentRole, out accessList, out accessTable);
                    Connector.AccessList = accessList;
                    Connector.AccessTable = accessTable;
                }
            }
        }

        /// <summary>
        /// Set default login account of database
        /// </summary>
        /// <param name="account"></param>
        /// <param name="_password"></param>
        public static void setAccount(string account, string _password)
        {
            Connector.userId = account;
            Connector.password = _password;
            Connector.connectString = string.Format(SQLScript.connectFormat, server, Connector.userId, Connector.password, connectDBName);
        }

        public static bool checkAndUpdatePsw(string oldPsw, string newPsw)
        {
            if(oldPsw == Connector.password && oldPsw != newPsw)
            {
                try
                {
                    using (MySqlConnection connection = getMySqlConnection())
                    {
                        connection.Open();
                        var command = new MySqlCommand(string.Format(SQLScript.updatePassword, newPsw), connection);
                        command.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    handException(ref ex);
                    return false;
                }
                return true;
            }
            return false;
        }
        
        #endregion
        
        #region Get Connection

        /// <summary>
        /// Get MySqlConnection
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection getMySqlConnection()
        {
            if (string.IsNullOrEmpty(Connector.connectString)) throw new ArgumentException("ConnectString is not set");
            if (string.IsNullOrEmpty(Connector.userId)) throw new ArgumentException("User id is null or empty");
            MySqlConnection connect = new MySqlConnection(Connector.connectString);
            return connect;
        }

        #endregion

        #region SqlOperation Extension

        public static bool doInsert(IList<Interfaces.ISqlModel> objs)
        {
            if (objs == null) return false;
            if (objs.Count < 0) return false;
            //Get tableName
            string tableName = ModelHelper.getTableName(objs[0].GetType());
            if (string.IsNullOrEmpty(tableName)) return false;

            var builder = new System.Text.StringBuilder();
            foreach (var obj in objs)
            {
                //Get insert columns and values
                Dictionary<string, string> result;
                if (!obj.ToScript(out result)) break;

                //Format insert string
                var valueBulider = new System.Text.StringBuilder();
                var columnBulider = new System.Text.StringBuilder();
                int count = 0;
                foreach (var pair in result)
                {
                    if (count != 0)
                    {
                        columnBulider.Append(SQLScript.comma);
                        valueBulider.Append(SQLScript.comma);
                    }
                    columnBulider.Append(pair.Key);
                    valueBulider.Append(SQLScript.quote);
                    valueBulider.Append(pair.Value);
                    valueBulider.Append(SQLScript.quote);
                    ++count;
                }
                builder.Append(SQLScript.formatInsert(tableName, columnBulider.ToString(), valueBulider.ToString()));
            }
            doInsert(builder.ToString());
            return true;
        }
        public static bool doInsert(Interfaces.ISqlModel obj)
        {
            //Get tableName
            string tableName = ModelHelper.getTableName(obj.GetType());
            if (string.IsNullOrEmpty(tableName)) return false;

            //Get insert columns and values
            Dictionary<string, string> result;
            if (!obj.ToScript(out result)) return false;

            //Format insert string
            var valueBulider = new System.Text.StringBuilder();
            var columnBulider = new System.Text.StringBuilder();
            int count = 0;
            foreach (var pair in result)
            {
                if (count != 0)
                {
                    columnBulider.Append(SQLScript.comma);
                    valueBulider.Append(SQLScript.comma);
                }
                columnBulider.Append(pair.Key);
                valueBulider.Append(SQLScript.quote);
                valueBulider.Append(pair.Value);
                valueBulider.Append(SQLScript.quote);
                ++count;
            }
            string insertString = SQLScript.formatInsert(tableName, columnBulider.ToString(), valueBulider.ToString());
            //Execute insert command
            return doInsert(insertString);
        }
        public static bool doInsert(string insertString)
        {
            if (string.IsNullOrEmpty(insertString)) return false;
            //Execute MySqlCommand
            try
            {
                using (MySqlConnection connection = getMySqlConnection())
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(insertString, connection);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                Alert.show(Properties.Resources.InsertSuccessful);
                return true;
            }
            catch (MySqlException ex)
            {
                handException(ref ex);
                return false;
            }
        }

        public static void doDelete(IList<Interfaces.ISqlModel> objs, ref DataTable table)
        {
            if (objs == null) return;
            if (objs.Count < 0) return;
            //Get table name
            string tableName = ModelHelper.getTableName(objs[0].GetType());
            if (string.IsNullOrEmpty(tableName)) return;
            var builder =  new System.Text.StringBuilder();
            foreach(var obj in objs)
            {
                //Get primary key and its value
                var type = obj.GetType();
                var properties = type.GetProperties();
                string conditions = string.Empty;//Store codition
                foreach (var property in properties)
                {
                    var columnAttribute = ModelHelper.getColumnAttribute(type, property.Name);
                    if (columnAttribute.IsPrimaryKey)
                    {
                        string columnName = columnAttribute.Name;
                        conditions = columnName + " = " + property.GetValue(obj);
                        break;
                    }
                }

                //Check codition and format command string
                if (string.IsNullOrEmpty(conditions)) break;
                builder.Append(SQLScript.formatDelete(tableName, conditions));
            }
            //Execute delete command
            var cmdString = builder.ToString();
            if (string.IsNullOrEmpty(cmdString)) return;
            doDelete(cmdString, ref table);
        }
        public static void doDelete(Interfaces.ISqlModel obj, ref DataTable table)
        {
            //Get table name
            string tableName = ModelHelper.getTableName(obj.GetType());
            if (string.IsNullOrEmpty(tableName)) return;

            //Get primary key and its value
            var type = obj.GetType();
            var properties = type.GetProperties();
            string conditions = string.Empty;//Store codition
            foreach (var property in properties)
            {
                var columnAttribute = ModelHelper.getColumnAttribute(type, property.Name);
                if (columnAttribute.IsPrimaryKey)
                {
                    string columnName = columnAttribute.Name;
                    conditions = columnName + " = " + property.GetValue(obj);
                    break;
                }
            }

            //Check codition and format command string
            if (string.IsNullOrEmpty(conditions)) return;
            string deletecmdString = SQLScript.formatDelete(tableName, conditions);
            System.Diagnostics.Debug.WriteLine(deletecmdString);

            //Execute delete command
            doDelete(deletecmdString, ref table);
        }
        public static void doDelete(string deletecmdString, ref DataTable table)
        {
            if (string.IsNullOrEmpty(deletecmdString)) return;
            //Execute delete command
            try
            {
                using (MySqlConnection connection = getMySqlConnection())
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    adapter.DeleteCommand = new MySqlCommand(deletecmdString, connection);
                    MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
                    adapter.Update(table);
                    builder.Dispose();
                    adapter.Dispose();
                    Alert.show(Properties.Resources.UpdateSuccessful);
                }
            }
            catch (MySqlException ex)
            {
                //Show catch exception message
                handException(ref ex);
            }
        }
        
        public static int doQuery(string sqlString, ref DataTable table)
        {
            if (string.IsNullOrEmpty(sqlString)) return 0;
            int count = 0;
            try
            {
                using (MySqlConnection connection = getMySqlConnection())
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    adapter.SelectCommand = new MySqlCommand(sqlString, connection);
                    MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
                    count = adapter.Fill(table);
                    builder.Dispose();
                    adapter.Dispose();
                }
            }
            catch (MySqlException ex)
            {
                handException(ref ex);
            }
            return count;
        }
        public static int doQuery(string sqlString, ref MySqlDataReader reader)
        {
            if (string.IsNullOrEmpty(sqlString)) return 0;
            int count = 0;
            try
            {
                using (MySqlConnection connection = getMySqlConnection())
                {
                    connection.Open();
                    MySqlCommand cmdSel = new MySqlCommand(sqlString, connection);
                    reader = cmdSel.ExecuteReader();
                }
            }
            catch (MySqlException ex)
            {
                handException(ref ex);
            }
            return count;
        }
        public static string doQuery(string sqlString)
        {
            string value = string.Empty;
            if (string.IsNullOrEmpty(sqlString)) return value;
            try
            {
                using (MySqlConnection connection = getMySqlConnection())
                {
                    connection.Open();
                    MySqlCommand cmdSel = new MySqlCommand(sqlString, connection);
                    var reader = cmdSel.ExecuteReader();
                    //Read result to list
                    while (reader.Read())
                    {
                        value = reader.GetString(0);
                    }
                    reader.Dispose();
                }
            }
            catch (MySqlException ex)
            {
                handException(ref ex);
            }
            catch(System.Data.SqlTypes.SqlNullValueException nullEx)
            {
                value = string.Empty;
            }
            return value;
        }

        public static void doUpdate(string sqlString, ref DataTable table)
        {
            if (string.IsNullOrEmpty(sqlString)) return;
            try
            {
                using (MySqlConnection connection = getMySqlConnection())
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    adapter.SelectCommand = new MySqlCommand(sqlString, connection);
                    MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

                    var changes = table.GetChanges();
                    if (changes != null)
                    {
                        //My custom method to build command string with table
                        //For class test only, not be use now in application.
                        //System.Diagnostics.Debug.WriteLine(SQLScript.buildCommandString(ref table));
                        adapter.Update(changes);
                        table.AcceptChanges();
                        Alert.show(Properties.Resources.UpdateSuccessful);
                    }
                    builder.Dispose();
                    adapter.Dispose();
                }
            }
            catch(MySqlException ex)
            {
                handException(ref ex);
            }
        }
        public static int doUpdate(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return 0;
            int count = 0;
            try
            {
                using (MySqlConnection connection = getMySqlConnection())
                {
                    connection.Open();
                    MySqlCommand cmdSel = new MySqlCommand(sqlString, connection);
                    count = cmdSel.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                handException(ref ex);
            }
            return count;
        }

        #endregion
        
        //Not be use now
        public static bool roleRedistribution(Role newRole, string loginID)
        {
            string role = string.Empty;
            using (MySqlConnection connection = getMySqlConnection())
            {
                connection.Open();
                string cmdString = string.Format(SQLScript.getRoleStaffIDFormat, connectDBName, loginID);
                MySqlCommand cmdSel = new MySqlCommand(cmdString, connection);
                var reader = cmdSel.ExecuteReader();
                //Read result to list
                while (reader.Read())
                {
                    role = reader.GetString(2);
                }
                var oldRole = (Role)int.Parse(role);
                if (oldRole == newRole) return false;
                string revokeCmdString = string.Format(SQLScript.revokeAllGrantsFormat, loginID, server);
                List<AccessTable> accessList;
                List<Pair<string, Type>> accessTable;
                initAccessTable(newRole, out accessList, out accessTable);
                foreach(var item in accessList)
                {
                    var auth = (Auth)item.AuthCode;
                }
            }
            return false;
        }

        //MySql Exception Handler
        private static string handException(ref MySqlException ex)
        {
            var innerEx = ex.InnerException as MySqlException;
            var handleEx = innerEx == null ? ex : innerEx;
            if (handleEx != null)
            {
                string errorMsg;
                switch (handleEx.Number)
                {
                    case 0: { errorMsg = Properties.Resources.Error0Tips; } break;
                    case 1046: { errorMsg = Properties.Resources.Error1046Tips; } break;
                    case 1045: { errorMsg = Properties.Resources.Error1045Tips; } break;
                    default: { errorMsg = handleEx.Message; } break;
                }
                Alert.show(errorMsg);
                return errorMsg;
            }
            return string.Empty;
        }

        private static string server = "localhost";
        private string connectString, userId, password;

        public const string defaultserver = "localhost";
        public static string connectDBName = "purchasemanagement";

        private List<Pair<string, Type>> accessTable;
        private List<Pair<string, Type>> timeSeriesTable;

        public List<Pair<string, Type>> AccessTable
        {
            get {
                return accessTable;
            }
            private set
            {
                accessTable = value;
            }
        }
        public List<Pair<string, Type>> TimeSeriesTable
        {
            get
            {
                if (timeSeriesTable == null)
                {
                    timeSeriesTable = new List<Pair<string, Type>>();
                    if (AccessTable != null)
                    {
                        ColumnAttribute colAttribute;
                        var dateTimeType = typeof(DateTime);
                        foreach (var table in AccessTable)
                        {
                            if(ModelHelper.tableHasColumn(table.Value,dateTimeType, out colAttribute))
                            {
                                timeSeriesTable.Add(table);
                            }
                        }
                    }
                }
                return timeSeriesTable;
            }
        }
        public List<AccessTable> AccessList { get; private set; }

        public int StaffID { get; private set; }
        public string UserName { get; private set; }
        public string UserID { get { return userId; } }
        public string RoleName { get { return CurrentRole.ToString(); } }
        public Role CurrentRole { get; private set; } = Role.Debug;
        
        #region Recycling

        /*
        public static void addAccount(string account, string password, string host = defaultserver)
        {
            using (MySqlConnection connection = getMySqlConnection())
            {
                connection.Open();
                string cmdString = string.Format(SQLScript.addUserFormat, account, password, host);
                MySqlCommand cmdSel = new MySqlCommand(cmdString, connection);
                var reader = cmdSel.ExecuteReader();
            }
        }
        
        public static void deleteAccount(string account, string host = defaultserver)
        {
            using (MySqlConnection connection = getMySqlConnection())
            {
                connection.Open();
                string cmdString = string.Format(SQLScript.deleteUser, account, host);
                MySqlCommand cmdSel = new MySqlCommand(cmdString, connection);
                var reader = cmdSel.ExecuteReader();
            }
        }
        */

        #endregion
    }
}
