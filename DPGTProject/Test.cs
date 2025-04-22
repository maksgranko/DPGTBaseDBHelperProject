namespace DPGTProject
{
    internal class Test
    {
        public static bool Initialized = false;
        public static void Init()
        {
            SystemConfig.databaseName = "DPGT_GIBDD";
            SystemConfig.connectionString = Database.ConnectionStringBuilder(SystemConfig.databaseName);
            Initialized = true;
        }
    }
}
