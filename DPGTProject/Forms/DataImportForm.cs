using DPGTProject.Configs;
using DPGTProject.Forms;
using Scraps.Import;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Scraps.Localization;
using PermissionFlags = Scraps.Security.PermissionFlags;
using Scraps.Security;

namespace DPGTProject
{
    public partial class DataImportForm : BaseForm
    {
        private DataTable _importData;

        public DataImportForm()
        {
            InitializeComponent();
            if (!SystemConfig.moreExitButtons) { exit_btn.Visible = false; }

            var filteredTables = SystemConfig.tables
                .Where(t =>
                {
                    var permissions = SystemConfig.GetEffectivePermissions(UserSession.UserRole, t);
                    return (permissions & (PermissionFlags.Import | PermissionFlags.Write)) ==
                           (PermissionFlags.Import | PermissionFlags.Write);
                })
                .Select(TranslationManager.Translate)
                .ToArray();

            tableComboBox.Items.AddRange(filteredTables);
        }

        private void SelectFile(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";

                if (openDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    _importData = openDialog.FilterIndex == 1
                        ? DataImportService.LoadExcelToDataTable(openDialog.FileName)
                        : DataImportService.LoadCsvToDataTable(openDialog.FileName, new[] { ',', ';', '\t' }, autoDetectDelimiter: true);

                    dataGridView1.DataSource = _importData;
                    importBtn.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки файла: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PreviewData(object sender, EventArgs e)
        {
            if (_importData == null || tableComboBox.SelectedItem == null)
            {
                MessageBox.Show("Сначала выберите файл и таблицу",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tableName = TranslationManager.Untranslate(tableComboBox.Text);

            if (!DataImportService.ValidateImport(_importData, tableName, out var errors, allowTranslatedColumns: true))
            {
                MessageBox.Show(string.Join("\n", errors),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                importBtn.Enabled = false;
                return;
            }

            importBtn.Enabled = true;
        }

        private void ImportData(object sender, EventArgs e)
        {
            if (_importData == null || tableComboBox.SelectedItem == null)
                return;

            try
            {
                string tableName = TranslationManager.Untranslate(tableComboBox.Text);
                var required = PermissionFlags.Import | PermissionFlags.Write;
                if (!SystemConfig.HasPermission(UserSession.UserRole, tableName, required))
                {
                    MessageBox.Show("У вас нет прав на импорт в эту таблицу",
                        "Ошибка прав", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!DataImportService.ValidateImport(_importData, tableName, out var errors, allowTranslatedColumns: true))
                {
                    MessageBox.Show(string.Join("\n", errors),
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataImportService.ImportToTable(tableName, _importData);

                MessageBox.Show("Данные успешно импортированы",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
