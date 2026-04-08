using Scraps.Databases.Utilities;
using System;

namespace DPGTProject
{
    public static partial class SystemConfig
    {
        internal static void Initialize()
        {
            if (string.IsNullOrEmpty(databaseName))
                throw new NullReferenceException("Не задано имя SystemConfig.databaseName!");

            tables = TableCatalog.InitializeTables(
                tableAutodetect,
                SystemConfig.tables,
                removeFromTableWhenStart,
                removeFromTableWhenAutodetect,
                virtualTables);
        }
    }
}
