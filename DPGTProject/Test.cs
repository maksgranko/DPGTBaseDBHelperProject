namespace DPGTProject
{
    internal class Test
    {
        public static bool Initialized = false;
        public static void Init()
        {
            SystemConfig.databaseName = "DPGT_GIBDD";
            SystemConfig.tables = new string[] { "DocumentHistory", "Documents", "Fines", "Owners", "Users", "Violations" };
            SystemConfig.connectionString = Database.ConnectionStringBuilder(SystemConfig.databaseName);
            Initialized = true;
        }
    }
}
