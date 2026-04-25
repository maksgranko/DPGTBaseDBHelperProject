using DPGTProject.Configs;
using DPGTProject.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Scraps.Localization;
using PermissionFlags = Scraps.Security.PermissionFlags;
using Scraps.Database.MSSQL;
using Scraps.Security;

namespace DPGTProject
{
    public partial class MainForm : BaseForm
    {
        private string[] tables;
        private readonly Dictionary<Type, Form> openForms = new Dictionary<Type, Form>();

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
            hello_lb.Text = "Здравствуй, " + UserSession.UserLogin + "!";
            role_lb.Text = "Ваша роль: " + UserSession.UserRole;

            // Базовые таблицы уже подготовлены в SystemConfig.Initialize().
            tables = (SystemConfig.tables ?? Array.Empty<string>())
                .Concat(SystemConfig.virtualTables ?? Array.Empty<string>())
                .Distinct()
                .ToArray();

            if (tables.Length == 0)
                throw new NullReferenceException("Не найдено ни одной таблицы!");

            var accessibleTables = new List<string>();
            bool hasExportRight = false;
            bool hasImportRight = false;
            foreach (var table in tables)
            {
                var permissions = SystemConfig.GetEffectivePermissions(UserSession.UserRole, table);
                if ((permissions & PermissionFlags.Read) == 0)
                    continue;

                if ((SystemConfig.virtualTables ?? Array.Empty<string>()).Contains(table) &&
                    !VirtualTableRegistry.CheckAccess(table, UserSession.UserRole, PermissionFlags.Read, out _))
                    continue;

                accessibleTables.Add(table);
                hasExportRight |= (permissions & PermissionFlags.Export) != 0;
                hasImportRight |= (permissions & PermissionFlags.Import) != 0;
            }

            tables = accessibleTables.Distinct().ToArray();

            // Для отладки: логируем доступные таблицы
            System.Diagnostics.Debug.WriteLine($"Доступные таблицы для {UserSession.UserRole}: {string.Join(", ", tables)}");

            table_cb.Items.AddRange(tables.Select(TranslationManager.Translate).ToArray());

            import_btn.Visible = hasImportRight;
            export_btn.Visible = hasExportRight;
        }

        public void unlogin_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы точно желаете выйти из аккаунта?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) return;

            // Закрываем все открытые формы
            foreach (var form in openForms.Values.ToList())
            {
                form.Close();
            }
            openForms.Clear();

            // Очищаем combobox
            table_cb.Items.Clear();

            UserSession.Logout();
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UserSession.Logout();
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            // Reserved for future UI state persistence.
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
            var selectedTable = TranslationManager.Untranslate(table_cb.Text);
            if (string.IsNullOrWhiteSpace(selectedTable))
            {
                MessageBox.Show("Выберите таблицу из списка.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!tables.Contains(selectedTable))
            {
                MessageBox.Show("Выбрана некорректная таблица!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }
            try
            {
                var virtualTables = SystemConfig.virtualTables ?? Array.Empty<string>();
                if (virtualTables.Contains(selectedTable))
                {
                    ShowOrActivateForm<UniversalTableViewerForm>(selectedTable, true);
                }
                else
                {
                    ShowOrActivateForm<UniversalTableViewerForm>(selectedTable);
                }
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

