namespace DPGTProject
{
    internal class Test
    {
        public static bool Initialized = true;
        public static string login = "q";
        public static string password = "Qwe123123@";

        public static void Init()
        {
            // Тестовый автозаполнитель формы авторизации.
            Initialized = !string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password);
        }
    }
}
