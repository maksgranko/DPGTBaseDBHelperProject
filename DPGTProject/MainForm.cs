﻿using System;
using System.Linq;
using System.Windows.Forms;

namespace DPGTProject
{
    public partial class MainForm : Form
    {
        private string[] tables;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            hello_lb.Text = "Здравствуй, " + UserConfig.userLogin + "!";
            role_lb.Text = "Ваша роль: " + UserConfig.userRole;

            //tables = new string[] { "Documents", "DocumentHistory", "Fines", "Owners", "Violations" };
            // Здесь названия таблиц, которые необходимо использовать, названия брать из БД, пример заполнения выше
            tables = null;
            if (tables == null || tables.Length == 0) throw new NullReferenceException("Заполните список отображаемых таблиц!");
            if (UserConfig.userRole == "Администратор") tables = tables.Append("Users").ToArray();
            table_cb.Items.AddRange(tables.Select(t => SystemConfig.TranslateComboBox(t)).ToArray());
        }

        private void unlogin_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы точно желаете выйти из аккаунта?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) { return; }
            UserConfig.Logout();
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UserConfig.Logout();
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            SystemConfig.LastLocation = this.Location;
        }

        private void dev_btn_Click(object sender, EventArgs e)
        {
            Functions.Developers();
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            Functions.Exit();
        }

        private void tables_Click(object sender, EventArgs e)
        {
            if (!tables.Contains(SystemConfig.UntranslateComboBox(table_cb.Text))) { MessageBox.Show("Выбрана некорректная таблица!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            try
            {
                // Получаем оригинальное название таблицы перед открытием
                string tableName = SystemConfig.UntranslateComboBox(table_cb.Text);
                var form = new UniversalTableViewerForm(tableName);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии таблиц: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void reports_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new ReportGeneratorForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии отчётов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
