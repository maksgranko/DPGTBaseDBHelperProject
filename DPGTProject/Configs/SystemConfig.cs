using DPGTProject.Configs;
using Scraps.Configs;
using Scraps.Databases.Utilities;
using Scraps.Security;
using MSSQL = Scraps.Databases.MSSQL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DPGTProject
{
    public static partial class SystemConfig
    {
        #region --- UserSpace ---
        #region --- Работа с базой данных ---
        public static string databaseName = "SinaiDB";                                                                         // !!! ВВЕДИТЕ НАЗВАНИЕ БД, ЭТО НЕОБХОДИМО В ПЕРВУЮ ОЧЕРЕДЬ !!!
        public static string connectionString = MSSQL.ConnectionStringBuilder(databaseName);                         // !!! ПОМЕНЯТЬ ОСНОВУ СТРОКИ МОЖНО В Database.cs МЕТОД: ConnectionStringBuilder !!!
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
        public static bool enableFilterInTables = true;                                                                 // Включить фильтр
        public static bool enableSearchInTables = false;                                                                // Включить поиск
        public static bool enableSortingInTables = true;                                                                // Включить сортировку
        #endregion +++ UniversalTableViewerForm функции +++                                 

        #region +++  RegisterForm функции +++
        public static bool addRolesWhenRegistering = false;                                                             // Добавить выбор роли при регистрации
        #endregion +++ RegisterForm функции +++
        #region +++ Auth / схема БД +++
        public static bool authHashPasswords = true;                                                                    // Хэшировать пароли при регистрации/авторизации
        public static DatabaseGenerationMode databaseGenerationMode = DatabaseGenerationMode.Simple;                    // Режим генерации БД: None/Simple/Standard/Full
        #endregion +++ Auth / схема БД +++


        #endregion --- Дополнительные функции ---

        #region --- Роли, необходимые для программы ---
        public static string[] roles = new string[] { "Администратор", "Менеджер" };                                    // Здесь прописываются роли! Вы можете добавить свою.
                                                                                                                        // Роли добавляются в список с начала в конец. По умолчанию при отключённом выборе, выдаётся последняя роль.

        #region +++ Права, индивидуальные к каждой РОЛИ ПО УМОЛЧАНИЮ +++
        public static Dictionary<string, PermissionFlags> DefaultRolePermissions = new Dictionary<string, PermissionFlags>()
        {
            ["default"] = PermissionFlags.None,                                                                         // default - права для ВСЕХ
            ["Администратор"] = PermissionFlags.All,
            ["Менеджер"] = PermissionFlags.ReadWrite | PermissionFlags.Delete
        };
        #endregion +++ Права, индивидуальные к каждой РОЛИ ПО УМОЛЧАНИЮ +++
        // Порядок прав: <-эти^ права важнее, чем те, что ниже, они работают глобально ко всем таблицам
        #region +++ Права, индивидуальные ПО РОЛЯМ к каждой ТАБЛИЦЕ +++
        // Можно добавить другие таблицы по аналогии
        public static Dictionary<string, List<TablePermission>> RolePermissions = new Dictionary<string, List<TablePermission>>()
        {
            ["Администратор"] = new List<TablePermission>
            {                                                                                                           /* ^РОЛЬ^, которой назначаются права ниже.*/
                new TablePermission(
                    ScrapsConfig.UsersTableName,                                                                       // <--- НАЗВАНИЕ ТАБЛИЦЫ, всё что ниже - касается именно ЭТОЙ таблицы.
                    PermissionFlags.All),
                new TablePermission(
                    "Здесь_Название_Таблицы",                                                                           // <--- НАЗВАНИЕ ТАБЛИЦЫ, всё что ниже - касается именно ЭТОЙ таблицы.
                    PermissionFlags.All),
            },
        };
        #endregion +++ Права, индивидуальные ПО РОЛЯМ к каждой ТАБЛИЦЕ +++

        #endregion --- Роли, необходимые для программы ---

        #region --- Таблицы и автоопределение таблиц ---
        public static string[] tables = new string[] { };                                                                // Пример заполнения: tables = new string[] { "Documents", "DocumentHistory", "Fines", "Owners", "Violations" }; (!) НЕОБХОДИМО ОТКЛЮЧИТЬ АВТООПРЕДЕЛЕНИЕ ДЛЯ КОРРЕКТНОЙ РАБОТЫ!
        public static string[] virtualTables = new string[] { };                                                         // Если необходимо создать виртуальную таблицу со своей логикой работы
        public static string[] removeFromTableWhenStart = new string[] { };  // (!) мб стало бесполезным из-за ролей     // Какие таблицы удалять, после запуска(из добавленных вручную или автоматически добавленных)
        public static bool tableAutodetect = true;                                                                       // Включить автоопределение таблиц из базы данных
        public static string[] removeFromTableWhenAutodetect = new string[] { };                                         // (!) Какие таблицы удалять, после автоматического определения (!) Не работает если отключён автодетект.
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

        #region --- Переводы таблиц ---
        public static Dictionary<string, string> Translations = new Dictionary<string, string>()
        {
            // Переводы таблиц:
            ["Sample"] = "Пример_названий_в_combobox1",
            ["Sample1"] = "Пример_названий_в_combobox2",
            ["Sample2"] = "Пример_названий_в_combobox3",
            ["Sample3"] = "Пример_названий_в_combobox4",
            [ScrapsConfig.UsersTableName] = "Пользователи",
            // Переводы колонок (глобально по имени колонки):
            ["UserID"] = "Идентификатор",
            ["Login"] = "Логин",
            ["Password"] = "Пароль",
            ["Role"] = "Роль"
        };
        #endregion --- Переводы таблиц ---

        #region --- FK отображение в Add/Edit ---
        // Позволяет вручную указать, какую колонку показывать в ComboBox для конкретного FK.
        // Ключ: "<ТекущаяТаблица>.<FKКолонка>", значение: "<КолонкаДляОтображенияИзСвязаннойТаблицы>".
        // Пример:
        // ["Users.Role"] = "RoleName"
        public static Dictionary<string, string> ForeignKeyDisplayColumnOverrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        #endregion --- FK отображение в Add/Edit ---

        #region --- Виртуальные таблицы ---
        // SQL для виртуальных таблиц. Ключ = имя виртуальной таблицы.
        // Примечание: пример VT_Client использует SinaiDB как экспериментальную БД.
        public static Dictionary<string, string> VirtualTableQueries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["VT_Client"] =
                "SELECT TOP (1000) [ID], [CodeName], [Type], [PricePerNight], [Capacity], [IsAvailable], [CleaningStatus] " +
                "FROM [SinaiDB].[dbo].[Rooms]"
        };
        #endregion --- Виртуальные таблицы ---
        #endregion --- UserSpace ---
        #region --- DevSpace ---
        // Системные настройки, здесь нет необходимости что-либо менять
        public static string lastError = "";
        // Системные настройки, здесь нет необходимости что-либо менять
        #endregion --- DevSpace ---
    }
}







