namespace DPGTProject
{
    internal class Test
    {
        public static bool Initialized = false;
        public static string login;
        public static string password;
        public static void Init()
        {
            SystemConfig.addRolesWhenRegistering = true;
            SystemConfig.databaseName = "OnlineStoreDB"; // Имя тестового БД
            SystemConfig.connectionString = Database.ConnectionStringBuilder(SystemConfig.databaseName); // Переинициализация БД
            // Database.Users.Create(Test.login, Test.password, SystemConfig.roles[0]); // создать тест-юзера
            Test.login = "testAdmin1"; // Тестовый автологин
            Test.password = "Qwe123123@";  // Тестовый автопароль
            Initialized = true;
        }
    }
}
