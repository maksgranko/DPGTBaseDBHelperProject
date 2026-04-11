using DPGTProject.Configs;
using MSSQL = Scraps.Databases.MSSQL;
using DPGTProject.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PermissionFlags = Scraps.Security.PermissionFlags;
using ScrapsRoleManager = Scraps.Security.RoleManager;
using Scraps.Localization;
using Scraps.Security;

namespace DPGTProject
{
    public partial class ReportGeneratorForm : BaseForm
    {
        public DataTable _translatedData;
        public string start_date = null;
        public string end_date = null;

        public ReportGeneratorForm(string[] ReportsNames) : this()
        {
            this.reportTypeComboBox.Items.AddRange(ReportsNames);
        }

        public ReportGeneratorForm()
        {
            InitializeComponent();
            ReportTypeChanged(null, null);
            if (!Test.Initialized || UserSession.UserRole != "Администратор") radioButtonExportTables.Visible = false;
        }

        private void DataTimePickerEnable(bool enable)
        {
            to_lb.Visible = enable;
            from_lb.Visible = enable;
            start_dtp.Visible = enable;
            end_dtp.Visible = enable;
        }

        private void ReportTypeChanged(object sender, EventArgs e)
        {
            start_date = null;
            end_date = null;
            reportTypeComboBox.Items.Clear();

            generate_btn.Enabled = true;
            reportTypeComboBox.Enabled = true;
            if (radioButtonExportTables.Checked && UserSession.UserRole != "Администратор")
            {
                DataTimePickerEnable(false);
                MessageBox.Show("Экспорт всех таблиц доступен только администраторам",
                    "Ошибка прав", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (radioButtonExportTables.Checked)
            {
                DataTimePickerEnable(false);
                generate_btn.Enabled = false;
                reportTypeComboBox.Enabled = false;
            }
            else if (radioPredefinedReport.Checked)
            {
                DataTimePickerEnable(true);
                reportTypeComboBox.Items.AddRange(new object[] {
                    "Отчёт 1",
                    "Отчёт 2"});
            }
            else
            {
                DataTimePickerEnable(false);
                var filteredTables = SystemConfig.tables
                    .Where(t =>
                    {
                        var permissions = ScrapsRoleManager.GetEffectivePermissions(UserSession.UserRole, t);
                        return (permissions & (PermissionFlags.Read | PermissionFlags.Export)) ==
                               (PermissionFlags.Read | PermissionFlags.Export);
                    })
                    .Select(TranslationManager.Translate)
                    .ToArray();
                reportTypeComboBox.Items.AddRange(filteredTables);
            }
        }

        public void GenerateReport(object sender, EventArgs e)
        {
            start_date = start_dtp.Value.ToShortDateString();
            end_date = end_dtp.Value.ToShortDateString();
            try
            {
                DataTable reportData = null;

                if (radioNormalTable.Checked)
                {
                    string selected = reportTypeComboBox.SelectedItem?.ToString();
                    if (string.IsNullOrWhiteSpace(selected))
                    {
                        MessageBox.Show("Пожалуйста, выберите таблицу из списка.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string tableName = TranslationManager.Untranslate(selected);
                    reportData = MSSQL.GetTableData(tableName);
                    if (reportData == null)
                    {
                        MessageBox.Show("Не удалось получить данные таблицы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    reportData = TranslationManager.Translate(reportData, tableName);
                    dataGridView1.DataSource = _translatedData = reportData;
                }
                else
                {
                    switch (reportTypeComboBox.SelectedItem?.ToString())
                    {
#pragma warning disable CS0162
                        case "Отчёт 1":
                            reportData = MSSQL.GetDataTableFromSQL("Здесь_Вы_Задаёте_SQL-запрос, ну это к примеру");
                            reportData = TranslationManager.Translate(reportData, "Пример 1");
                            throw new NotImplementedException("Задайте корректный алгоритм репорта!");
                        case "Отчёт 2":
                            throw new NotImplementedException("Задайте корректный алгоритм репорта!");
                        default:
                            MessageBox.Show("Пожалуйста, выберите тип отчёта из списка", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                    }
#pragma warning restore CS0162

                    if (reportData == null)
                    {
                        MessageBox.Show("Нет данных для отображения!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    dataGridView1.DataSource = _translatedData = reportData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сформировать отчёт. Обратитесь к администратору. \n Стек ошибки: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void reportTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (reportTypeComboBox.SelectedItem?.ToString())
            {
                case "Отчёт 1":
                    break;
                case "Отчёт 2":
                    break;
                default:
                    return;
            }
        }

        public void ExportReport(object sender, EventArgs e)
        {
            if (radioButtonExportTables.Checked)
            {
                ExportAllTables();
                return;
            }

            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Пожалуйста, сначала сформируйте отчёт",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var saveDialog = new SaveFileDialog { Filter = "Excel Files|*.xlsx|PDF Files|*.pdf" })
                {
                    if (saveDialog.ShowDialog() != DialogResult.OK)
                        return;

                    var data = _translatedData;
                    if (data == null)
                    {
                        MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (saveDialog.FilterIndex == 1)
                    {
                        ExportDataTableToExcel(data, saveDialog.FileName, "Отчёт");
                    }
                    else
                    {
                        ExportDataTableToPdf(data, saveDialog.FileName);
                    }

                    MessageBox.Show($"Отчет успешно экспортирован в:\n{saveDialog.FileName}",
                        "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportAllTables()
        {
            try
            {
                string exportDir = BuildExportDirectoryPath();
                Directory.CreateDirectory(exportDir);

                int exportedCount = 0;
                foreach (string tableName in SystemConfig.tables)
                {
                    try
                    {
                        DataTable tableData = MSSQL.GetTableData(tableName);
                        string filePath = Path.Combine(exportDir, $"{tableName}.xlsx");
                        ExportDataTableToExcel(tableData, filePath, tableName);
                        exportedCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте таблицы {tableName}:\n{ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                MessageBox.Show($"Успешно экспортировано {exportedCount} таблиц в:\n{exportDir}",
                    "Экспорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте таблиц:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string BuildExportDirectoryPath()
        {
            return Path.Combine(Application.StartupPath, "exports", DateTime.Now.ToString("yyyy-MM-dd"));
        }

        private static void ExportDataTableToExcel(DataTable data, string filePath, string sheetName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1"].LoadFromDataTable(data, true);
                worksheet.Cells.AutoFitColumns();
                excelPackage.SaveAs(new FileInfo(filePath));
            }
        }

        private void ExportDataTableToPdf(DataTable data, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                var document = new Document();
                PdfWriter.GetInstance(document, fs);
                var baseFont = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                document.Open();

                AddPdfTitleBlock(document, baseFont);
                AddPdfTable(document, data, baseFont);
                document.Close();
            }
        }

        private void AddPdfTitleBlock(Document document, BaseFont baseFont)
        {
            var title = new Paragraph("Отчёт", new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER
            };
            document.Add(title);
            document.Add(new Paragraph(" "));

            var reportName = new Paragraph(reportTypeComboBox.SelectedItem?.ToString() ?? "Без названия",
                new iTextSharp.text.Font(baseFont, 12))
            {
                Alignment = Element.ALIGN_CENTER
            };
            document.Add(reportName);
            document.Add(new Paragraph(" "));

            var reportDate = new Paragraph("Дата создания отчёта: " + DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")),
                new Font(baseFont, 10))
            {
                Alignment = Element.ALIGN_LEFT
            };
            document.Add(reportDate);

            if (start_date != null || end_date != null)
            {
                var period = new Paragraph($"Отчёт {(!(start_date is null) ? "с " + start_date : null)} {(!(end_date is null) ? "по " + end_date : null)}",
                    new Font(baseFont, 10))
                {
                    Alignment = Element.ALIGN_LEFT
                };
                document.Add(period);
            }

            document.Add(new Paragraph(" "));
        }

        private static void AddPdfTable(Document document, DataTable data, BaseFont baseFont)
        {
            var table = new PdfPTable(data.Columns.Count);
            table.SetWidths(Enumerable.Repeat(1f, data.Columns.Count).ToArray());

            var headerFont = new Font(baseFont, 10);
            var normalFont = new Font(baseFont, 9);

            foreach (DataColumn col in data.Columns)
            {
                table.AddCell(new Phrase(col.ColumnName, headerFont));
            }

            foreach (DataRow row in data.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    table.AddCell(new Phrase(item?.ToString() ?? string.Empty, normalFont));
                }
            }

            document.Add(table);
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
