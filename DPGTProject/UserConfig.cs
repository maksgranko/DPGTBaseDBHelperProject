namespace DPGTProject
{
    internal static class UserConfig
    {
        public static string userLogin = "";
        public static string userRole = "";

        public static void Logout()
        {
            userLogin = "";
            userRole = "";
        }
    }
}
