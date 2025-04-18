using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DPGTProject.Forms
{
    public partial class UniversalAddEditForm : Form
    {
        public string GeneratedInsertQuery { get; private set; }
        public string GeneratedUpdateQuery { get; private set; }

        private Dictionary<string, object> _columnDefinitions;
        private Dictionary<string, Control> _dynamicControls = new Dictionary<string, Control>();
        private bool _isEditMode;
        private Dictionary<string, object> _existingData;

        private string _tableName;

        public UniversalAddEditForm(Dictionary<string, object> columnDefinitions, string tableName)
        {
            _tableName = tableName;
            InitializeComponent(columnDefinitions, false);
        }

        public UniversalAddEditForm(
            Dictionary<string, object> columnDefinitions,
            Dictionary<string, object> existingData,
            string tableName)
        {
            _existingData = existingData;
            _tableName = tableName;
            InitializeComponent(columnDefinitions, true);
        }

        private void InitializeComponent(Dictionary<string, object> columnDefinitions, bool isEditMode)
        {
            this.SuspendLayout();
            // 
            // UniversalAddEditForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DoubleBuffered = true;
            this.Name = "UniversalAddEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        private void CreateDynamicControls()
        {
            int yOffset = 20;
            int maxLabelWidth = 0;
            var labels = new List<Label>();

            // Сначала создаем все Label чтобы вычислить максимальную ширину
            foreach (var column in _columnDefinitions)
            {
                var label = new Label
                {
                    Text = column.Key,
                    Location = new Point(10, yOffset),
                    AutoSize = true
                };
                labels.Add(label);
                Controls.Add(label);
                maxLabelWidth = Math.Max(maxLabelWidth, label.Width);
                yOffset += 40;
            }

            // Затем создаем контролы ввода с правильным позиционированием
            yOffset = 20;
            for (int i = 0; i < _columnDefinitions.Count; i++)
            {
                var column = _columnDefinitions.ElementAt(i);
                var input = CreateInputControl(column.Key, column.Value);
                input.Location = new Point(maxLabelWidth + 20, yOffset);
                input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                if (_isEditMode && _existingData != null && _existingData.ContainsKey(column.Key))
                {
                    SetControlValue(input, _existingData[column.Key]);
                }

                Controls.Add(input);
                _dynamicControls[column.Key] = input;

                // Настраиваем размер Label под максимальную ширину
                labels[i].Width = maxLabelWidth;
                labels[i].Anchor = AnchorStyles.Top | AnchorStyles.Left;

                yOffset += 40;
            }

            // Автоматический размер формы
            int formWidth = maxLabelWidth + 250; // Ширина label + отступ + ширина контрола + правый отступ
            int formHeight = yOffset + 80; // Высота всех контролов + место для кнопок
            ClientSize = new Size(formWidth, formHeight);
            MinimumSize = new Size(300, 200);
            MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width - 20, Screen.PrimaryScreen.WorkingArea.Height - 20);
        }

        private Control CreateInputControl(string columnName, object type)
        {
#pragma warning disable CS0252
            if (type == typeof(string))
                return new TextBox { Width = 150 };
            if (type == typeof(int))
                return new NumericUpDown
                {
                    Minimum = int.MinValue,
                    Maximum = int.MaxValue,
                    DecimalPlaces = 0,
                    Width = 150
                };
            if (type == typeof(double))
                return new NumericUpDown
                {
                    Minimum = decimal.MinValue,
                    Maximum = decimal.MaxValue,
                    DecimalPlaces = 2,
                    Width = 150
                };
            if (type == typeof(DateTime))
                return new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Width = 150
                };
            if (type == typeof(bool))
                return new CheckBox { Width = 150 };
#pragma warning restore CS0252

            throw new ArgumentException($"Неподдерживаемый тип для {columnName}");
        }

        private void SetControlValue(Control control, object value)
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox.Text = value?.ToString();
                    break;
                case NumericUpDown numericUpDown:
                    numericUpDown.Value = Convert.ToDecimal(value);
                    break;
                case DateTimePicker dateTimePicker:
                    dateTimePicker.Value = Convert.ToDateTime(value);
                    break;
                case CheckBox checkBox:
                    checkBox.Checked = Convert.ToBoolean(value);
                    break;
            }
        }

        private bool ValidateInput()
        {
            foreach (var control in _dynamicControls)
            {
                try
                {
                    object value = GetControlValue(control.Value);

                    if (value == null ||
                        (value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                    {
                        MessageBox.Show($"Поле {control.Key} не может быть пустым");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка валидации {control.Key}: {ex.Message}");
                    return false;
                }
            }
            return true;
        }

        private object GetControlValue(Control control)
        {
            if (control is TextBox textBox)
                return textBox.Text;
            if (control is NumericUpDown numericUpDown)
                return numericUpDown.Value;
            if (control is DateTimePicker dateTimePicker)
                return dateTimePicker.Value;
            if (control is CheckBox checkBox)
                return checkBox.Checked;

            throw new ArgumentException("Неподдерживаемый тип контрола");
        }

        private void ConfigureButtons()
        {
            Button btnSave = new Button
            {
                Text = _isEditMode ? "Изменить" : "Добавить",
                Width = 100,
                DialogResult = DialogResult.OK
            };
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            Button btnClose = new Button
            {
                Text = "Закрыть",
                Width = 100,
                DialogResult = DialogResult.Cancel
            };
            btnClose.Click += (s, e) => Close();
            Controls.Add(btnClose);

            // Расположение кнопок с учетом размера формы
            int buttonY = Height - 70;
            btnSave.Location = new Point(Width / 2 - btnSave.Width - 10, buttonY);
            btnClose.Location = new Point(Width / 2 + 10, buttonY);

            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                GeneratedInsertQuery = GenerateSqlQuery(SqlQueryType.Insert);
                GeneratedUpdateQuery = GenerateSqlQuery(SqlQueryType.Update);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации SQL: {ex.Message}");
                DialogResult = DialogResult.None;
            }
        }

        private enum SqlQueryType { Insert, Update }

        private string GenerateSqlQuery(SqlQueryType queryType)
        {
            if (queryType == SqlQueryType.Insert)
            {
                var columns = new List<string>();
                var values = new List<string>();

                foreach (var control in _dynamicControls)
                {
                    columns.Add(control.Key);
                    values.Add(FormatSqlValue(GetControlValue(control.Value)));
                }

                return $"INSERT INTO %TABLENAME% ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";
            }
            else // Update
            {
                var setParts = new List<string>();
                foreach (var control in _dynamicControls)
                {
                    setParts.Add($"{control.Key} = {FormatSqlValue(GetControlValue(control.Value))}");
                }
                return $"UPDATE %TABLENAME% SET {string.Join(", ", setParts)} WHERE %WHERE%";
            }
        }

        private string FormatSqlValue(object value)
        {
            if (value == null) return "NULL";
            if (value is string) return $"'{value}'";
            if (value is DateTime dt) return $"'{dt:yyyy-MM-dd HH:mm:ss}'";
            return value.ToString();
        }
    }
}
