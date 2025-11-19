using DPGTProject.Databases;

namespace DPGTProject
{
    internal class Test
    {
        public static bool Initialized = true;
        public static string login;
        public static string password;
        public static void Init()
        {
            SystemConfig.addRolesWhenRegistering = true;
            SystemConfig.databaseName = ""; // Имя тестового БД
            SystemConfig.connectionString = MSSQL.ConnectionStringBuilder(SystemConfig.databaseName); // Переинициализация БД
            // Database.Users.Create(Test.login, Test.password, SystemConfig.roles[0]); // создать тест-юзера
            Test.login = "q"; // Тестовый автологин
            Test.password = "Qwe123123@";  // Тестовый автопароль
            Initialized = true;
        }
    }
}
