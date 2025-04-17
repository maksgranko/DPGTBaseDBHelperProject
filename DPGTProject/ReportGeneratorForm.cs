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
    public partial class ReportGeneratorForm : Form
    {
        private DataTable _translatedData;

        public ReportGeneratorForm(string[] ReportsNames)
        {
            InitializeComponent();
            this.reportTypeComboBox.Items.AddRange(ReportsNames);
        }
        public ReportGeneratorForm()
        {
            InitializeComponent();
            this.reportTypeComboBox.Items.AddRange(new object[] {
            // Здесь задаются названия репортов
            "Отчёт 1",
            "Отчёт 2"});
        }
        private void GenerateReport(object sender, EventArgs e)
        {
            try
            {
                DataTable reportData = null;
                switch (reportTypeComboBox.SelectedItem?.ToString())
                {
#pragma warning disable CS0162
                    // Здесь алгоритм репортов.
                    case "Отчёт 1":
                        reportData = Database.Translate(reportData, "Пример 1"); // как переводить колоны у таблиц
                        throw new Exception("Задайте корректный алгоритм репорта!");
                        break;
                    case "Отчёт 2":
                        reportData = Database.Translate(reportData, "Пример 2"); // как переводить колоны у таблиц
                        throw new Exception("Задайте корректный алгоритм репорта!");
                        break;
                    default:
                        MessageBox.Show("Пожалуйста, выберите тип отчёта из списка",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }
                if (reportData == null) throw new NullReferenceException("Нет данных для отображения!");
                _translatedData = reportData;
                dataGridView1.DataSource = _translatedData;
#pragma warning restore CS0162
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сформировать отчёт. Обратитесь к администратору. \n Стек ошибки: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportReport(object sender, EventArgs e)
        {
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

                                // Добавляем пустую строку перед датой
                                document.Add(new Paragraph(" "));

                                // Добавляем дату отчёта
                                var reportDate = new Paragraph(DateTime.Now.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("ru-RU")),
                                    new iTextSharp.text.Font(baseFont, 10))
                                {
                                    Alignment = Element.ALIGN_LEFT
                                };
                                document.Add(reportDate);

                                // Добавляем пустую строку перед таблицей
                                document.Add(new Paragraph(" "));

                                var table = new PdfPTable(data.Columns.Count);
                                table.SetWidths(Enumerable.Repeat(1f, data.Columns.Count).ToArray());

                                var boldFont = new iTextSharp.text.Font(baseFont, 10);
                                var normalFont = new iTextSharp.text.Font(baseFont, 9);

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
