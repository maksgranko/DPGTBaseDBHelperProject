using DPGTProject.Configs;
using DPGTProject.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DPGTProject
{
    public partial class MainForm : BaseForm
    {
        private string[] tables;
        private Dictionary<Type, Form> openForms = new Dictionary<Type, Form>();

        private void ShowOrActivateForm<T>(params object[] args) where T : Form
        {
            if (openForms.TryGetValue(typeof(T), out var existingForm))
            {
                existingForm.BringToFront();
                if (existingForm.WindowState == FormWindowState.Minimized)
                    existingForm.WindowState = FormWindowState.Normal;
                return;
            }

            var form = (T)Activator.CreateInstance(typeof(T), args);
            form.FormClosed += (s, e) => openForms.Remove(typeof(T));
            openForms[typeof(T)] = form;

            if (!SystemConfig.openEveryWindowInNew) form.ShowDialog();
            else form.Show();
        }
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            hello_lb.Text = "Здравствуй, " + UserConfig.userLogin + "!";
            role_lb.Text = "Ваша роль: " + UserConfig.userRole;

            // В SystemConfig.cs переменная tables содержит названия таблиц, редактировать там
            tables = SystemConfig.tables;
            if (tables == null || tables.Length == 0) throw new NullReferenceException("Заполните список отображаемых таблиц!");
            if (UserConfig.userRole == "Администратор") tables = tables.Append("Users").ToArray();
            //Здесь прописывать условия в зависимости от ролей. Выше пример. Перед прописыванием ролей, их необходимо прописать в SystemConfig.cs
            table_cb.Items.AddRange(tables.Select(t => SystemConfig.TranslateComboBox(t)).ToArray());

            // Проверка прав на экспорт/импорт
            bool hasExportRight = tables.Any(t =>
                RoleManager.CheckAccess(UserConfig.userRole, t, "export"));
            bool hasImportRight = tables.Any(t =>
                RoleManager.CheckAccess(UserConfig.userRole, t, "import"));

            import_btn.Visible = hasImportRight;
            export_btn.Visible = hasExportRight;
        }

        private void unlogin_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы точно желаете выйти из аккаунта?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) return;
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
            if (!tables.Contains(SystemConfig.UntranslateComboBox(table_cb.Text)))
            {
                MessageBox.Show("Выбрана некорректная таблица!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }
            try
            {
                // Получаем оригинальное название таблицы перед открытием
                string tableName = SystemConfig.UntranslateComboBox(table_cb.Text);
                ShowOrActivateForm<UniversalTableViewerForm>(tableName);

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
                ShowOrActivateForm<ReportGeneratorForm>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии отчётов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void import_btn_Click(object sender, EventArgs e)
        {
            try
            {
                ShowOrActivateForm<DataImportForm>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии импорта: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
