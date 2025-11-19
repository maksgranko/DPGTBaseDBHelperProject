using DPGTProject.Configs;
using DPGTProject.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
            if (!Test.Initialized || UserConfig.userRole != "Администратор") radioButtonExportTables.Visible = false;
        }

        private void DataTimePickerEnable(bool enable) {
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
            if (radioButtonExportTables.Checked && UserConfig.userRole != "Администратор")
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
                    "Отчёт 2"}); // Здесь вы добавляете отчёты(репорты), а ниже инициализируете и программируете
            }
            else
            {
                DataTimePickerEnable(false);
                var filteredTables = SystemConfig.tables
                    .Where(t =>
                    {
                        bool hasAccess = RoleManager.CheckAccess(UserConfig.userRole, t, "read") && RoleManager.CheckAccess(UserConfig.userRole, t, "export");
                        return hasAccess;
                    })
                    .Select(t => SystemConfig.TranslateComboBox(t))
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
                    string tableName = reportTypeComboBox.SelectedItem?.ToString();

                    try
                    {
                        this.dataGridView1.DataSource = reportData = _translatedData;
                    }
                    catch {
                        // TODO: rep[]; 
                    }
                    reportData = Database.Translate(reportData, tableName);

                    if (reportData == null)
                    {
                        MessageBox.Show("Не удалось получить данные таблицы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    _translatedData = reportData;
                    dataGridView1.DataSource = _translatedData;
                }
                else
                {
                    switch (reportTypeComboBox.SelectedItem?.ToString())
                    {
#pragma warning disable CS0162
                        // Здесь алгоритм репортов.
                        case "Отчёт 1":
                            reportData = Database.GetDataTableFromSQL("Здесь_Вы_Задаёте_SQL-запрос, ну это к примеру"); // Получение данных
                            // Примечание: Для дат с-по в SQL используйте переменные dtp(DateTimePicker, intellisense подскажет при написании)
                            reportData = Database.Translate(reportData, "Пример 1");                                    // Как переводить колоны у таблиц
                            throw new NotImplementedException("Задайте корректный алгоритм репорта!");                  // Стереть, после того, как функция будет реализована корректно
                            break;                                                                                      // Ниже можно указать выполнение кода, при выборе любого из отчётов, работает также по названию.
                        case "Отчёт 2":
                            // ...
                            throw new NotImplementedException("Задайте корректный алгоритм репорта!");
                            break;
                        default:
                            MessageBox.Show("Пожалуйста, выберите тип отчёта из списка", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                    }
                    if (reportData == null)
                    {
                        MessageBox.Show("Нет данных для отображения!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    _translatedData = reportData;
                    dataGridView1.DataSource = _translatedData;
#pragma warning restore CS0162
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
                // Здесь можно добавить код, который работает при выборе любого из значений.
                case "Отчёт 1":
                    break;
                case "Отчёт 2":
                    break;
                default:
                    //DataTimePickerEnable(false); // Уберёт DateTimePickе'ры при выборе любого
                    return;
            }
        }

        public void ExportReport(object sender, EventArgs e)
        {
            if (radioButtonExportTables.Checked)
            {
                try
                {
                    // Создаем папку для экспорта
                    string exportDir = Path.Combine(Application.StartupPath, "exports", DateTime.Now.ToString("yyyy-MM-dd"));
                    Directory.CreateDirectory(exportDir);

                    int exportedCount = 0;

                    // Экспортируем каждую таблицу
                    foreach (string tableName in SystemConfig.tables)
                    {
                        try
                        {
                            // Получаем данные таблицы
                            DataTable tableData = Database.GetTableData(tableName);

                            // Создаем Excel файл
                            string filePath = Path.Combine(exportDir, $"{tableName}.xlsx");
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (var excelPackage = new ExcelPackage())
                            {
                                var worksheet = excelPackage.Workbook.Worksheets.Add(tableName);
                                worksheet.Cells["A1"].LoadFromDataTable(tableData, true);
                                worksheet.Cells.AutoFitColumns();
                                excelPackage.SaveAs(new FileInfo(filePath));
                            }
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
                return;
            }

            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Пожалуйста, сначала сформируйте отчет",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx|PDF Files|*.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Use pre-translated data
                    var data = _translatedData;

                    try
                    {
                        if (saveDialog.FilterIndex == 1)
                        {
                            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                            using (var excelPackage = new ExcelPackage())
                            {
                                var worksheet = excelPackage.Workbook.Worksheets.Add("Отчёт");
                                worksheet.Cells["A1"].LoadFromDataTable(data, true);
                                worksheet.Cells.AutoFitColumns();
                                excelPackage.SaveAs(new FileInfo(saveDialog.FileName));
                            }
                        }
                        else
                        {
                            using (var fs = new FileStream(saveDialog.FileName, FileMode.Create))
                            {
                                var document = new Document();
                                var writer = PdfWriter.GetInstance(document, fs);
                                var baseFont = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                document.Open();

                                // Добавляем центрированный заголовок "Отчёт"
                                var title = new Paragraph("Отчёт", new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.BOLD))
                                {
                                    Alignment = Element.ALIGN_CENTER
                                };
                                document.Add(title);

                                // Добавляем пустую строку
                                document.Add(new Paragraph(" "));

                                // Добавляем название отчёта
                                var reportName = new Paragraph(reportTypeComboBox.SelectedItem?.ToString() ?? "Без названия",
                                    new iTextSharp.text.Font(baseFont, 12))
                                {
                                    Alignment = Element.ALIGN_CENTER
                                };
                                document.Add(reportName);

                                document.Add(new Paragraph(" "));

                                var reportDate = new Paragraph("Дата создания отчёта: " + DateTime.Now.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("ru-RU")),
                                    new Font(baseFont, 10))
                                {
                                    Alignment = Element.ALIGN_LEFT
                                };
                                document.Add(reportDate);
                                if (start_date != null || end_date != null)
                                {
                                    reportDate = new Paragraph($"Отчёт {(!(start_date is null) ? "с " + start_date : null)} {(!(end_date is null) ? "по " + end_date : null)}", new Font(baseFont, 10))
                                    {
                                        Alignment = Element.ALIGN_LEFT
                                    };
                                    document.Add(reportDate);
                                }

                                // Добавляем пустую строку перед таблицей
                                document.Add(new Paragraph(" "));

                                var table = new PdfPTable(data.Columns.Count);
                                table.SetWidths(Enumerable.Repeat(1f, data.Columns.Count).ToArray());

                                var boldFont = new Font(baseFont, 10);
                                var normalFont = new Font(baseFont, 9);

                                // Headers
                                foreach (DataColumn col in data.Columns)
                                {
                                    table.AddCell(new Phrase(col.ColumnName, boldFont));
                                }

                                // Data
                                foreach (DataRow row in data.Rows)
                                {
                                    foreach (var item in row.ItemArray)
                                    {
                                        table.AddCell(new Phrase(item?.ToString() ?? "", normalFont));
                                    }
                                }

                                document.Add(table);
                                document.Close();
                            }
                        }

                        MessageBox.Show($"Отчет успешно экспортирован в:\n{saveDialog.FileName}",
                            "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Не удалось экспортировать отчет. Проверьте доступ к выбранной папке.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
