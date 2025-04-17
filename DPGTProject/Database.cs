using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DPGTProject
{
    public static class Database
    {
        public static class Users
        {
            public static DataTable GetAll()
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", conn);
                    da.Fill(dt);
                }
                return dt;
            }
            public static DataRow Get(int userId)
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
                {
                    string query = "SELECT * FROM Users WHERE UserID = @UserID";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@UserID", userId);
                    da.Fill(dt);
                }
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
            public static DataRow GetByLogin(string login)
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
                {
                    string query = "SELECT * FROM Users WHERE Login = @Login";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@Login", login);
                    da.Fill(dt);
                }
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
            public static string GetUserStatus(string login)
            {
                var user = GetByLogin(login);
                return user?["Role"]?.ToString();
            }
            public static bool Create(string login, string password, string role)
            {
                using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
                {
                    string query = "INSERT INTO Users (Login, Password, Role) VALUES (@Login, @Password, @Role)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", role);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            public static bool Update(int userId, string login, string password, string role)
            {
                using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
                {
                    string query = "UPDATE Users SET Login = @Login, Password = @Password, Role = @Role WHERE UserID = @UserID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", role);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            public static bool Delete(int userId)
            {
                using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
                {
                    string query = "DELETE FROM Users WHERE UserID = @UserID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public static string ConnectionStringBuilder(string databaseName)
        {
            return $"Data Source={Environment.MachineName};Initial Catalog={databaseName};Integrated Security=True;Encrypt=False";
        }
        public static DataTable Translate(DataTable dt, string tableName)
        {
            if (dt == null)
                throw new ArgumentNullException(nameof(dt));

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            try
            {
                if (SystemConfig.ColumnTranslations.TryGetValue(tableName, out var translations))
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column == null || string.IsNullOrEmpty(column.ColumnName))
                            continue;

                        if (translations.TryGetValue(column.ColumnName, out var translatedName))
                        {
                            column.ColumnName = translatedName;
                        }
                    }
                }
                return dt;
            }
            catch
            {
                return dt;
            }
        }
        public static DataTable Untranslate(DataTable dt, string tableName)
        {
            if (dt == null)
                throw new ArgumentNullException(nameof(dt));

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            try
            {
                if (SystemConfig.ColumnTranslations.TryGetValue(tableName, out var translations))
                {
                    var reverseTranslations = translations.ToDictionary(x => x.Value, x => x.Key);
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column == null || string.IsNullOrEmpty(column.ColumnName))
                            continue;

                        if (reverseTranslations.TryGetValue(column.ColumnName, out var originalName))
                        {
                            column.ColumnName = originalName;
                        }
                    }
                }
                return dt;
            }
            catch
            {
                return dt;
            }
        }
        public static DataTable GetByAttribute(string tableName, string attributeName, object attributeValue, bool useLike = true)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
            {
                string operatorStr = useLike ? "LIKE" : "=";
                string valueStr = useLike ? $"%{attributeValue}%" : attributeValue.ToString();

                string query = $"SELECT * FROM [{tableName}] WHERE [{attributeName}] {operatorStr} @Value";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                if (attributeValue == null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Value", DBNull.Value);
                }
                else
                {
                    da.SelectCommand.Parameters.AddWithValue("@Value", useLike ? valueStr : attributeValue);
                }

                da.Fill(dt);
            }
            return dt;
        }
        public static int BulkUpdate(string tableName, DataTable data)
        {
            using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM {tableName}", conn);
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.UpdateCommand = cb.GetUpdateCommand();
                da.InsertCommand = cb.GetInsertCommand();
                da.DeleteCommand = cb.GetDeleteCommand();

                conn.Open();
                return da.Update(data);
            }
        }
        public static int ExecuteNonQuery(string query)
        {
            using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        public static object ExecuteScalar(string query)
        {
            using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
        public static bool CheckConnection()
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionStringBuilder("master")))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static SqlDataReader ExecuteReader(string query)
        {
            SqlConnection conn = new SqlConnection(SystemConfig.connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        internal static DataTable GetAll(string tableName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM {tableName}", conn);
                da.Fill(dt);
            }
            return dt;
        }
        public static string[] GetTableColumns(string tableName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
            {
                string query = @"
                    SELECT COLUMN_NAME 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = @TableName
                    ORDER BY ORDINAL_POSITION";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                da.Fill(dt);
            }
            return dt.Rows.Cast<DataRow>().Select(r => r[0].ToString()).ToArray();
        }
        public static Dictionary<string, string> GetTableSchema(string tableName)
        {
            var schema = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
            {
                string query = @"
                    SELECT 
                        COLUMN_NAME, 
                        DATA_TYPE 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = @TableName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        schema[reader["COLUMN_NAME"].ToString()] =
                            reader["DATA_TYPE"].ToString();
                    }
                }
            }
            return schema;
        }
        public static int ImportData(string tableName, DataTable data)
        {
            using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
            {
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = tableName;

                    try
                    {
                        // Сопоставление столбцов
                        foreach (DataColumn column in data.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }

                        bulkCopy.WriteToServer(data);
                        return data.Rows.Count;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка импорта в таблицу {tableName}: {ex.Message}");
                    }
                }
            }
        }
        public static bool GetDataTableFromSQL(string SQLRequest, out DataTable Dt)
        {
            Dt = null;
            try
            {
                Dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
                {
                    new SqlDataAdapter(SQLRequest, conn).Fill(Dt);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DataTable GetDataTableFromSQL(string SQLRequest)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(SystemConfig.connectionString))
                {
                    new SqlDataAdapter(SQLRequest, conn).Fill(dt);
                }
                return dt;
            }
            catch
            {
                return null;
            }
        }

        public static void FillDataGridViewFromSQL(DataGridView dgv, string SQLRequest)
        {
            try
            {
                dgv.DataSource = GetDataTableFromSQL(SQLRequest);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка заполнения DataGridView: {ex.Message}");
            }
        }

        internal static void PreCheck()
        {
            if (!Database.CheckConnection())
            {
                throw new Exception("Ошибка подключения к базе данных!");
            }

            if (string.IsNullOrEmpty(SystemConfig.databaseName))
                throw new SystemException("Пропишите название базы данных в основе!\n " +
                    "Заполните это в переменной databaseName класса SystemConfig.\n" +
                    "!!!Код автоматически создаст необходимую таблицу Users для работы с пользователями и базу данных!!!\n" +
                    "P.S.: Таблица будет в формате: Название Users, колонки: UserID, Role и др.\n" +
                    "Старайтесь соблюдать синтаксис в БД!");

            // Проверка существования БД
            using (var conn = new SqlConnection(ConnectionStringBuilder("master")))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{SystemConfig.databaseName}') " +
                    $"CREATE DATABASE {SystemConfig.databaseName}", conn);
                cmd.ExecuteNonQuery();
            }

            // Проверка таблицы Users
            using (var conn = new SqlConnection(ConnectionStringBuilder(SystemConfig.databaseName)))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users') " +
                    "BEGIN " +
                    "CREATE TABLE Users (" +
                    "UserID int IDENTITY(1,1) PRIMARY KEY, " +
                    "Login nvarchar(64) NOT NULL, " +
                    "Password nvarchar(64) NOT NULL, " +
                    "Role nvarchar(32) NOT NULL); " +
                    "CREATE INDEX IX_Users_Login ON Users(Login); " +
                    "END", conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
