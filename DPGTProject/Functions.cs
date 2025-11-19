using System;
using System.Linq;
using System.Windows.Forms;

namespace DPGTProject
{
    internal static class Functions
    {
        public static void Developers()
        {
            MessageBox.Show("Разработчики: \n- Гранько Максим", "О проекте", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        
        public static void Exit()
        {
            Application.Exit();
        }
    }
}
