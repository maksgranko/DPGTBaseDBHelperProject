using DPGTProject.Configs;
using MSSQL = Scraps.Database.MSSQL.MSSQL;
using DPGTProject.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PermissionFlags = Scraps.Security.PermissionFlags;
using Scraps.Localization;
using Scraps.Database.MSSQL;
using Scraps.Security;
using Scraps.Database;

namespace DPGTProject
{
    public partial class UniversalTableViewerForm : BaseForm
    {
        private string _tableName;
        private string _currentFilter;
        private DataTable _originalData;
        private DataTable _filteredData;
        private string _lastSearchText = string.Empty;
        private readonly Dictionary<string, Dictionary<string, string>> _fkDisplayMap =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
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
        private bool _useVirtualTableRegistry;
        private bool request = false;
        private string SQLRequest = "";

        public string TableName
        {
            get => _tableName;
            set
            {
                _tableName = value;
                Text = $"Просмотр таблицы: {value}";
                LoadData();
            }
        }

        public UniversalTableViewerForm()
        {
            InitializeComponent();
            dataGridView1.DataError += DataGridView1_DataError;
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
        }

        private PermissionFlags GetCurrentPermissions()
        {
            return SystemConfig.GetEffectivePermissions(UserSession.UserRole, _tableName ?? string.Empty);
        }

        private bool HasCurrentPermission(PermissionFlags flag)
        {
            return (GetCurrentPermissions() & flag) == flag;
        }

        private void UpdateButtonsVisibility()
        {
            var permissions = GetCurrentPermissions();
            bool write = (permissions & PermissionFlags.Write) != 0;
            export_btn.Visible = this.request || (SystemConfig.exportRightInTables && (permissions & PermissionFlags.Export) != 0);
            help_btn.Visible = SystemConfig.helpButtonInTables;
            toolStripSeparator2.Visible = help_btn.Visible || export_btn.Visible;

            filter_label.Visible = filter_tb.Visible = SystemConfig.enableFilterInTables;
            toolStripSeparator2.Visible = filter_label.Visible || help_btn.Visible || export_btn.Visible;

            find_label.Visible = find_next_btn.Visible = find_previous_btn.Visible = find_tb.Visible = SystemConfig.enableSearchInTables;
            toolStripSeparator1.Visible = SystemConfig.enableSearchInTables;

            exit_btn.Visible = toolStripSeparator5.Visible = SystemConfig.moreExitButtons;

            addrow_btn.Visible = editrow_btn.Visible = !request && SystemConfig.additionalButtonsInTables && write;
            save_btn.Visible = !request && write;
            removerow_btn.Visible = !request && (write && (permissions & PermissionFlags.Delete) != 0);
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            _ = e?.Exception;
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Value == null || e.Value == DBNull.Value)
                return;

            var columnName = dataGridView1.Columns[e.ColumnIndex]?.Name;
            if (string.IsNullOrWhiteSpace(columnName))
                return;

            if (!_fkDisplayMap.TryGetValue(columnName, out var valueMap))
                return;

            var raw = Convert.ToString(e.Value, CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(raw))
                return;

            if (!valueMap.TryGetValue(raw, out var formatted))
                return;

            e.Value = formatted;
            e.FormattingApplied = true;
        }

        public UniversalTableViewerForm(string tableName) : this()
        {
            TableName = tableName;
            UpdateButtonsVisibility();
        }

        public UniversalTableViewerForm(string tableName, bool useVirtualTableRegistry) : this()
        {
            _useVirtualTableRegistry = useVirtualTableRegistry;
            request = useVirtualTableRegistry;
            TableName = tableName;
            UpdateButtonsVisibility();
        }

        public UniversalTableViewerForm(string tableName, string SQLReq, bool req) : this()
        {
            SQLRequest = SQLReq;
            request = req;
            TableName = tableName;
            UpdateButtonsVisibility();
        }

        private void LoadData()
        {
            try
            {
                if (_useVirtualTableRegistry)
                {
                    _originalData = VirtualTableRegistry.GetData(TableName, UserSession.UserRole, PermissionFlags.Read);
                }
                else if (request)
                {
                    _originalData = MSSQL.GetDataTableFromSQL(SQLRequest);
                }
                else
                {
                    _originalData = MSSQL.GetTableData(TableName);
                }

                BuildForeignKeyDisplayMap();
                ResetSearchState();
                dataGridView1.DataSource = TranslateForView(_originalData);
                statusLabel.Text = $"Загружено записей: {_originalData.Rows.Count}";
            }
            catch
            {
                MessageBox.Show("Не удалось загрузить данные. Обратитесь к администратору.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            if (!HasCurrentPermission(PermissionFlags.Write))
            {
                MessageBox.Show("У вас нет прав на редактирование записей", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                dataGridView1.EndEdit();
                if (dataGridView1.DataSource != null && BindingContext[dataGridView1.DataSource] is CurrencyManager currencyManager)
                    currencyManager.EndCurrentEdit();
                dataGridView1.CurrentCell = null;
                var changedData = (DataTable)dataGridView1.DataSource;
                var untranslated = TranslationManager.Untranslate(changedData, TableName);

                // Проверяем, есть ли удаленные строки
                if (untranslated.GetChanges(DataRowState.Deleted) != null &&
                    !HasCurrentPermission(PermissionFlags.Delete))
                {
                    MessageBox.Show("У вас нет прав на удаление записей", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MSSQL.ApplyTableChanges(TableName, untranslated);
            }
            catch
            {
                MessageBox.Show("Не удалось сохранить изменения. Проверьте данные и попробуйте снова.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadData();
            statusLabel.Text = "Изменения сохранены";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void removerow_btn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) { MessageBox.Show("Выберите одну или более строк!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (MessageBox.Show("Вы точно желаете удалить запись(и)?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) return;
            try
            {
                for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--)
                {
                    dataGridView1.Rows.Remove(dataGridView1.SelectedRows[i]);
                }
                save_btn_Click(null, null);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("Данную строку невозможно удалить. Подробнее: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неизвестная ошибка. Подробнее: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void help_btn_Click(object sender, EventArgs e)
        {
            List<string> helpList = new List<string> { };
            string text = "Вы можете использовать кнопки на верхней панели.\nДополнительные подсказки:\n";
            if (SystemConfig.exportRightInTables) helpList.Add("Для экспорта нажмите \"Экспорт\".");
            if (!SystemConfig.additionalButtonsInTables) helpList.Add("Редактируйте напрямую в полях.");
            if (!HasCurrentPermission(PermissionFlags.Delete)) helpList.Add("Для удаления выделите строки и нажмите \"Удалить строку\".");
            if (SystemConfig.enableFilterInTables) helpList.Add("Для фильтрации данных используйте текстовое поле \"Фильтр\" и кнопку \"Enter\".");
            if (SystemConfig.enableSearchInTables) helpList.Add("Для поиска введите текст в поле \"Найти\" и используйте кнопки ↑↓.");
            if (SystemConfig.moreExitButtons) helpList.Add("Для того, чтобы закрыть форму, вы также можете использовать кнопку \"Выход\".");
            for (int i = 0; i < helpList.Count; i++)
            {
                text += $"{i + 1}. {helpList[i]}\n";
            }
            MessageBox.Show(text, "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HandleSearch(bool isNext)
        {
            var source = _filteredData ?? _originalData;
            if (source == null)
                return;

            var searchText = find_tb.Text;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ResetSearchState();
                statusLabel.Text = "Введите текст для поиска";
                return;
            }

            bool needRebuild = _searchMatches == null ||
                               !string.Equals(_lastSearchText, searchText, StringComparison.OrdinalIgnoreCase);

            if (needRebuild)
            {
                var searchResults = SearchByDisplayValues(searchText);
                if (searchResults.Count == 0)
                {
                    statusLabel.Text = "Ничего не найдено";
                    return;
                }
                _lastSearchText = searchText;
                _searchMatches = searchResults;
                _currentSearchIndex = 0;
            }

            if (_searchMatches == null || _searchMatches.Count == 0)
            {
                statusLabel.Text = "Ничего не найдено";
                return;
            }

            if (needRebuild)
                _currentSearchIndex = isNext ? 0 : _searchMatches.Count - 1;
            else
                _currentSearchIndex = isNext ? (_currentSearchIndex + 1) % _searchMatches.Count
                                          : (_currentSearchIndex - 1 + _searchMatches.Count) % _searchMatches.Count;

            NavigateToResult(_searchMatches[_currentSearchIndex]);
            statusLabel.Text = $"Найдено: {_searchMatches.Count} (позиция {_currentSearchIndex + 1})";
        }

        private List<DataGridViewCell> _searchMatches = new List<DataGridViewCell>();
        private int _currentSearchIndex = -1;

        private List<DataGridViewCell> SearchByDisplayValues(string searchText)
        {
            var results = new List<DataGridViewCell>();
            if (dataGridView1.Rows.Count == 0)
                return results;

            string searchLower = searchText.Trim().ToLowerInvariant();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.OwningColumn.Name == "RowError") continue;
                    try
                    {
                        string displayText = cell.FormattedValue?.ToString() ?? string.Empty;
                        if (displayText.ToLowerInvariant().Contains(searchLower))
                        {
                            results.Add(cell);
                        }
                    }
                    catch { }
                }
            }

            return results;
        }

        private void FindNext_Click(object sender, EventArgs e) => HandleSearch(true);

        private void ResetSearchState()
        {
            _searchMatches = null;
            _lastSearchText = string.Empty;
        }
        private void FindPrevious_Click(object sender, EventArgs e) => HandleSearch(false);

        private void NavigateToResult(DataGridViewCell cell)
        {
            if (cell == null || cell.RowIndex < 0 || cell.RowIndex >= dataGridView1.Rows.Count)
                return;

            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = cell;
            dataGridView1.Rows[cell.RowIndex].Selected = true;
            dataGridView1.FirstDisplayedScrollingRowIndex = cell.RowIndex;
        }

        private void Filter_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFilter(filter_tb.Text);
            }
        }

        private void ApplyFilter(string filterText)
        {
            try
            {
                if (_originalData == null)
                    return;

                _currentFilter = filterText ?? string.Empty;

                if (string.IsNullOrWhiteSpace(_currentFilter))
                {
                    _filteredData = _originalData.Copy();
                }
                else
                {
                    _filteredData = FilterByDisplayValues(_currentFilter);
                }
            }
            catch
            {
                MessageBox.Show("Не удалось применить фильтр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ResetSearchState();
            dataGridView1.DataSource = TranslateForView(_filteredData);
            statusLabel.Text = $"Отфильтровано записей: {_filteredData.Rows.Count}";
        }

        private DataTable FilterByDisplayValues(string filterText)
        {
            if (_originalData == null)
                return null;

            if (string.IsNullOrWhiteSpace(filterText))
                return _originalData.Copy();

            string needle = filterText.Trim().ToLowerInvariant();
            var filtered = _originalData.Clone();

            foreach (DataRow row in _originalData.Rows)
            {
                if (RowMatchesFilter(row, needle))
                    filtered.ImportRow(row);
            }

            return filtered;
        }

        private bool RowMatchesFilter(DataRow row, string needle)
        {
            if (row == null)
                return false;

            foreach (DataColumn column in _originalData.Columns)
            {
                var value = row[column];
                if (value == null || value == DBNull.Value)
                    continue;

                var raw = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
                if (raw.ToLowerInvariant().Contains(needle))
                    return true;

                if (_fkDisplayMap.TryGetValue(column.ColumnName, out var valueMap) &&
                    !string.IsNullOrWhiteSpace(raw) &&
                    valueMap.TryGetValue(raw, out var displayText) &&
                    !string.IsNullOrWhiteSpace(displayText) &&
                    displayText.ToLowerInvariant().Contains(needle))
                {
                    return true;
                }
            }

            return false;
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Dictionary<string, object> GetTableColumnDefinitions()
        {
            var columns = new Dictionary<string, object>();
            foreach (DataColumn column in _originalData.Columns)
            {
                // Используем оригинальные имена столбцов
                columns[column.ColumnName] = column.DataType;
            }
            return columns;
        }

        private string GetTranslatedColumnName(string originalColumnName)
        {
            return TranslationManager.TranslateColumnName(TableName, originalColumnName);
        }

        private DataTable TranslateForView(DataTable source)
        {
            if (source == null)
                return null;

            // Translate(...) работает in-place, поэтому для UI используем копию.
            var copy = source.Copy();
            return TranslationManager.Translate(copy, TableName);
        }

        private void BuildForeignKeyDisplayMap()
        {
            _fkDisplayMap.Clear();

            if (string.IsNullOrWhiteSpace(TableName) || request)
                return;

            try
            {
                var metadata = MSSQL.GetTableEditMetadata(TableName, null);
                foreach (var column in metadata.Columns.Where(c => c.ForeignKey != null))
                {
                    if (string.IsNullOrWhiteSpace(column.Column))
                        continue;

                    string displayColumn = ResolveLookupDisplayColumn(
                        column.Column,
                        column.LookupDisplayColumn,
                        column.ForeignKey.RefTable,
                        column.ForeignKey.RefColumn);

                    var lookupItems = MSSQL.GetForeignKeyLookupItems(TableName, column.Column, displayColumn, null, null, null);
                    if (lookupItems == null || lookupItems.Count == 0)
                        continue;

                    var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in lookupItems)
                    {
                        if (item?.Value == null || item.Value == DBNull.Value)
                            continue;

                        var valueText = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                        if (string.IsNullOrWhiteSpace(valueText))
                            continue;

                        var displayText = string.IsNullOrWhiteSpace(item.Display)
                            ? valueText
                            : item.Display;

                        map[valueText] = FormatLookupDisplay(displayText, valueText);
                    }

                    if (map.Count == 0)
                        continue;

                    _fkDisplayMap[column.Column] = map;
                    var translatedColumn = TranslationManager.TranslateColumnName(TableName, column.Column);
                    _fkDisplayMap[translatedColumn] = map;
                }
            }
            catch
            {
                // Для виртуальных/нестандартных таблиц FK-метаданные могут быть недоступны.
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

        private string ResolveLookupDisplayColumn(string columnName, string metadataDisplayColumn, string refTable, string refColumn)
        {
            string overrideKey = $"{TableName}.{columnName}";
            string displayColumn = null;

            if (SystemConfig.ForeignKeyDisplayColumnOverrides.TryGetValue(overrideKey, out var configuredDisplay) &&
                !string.IsNullOrWhiteSpace(configuredDisplay))
            {
                displayColumn = configuredDisplay;
            }

            if (string.IsNullOrWhiteSpace(displayColumn))
                displayColumn = metadataDisplayColumn;

            if (string.IsNullOrWhiteSpace(displayColumn))
                displayColumn = MSSQL.ResolveDisplayColumn(refTable, refColumn);

            return ResolveDisplayColumnFallback(refTable, refColumn, displayColumn);
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

        private object GetCellValue(DataGridViewRow row, string originalColumnName)
        {
            var translatedName = GetTranslatedColumnName(originalColumnName);

            if (row.DataGridView != null && row.DataGridView.Columns.Contains(originalColumnName))
                return row.Cells[originalColumnName].Value;
            if (row.DataGridView != null && row.DataGridView.Columns.Contains(translatedName))
                return row.Cells[translatedName].Value;

            return null;
        }

        private static bool AreSameKey(object left, object right)
        {
            bool leftNull = left == null || left == DBNull.Value;
            bool rightNull = right == null || right == DBNull.Value;
            if (leftNull || rightNull) return leftNull == rightNull;
            return string.Equals(left.ToString(), right.ToString(), StringComparison.Ordinal);
        }

        private void addrow_btn_Click(object sender, EventArgs e)
        {
            var columnDefinitions = GetTableColumnDefinitions();
            var form = new UniversalAddEditForm(columnDefinitions, TableName);
            try
            {
                if (form.IsDisposed) throw new Exception("Произошла критическая ошибка формы AddEdit.");
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.SubmittedValues == null) throw new NullReferenceException("Ошибка формы. Вероятно, введены некорректные значения.");

                    var tableData = MSSQL.GetTableData(TableName);
                    var newRow = tableData.NewRow();
                    foreach (var pair in form.SubmittedValues)
                    {
                        if (!tableData.Columns.Contains(pair.Key)) continue;
                        newRow[pair.Key] = pair.Value ?? DBNull.Value;
                    }
                    tableData.Rows.Add(newRow);

                    var changes = tableData.GetChanges();
                    if (changes == null) throw new Exception("Изменения для добавления не найдены.");

                    MSSQL.ApplyTableChanges(TableName, changes);
                    LoadData();
                    statusLabel.Text = "Запись успешно добавлена";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                form.Dispose();
            }
        }

        private void editrow_btn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            {
                MessageBox.Show("Выберите одну строку для редактирования",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRow = dataGridView1.SelectedRows[0];
            var columnDefinitions = GetTableColumnDefinitions();
            var existingData = new Dictionary<string, object>();

            foreach (DataGridViewCell cell in selectedRow.Cells)
            {
                if (cell.OwningColumn.Name != "RowError")
                {
                    var columnName = TranslationManager.UntranslateColumnName(TableName, cell.OwningColumn.Name);
                    existingData[columnName] = cell.Value;
                }
            }

            var form = new UniversalAddEditForm(columnDefinitions, existingData, TableName);
            try
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.SubmittedValues == null) throw new NullReferenceException("Ошибка формы. Вероятно, введены некорректные значения.");

                    var tableData = MSSQL.GetTableData(TableName);

                    string keyColumn = tableData.PrimaryKey.Length > 0
                        ? tableData.PrimaryKey[0].ColumnName
                        : tableData.Columns[0].ColumnName;

                    object keyValue = GetCellValue(selectedRow, keyColumn);

                    var targetRow = tableData.AsEnumerable()
                        .FirstOrDefault(r => AreSameKey(r[keyColumn], keyValue));

                    if (targetRow == null)
                        throw new Exception("Не удалось найти строку для обновления.");

                    foreach (var pair in form.SubmittedValues)
                    {
                        if (!tableData.Columns.Contains(pair.Key)) continue;
                        targetRow[pair.Key] = pair.Value ?? DBNull.Value;
                    }

                    var changes = tableData.GetChanges();
                    if (changes == null) return;

                    MSSQL.ApplyTableChanges(TableName, changes);
                    LoadData();
                    statusLabel.Text = "Запись успешно изменена";
                }
                else if (form.DialogResult == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    MessageBox.Show("Произошла ошибка во время изменения или изменение было прервано пользователем.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (System.ObjectDisposedException) { }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении записи: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void export_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.DataSource == null)
                {
                    MessageBox.Show("Нет данных для экспорта", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var reportForm = new ReportGeneratorForm();
                reportForm.radioNormalTable.Checked = true;

                // Установить выбранную таблицу в комбобокс
                if(!reportForm.reportTypeComboBox.Items.Contains(_tableName)) reportForm.reportTypeComboBox.Items.Add(_tableName);
                reportForm.reportTypeComboBox.SelectedItem = TranslationManager.Translate(_tableName);

                // Установить данные для экспорта
                reportForm._translatedData = (DataTable)dataGridView1.DataSource;

                // Вызвать экспорт
                reportForm.GenerateReport(null, null);
                reportForm.ExportReport(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void find_tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                FindNext_Click(null, null);
            }
        }
    }
}
