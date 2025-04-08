﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPGTProject
{
    internal static class Functions
    {
        public static void Developers()
        {
            MessageBox.Show("Разработчики: \n- Чечитов Глеб\n- Моисеев Николай", 
                "О проекте", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void Exit()
        {
            Application.Exit();
        }
        public static bool IsRoleExists(string value)
        {
            switch(value){
                case "Администратор":
                case "Менеджер":
                    return true;
                default: return false;
            }
        }
    }
}
