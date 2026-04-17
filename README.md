# DPGTProject

Практическое руководство для первого запуска и настройки проекта.

Этот репозиторий - универсальный WinForms-конструктор приложений для работы с SQL Server: авторизация, роли/права, просмотр и редактирование таблиц, импорт/экспорт, отчеты и виртуальные таблицы.

## Что вам нужно перед стартом

- Windows + Visual Studio 2022 (рекомендуется)
- .NET Framework 4.7.2+
- SQL Server (LocalDB/Express/Developer - любой совместимый)
- Доступ к базе с правами на создание таблиц (если включена автогенерация)

## Быстрый старт (10 минут)

1. Откройте решение `DPGTProject.sln`.
2. В файле `DPGTProject/Configs/SystemConfig.cs` заполните минимум:

```csharp
public static string databaseName = "MyDatabase";
public static string connectionString = MSSQL.ConnectionStringBuilder(databaseName);
```

3. Оставьте на первый запуск:

```csharp
public static bool tableAutodetect = true;
public static DatabaseGenerationMode databaseGenerationMode = DatabaseGenerationMode.Simple;
```

4. Запустите проект (`F5`).
5. Зарегистрируйте первого пользователя в `RegisterForm`.
6. Войдите в систему и откройте любую таблицу.

Готово: базовый сценарий работает.

## Как устроен проект

- `DPGTProject/Program.cs` - последовательность старта приложения:
  - конфигурация подключения и auth
  - генерация/проверка БД
  - инициализация таблиц
  - загрузка переводов
  - инициализация ролей
  - регистрация виртуальных таблиц
- `DPGTProject/Configs/SystemConfig.cs` - основные настройки поведения приложения
- `DPGTProject/Forms` - UI формы (авторизация, таблицы, отчеты, импорт)

## Настройка базы данных

### Обязательные поля

В `DPGTProject/Configs/SystemConfig.cs`:

```csharp
public static string databaseName = "MyDatabase";
public static string connectionString = MSSQL.ConnectionStringBuilder(databaseName);
```

Если используете свой SQL Server-инстанс, задайте строку подключения вручную.

### Режим генерации схемы

```csharp
public static DatabaseGenerationMode databaseGenerationMode = DatabaseGenerationMode.Simple;
```

Ориентиры:
- `None` - не изменяет схему
- `Simple` - минимальная схема для запуска
- `Standard` / `Full` - расширенные режимы

Для первого запуска обычно достаточно `Simple`.

## Настройка таблиц

В `SystemConfig` есть 2 режима:

1) Автоопределение таблиц (рекомендуется новичкам)

```csharp
public static bool tableAutodetect = true;
public static string[] removeFromTableWhenAutodetect = new string[] { "sysdiagrams" };
```

2) Ручной список таблиц

```csharp
public static bool tableAutodetect = false;
public static string[] tables = new string[] { "Users", "Orders", "Products" };
```

Дополнительно:

```csharp
public static string[] removeFromTableWhenStart = new string[] { };
```

## Роли и права (самое важное)

### 1) Список ролей

```csharp
public static string[] roles = new string[] { "Администратор", "Менеджер", "Оператор" };
```

### 2) Права роли по умолчанию

```csharp
public static Dictionary<string, PermissionFlags> DefaultRolePermissions = new Dictionary<string, PermissionFlags>
{
    ["default"] = PermissionFlags.None,
    ["Администратор"] = PermissionFlags.All,
    ["Менеджер"] = PermissionFlags.ReadWrite,
    ["Оператор"] = PermissionFlags.Read
};
```

### 3) Точечные права по таблицам

```csharp
public static Dictionary<string, List<TablePermission>> RolePermissions = new Dictionary<string, List<TablePermission>>
{
    ["Менеджер"] = new List<TablePermission>
    {
        new TablePermission("Orders", PermissionFlags.ReadWrite | PermissionFlags.Export),
        new TablePermission("Products", PermissionFlags.Read)
    }
};
```

Важно:
- `DefaultRolePermissions` - базовые (глобальные) права роли
- `RolePermissions` - точечная настройка по конкретным таблицам

## Регистрация и авторизация

Ключевые флаги в `SystemConfig`:

```csharp
public static bool authHashPasswords = true;
public static bool addRolesWhenRegistering = false;
```

Рекомендация для production: `authHashPasswords = true`.

## Переводы интерфейса

Все переводы настраиваются в:

```csharp
public static Dictionary<string, string> Translations = new Dictionary<string, string>()
{
    ["Users"] = "Пользователи",
    ["Login"] = "Логин",
    ["Password"] = "Пароль"
};
```

Можно задавать:
- переводы таблиц
- переводы колонок

## Виртуальные таблицы

Если нужен специальный SQL-вид (не физическая таблица), используйте виртуальные таблицы.

1. Добавьте имя в список:

```csharp
public static string[] virtualTables = new string[] { "VT_Client" };
```

2. Добавьте SQL в `VirtualTableQueries`:

```csharp
public static Dictionary<string, string> VirtualTableQueries = new Dictionary<string, string>
{
    ["VT_Client"] = "SELECT TOP (1000) ..."
};
```

После запуска виртуальные таблицы автоматически регистрируются в старте приложения.

## FK-поля в Add/Edit форме

Если в выпадающем списке FK нужно показывать не ID, а конкретную колонку:

```csharp
public static Dictionary<string, string> ForeignKeyDisplayColumnOverrides =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["Orders.CustomerID"] = "FullName"
};
```

## UI и поведение форм

Полезные настройки:

```csharp
public static bool openEveryWindowInNew = true;
public static bool additionalButtonsInTables = true;
public static bool enableFilterInTables = true;
public static bool enableSearchInTables = false;
public static bool exportRightInTables = false;
```

Также можно настроить тему и иконку:

```csharp
public static bool applyCustomThemes = true;
public static DesignConfig.ApplicationTheme applicationTheme = DesignConfig.ApplicationTheme.SystemDefault;
public static Icon Icon = File.Exists("icon.ico") ? new Icon("icon.ico") : null;
```

## Импорт и отчеты

- Импорт данных доступен из `DataImportForm`
- Генерация отчётов - `ReportGeneratorForm`
- В этом проекте отчеты намеренно остаются с ручным формированием, чтобы можно было делать визуально разные шаблоны

## Первый checklist перед запуском

Проверьте в `SystemConfig`:

- `databaseName` не пустой
- `connectionString` корректный
- `roles` содержит нужные роли
- `tableAutodetect` включен (если не ведете список таблиц вручную)
- `authHashPasswords = true`

## Частые проблемы

### Не видно таблиц в главном окне

Проверьте:
- есть ли таблицы в БД
- включен ли `tableAutodetect`
- не удаляются ли таблицы через `removeFromTableWhenAutodetect`/`removeFromTableWhenStart`
- есть ли у роли права `Read` на эти таблицы

### Ошибка при старте про databaseName

В `SystemConfig.databaseName` указано пустое значение. Заполните его.

### Не получается экспорт/импорт

Проверьте права роли:
- для экспорта нужен `PermissionFlags.Export`
- для импорта нужен `PermissionFlags.Import`

## Для разработчиков

- Основная точка расширения: `SystemConfig` + формы в `DPGTProject/Forms`
- Если добавляете новую бизнес-логику, сначала проверьте, нет ли уже подходящего API в библиотеке
