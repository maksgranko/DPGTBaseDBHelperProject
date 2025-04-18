using DPGTProject.Configs;
using System.Collections.Generic;
using System.Drawing;

namespace DPGTProject
{
    public static partial class SystemConfig
    {
        public static string databaseName = "";
        public static string connectionString = Database.ConnectionStringBuilder(databaseName); // !!! ПОМЕНЯТЬ ОСНОВУ СТРОКИ МОЖНО В Database.cs МЕТОД: ConnectionStringBuilder !!!
        public static string lastError = "";

        public static string[] tables = new string[] { };                                       // Пример заполнения: tables = new string[] { "Documents", "DocumentHistory", "Fines", "Owners", "Violations" };
        public static string[] roles = new string[] { "Администратор", "Менеджер" };            // Здесь прописываются роли!
        public static string[] removeFromTableWhenStart = new string[] { "Users" };
        public static bool tableAutodetect = true;
        public static bool enableFilter = true;                                                 // Включить фильтр в универсальной форме
        public static bool enableSearch = true;                                                 // Включить поиск в универсальной форме
        public static bool openEveryWindowInNew = false;                                        // Открывать новые окна в каждом новом
        public static string[] removeFromTableWhenAutodetect = new string[] { };

        public static Point LastLocation = new Point(400, 300);
        public static DesignConfig.ApplicationTheme applicationTheme = DesignConfig.ApplicationTheme.Blue;

        public static Dictionary<string, Dictionary<string, string>> ColumnTranslations = new Dictionary<string, Dictionary<string, string>>()
        {
            ["Owners"] = new Dictionary<string, string>()
            {
                ["Sample"] = "Пример_поля_таблицы",
                ["Sample1"] = "Пример_поля_таблицы1",
                ["Sample2"] = "Пример_поля_таблицы2",
                ["Sample3"] = "Пример_поля_таблицы3"
            }
        };
        public static Dictionary<string, string> TableTranslations = new Dictionary<string, string>()
        {
            ["Sample"] = "Пример_названий_в_combobox1",
            ["Sample1"] = "Пример_названий_в_combobox2",
            ["Sample2"] = "Пример_названий_в_combobox3",
            ["Sample3"] = "Пример_названий_в_combobox4",
            ["Users"] = "Пользователи" // Строку не трогать, она нужна для перевода
        };
    }
}
