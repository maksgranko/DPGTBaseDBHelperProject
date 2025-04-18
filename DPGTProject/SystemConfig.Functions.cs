using System;
using System.Collections.Generic;
using System.Linq;

namespace DPGTProject
{
    public static partial class SystemConfig
    {

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
        internal static void Initialize()
        {
            if (string.IsNullOrEmpty(databaseName)) throw new NullReferenceException("Не задано имя SystemConfig.databaseName!");
            List<string> tablesTemp = Database.GetTables().ToList();
            List<string> TempDelete = new List<string>();
            TempDelete.AddRange(removeFromTableWhenStart);
            if (tableAutodetect) TempDelete.AddRange(removeFromTableWhenAutodetect);
            tablesTemp.RemoveAll(x => TempDelete.Contains(x));
            tables = tablesTemp.Distinct().ToArray();
        }
    }
}
