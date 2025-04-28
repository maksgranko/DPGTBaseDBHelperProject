using DPGTProject.Configs;
using DPGTProject.Forms;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
                    bool hasAccess = RoleManager.CheckAccess(UserConfig.userRole, t, "import") && RoleManager.CheckAccess(UserConfig.userRole, t, "write");
                    return hasAccess;
                })
                .Select(t => SystemConfig.TranslateComboBox(t))
                .ToArray();
            this.tableComboBox.Items.AddRange(filteredTables);
        }

        private void SelectFile(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (openDialog.FilterIndex == 1) // Excel
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var excel = new ExcelPackage(new FileInfo(openDialog.FileName)))
                        {
                            var worksheet = excel.Workbook.Worksheets.First();
                            _importData = new DataTable();

                            // Заголовки
                            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                            {
                                _importData.Columns.Add(worksheet.Cells[1, col].Text);
                            }

                            // Данные
                            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                            {
                                var newRow = _importData.NewRow();
                                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                                {
                                    newRow[col - 1] = worksheet.Cells[row, col].Text;
                                }
                                _importData.Rows.Add(newRow);
                            }
                        }
                    }
                    else // CSV
                    {
                        _importData = new DataTable();
                        using (var reader = new StreamReader(openDialog.FileName))
                        {
                            string[] headers = reader.ReadLine().Split(',');
                            foreach (string header in headers)
                            {
                                _importData.Columns.Add(header);
                            }

                            while (!reader.EndOfStream)
                            {
                                string[] rows = reader.ReadLine().Split(',');
                                _importData.Rows.Add(rows);
                            }
                        }
                    }

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

            string tableName = SystemConfig.UntranslateComboBox(tableComboBox.Text);
            var dbSchema = Database.GetTableSchema(tableName);

            // Проверка количества колонок
            if (_importData.Columns.Count != dbSchema.Keys.Count)
            {
                MessageBox.Show($"Несовпадение количества колонок: в таблице {dbSchema.Keys.Count}, в файле {_importData.Columns.Count}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                importBtn.Enabled = false;
                return;
            }

            // Проверка названий колонок (учитываем переведенные имена)
            var importColumns = _importData.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var missingColumns = new List<string>();

            // Проверяем каждую колонку из БД
            foreach (var dbColumn in dbSchema.Keys)
            {
                // Ищем оригинальное имя
                bool found = importColumns.Contains(dbColumn);

                // Если не нашли, ищем переведенное имя
                if (!found && SystemConfig.ColumnTranslations.TryGetValue(tableName, out var translations))
                {
                    if (translations.TryGetValue(dbColumn, out var translatedName))
                    {
                        found = importColumns.Contains(translatedName);
                    }
                }

                if (!found)
                {
                    missingColumns.Add(dbColumn);
                }
            }

            if (missingColumns.Any())
            {
                MessageBox.Show($"Отсутствуют обязательные колонки: {string.Join(", ", missingColumns)}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                importBtn.Enabled = false;
                return;
            }

            // Проверка типов данных (учитываем переведенные имена)
            var typeErrors = new List<string>();
            foreach (DataColumn column in _importData.Columns)
            {
                string columnName = column.ColumnName;
                string originalName = columnName;

                // Если имя колонки переведено, получаем оригинальное
                if (SystemConfig.ColumnTranslations.TryGetValue(tableName, out var translations))
                {
                    var reverseTranslations = translations.ToDictionary(x => x.Value, x => x.Key);
                    if (reverseTranslations.TryGetValue(columnName, out var origName))
                    {
                        originalName = origName;
                    }
                }

                if (dbSchema.TryGetValue(originalName, out var dbType))
                {
                    // Простая проверка числовых типов
                    if ((dbType == "int" || dbType == "decimal") &&
                        !double.TryParse(_importData.Rows[0][column].ToString(), out _))
                    {
                        typeErrors.Add($"{columnName}: ожидается {dbType}, получено string");
                    }
                }
            }

            if (typeErrors.Any())
            {
                MessageBox.Show($"Несовместимость типов данных:\n{string.Join("\n", typeErrors)}",
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
                string tableName = SystemConfig.UntranslateComboBox(tableComboBox.Text);
                Database.ImportData(tableName, _importData);

                MessageBox.Show("Данные успешно импортированы",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
