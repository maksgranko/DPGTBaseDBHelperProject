using DPGTProject.Configs;
using Scraps.Configs;
using Scraps.Databases.Utilities;
using Scraps.Localization;
using Scraps.Security;
using MSSQL = Scraps.Databases.MSSQL;
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AuthForm());
        }

        private static void ConfigureScraps()
        {
            ScrapsConfig.DatabaseName = SystemConfig.databaseName;
            ScrapsConfig.ConnectionString = string.IsNullOrWhiteSpace(SystemConfig.connectionString)
                ? MSSQL.ConnectionStringBuilder(SystemConfig.databaseName)
                : SystemConfig.connectionString;

            ScrapsConfig.AuthHashPasswords = SystemConfig.authHashPasswords;

            ScrapsConfig.UseRoleIdMapping =
                SystemConfig.databaseGenerationMode >= DatabaseGenerationMode.Standard;
        }

        private static void EnsureDatabase()
        {
            if (string.IsNullOrWhiteSpace(SystemConfig.databaseName))
                throw new SystemException("Пропишите название базы данных в основе!\nЗаполните это в переменной databaseName класса SystemConfig.");

            var options = new DatabaseGenerationOptions
            {
                DatabaseName = SystemConfig.databaseName,
                Mode = SystemConfig.databaseGenerationMode,
                UsersTableName = ScrapsConfig.UsersTableName,
                UsersTableColumnsNames = new Dictionary<string, string>(ScrapsConfig.UsersTableColumnsNames),
                ApplyUsersMappingToScrapsConfig = true
            };

            MSSQL.GenerateIfNotExists(options);
            SystemConfig.connectionString = ScrapsConfig.ConnectionString;
        }

        private static void InitializeSecurity()
        {
            var roles = new List<Role>();
            foreach (var roleEntry in SystemConfig.RolePermissions)
            {
                var role = new Role { Name = roleEntry.Key };
                role.TablePermissions.AddRange(roleEntry.Value);
                roles.Add(role);
            }

            // Scraps 0.15: fallback-права по роли применяются самим RoleManager.
            RoleManager.Initialize(roles, SystemConfig.DefaultRolePermissions);
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
                    var schema = MSSQL.GetTableSchema(tableName);
                    foreach (var columnName in schema.Keys)
                    {
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
