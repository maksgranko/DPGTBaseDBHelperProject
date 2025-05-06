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
        public static bool enableFilterInTables = true;                                                                // Включить фильтр
        public static bool enableSearchInTables = false;                                                                 // Включить поиск
        public static bool enableSortingInTables = true;                                                                // Включить сортировку
        #endregion +++ UniversalTableViewerForm функции +++                                 

        #region +++  RegisterForm функции +++
        public static bool addRolesWhenRegistering = false;                                                            // Добавить выбор роли при регистрации
        #endregion +++ RegisterForm функции +++

        #endregion --- Дополнительные функции ---

        #region --- Роли, необходимые для программы ---
        public static string[] roles = new string[] { "Администратор", "Менеджер" };                                   // Здесь прописываются роли! Вы можете добавить свою.
                                                                                                                       // Роли добавляются в список с начала в конец. По умолчанию при отключённом выборе, выдаётся последняя роль.

        #region +++ Права, индивидуальные к каждой РОЛИ ПО УМОЛЧАНИЮ +++
        public static Dictionary<string, TablePermission> DefaultRolePermissions = new Dictionary<string, TablePermission>()
        {
            ["default"] = new TablePermission                                                                          // default - права для ВСЕХ
            {
                CanRead = false,
                CanWrite = false,
                CanDelete = false,
                CanExport = false,
                CanImport = false
            },
            ["Администратор"] = new TablePermission                                                                    // Администратор - права для Администратора
            {
                CanRead = true,
                CanWrite = true,
                CanDelete = true,
                CanExport = true,
                CanImport = true
            },
            ["Менеджер"] = new TablePermission                                                                         // Менеджер - права для Менеджера
            {
                CanRead = true,
                CanWrite = true,
                CanDelete = true,
                CanExport = false,
                CanImport = false
            }
        };
        #endregion +++ Права, индивидуальные к каждой РОЛИ ПО УМОЛЧАНИЮ +++
                                                                                                                       // Порядок прав: <-эти^ права важнее, чем те, что ниже, они работают глобально ко всем таблицам
        #region +++ Права, индивидуальные ПО РОЛЯМ к каждой ТАБЛИЦЕ +++
        // Можно добавить другие таблицы по аналогии
        public static Dictionary<string, List<TablePermission>> RolePermissions = new Dictionary<string, List<TablePermission>>()
        {
            ["Администратор"] = new List<TablePermission>
            {                                                                                                           /* ^РОЛЬ^, которой назначаются права ниже.*/
                new TablePermission {
                    TableName = Database.Users.UsersTableName,                                                          // <--- НАЗВАНИЕ ТАБЛИЦЫ, всё что ниже - касается именно ЭТОЙ таблицы.
                                                                                                                        // В данном случае, это касается именно таблицы USERS, название которой берётся из переменной
                    CanRead   =  true,                                                                                  // <--- Может ли просматривать?
                    CanWrite  =  true,                                                                                  // <--- Может ли записывать/редактировать?
                    CanDelete =  true,                                                                                  // <--- Может ли удалять?
                    CanExport =  true,                                                                                  // <--- Может ли экспортировать?
                    CanImport =  true                                                                                   // <--- Может ли импортировать?
                },                                                                                                   
                new TablePermission {
                    TableName =  "Здесь_Название_Таблицы",                                                              // <--- НАЗВАНИЕ ТАБЛИЦЫ, всё что ниже - касается именно ЭТОЙ таблицы.
                    CanRead   =  true,
                    CanWrite  =  true,
                    CanDelete =  true,
                    CanExport =  true,
                    CanImport =  true 
                }, // аналогично далее
            },
        };
        #endregion +++ Права, индивидуальные ПО РОЛЯМ к каждой ТАБЛИЦЕ +++

        #endregion --- Роли, необходимые для программы ---

        #region --- Таблицы и автоопределение таблиц ---
        public static string[] tables = new string[] { };                                                               // Пример заполнения: tables = new string[] { "Documents", "DocumentHistory", "Fines", "Owners", "Violations" }; (!) НЕОБХОДИМО ОТКЛЮЧИТЬ АВТООПРЕДЕЛЕНИЕ ДЛЯ КОРРЕКТНОЙ РАБОТЫ!
        public static string[] removeFromTableWhenStart = new string[] { };  // (!) мб стало бесполезным из-за ролей    // Какие таблицы удалять, после запуска(из добавленных вручную или автоматически добавленных)
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
            [Database.Users.UsersTableName] = "Пользователи" // Строку не трогать, она нужна для перевода
        };
        #endregion --- Переводы таблиц и других элементов ---
        #endregion --- UserSpace ---
        #region --- DevSpace ---
        // Системные настройки, здесь нет необходимости что-либо менять
        public static string lastError = "";
        public static Point LastLocation = new Point(400, 300); // от этого вообще есть ли смысл?
        // Системные настройки, здесь нет необходимости что-либо менять
        #endregion --- DevSpace ---
    }
}
