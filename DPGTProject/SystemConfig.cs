using DPGTProject.Configs;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DPGTProject
{
    public static partial class SystemConfig
    {
        #region --- UserSpace ---
        #region --- Работа с базой данных ---
        public static string databaseName = "";                                                                         // !!! ВВЕДИТЕ НАЗВАНИЕ БД, ЭТО НЕОБХОДИМО В ПЕРВУЮ ОЧЕРЕДЬ !!!
        public static string connectionString = Database.ConnectionStringBuilder(databaseName);                         // !!! ПОМЕНЯТЬ ОСНОВУ СТРОКИ МОЖНО В Database.cs МЕТОД: ConnectionStringBuilder !!!
        #endregion --- Работа с базой данных ---

        #region --- Дополнительные функции ---

        #region +++ Глобальные функции +++
        public static bool openEveryWindowInNew = true;                                                                 // Открывать новые окна в каждом новом
        public static bool moreExitButtons = false;                                                                     // БОЛЬШЕ КНОПОЧЕК "ВЫХОД" !!!
        #endregion +++ Глобальные функции +++

        #region +++  UniversalTableViewerForm функции +++
        public static bool additionalButtonsInTables = true;                                                            // Добавить кнопки добавления и изменения
        public static bool exportRightInTables = false;                                                                 // Добавить прямой экспорт
        public static bool helpButtonInTables = true;                                                                   // Добавить кнопку помощи
        public static bool enableFilterInTables = false;                                                                // Включить фильтр
        public static bool enableSearchInTables = true;                                                                 // Включить поиск
        #endregion +++ UniversalTableViewerForm функции +++

        #endregion --- Дополнительные функции ---

        #region --- Роли, необходимые для программы ---
        public static string[] roles = new string[] { "Администратор", "Менеджер" };                                    // Здесь прописываются роли!
        #endregion --- Роли, необходимые для программы ---

        #region --- Таблицы и автоопределение таблиц ---
        public static string[] tables = new string[] { };                                                               // Пример заполнения: tables = new string[] { "Documents", "DocumentHistory", "Fines", "Owners", "Violations" }; (!) НЕОБХОДИМО ОТКЛЮЧИТЬ АВТООПРЕДЕЛЕНИЕ ДЛЯ КОРРЕКТНОЙ РАБОТЫ!
        public static string[] removeFromTableWhenStart = new string[] { "Users" };                                     // Какие таблицы удалять, после запуска(из добавленных вручную или автоматически добавленных)
        public static bool tableAutodetect = true;                                                                      // Включить автоопределение таблиц из базы данных
        public static string[] removeFromTableWhenAutodetect = new string[] { };                                        // (!) Какие таблицы удалять, после автоматического определения (!) Не работает если отключён автодетект.
        #endregion --- Таблицы и автоопределение таблиц ---

        #region --- Цветовая тема и иконка ---
        public static bool applyCustomThemes = true;                                                                    // Применять кастомные темы к окнам
        public static DesignConfig.ApplicationTheme applicationTheme = DesignConfig.ApplicationTheme.SystemDefault;     // Указать цветовую палитру, если отключено applyCustomThemes, тема не будет применена
        public static Icon Icon = File.Exists("icon.ico") ? new Icon("icon.ico") : null;                                // Иконка для всех форм, если добавляете иконку "нагло", как с new Icon(прописано по умолчанию), в свойствах необходимо указать "Всегда копировать"
                                                                                                                        // Пример через ресурсы: 
                                                                                                                        // Icon = Properties.Resources.AppIcon;
                                                                                                                        // 
                                                                                                                        // Пример через файл:
                                                                                                                        // Icon = new Icon("C:\\path\\to\\icon.ico");
        #endregion --- Цветовая тема и иконка ---

        #region --- Переводы таблиц и других элементов ---
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
        #endregion --- Переводы таблиц и других элементов ---
        #endregion --- UserSpace ---
        #region --- DevSpace ---
        // Системные настройки, здесь нет необходимости что-либо менять
        public static string lastError = "";
        public static Point LastLocation = new Point(400, 300);
        // Системные настройки, здесь нет необходимости что-либо менять
        #endregion --- DevSpace ---
    }
}
