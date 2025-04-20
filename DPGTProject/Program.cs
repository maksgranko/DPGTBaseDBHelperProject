using System;
using System.Windows.Forms;

namespace DPGTProject
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Test.Init(); // Инициирует тестовые данные для проекта. Убрать, если не используется(вне разработки) В Test.cs прописываются тестовые значения.
            SystemConfig.Initialize();
            Database.PreCheck();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AuthForm());
        }
    }
}
