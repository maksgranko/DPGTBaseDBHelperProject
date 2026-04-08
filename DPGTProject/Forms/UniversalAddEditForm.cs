using MSSQL = Scraps.Databases.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Scraps.Localization;

namespace DPGTProject.Forms
{
    public partial class UniversalAddEditForm : BaseForm
    {
        public Dictionary<string, object> SubmittedValues { get; private set; }

        private Dictionary<string, object> _columnDefinitions;
        private Dictionary<string, Control> _dynamicControls = new Dictionary<string, Control>();
        private bool _isEditMode;
        private Dictionary<string, object> _existingData;
        private Button _btnSave;
        private Button _btnClose;

        private string _tableName;

        public UniversalAddEditForm(Dictionary<string, object> columnDefinitions, string tableName)
        {
            try
            {
                _tableName = tableName;
                InitializeComponent();
                InitializeDynamicForm(columnDefinitions, false);
            }
            catch
            {
                DialogResult = DialogResult.Cancel;
                this.Dispose();
            }
        }

        public UniversalAddEditForm(
            Dictionary<string, object> columnDefinitions,
            Dictionary<string, object> existingData,
            string tableName)
        {
            try
            {
                _existingData = existingData;
                _tableName = tableName;
                InitializeComponent();
                InitializeDynamicForm(columnDefinitions, true);
            }
            catch
            {
                DialogResult = DialogResult.Cancel;
                this.Dispose();
            }
        }

        private void InitializeDynamicForm(Dictionary<string, object> columnDefinitions, bool isEditMode)
        {
            _columnDefinitions = columnDefinitions;
            _isEditMode = isEditMode;

            this.SuspendLayout();
            string translatedTableName = TranslationManager.Translate(_tableName);
            this.Text = _isEditMode ? $"Редактирование: {translatedTableName}" : $"Добавление: {translatedTableName}";
            this.Resize += (s, e) => LayoutBottomButtons();


            try
            {
                CreateDynamicControls();
                ConfigureButtons();
            }
            catch
            {
                MessageBox.Show(_isEditMode ?
                    "Данную строку невозможно отредактировать." :
                    "Здесь невозможно добавить строку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (!this.IsDisposed) this.Close();
            }

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
                string translatedColumnName = TranslationManager.TranslateColumnName(_tableName, column.Key);

                var label = new Label
                {
                    Text = translatedColumnName,
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
            if (TryCreateForeignKeyControl(columnName, out var fkControl))
            {
                return MSSQL.IsNullableColumn(_tableName, columnName)
                    ? WrapWithNullablePanel(fkControl)
                    : fkControl;
            }

            // Для bool полей возвращаем обычный CheckBox
            if ((Type)type == typeof(bool))
                return new CheckBox { Width = 150 };

            // Создаем основной контрол ввода
            Control inputControl;
#pragma warning disable CS0252
            if (type == typeof(string))
                inputControl = new TextBox { Width = 150 };
            else if (type == typeof(int))
                inputControl = new NumericUpDown
                {
                    Minimum = int.MinValue,
                    Maximum = int.MaxValue,
                    DecimalPlaces = 0,
                    Width = 150
                };
            else if (type == typeof(byte))
                inputControl = new NumericUpDown
                {
                    Minimum = byte.MinValue,
                    Maximum = byte.MaxValue,
                    DecimalPlaces = 0,
                    Width = 150
                };
            else if (type == typeof(double) || type == typeof(decimal) || type == typeof(float))
                inputControl = new NumericUpDown
                {
                    Minimum = decimal.MinValue,
                    Maximum = decimal.MaxValue,
                    DecimalPlaces = 2,
                    Width = 150
                };
            else if (type == typeof(DateTime))
                inputControl = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Width = 150
                };
            else
            {
                SystemConfig.lastError = $"Неподдерживаемый тип для {columnName}";
                throw new ArgumentException(SystemConfig.lastError);
            }
#pragma warning restore CS0252

            // Проверяем, поддерживает ли колонка NULL значения
            if (MSSQL.IsNullableColumn(_tableName, columnName))
                return WrapWithNullablePanel(inputControl);

            return inputControl;
        }

        private static Panel WrapWithNullablePanel(Control inputControl)
        {
            var container = new Panel
            {
                Width = 180,
                Height = 30
            };

            inputControl.Width = 120;
            inputControl.Left = 0;
            container.Controls.Add(inputControl);

            var nullCheckBox = new CheckBox
            {
                Text = "NULL",
                Left = 125,
                Width = 50,
                Checked = false
            };

            nullCheckBox.CheckedChanged += (sender, e) =>
            {
                inputControl.Enabled = !nullCheckBox.Checked;
                if (!nullCheckBox.Checked) return;

                if (inputControl is TextBox textBox) textBox.Text = string.Empty;
                else if (inputControl is NumericUpDown numeric) numeric.Value = 0;
                else if (inputControl is DateTimePicker datePicker) datePicker.Value = DateTime.Now;
                else if (inputControl is ComboBox comboBox)
                {
                    if (comboBox.Items.Count > 0) comboBox.SelectedIndex = -1;
                    else comboBox.SelectedItem = null;
                }
            };

            container.Controls.Add(nullCheckBox);
            return container;
        }

        private bool TryCreateForeignKeyControl(string columnName, out Control control)
        {
            control = null;

            try
            {
                var fk = MSSQL.GetForeignKeys(_tableName, null, null)
                    .FirstOrDefault(x => string.Equals(x.Column, columnName, StringComparison.OrdinalIgnoreCase));
                if (fk == null)
                    return false;

                string overrideKey = $"{_tableName}.{columnName}";
                string displayColumn = null;
                if (SystemConfig.ForeignKeyDisplayColumnOverrides.TryGetValue(overrideKey, out var configuredDisplay) &&
                    !string.IsNullOrWhiteSpace(configuredDisplay))
                {
                    displayColumn = configuredDisplay;
                }

                var data = MSSQL.GetForeignKeyLookup(_tableName, columnName, displayColumn, null, null, null);
                if (data == null || !data.Columns.Contains("Value"))
                    return false;

                if (!data.Columns.Contains("DisplayWithId"))
                    data.Columns.Add("DisplayWithId", typeof(string));

                foreach (DataRow row in data.Rows)
                {
                    string valueText = row["Value"] == DBNull.Value || row["Value"] == null
                        ? string.Empty
                        : row["Value"].ToString();
                    string displayText = data.Columns.Contains("Display") && row["Display"] != DBNull.Value && row["Display"] != null
                        ? row["Display"].ToString()
                        : valueText;

                    row["DisplayWithId"] = $"{displayText}[{valueText}]";
                }

                var combo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 150,
                    DataSource = data,
                    ValueMember = "Value",
                    DisplayMember = "DisplayWithId"
                };

                control = combo;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetControlValue(Control control, object value)
        {
            // Если это Panel с чекбоксом NULL
            if (control is Panel panel && panel.Controls.Count == 2)
            {
                var inputControl = panel.Controls[0];
                var nullCheckBox = panel.Controls[1] as CheckBox;

                if (value == null || value is DBNull)
                {
                    nullCheckBox.Checked = true;
                    inputControl.Enabled = false;
                    return;
                }

                nullCheckBox.Checked = false;
                inputControl.Enabled = true;
                SetControlValue(inputControl, value);
                return;
            }

            switch (control)
            {
                case TextBox textBox:
                    textBox.Text = value?.ToString();
                    break;
                case NumericUpDown numericUpDown:
                    numericUpDown.Value = value == null ? 0 : Convert.ToDecimal(value);
                    break;
                case DateTimePicker dateTimePicker:
                    dateTimePicker.Value = value == null ? DateTime.Now : Convert.ToDateTime(value);
                    break;
                case CheckBox checkBox:
                    checkBox.Checked = value != null && Convert.ToBoolean(value);
                    break;
                case ComboBox comboBox:
                    if (comboBox.DataSource != null && !string.IsNullOrWhiteSpace(comboBox.ValueMember))
                        comboBox.SelectedValue = value;
                    else
                        comboBox.SelectedItem = value;
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

                    // Для Panel с чекбоксом NULL пропускаем проверку
                    if (control.Value is Panel panel)
                    {
                        var checkbox = panel.Controls.OfType<CheckBox>().FirstOrDefault();
                        if (checkbox != null && checkbox.Checked) continue;
                    }

                    if (value == null ||
                        (value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                    {
                        MessageBox.Show($"Поле {control.Key} не может быть пустым");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка валивации {control.Key}: {ex.Message}");
                    return false;
                }
            }
            return true;
        }

        private object GetControlValue(Control control)
        {
            // Если это Panel с чекбоксом NULL и вложенным контролом
            if (control is Panel panel && panel.Controls.Count == 2)
            {
                var inputControl = panel.Controls[0];
                var nullCheckBox = panel.Controls[1] as CheckBox;

                // Если чекбокс NULL отмечен - возвращаем null
                if (nullCheckBox != null && nullCheckBox.Checked)
                    return null;

                // Для Foreign Key проверяем специальные условия
                if (inputControl is ComboBox cb && cb.SelectedItem == null)
                    return null;

                control = inputControl;
            }
            if (control is TextBox textBox)
                return string.IsNullOrEmpty(textBox.Text) ? null : textBox.Text;
            if (control is NumericUpDown numericUpDown)
                return numericUpDown.Value;
            if (control is DateTimePicker dateTimePicker)
                return dateTimePicker.Value;
            if (control is CheckBox checkBox)
                return checkBox.Checked;
            if (control is ComboBox comboBox)
            {
                if (comboBox.DataSource != null && !string.IsNullOrWhiteSpace(comboBox.ValueMember))
                    return comboBox.SelectedValue;
                return comboBox.SelectedItem;
            }

            throw new ArgumentException("Неподдерживаемый тип контрола");
        }

        private void ConfigureButtons()
        {
            _btnSave = new Button
            {
                Text = _isEditMode ? "Изменить" : "Добавить",
                Width = 100,
                DialogResult = DialogResult.OK
            };
            _btnSave.Click += BtnSave_Click;
            _btnSave.Anchor = AnchorStyles.Bottom;
            Controls.Add(_btnSave);

            _btnClose = new Button
            {
                Text = "Закрыть",
                Width = 100,
                DialogResult = DialogResult.Cancel
            };
            _btnClose.Click += (s, e) => Close();
            _btnClose.Anchor = AnchorStyles.Bottom;
            Controls.Add(_btnClose);

            LayoutBottomButtons();
        }

        private void LayoutBottomButtons()
        {
            if (_btnSave == null || _btnClose == null)
                return;

            const int spacing = 12;
            const int bottomMargin = 16;
            int totalWidth = _btnSave.Width + spacing + _btnClose.Width;
            int startX = (ClientSize.Width - totalWidth) / 2;
            int y = ClientSize.Height - _btnSave.Height - bottomMargin;

            _btnSave.Location = new Point(startX, y);
            _btnClose.Location = new Point(startX + _btnSave.Width + spacing, y);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                SubmittedValues = BuildSubmittedValues();
            }
            catch (Exception ex)
            {
                string errorDetails = $"Ошибка при подготовке данных:\n{ex.Message}";
                if (ex.InnerException != null)
                {
                    errorDetails += $"\nВнутренняя ошибка: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorDetails, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private Dictionary<string, object> BuildSubmittedValues()
        {
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            foreach (var control in _dynamicControls)
            {
                // Identity поля не редактируем и не добавляем через форму.
                if (MSSQL.IsIdentityColumn(_tableName, control.Key))
                    continue;

                values[control.Key] = GetControlValue(control.Value);
            }

            return values;
        }
    }
}


