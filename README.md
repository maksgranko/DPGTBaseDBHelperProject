# 🗃️ Универсальная база данных DPGTProject

![.NET Version](https://img.shields.io/badge/.NET-4.7.2+-blue.svg)

## Содержание

- [Возможности](#-возможности)
- [Требования](#-требования)
- [Быстрый старт](#-быстрый-старт)
- [Архитектура проекта](#-архитектура-проекта)
- [Конфигурация SystemConfig](#-systemconfig---ключ-к-настройкам)
- [Частые вопросы](#-частые-вопросы)

## 🚀 Возможности

- Управление ролями и правами доступа
- Гибкая система конфигурации через SystemConfig
- Автоматическое определение таблиц БД
- Кастомные цветовые темы интерфейса
- Поддержка перевода интерфейса
- Интеграция с Excel через EPPlus
- Модульная архитектура с разделением форм и логики

## 📦 Требования

- **Платформа**: .NET Framework 4.7.2+
- **Базы данных**:
  - SQL Server и аналоги
- **Зависимости**:
  - EPPlus 7.1.0+ (Excel экспорт/импорт)
  - Newtonsoft.Json 13.0.1+ (сериализация)

## 🚀 Быстрый старт

**Если вы используете Visual Studio 2022**

1. **Клонирование репозитория**

   1. Откройте Visual Studio 2022.
   2. В главном меню выберите **Git** -> **Клонировать репозиторий**.
   3. В поле **URL-адрес репозитория** вставьте ссылку на репозиторий (https://github.com/your-repo/UniversalBaseDBProject.git).
   4. Укажите локальный путь, куда хотите клонировать репозиторий.
   5. Нажмите кнопку **Клонировать**.

2. **Восстановление зависимостей**

   1. Откройте Visual Studio 2022.
   2. В главном меню выберите **Сервис** -> **Диспетчер пакетов NuGet** -> **Восстановить пакеты NuGet**.
   3. Убедитесь, что все пакеты восстановлены успешно.

**_(Далее, смотреть под инструкцией к консоли, начиная с 3)_**

**Если вы используете Visual Studio Code или вообще консоль(?мистер мазохист?)**

1. **Клонирование репозитория**

```bash
git clone https://github.com/your-repo/UniversalBaseDBProject.git
cd UniversalBaseDBProject
```

2. **Восстановление зависимостей**

```bash
nuget restore DPGTProject.sln
```

3.  **Настройка БД**

- Создайте базу данных в SQL Server
- Обновите параметры в `SystemConfig.cs`:

```csharp
public static string databaseName = "YourDatabaseName";
```

4.  **Настройка ролей и пользователей**

Для управления ролями и пользователями необходимо настроить параметры в файле `SystemConfig.cs`.

**4.1. Добавление ролей**

Добавьте новые роли в массив `roles`:

```csharp
public static string[] roles = new string[] { "Администратор", "Менеджер", "НоваяРоль" };
```

**4.2. Настройка прав по умолчанию для ролей**

Настройте права по умолчанию для каждой роли в словаре `DefaultRolePermissions`:

```csharp
public static Dictionary<string, TablePermission> DefaultRolePermissions = new Dictionary<string, TablePermission>()
{
    ["default"] = new TablePermission // default - права для ВСЕХ
    {
        CanRead = false,
        CanWrite = false,
        CanDelete = false,
        CanExport = false,
        CanImport = false
    },
    ["Администратор"] = new TablePermission // Администратор - права для Администратора
    {
        CanRead = true,
        CanWrite = true,
        CanDelete = true,
        CanExport = true,
        CanImport = true
    },
    ["Менеджер"] = new TablePermission // Менеджер - права для Менеджера
    {
        CanRead = true,
        CanWrite = true,
        CanDelete = true,
        CanExport = false,
        CanImport = false
    },
    ["НоваяРоль"] = new TablePermission // НоваяРоль - права для НовойРоли
    {
        CanRead = false,
        CanWrite = false,
        CanDelete = false,
        CanExport = false,
        CanImport = false
    }
};
```

**4.3. Настройка прав для каждой роли для каждой конкретной таблицы**

Настройте права для каждой роли для каждой конкретной таблицы в словаре `RolePermissions`:

```csharp
public static Dictionary<string, List<TablePermission>> RolePermissions = new Dictionary<string, List<TablePermission>>()
{
    ["Администратор"] = new List<TablePermission>
    {
        new TablePermission {
            TableName = "Users",
            CanRead   =  true,
            CanWrite  =  true,
            CanDelete =  true,
            CanExport =  true,
            CanImport =  true
        },
    },
    ["Менеджер"] = new List<TablePermission>
    {
        new TablePermission {
            TableName = "Users",
            CanRead   =  true,
            CanWrite  =  false,
            CanDelete =  false,
            CanExport =  false,
            CanImport =  false
        },
    },
    ["НоваяРоль"] = new List<TablePermission>
    {
        new TablePermission {
            TableName = "Users",
            CanRead   =  false,
            CanWrite  =  false,
            CanDelete =  false,
            CanExport =  false,
            CanImport =  false
        },
    }
};
```

**4.4. Регистрация новых пользователей**

Для регистрации новых пользователей используется форма `RegisterForm`. При регистрации необходимо указать логин, пароль и роль пользователя.

**4.5. Проверка прав доступа**

Для проверки прав доступа используется класс `RoleManager` и метод `CheckAccess`.

4.1. **Добавление таблиц вручную и включение/отключение дополнительных настроек, если это необходимо**

5.  **Сборка проекта**

**Сборка проекта в Visual Studio 2022:**

1.  Откройте Visual Studio 2022.
2.  В главном меню выберите **Сборка** -> **Собрать решение**.
3.  Убедитесь, что сборка прошла успешно и нет ошибок.

## 🏗️ Архитектура проекта

Основные компоненты:

- `Configs/`: Конфигурационные классы
- `Forms/`: Windows Forms приложения
- `Database.cs`: Управление подключением к БД
- `RoleManager.cs`: Система ролей и прав доступа

## 🧩 Конфигурация приложения

## ⚙️ SystemConfig - Ключ к настройкам!

SystemConfig - это как пульт управления для вашего приложения. Здесь собраны основные параметры: подключение к базе данных, роли пользователей, права доступа и многое другое. Подробное описание каждого параметра вы найдете ниже.

**Основные параметры SystemConfig:**

- `databaseName`: Имя твоей базы данных. **Обязательно укажи его!** Без этого ничего не заработает. Укажите имя базы данных, с которой будет работать приложение.

```csharp
public static string databaseName = "MyDatabase"; // Здесь пишем имя нашей базы
```

- `connectionString`: Строка подключения к базе данных. Обычно формируется автоматически на основе `databaseName` в классе `Database.cs` (метод `ConnectionStringBuilder`). Если нужно изменить строку подключения, это можно сделать здесь или непосредственно в `Database.cs`.

```csharp
public static string connectionString = Database.ConnectionStringBuilder(databaseName);
```

2. **Дополнительные функции:**

   - `openEveryWindowInNew`: Открывать каждое окно в новом экземпляре или нет. Если `true`, то каждое окно будет открываться отдельно. Если `false`, то окна будут открываться в одном экземпляре.

   ```csharp
   public static bool openEveryWindowInNew = true; // Открывать новые окна в каждом новом
   ```

   - `moreExitButtons`: Добавляет больше кнопок "Выход" в приложении.

   ```csharp
   public static bool moreExitButtons = false; // БОЛЬШЕ КНОПОЧЕК "ВЫХОД" !!!
   ```

   - `additionalButtonsInTables`: Добавляет кнопки добавления и изменения в таблицы.

   ```csharp
   public static bool additionalButtonsInTables = true; // Добавить кнопки добавления и изменения
   ```

   - `exportRightInTables`: Добавляет возможность экспорта данных прямо из таблиц.

   ```csharp
   public static bool exportRightInTables = false; // Добавить прямой экспорт
   ```

   - `helpButtonInTables`: Добавляет кнопку помощи в таблицы.

   ```csharp
   public static bool helpButtonInTables = true; // Добавить кнопку помощи
   ```

   - `enableFilterInTables`: Включает фильтрацию в таблицах.

   ```csharp
   public static bool enableFilterInTables = false; // Включить фильтр
   ```

   - `enableSearchInTables`: Включает поиск в таблицах.

   ```csharp
   public static bool enableSearchInTables = true; // Включить поиск
   ```

   - `addRolesWhenRegistering`: Добавляет выбор роли при регистрации пользователя.

   ```csharp
   public static bool addRolesWhenRegistering = false; // Добавить выбор роли при регистрации
   ```

3. **Роли и права:**

   - `roles`: Массив строк, содержащий список всех ролей в системе.

   ```csharp
   public static string[] roles = new string[] { "Администратор", "Менеджер" }; // Здесь прописываются роли!
   ```

   - `DefaultRolePermissions`: Словарь, определяющий права по умолчанию для каждой роли.

   ```csharp
   public static Dictionary<string, TablePermission> DefaultRolePermissions = new Dictionary<string, TablePermission>()
   {
       ["default"] = new TablePermission // default - права для ВСЕХ
       {
           CanRead = false,
           CanWrite = false,
           CanDelete = false,
           CanExport = false,
           CanImport = false
       },
       ["Администратор"] = new TablePermission // Администратор - права для Администратора
       {
           CanRead = true,
           CanWrite = true,
           CanDelete = true,
           CanExport = true,
           CanImport = true
       },
       ["Менеджер"] = new TablePermission // Менеджер - права для Менеджера
       {
           CanRead = true,
           CanWrite = true,
           CanDelete = true,
           CanExport = false,
           CanImport = false
       }
   };
   ```

   - `RolePermissions`: Словарь, определяющий права для каждой роли для каждой конкретной таблицы.

   ```csharp
   public static Dictionary<string, List<TablePermission>> RolePermissions = new Dictionary<string, List<TablePermission>>()
   {
       ["Администратор"] = new List<TablePermission>
       { /* ^РОЛЬ^, которой назначаются права ниже.*/
           new TablePermission {
               TableName = "Users", // <--- НАЗВАНИЕ ТАБЛИЦЫ, всё что ниже - касается именно ЭТОЙ таблицы.
               CanRead   =  true,    // <--- Может ли просматривать?
               CanWrite  =  true,    // <--- Может ли записывать/редактировать?
               CanDelete =  true,    // <--- Может ли удалять?
               CanExport =  true,    // <--- Может ли экспортировать?
               CanImport =  true     // <--- Может ли импортировать?
           },
           // Можно добавлять таблицы дальше, аналогично.
       },
   };
   ```

4. **Таблицы:**

   - `tables`: Массив строк, содержащий список таблиц, с которыми работает приложение. Если `tableAutodetect` включен, то этот список будет игнорироваться.

   ```csharp
   public static string[] tables = new string[] { }; // Пример заполнения: tables = new string[] { "Documents", "DocumentHistory", "Fines", "Owners", "Violations" }; (!) НЕОБХОДИМО ОТКЛЮЧИТЬ АВТООПРЕДЕЛЕНИЕ ДЛЯ КОРРЕКТНОЙ РАБОТЫ!
   ```

   - `removeFromTableWhenStart`: Массив строк, содержащий список таблиц, которые нужно удалить после запуска приложения.

   ```csharp
   public static string[] removeFromTableWhenStart = new string[] { };  // (!) мб стало бесполезным из-за ролей    // Какие таблицы удалять, после запуска(из добавленных вручную или автоматически добавленных)
   ```

   - `tableAutodetect`: Включает или отключает автоматическое определение таблиц из базы данных.

   ```csharp
   public static bool tableAutodetect = true; // Включить автоопределение таблиц из базы данных
   ```

   - `removeFromTableWhenAutodetect`: Массив строк, содержащий список таблиц, которые нужно исключить из автоматического определения.

   ```csharp
   public static string[] removeFromTableWhenAutodetect = new string[] { }; // (!) Какие таблицы удалять, после автоматического определения (!) Не работает если отключён автодетект.
   ```

5. **Цветовая тема и иконка:**

   - `applyCustomThemes`: Включает или отключает применение кастомных тем к окнам.

   ```csharp
   public static bool applyCustomThemes = true; // Применять кастомные темы к окнам
   ```

   - `applicationTheme`: Указывает цветовую палитру, если `applyCustomThemes` включен.

   ```csharp
   public static DesignConfig.ApplicationTheme applicationTheme = DesignConfig.ApplicationTheme.SystemDefault; // Указать цветовую палитру, если отключено applyCustomThemes, тема не будет применена
   ```

   - `Icon`: Иконка для всех форм приложения.

   ```csharp
   public static Icon Icon = File.Exists("icon.ico") ? new Icon("icon.ico") : null; // Иконка для всех форм, если добавляете иконку "нагло", как с new Icon(прописано по умолчанию), в свойствах необходимо указать "Всегда копировать"
   ```

6. **Переводы:**

   - `ColumnTranslations`: Словарь, содержащий переводы названий столбцов для каждой таблицы.

   ```csharp
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
   ```

   - `TableTranslations`: Словарь, содержащий переводы названий таблиц.

   ```csharp
   public static Dictionary<string, string> TableTranslations = new Dictionary<string, string>()
   {
       ["Sample"] = "Пример_названий_в_combobox1",
       ["Sample1"] = "Пример_названий_в_combobox2",
       ["Sample2"] = "Пример_названий_в_combobox3",
       ["Sample3"] = "Пример_названий_в_combobox4",
       ["Users"] = "Пользователи" // Строку не трогать, она нужна для перевода
   };
   ```

**Короче говоря, SystemConfig - это ваш главный файл настроек.**

## ❓ Частые вопросы

**Q: Как изменить строку подключения к БД?**  
A: Настройки в `Database.cs` → `ConnectionStringBuilder`. Поддерживает SQL Server и совместимое.
Учтите! Если вы собираетесь вводить вручную, установите auto = false, вместо true

**Q: Как добавить новую роль?**

1. Добавить название в массив `roles`
2. Настроить права в `DefaultRolePermissions`
3. Указать особые права для таблиц в `RolePermissions`

**Q: Как создать новую цветовую тему?**  
A: В `DesignConfig.cs` создать новый класс темы и добавить в `ApplicationTheme`.

**Q: Почему не отображаются таблицы?**  
A: Проверьте:

1. Корректность имени БД в SystemConfig.cs
2. Наличие прав доступа у текущей роли
3. Состояние флага `tableAutodetect`

**Q: При запуске программы, создаётся Users в указанной базе данных, а я хочу другое название, как это сделать?**  
A: Измените код в Database.cs, в классе Users. Необходимо изменить следующие строки:

```csharp
    public static string UsersTableName = "Users"; // Если вам необходимо изменить таблицу "Users", измените здесь
    public static Dictionary<string, string> UsersTableColumnsNames = new Dictionary<string, string> { // Если вам необходимо изменить названия колонок в таблице "Users", измените в этой переменной
        { "UserID","UserID"}, // Менять необходимо ВТОРОЕ значение, именно оно влияет на название таблицы, а первое используется в коде.
        { "Login","Login"},
        { "Password","Password"},
        { "Role","Role"},
    };
```

Например так:

```csharp
    public static string UsersTableName = "Юзеры";
    public static Dictionary<string, string> UsersTableColumnsNames = new Dictionary<string, string> {
        { "UserID","Идентификатор"},
        { "Login","Логин"},
        { "Password","Пароль"},
        { "Role","Роли"},
 };
```

**Q: Я добавил иконку с названием icon.ico в проект, но она не работает, как быть?**  
A: Нажмите "Показать все файлы" в обозревателе решений в Visual Studio → правой кнопкой мыши по icon.ico → "Включить в проект"
