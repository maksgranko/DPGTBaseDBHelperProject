# Универсальная база данных - Полное руководство по SystemConfig.cs

## 📦 Требования

- .NET Framework 4.7.2+
- EPPlus версии 7.1.0 для работы с Excel (устанавливается через NuGet)
- SQL Server (или другая совместимая СУБД)

> 🔥 Важно-преважно:
>
> - Не забудь дать имя базе данных (databaseName)!
> - Если есть проблемы с базой данных, она не работает и выдаёт ошибки, в первую очередь посмотри в Database.cs в метод ConnectionStringBuilder, возможно, строка в твоём случае должна быть другой.
> - Цвета можно менять внутри DesignConfig.cs как хочешь!
> - ВСЕ классы, в которые необходимо вносить изменения находятся в Config.
>   > - SystemConfig.cs отвечает за основные изменения программы.
>   > - DesignConfig.cs отвечает за темы, вы можете добавить также свои темы, если это необходимо.
> - Вы также можете менять весь остальной код, но на ваш страх и риск.

# 🧩 Конфигурация приложения (как конструктор)

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

**Q: Где настраивается подключение к БД?**
A: В Database.cs, метод ConnectionStringBuilder создаёт строку подключения, там уже прописаны шаблоны, какой-то из них вам, вероятно, подойдёт.

**Q: Как добавить новую роль?**

1. Добавьте название в массив `roles`
2. Настройте права в `DefaultRolePermissions`
3. При необходимости укажите особые права для таблиц в `RolePermissions`
