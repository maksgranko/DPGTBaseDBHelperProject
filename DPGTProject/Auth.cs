using System;
using System.Text.RegularExpressions;
using static DPGTProject.Database;

namespace DPGTProject
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
    }

    public static class Auth
    {
        public static string GetUserStatus(string login)
        {
            return Database.Users.GetUserStatus(login);
        }

        public static bool CheckIsUserValid(string login, string password)
        {
            var user = Database.Users.GetByLogin(login);
            if (user == null) return false;

            string storedHash = user[Users.UsersTableColumnsNames["Password"]].ToString();
            string inputHash = SimpleHash(password);

            return storedHash == inputHash;
        }

        public static bool CheckIsUserExists(string login)
        {
            return Database.Users.GetByLogin(login) != null;
        }

        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[!@#$%^&*]).+$");
        }

        public static bool RegisterUser(string login, string password, string role)
        {
            if (CheckIsUserExists(login))
            {
                SystemConfig.lastError = "Пользователь с указанным логином уже существует!";
                return false;
            }

            if (!IsPasswordValid(password))
            {
                SystemConfig.lastError = "Пароль не подходит по условиям!\nУсловия: \nСодержится хотя-бы одна заглавная латинская буква\nСодержится спец. символ\nКоличество символов не менее 8-ми.";
                return false;
            }

            string hashedPassword = SimpleHash(password);
            bool result = Database.Users.Create(login, hashedPassword, role);
            if (result) SystemConfig.lastError = "Произошла ошибка при создании пользователя!";
            return result;
        }

        private static string SimpleHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
