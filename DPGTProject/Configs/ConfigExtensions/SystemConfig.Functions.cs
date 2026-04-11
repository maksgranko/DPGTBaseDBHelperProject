using Scraps.Databases.Utilities;
using System;
using System.Linq;

namespace DPGTProject
{
    public static partial class SystemConfig
    {
        internal static void Initialize()
        {
            if (string.IsNullOrEmpty(databaseName))
                throw new NullReferenceException("Не задано имя SystemConfig.databaseName!");

            var mergedVirtualTables = (virtualTables ?? Array.Empty<string>())
                .Concat((VirtualTableQueries ?? new System.Collections.Generic.Dictionary<string, string>()).Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            virtualTables = mergedVirtualTables;

            tables = TableCatalog.InitializeTables(
                tableAutodetect,
                SystemConfig.tables,
                removeFromTableWhenStart,
                removeFromTableWhenAutodetect,
                mergedVirtualTables);
        }
    }
}
