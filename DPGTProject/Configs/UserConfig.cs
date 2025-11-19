using System.Data;

namespace DPGTProject
{
    internal static class UserConfig
    {
        public static int userId = -1;
        public static string userLogin = "";
        public static string userRole = "";
        
        public static DataRow userData;

        public static void ReparseConfig()
        {
            string tempLogin = userLogin;
            Logout();
            Login(tempLogin);
        }
        public static void Login(string login)
        {
            userLogin = login;
            userRole = Auth.GetUserStatus(login);
            userData = Database.Users.GetByLogin(login);
            userId = (int)userData[0];
        }

        public static void Logout()
        {
            userId = -1;
            userLogin = "";
            userRole = "";
            userData = null;
        }
    }
}
