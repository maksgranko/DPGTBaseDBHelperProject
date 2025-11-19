using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DPGTProject.Forms
{
    public partial class UniversalAddEditForm : BaseForm
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
            try
            {
                _tableName = tableName;
                InitializeComponent(columnDefinitions, false);
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
                InitializeComponent(columnDefinitions, true);
            }
            catch
            {
                DialogResult = DialogResult.Cancel;
                this.Dispose();
            }
        }

        private void InitializeComponent(Dictionary<string, object> columnDefinitions, bool isEditMode)
        {
            _columnDefinitions = columnDefinitions;
            _isEditMode = isEditMode;

            this.SuspendLayout();
            // 
            // UniversalAddEditForm
            // 
            this.ClientSize = new Size(284, 261);
            this.DoubleBuffered = true;
            this.Name = "UniversalAddEditForm";
            string translatedTableName = SystemConfig.TranslateComboBox(_tableName);
            this.Text = _isEditMode ? $"Редактирование: {translatedTableName}" : $"Добавление: {translatedTableName}";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DialogResult = DialogResult.Abort;
            this.MaximizeBox = false;


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
                string translatedColumnName = column.Key;
                if (SystemConfig.ColumnTranslations.TryGetValue(_tableName, out var translations) &&
                    translations.TryGetValue(column.Key, out var translatedName))
                {
                    translatedColumnName = translatedName;
                }

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
            if (Database.IsNullableColumn(_tableName, columnName))
            {
                // Создаем контейнер для основного контрола и чекбокса NULL
                var container = new Panel
                {
                    Width = 180,
                    Height = 30
                };

                // Настраиваем основной контрол
                inputControl.Width = 120;
                inputControl.Left = 0;
                container.Controls.Add(inputControl);

                // Добавляем чекбокс NULL
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
                    if (nullCheckBox.Checked)
                    {
                        if (inputControl is TextBox textBox) textBox.Text = string.Empty;
                        else if (inputControl is NumericUpDown numeric) numeric.Value = 0;
                        else if (inputControl is DateTimePicker datePicker) datePicker.Value = DateTime.Now;
                    }
                };

                container.Controls.Add(nullCheckBox);
                return container;
            }

            return inputControl;
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
                return comboBox.SelectedItem;

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
                string errorDetails = $"Ошибка при генерации SQL запроса:\n{ex.Message}";
                if (ex.InnerException != null)
                {
                    errorDetails += $"\nВнутренняя ошибка: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorDetails, "Ошибка SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string untranslatedColumn = control.Key;
                    if (SystemConfig.ColumnTranslations.TryGetValue(_tableName, out var translations) &&
                        translations.ContainsValue(control.Key))
                    {
                        untranslatedColumn = translations.First(x => x.Value == control.Key).Key;
                    }

                    // Не добавляем identity колонки в INSERT
                    if (!Database.IsIdentityColumn(_tableName, untranslatedColumn))
                    {
                        columns.Add(untranslatedColumn);
                        values.Add(FormatSqlValue(GetControlValue(control.Value)));
                    }
                }

                string untranslatedTableName = _tableName;
                if (SystemConfig.TableTranslations.ContainsValue(_tableName))
                {
                    untranslatedTableName = SystemConfig.TableTranslations.First(x => x.Value == _tableName).Key;
                }
                return $"INSERT INTO {untranslatedTableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";
            }
            else // Update
            {
                var setParts = new List<string>();
                foreach (var control in _dynamicControls)
                {
                    string untranslatedColumn = control.Key;
                    if (SystemConfig.ColumnTranslations.TryGetValue(_tableName, out var translations) &&
                        translations.ContainsValue(control.Key))
                    {
                        untranslatedColumn = translations.First(x => x.Value == control.Key).Key;
                    }

                    // Не добавляем identity колонки в SET
                    if (!Database.IsIdentityColumn(_tableName, untranslatedColumn))
                    {
                        setParts.Add($"{untranslatedColumn} = {FormatSqlValue(GetControlValue(control.Value))}");
                    }
                }
                string untranslatedTableName = _tableName;
                if (SystemConfig.TableTranslations.ContainsValue(_tableName))
                {
                    untranslatedTableName = SystemConfig.TableTranslations.First(x => x.Value == _tableName).Key;
                }
                return $"UPDATE {untranslatedTableName} SET {string.Join(", ", setParts)} WHERE %WHERE%";
            }
        }

        private string FormatSqlValue(object value)
        {
            if (value == null) return "NULL";
            if (value is string strValue) return $"'{strValue.Replace("'", "''")}'";
            if (value is DateTime dt) return $"'{dt:yyyy-MM-dd HH:mm:ss}'";
            if (value is bool boolValue) return boolValue ? "1" : "0";
            if (value is decimal || value is float || value is double) return value.ToString().Replace(",",".");
            return value.ToString();
        }
    }
}
