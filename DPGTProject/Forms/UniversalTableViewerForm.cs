using DPGTProject.Configs;
using MSSQL = Scraps.Databases.MSSQL;
using DPGTProject.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PermissionFlags = Scraps.Security.PermissionFlags;
using ScrapsRoleManager = Scraps.Security.RoleManager;
using Scraps.Localization;
using Scraps.Databases;
using DataTableSearch = Scraps.Data.DataTables.Search;
using Scraps.Security;

namespace DPGTProject
{
    public partial class UniversalTableViewerForm : BaseForm
    {
        private string _tableName;
        private string _currentFilter;
        private DataTable _originalData;
        private DataTable _filteredData;
        private DataTableSearch.MatchNavigator _searchNavigator;
        private string _lastSearchText = string.Empty;
        private readonly Dictionary<string, Dictionary<string, string>> _fkDisplayMap =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
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
            return ScrapsRoleManager.GetEffectivePermissions(UserSession.UserRole, _tableName ?? string.Empty);
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
            try
            {
                throw e.Exception;
            }
            catch (FormatException ex)
            {
                SystemConfig.lastError = ex.ToString();
            }
            catch
            {
                SystemConfig.lastError = "Неизвестная ошибка.";
            }
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
            var source = dataGridView1.DataSource as DataTable;
            if (source == null)
                return;

            var searchText = find_tb.Text;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ResetSearchState();
                statusLabel.Text = "Введите текст для поиска";
                return;
            }

            bool needRebuild = _searchNavigator == null ||
                               !string.Equals(_lastSearchText, searchText, StringComparison.OrdinalIgnoreCase);

            if (needRebuild)
            {
                _searchNavigator = DataTableSearch.CreateNavigator(source, searchText, ignoreCase: true);
                _lastSearchText = searchText;
            }

            if (_searchNavigator == null || _searchNavigator.Count == 0)
            {
                statusLabel.Text = "Ничего не найдено";
                return;
            }

            DataTableSearch.DataCellMatch match;
            if (needRebuild)
                match = isNext ? _searchNavigator.First() : _searchNavigator.Last();
            else
                match = isNext ? _searchNavigator.Next(wrap: true) : _searchNavigator.Prev(wrap: true);

            NavigateToResult(match);
            statusLabel.Text = $"Найдено: {_searchNavigator.Count} (позиция {_searchNavigator.Index + 1})";
        }

        private void FindNext_Click(object sender, EventArgs e) => HandleSearch(true);

        private void ResetSearchState()
        {
            _searchNavigator = null;
            _lastSearchText = string.Empty;
        }
        private void FindPrevious_Click(object sender, EventArgs e) => HandleSearch(false);

        private void NavigateToResult(DataTableSearch.DataCellMatch match)
        {
            if (match == null || match.RowIndex < 0 || match.RowIndex >= dataGridView1.Rows.Count)
                return;

            if (!dataGridView1.Columns.Contains(match.ColumnName))
                return;

            var targetCell = dataGridView1.Rows[match.RowIndex].Cells[match.ColumnName];
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = targetCell;
            dataGridView1.Rows[match.RowIndex].Selected = true;
            dataGridView1.FirstDisplayedScrollingRowIndex = match.RowIndex;
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
                _filteredData = string.IsNullOrWhiteSpace(_currentFilter)
                    ? _originalData.Copy()
                    : DataTableSearch.FilterRows(_originalData, _currentFilter, ignoreCase: true);
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

                    string overrideKey = $"{TableName}.{column.Column}";
                    string displayColumn = null;
                    if (SystemConfig.ForeignKeyDisplayColumnOverrides.TryGetValue(overrideKey, out var configuredDisplay) &&
                        !string.IsNullOrWhiteSpace(configuredDisplay))
                    {
                        displayColumn = configuredDisplay;
                    }

                    if (string.IsNullOrWhiteSpace(displayColumn))
                        displayColumn = column.LookupDisplayColumn;

                    var lookup = MSSQL.GetForeignKeyLookup(TableName, column.Column, displayColumn, null, null, null);
                    if (lookup == null || !lookup.Columns.Contains("Value"))
                        continue;

                    var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRow row in lookup.Rows)
                    {
                        if (row["Value"] == null || row["Value"] == DBNull.Value)
                            continue;

                        var valueText = Convert.ToString(row["Value"], CultureInfo.InvariantCulture);
                        if (string.IsNullOrWhiteSpace(valueText))
                            continue;

                        var displayText = lookup.Columns.Contains("Display") && row["Display"] != null && row["Display"] != DBNull.Value
                            ? row["Display"].ToString()
                            : valueText;

                        map[valueText] = $"{displayText}[{valueText}]";
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
                    // Используем оригинальное имя столбца (как в DataTable)
                    string columnName = cell.OwningColumn.Name;
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




