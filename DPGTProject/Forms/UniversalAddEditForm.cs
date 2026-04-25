using MSSQL = Scraps.Database.MSSQL.MSSQL;
using Scraps.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
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
        private Dictionary<string, TableEditColumnMetadata> _metadataByColumn =
            new Dictionary<string, TableEditColumnMetadata>(StringComparer.OrdinalIgnoreCase);

        private static readonly string[] PreferredDisplayColumns =
        {
            "Name",
            "Title",
            "DisplayName",
            "CodeName",
            "Caption",
            "Label",
            "FullName",
            "Description",
            "Login",
            "Code"
        };

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
                LoadTableMetadata();
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
        private void LoadTableMetadata()
        {
            _metadataByColumn.Clear();
            try
            {
                var metadata = MSSQL.GetTableEditMetadata(_tableName, null);
                foreach (var column in metadata.Columns)
                {
                    if (column == null || string.IsNullOrWhiteSpace(column.Column))
                        continue;

                    _metadataByColumn[column.Column] = column;
                }
            }
            catch
            {
                // fallback to point-queries MSSQL.IsNullableColumn/MSSQL.IsIdentityColumn
            }
        }

        private TableEditColumnMetadata GetColumnMetadata(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return null;

            return _metadataByColumn.TryGetValue(columnName, out var metadata)
                ? metadata
                : null;
        }

        private bool IsNullableColumn(string columnName)
        {
            var metadata = GetColumnMetadata(columnName);
            if (metadata != null)
                return metadata.IsNullable;

            return MSSQL.IsNullableColumn(_tableName, columnName);
        }

        private bool IsIdentityColumn(string columnName)
        {
            var metadata = GetColumnMetadata(columnName);
            if (metadata != null)
                return metadata.IsIdentity;

            return MSSQL.IsIdentityColumn(_tableName, columnName);
        }

        private string GetColumnDataType(string columnName)
        {
            var metadata = GetColumnMetadata(columnName);
            return (metadata?.DataType ?? string.Empty).Trim().ToLowerInvariant();
        }

        private Type GetColumnType(string columnName, object fallbackType)
        {
            switch (GetColumnDataType(columnName))
            {
                case "int":
                case "smallint":
                case "tinyint":
                    return typeof(int);
                case "bigint":
                    return typeof(long);
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    return typeof(decimal);
                case "float":
                case "real":
                    return typeof(double);
                case "bit":
                    return typeof(bool);
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return typeof(DateTime);
                case "time":
                    return typeof(TimeSpan);
                case "uniqueidentifier":
                    return typeof(Guid);
            }

            return fallbackType as Type ?? typeof(string);
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
                return IsNullableColumn(columnName)
                    ? WrapWithNullablePanel(fkControl)
                    : fkControl;
            }

            var inputType = GetColumnType(columnName, type);

            // Для bool полей возвращаем обычный CheckBox
            if (inputType == typeof(bool))
                return new CheckBox { Width = 150 };

            // Создаем основной контрол ввода
            Control inputControl;
            if (inputType == typeof(string))
                inputControl = new TextBox { Width = 150 };
            else if (inputType == typeof(int) || inputType == typeof(byte))
                inputControl = new NumericUpDown
                {
                    Minimum = int.MinValue,
                    Maximum = int.MaxValue,
                    DecimalPlaces = 0,
                    Width = 150
                };
            else if (inputType == typeof(long))
                inputControl = new NumericUpDown
                {
                    Minimum = long.MinValue,
                    Maximum = long.MaxValue,
                    DecimalPlaces = 0,
                    Width = 150
                };
            else if (inputType == typeof(double) || inputType == typeof(decimal) || inputType == typeof(float))
                inputControl = new NumericUpDown
                {
                    Minimum = decimal.MinValue,
                    Maximum = decimal.MaxValue,
                    DecimalPlaces = 4,
                    Width = 150
                };
            else if (inputType == typeof(DateTime))
                inputControl = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Width = 150
                };
            else if (inputType == typeof(TimeSpan) || inputType == typeof(Guid))
                inputControl = new TextBox { Width = 150 };
            else
            {
                throw new ArgumentException($"Неподдерживаемый тип для {columnName}");
            }

            if (IsNullableColumn(columnName))
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
                var metadata = GetColumnMetadata(columnName);
                if (metadata == null || metadata.ForeignKey == null)
                    return false;

                string displayColumn = ResolveLookupDisplayColumn(columnName, metadata);

                var lookupItems = MSSQL.GetForeignKeyLookupItems(_tableName, columnName, displayColumn, null, null, null);
                if (lookupItems == null || lookupItems.Count == 0)
                    return false;

                var data = new DataTable();
                data.Columns.Add("Value", typeof(object));
                data.Columns.Add("DisplayWithId", typeof(string));

                foreach (var item in lookupItems)
                {
                    string valueText = item?.Value == DBNull.Value || item?.Value == null
                        ? string.Empty
                        : Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                    string displayText = string.IsNullOrWhiteSpace(item?.Display)
                        ? valueText
                        : item.Display;

                    var resultRow = data.NewRow();
                    resultRow["Value"] = item.Value ?? (object)DBNull.Value;
                    resultRow["DisplayWithId"] = FormatLookupDisplay(displayText, valueText);
                    data.Rows.Add(resultRow);
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

        private static string FormatLookupDisplay(string displayText, string valueText)
        {
            if (string.IsNullOrWhiteSpace(displayText))
                return valueText ?? string.Empty;

            if (string.IsNullOrWhiteSpace(valueText) ||
                string.Equals(displayText, valueText, StringComparison.OrdinalIgnoreCase))
            {
                return displayText;
            }

            return $"{displayText} [{valueText}]";
        }

        private string ResolveLookupDisplayColumn(string columnName, TableEditColumnMetadata metadata)
        {
            if (metadata == null || metadata.ForeignKey == null)
                return null;

            string overrideKey = $"{_tableName}.{columnName}";
            string displayColumn = null;

            if (SystemConfig.ForeignKeyDisplayColumnOverrides.TryGetValue(overrideKey, out var configuredDisplay) &&
                !string.IsNullOrWhiteSpace(configuredDisplay))
            {
                displayColumn = configuredDisplay;
            }

            if (string.IsNullOrWhiteSpace(displayColumn))
                displayColumn = metadata.LookupDisplayColumn;

            if (string.IsNullOrWhiteSpace(displayColumn))
                displayColumn = MSSQL.ResolveDisplayColumn(metadata.ForeignKey.RefTable, metadata.ForeignKey.RefColumn);

            return ResolveDisplayColumnFallback(metadata.ForeignKey.RefTable, metadata.ForeignKey.RefColumn, displayColumn);
        }

        private static string ResolveDisplayColumnFallback(string refTable, string refColumn, string selectedDisplayColumn)
        {
            if (string.IsNullOrWhiteSpace(refTable))
                return selectedDisplayColumn;

            try
            {
                var schema = Current.GetTableSchema(refTable);
                if (schema == null || !schema.Columns.Contains("ColumnName"))
                    return selectedDisplayColumn;

                var availableColumns = schema.Rows.Cast<DataRow>()
                    .Select(r => r["ColumnName"]?.ToString())
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (availableColumns.Count == 0)
                    return selectedDisplayColumn;

                if (IsUsableDisplayColumn(selectedDisplayColumn, refColumn, availableColumns))
                    return availableColumns.First(c => string.Equals(c, selectedDisplayColumn, StringComparison.OrdinalIgnoreCase));

                foreach (var candidate in PreferredDisplayColumns)
                {
                    if (IsUsableDisplayColumn(candidate, refColumn, availableColumns))
                        return availableColumns.First(c => string.Equals(c, candidate, StringComparison.OrdinalIgnoreCase));
                }

                var keywordMatch = availableColumns.FirstOrDefault(c =>
                    !string.Equals(c, refColumn, StringComparison.OrdinalIgnoreCase) &&
                    ContainsDisplayKeyword(c));

                if (!string.IsNullOrWhiteSpace(keywordMatch))
                    return keywordMatch;

                return availableColumns.FirstOrDefault(c =>
                           !string.Equals(c, refColumn, StringComparison.OrdinalIgnoreCase))
                       ?? selectedDisplayColumn;
            }
            catch
            {
                return selectedDisplayColumn;
            }
        }

        private static bool IsUsableDisplayColumn(string columnName, string refColumn, List<string> availableColumns)
        {
            if (string.IsNullOrWhiteSpace(columnName) || availableColumns == null || availableColumns.Count == 0)
                return false;

            if (string.Equals(columnName, refColumn, StringComparison.OrdinalIgnoreCase))
                return false;

            return availableColumns.Any(c => string.Equals(c, columnName, StringComparison.OrdinalIgnoreCase));
        }

        private static bool ContainsDisplayKeyword(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return false;

            string lower = columnName.ToLowerInvariant();
            return lower.Contains("name") ||
                   lower.Contains("title") ||
                   lower.Contains("display") ||
                   lower.Contains("caption") ||
                   lower.Contains("label") ||
                   lower.Contains("description") ||
                   lower.Contains("code") ||
                   lower.Contains("login") ||
                   lower.Contains("email");
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
                    if (value == null)
                    {
                        numericUpDown.Value = 0;
                    }
                    else
                    {
                        var numericValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                        if (numericValue < numericUpDown.Minimum) numericValue = numericUpDown.Minimum;
                        if (numericValue > numericUpDown.Maximum) numericValue = numericUpDown.Maximum;
                        numericUpDown.Value = numericValue;
                    }
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

        private string GetDisplayColumnName(string columnName)
        {
            return TranslationManager.TranslateColumnName(_tableName, columnName);
        }

        private bool ValidateInput()
        {
            foreach (var control in _dynamicControls)
            {
                try
                {
                    object value = GetControlValue(control.Value);

                    if (control.Value is Panel panel)
                    {
                        var checkbox = panel.Controls.OfType<CheckBox>().FirstOrDefault();
                        if (checkbox != null && checkbox.Checked) continue;
                    }

                    if (value == null || (value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                    {
                        MessageBox.Show($"Поле '{GetDisplayColumnName(control.Key)}' не может быть пустым");
                        return false;
                    }

                    if (!IsValueCompatibleWithColumn(control.Key, value, out var validationError))
                    {
                        MessageBox.Show($"Поле '{GetDisplayColumnName(control.Key)}': {validationError}", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка валидации поля '{GetDisplayColumnName(control.Key)}': {ex.Message}");
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

        private bool IsValueCompatibleWithColumn(string columnName, object value, out string error)
        {
            error = null;
            try
            {
                ConvertValueForColumn(columnName, value);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private object ConvertValueForColumn(string columnName, object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var dataType = GetColumnDataType(columnName);
            var textValue = value.ToString();

            switch (dataType)
            {
                case "tinyint":
                    return Convert.ToByte(value, CultureInfo.InvariantCulture);
                case "smallint":
                    return Convert.ToInt16(value, CultureInfo.InvariantCulture);
                case "int":
                    return Convert.ToInt32(value, CultureInfo.InvariantCulture);
                case "bigint":
                    return Convert.ToInt64(value, CultureInfo.InvariantCulture);
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                case "float":
                case "real":
                    return Convert.ToDouble(value, CultureInfo.InvariantCulture);
                case "bit":
                    return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                case "time":
                    if (value is TimeSpan ts) return ts;
                    if (TimeSpan.TryParse(textValue, CultureInfo.InvariantCulture, out var parsedTs)) return parsedTs;
                    if (TimeSpan.TryParse(textValue, CultureInfo.CurrentCulture, out parsedTs)) return parsedTs;
                    throw new FormatException("ожидается значение времени (например 12:30:00).");
                case "uniqueidentifier":
                    if (value is Guid g) return g;
                    if (Guid.TryParse(textValue, out var parsedGuid)) return parsedGuid;
                    throw new FormatException("ожидается корректный GUID.");
                default:
                    return value;
            }
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
                if (IsIdentityColumn(control.Key))
                    continue;

                values[control.Key] = ConvertValueForColumn(control.Key, GetControlValue(control.Value));
            }

            return values;
        }
    }
}
