using Scraps.Database.MSSQL.Utilities;
using Scraps.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DPGTProject
{
    public static partial class SystemConfig
    {
        internal static PermissionFlags GetEffectivePermissions(string roleName, string tableName)
        {
            var effective = ResolveDefaultPermission(roleName);

            if (effective == PermissionFlags.None &&
                TryResolveTablePermission(roleName, tableName, out var tableSpecific))
            {
                return tableSpecific;
            }

            return effective;
        }

        internal static bool HasPermission(string roleName, string tableName, PermissionFlags required)
        {
            return (GetEffectivePermissions(roleName, tableName) & required) == required;
        }

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

        private static PermissionFlags ResolveDefaultPermission(string roleName)
        {
            if (DefaultRolePermissions == null || DefaultRolePermissions.Count == 0)
                return PermissionFlags.None;

            if (!string.IsNullOrWhiteSpace(roleName) &&
                DefaultRolePermissions.TryGetValue(roleName, out var roleDefault))
            {
                return roleDefault;
            }

            return DefaultRolePermissions.TryGetValue("default", out var commonDefault)
                ? commonDefault
                : PermissionFlags.None;
        }

        private static bool TryResolveTablePermission(string roleName, string tableName, out PermissionFlags permission)
        {
            permission = PermissionFlags.None;

            if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(tableName) || RolePermissions == null)
                return false;

            if (!RolePermissions.TryGetValue(roleName, out var permissionsForRole) || permissionsForRole == null)
                return false;

            var explicitPermission = permissionsForRole.FirstOrDefault(p =>
                p != null && string.Equals(p.TableName, tableName, StringComparison.OrdinalIgnoreCase));

            if (explicitPermission != null)
            {
                permission = explicitPermission.Flags;
                return true;
            }

            var wildcardPermission = permissionsForRole.FirstOrDefault(p =>
                p != null && string.Equals(p.TableName, TablePermission.AnyTable, StringComparison.OrdinalIgnoreCase));

            if (wildcardPermission == null)
                return false;

            permission = wildcardPermission.Flags;
            return true;
        }
    }
}
