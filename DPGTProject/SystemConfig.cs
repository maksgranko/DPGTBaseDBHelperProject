﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DPGTProject
{
    public static class SystemConfig
    {
        public static string databaseName = "";
        public static string connectionString = $"Data Source={Environment.MachineName};Initial Catalog={databaseName};Integrated Security=True;Encrypt=False";
        // !!! ЕСЛИ ВЫ ПОМЕНЯЛИ ЭТО ЗДЕСЬ, ПОМЕНЯЙТЕ ТАКЖЕ НА ПОДОБНОЕ В Database.cs МЕТОД: ConnectionStringBuilder !!!
        //Иногда работают следующие варианты:
        //public static string connectionString = $"Data Source={Environment.MachineName}\\SQLEXPRESS;Initial Catalog={databaseName};Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        //public static string connectionString = $"Data Source={Environment.MachineName}\\SQLEXPRESS;Initial Catalog={databaseName};Integrated Security=True;Encrypt=False";

        public static Point LastLocation = new Point(400, 300);
        public static string lastError = "";
        //Пример заполнения:
        //tables = new string[] { "Documents", "DocumentHistory", "Fines", "Owners", "Violations" };
        public static string[] tables = new string[] { };
        public static string[] roles = new string[] { "Администратор", "Менеджер" };

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

        public static string TranslateComboBox(string value)
        {
            return TableTranslations.TryGetValue(value, out var translation)
                ? translation
                : value;
        }
        public static string UntranslateComboBox(string value)
        {
            return TableTranslations.FirstOrDefault(x => x.Value == value).Key ?? value;
        }
    }
}
