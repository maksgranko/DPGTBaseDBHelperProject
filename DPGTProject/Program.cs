using DPGTProject.Configs;
using Scraps.Configs;
using Scraps.Database;
using Scraps.Localization;
using Scraps.Security;
using MSSQL = Scraps.Database.MSSQL.MSSQL;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DPGTProject
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Test.Init();
            ConfigureScraps();
            EnsureDatabase();
            SystemConfig.Initialize();
            SyncTranslations();
            InitializeSecurity();
            RegisterVirtualTables();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AuthForm());
        }

        private static void ConfigureScraps()
        {
            if (SystemConfig.databaseProvider != DatabaseProvider.MSSQL)
                throw new NotSupportedException($"Провайдер '{SystemConfig.databaseProvider}' в этом проекте сейчас не поддержан. Используйте MSSQL.");

            ScrapsConfig.DatabaseProvider = SystemConfig.databaseProvider;
            ScrapsConfig.DatabaseName = SystemConfig.databaseName;
            ScrapsConfig.ConnectionString = string.IsNullOrWhiteSpace(SystemConfig.connectionString)
                ? MSSQL.ConnectionStringBuilder(SystemConfig.databaseName)
                : SystemConfig.connectionString;

            ScrapsConfig.AuthHashPasswords = SystemConfig.authHashPasswords;

            ScrapsConfig.UseRoleIdMapping =
                SystemConfig.databaseGenerationMode >= DatabaseGenerationMode.Standard;

            DatabaseProviderFactory.Reset();
        }

        private static void EnsureDatabase()
        {
            if (string.IsNullOrWhiteSpace(SystemConfig.databaseName))
                throw new SystemException("Пропишите название базы данных в основе!\nЗаполните это в переменной databaseName класса SystemConfig.");

            var options = new DatabaseGenerationOptions
            {
                DatabaseName = SystemConfig.databaseName,
                Mode = SystemConfig.databaseGenerationMode,
                SeedRoles = SystemConfig.roles,
                UsersTableName = ScrapsConfig.UsersTableName,
                UsersTableColumnsNames = new Dictionary<string, string>(ScrapsConfig.UsersTableColumnsNames),
                ApplyUsersMappingToScrapsConfig = true
            };

            MSSQL.GenerateIfNotExists(options);
            SystemConfig.connectionString = ScrapsConfig.ConnectionString;
        }

        private static void InitializeSecurity()
        {
            // Ролевая модель берется из SystemConfig и читается локальным резолвером прав.
            // Для Standard/Full режима роли предварительно создаются через SeedRoles в EnsureDatabase().
        }

        private static void RegisterVirtualTables()
        {
            Current.VirtualTables.Clear();

            foreach (var pair in SystemConfig.VirtualTableQueries ?? new Dictionary<string, string>())
            {
                if (string.IsNullOrWhiteSpace(pair.Key) || string.IsNullOrWhiteSpace(pair.Value))
                    continue;

                Current.VirtualTables.Register(pair.Key, pair.Value);
            }
        }

        private static void SyncTranslations()
        {
            var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in SystemConfig.Translations ?? new Dictionary<string, string>())
            {
                if (string.IsNullOrWhiteSpace(pair.Key)) continue;
                merged[pair.Key] = pair.Value ?? string.Empty;
            }

            foreach (var tableName in SystemConfig.tables ?? Array.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(tableName)) continue;

                try
                {
                    var schema = Current.GetTableSchema(tableName);
                    foreach (System.Data.DataRow row in schema.Rows)
                    {
                        var columnName = row["ColumnName"]?.ToString();
                        if (string.IsNullOrWhiteSpace(columnName))
                            continue;

                        if (SystemConfig.Translations.TryGetValue(columnName, out var translated))
                            merged[TranslationManager.ColumnKey(tableName, columnName)] = translated;
                    }
                }
                catch
                {
                    // Таблица могла быть виртуальной или недоступной в текущем контексте.
                }
            }

            TranslationManager.Replace(merged);
        }
    }
}
