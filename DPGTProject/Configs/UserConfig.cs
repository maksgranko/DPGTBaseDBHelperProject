using Scraps.Security;
using System.Data;

namespace DPGTProject
{
    internal static class UserConfig
    {
        public static int userId => UserSession.UserId;
        public static string userLogin => UserSession.UserLogin;
        public static string userRole => UserSession.UserRole;
        public static DataRow userData => UserSession.UserData;

        public static void ReparseConfig()
        {
            UserSession.Reload();
        }

        public static void Login(string login)
        {
            UserSession.LoginByName(login);
        }

        public static void Logout()
        {
            UserSession.Logout();
        }
    }
}
