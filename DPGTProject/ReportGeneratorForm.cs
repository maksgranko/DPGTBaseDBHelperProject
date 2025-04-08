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
            "Отчёт 1",
            "Отчёт 2"});
        }

        private DataGridView dataGridView1;
        private ComboBox reportTypeComboBox;
        private Button generate_btn;
        private Button export_btn;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportGeneratorForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.reportTypeComboBox = new System.Windows.Forms.ComboBox();
            this.generate_btn = new System.Windows.Forms.Button();
            this.export_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.Location = new System.Drawing.Point(0, 100);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(500, 300);
            this.dataGridView1.TabIndex = 0;
            // 
            // reportTypeComboBox
            // 
            this.reportTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.reportTypeComboBox.Location = new System.Drawing.Point(10, 10);
            this.reportTypeComboBox.Name = "reportTypeComboBox";
            this.reportTypeComboBox.Size = new System.Drawing.Size(200, 21);
            this.reportTypeComboBox.TabIndex = 1;
            // 
            // generate_btn
            // 
            this.generate_btn.Location = new System.Drawing.Point(12, 37);
            this.generate_btn.Name = "generate_btn";
            this.generate_btn.Size = new System.Drawing.Size(117, 23);
            this.generate_btn.TabIndex = 2;
            this.generate_btn.Text = "Сформировать";
            this.generate_btn.Click += new System.EventHandler(this.GenerateReport);
            // 
            // export_btn
            // 
            this.export_btn.Location = new System.Drawing.Point(135, 37);
            this.export_btn.Name = "export_btn";
            this.export_btn.Size = new System.Drawing.Size(75, 23);
            this.export_btn.TabIndex = 3;
            this.export_btn.Text = "Экспорт";
            this.export_btn.Click += new System.EventHandler(this.ExportReport);
            // 
            // ReportGeneratorForm
            // 
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.reportTypeComboBox);
            this.Controls.Add(this.generate_btn);
            this.Controls.Add(this.export_btn);
            this.Name = "ReportGeneratorForm";
            this.Text = "Генератор отчётов";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }
        private void GenerateReport(object sender, EventArgs e)
        {
            try
            {
                DataTable reportData = null;
                switch (reportTypeComboBox.SelectedItem?.ToString())
                {
                    case "Отчёт 1":
                        reportData = Database.Translate(reportData, "Пример 1");
                        break;
                    case "Отчёт 2":
                        reportData = Database.Translate(reportData, "Пример 2");
                        break;
                    default:
                        MessageBox.Show("Пожалуйста, выберите тип отчёта из списка", 
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }
                if(reportData == null) throw new NullReferenceException("Нет данных для отображения!");
                _translatedData = reportData;
                dataGridView1.DataSource = _translatedData;
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
                            // Excel export
                            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                            using (var excelPackage = new ExcelPackage())
                            {
                                var worksheet = excelPackage.Workbook.Worksheets.Add("Отчёт");
                                worksheet.Cells["A1"].LoadFromDataTable(data, true);
                                excelPackage.SaveAs(new FileInfo(saveDialog.FileName));
                            }
                        }
                        else // PDF
                        {
                            using (var fs = new FileStream(saveDialog.FileName, FileMode.Create))
                            {
                                var document = new Document();
                                var writer = PdfWriter.GetInstance(document, fs);
                                document.Open();

                                var table = new PdfPTable(data.Columns.Count);
                                table.SetWidths(Enumerable.Repeat(1f, data.Columns.Count).ToArray());

                                // Use Arial font with Cyrillic support
                                var baseFont = BaseFont.CreateFont("c:/windows/fonts/arial.ttf",
                                    BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
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
    }
}
