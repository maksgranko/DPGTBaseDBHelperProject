using System;
using System.Collections.Generic;
using System.Linq;

namespace DPGTProject.Configs
{
    public class TablePermission
    {
        public string TableName { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public bool CanImport { get; set; }
    }

    public class Role
    {
        public string Name { get; set; }
        public List<TablePermission> TablePermissions { get; set; } = new List<TablePermission>();

        public bool HasPermission(string tableName, string permissionType)
        {
            var permission = TablePermissions.FirstOrDefault(p =>
                p.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase));

            if (permission == null) return false;

            switch (permissionType)
            {
                case "read":
                    return permission.CanRead;
                case "write":
                    return permission.CanWrite;
                case "delete":
                    return permission.CanDelete;
                case "export":
                    return permission.CanExport;
                case "import":
                    return permission.CanImport;
                default:
                    return false;
            }
        }
    }

    public static class RoleManager
    {
        private static readonly Dictionary<string, Role> _roles = new Dictionary<string, Role>();

        public static void Initialize(IEnumerable<Role> roles)
        {
            foreach (var role in roles)
            {
                _roles[role.Name] = role;
            }
        }

        public static void Initialize()
        {
            foreach (var rolePerm in SystemConfig.RolePermissions)
            {
                var role = new Role
                {
                    Name = rolePerm.Key,
                    TablePermissions = rolePerm.Value
                };
                _roles[role.Name] = role;
            }

            // Применяем права по умолчанию ко всем таблицам
            foreach (var table in SystemConfig.tables)
            {
                ApplyDefaultPermissions(table);
            }
        }

        public static void ApplyDefaultPermissions(string tableName)
        {
            foreach (var role in _roles.Values)
            {
                if (!role.TablePermissions.Any(p => p.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)))
                {
                    var defaultPerm = new TablePermission
                    {
                        TableName = tableName
                    };

                    if (SystemConfig.DefaultRolePermissions.TryGetValue(role.Name, out var roleDefaults))
                    {
                        defaultPerm.CanRead = roleDefaults.CanRead;
                        defaultPerm.CanWrite = roleDefaults.CanWrite;
                        defaultPerm.CanDelete = roleDefaults.CanDelete;
                        defaultPerm.CanExport = roleDefaults.CanExport;
                        defaultPerm.CanImport = roleDefaults.CanImport;
                    }
                    else
                    {
                        var globalDefaults = SystemConfig.DefaultRolePermissions["default"];
                        defaultPerm.CanRead = globalDefaults.CanRead;
                        defaultPerm.CanWrite = globalDefaults.CanWrite;
                        defaultPerm.CanDelete = globalDefaults.CanDelete;
                        defaultPerm.CanExport = globalDefaults.CanExport;
                        defaultPerm.CanImport = globalDefaults.CanImport;
                    }
                    role.TablePermissions.Add(defaultPerm);
                }
            }
        }

        public static bool CheckAccess(string roleName, string tableName, string permissionType)
        {
            if (!_roles.TryGetValue(roleName, out var role))
                return false;

            return role.HasPermission(tableName, permissionType);
        }

        public static void AddRole(Role role)
        {
            _roles[role.Name] = role;
        }

        public static Role GetRole(string roleName)
        {
            return _roles.TryGetValue(roleName, out var role) ? role : null;
        }
        public static bool IsRoleExists(string value)
        {
            if (SystemConfig.roles.Contains(value)) return true;
            else return false;
        }
    }
}
