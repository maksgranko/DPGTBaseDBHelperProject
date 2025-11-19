using DPGTProject.Configs;
using DPGTProject.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DPGTProject
{
    public partial class UniversalTableViewerForm : BaseForm
    {
        private string _tableName;
        private string _currentFilter;
        private DataTable _originalData;
        private DataTable _filteredData;
        private List<DataGridViewCell> _searchResults = new List<DataGridViewCell>();
        private int _currentSearchIndex = -1;
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
        }

        private void UpdateButtonsVisibility()
        {
            string role = UserConfig.userRole;
            string table = _tableName ?? "";
            bool write = RoleManager.CheckAccess(role, table, "write");
            export_btn.Visible = this.request || (SystemConfig.exportRightInTables && RoleManager.CheckAccess(role, table, "export"));
            help_btn.Visible = SystemConfig.helpButtonInTables;
            toolStripSeparator2.Visible = help_btn.Visible || export_btn.Visible;

            filter_label.Visible = filter_tb.Visible = SystemConfig.enableFilterInTables;
            toolStripSeparator2.Visible = filter_label.Visible || help_btn.Visible || export_btn.Visible;

            find_label.Visible = find_next_btn.Visible = find_previous_btn.Visible = find_tb.Visible = SystemConfig.enableSearchInTables;
            toolStripSeparator1.Visible = SystemConfig.enableSearchInTables;

            exit_btn.Visible = toolStripSeparator5.Visible = SystemConfig.moreExitButtons;

            addrow_btn.Visible = editrow_btn.Visible = !request && SystemConfig.additionalButtonsInTables && write;
            save_btn.Visible = !request && write;
            removerow_btn.Visible = !request && (write && RoleManager.CheckAccess(role, table, "delete"));
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

        public UniversalTableViewerForm(string tableName) : this()
        {
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
                if (request)
                {
                    Database.GetDataTableFromSQL(SQLRequest, out DataTable dt);
                    _originalData = dt;
                }
                else _originalData = Database.GetAll(TableName);
                dataGridView1.DataSource = Database.Translate(_originalData, TableName);
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
            if (!RoleManager.CheckAccess(UserConfig.userRole, _tableName, "write"))
            {
                MessageBox.Show("У вас нет прав на редактирование записей", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var changedData = (DataTable)dataGridView1.DataSource;
                var untranslated = Database.Untranslate(changedData, TableName);

                // Проверяем, есть ли удаленные строки
                if (untranslated.GetChanges(DataRowState.Deleted) != null &&
                    !RoleManager.CheckAccess(UserConfig.userRole, _tableName, "delete"))
                {
                    MessageBox.Show("У вас нет прав на удаление записей", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Database.BulkUpdate(TableName, untranslated);
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
            if (!RoleManager.CheckAccess(UserConfig.userRole, TableName, "delete")) helpList.Add("Для удаления выделите строки и нажмите \"Удалить строку\".");
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
            SearchAllColumns();
            if (_searchResults.Count == 0) return;

            _currentSearchIndex = isNext ? _currentSearchIndex + 1 : _currentSearchIndex - 1;
            if (_currentSearchIndex > _searchResults.Count) _currentSearchIndex = _searchResults.Count-1;
            else if (_currentSearchIndex < 0) _currentSearchIndex = 0;
            NavigateToResult();
        }

        private void FindNext_Click(object sender, EventArgs e) => HandleSearch(true);
        private void FindPrevious_Click(object sender, EventArgs e) => HandleSearch(false);

        private void SearchAllColumns()
        {
            _searchResults.Clear();
            string searchText = find_tb.Text.ToLower();
            if (string.IsNullOrEmpty(searchText)) return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText))
                    {
                        _searchResults.Add(cell);
                    }
                }
            }

            if (_searchResults.Count > 0)
            {
                statusLabel.Text = $"Найдено: {_searchResults.Count}";
            }
            else
            {
                statusLabel.Text = "Ничего не найдено";
            }
        }

        private void NavigateToResult()
        {
            var cell = _searchResults[_currentSearchIndex];
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
                if (_originalData == null) return;
                if (filterText == null) { LoadData(); }
                _currentFilter = filterText;
                if (string.IsNullOrEmpty(filterText))
                {
                    _filteredData = _originalData.Copy();
                }
                else
                {
                    var filterParts = new List<string>();
                    foreach (DataColumn column in _originalData.Columns)
                    {
                        string condition = null;

                        if (column.DataType == typeof(string)) condition = $"[{column.ColumnName}] LIKE '%{filterText}%'";
                        else if (column.DataType == typeof(int) || column.DataType == typeof(decimal))
                        {
                            if (int.TryParse(filterText, out _) || decimal.TryParse(filterText, out _))
                            {
                                condition = $"[{column.ColumnName}] = {filterText}";
                            }
                        }
                        else if (column.DataType == typeof(DateTime))
                        {
                            if (DateTime.TryParse(filterText, out _)) condition = $"[{column.ColumnName}] = #{filterText}#";
                        }

                        if (!string.IsNullOrEmpty(condition)) filterParts.Add(condition);
                    }

                    string filterExpression = string.Join(" OR ", filterParts);

                    _filteredData = _originalData.Clone();
                    var rows = _originalData.Select(filterExpression);
                    foreach (var row in rows) _filteredData.ImportRow(row);
                }
            }
            catch { MessageBox.Show("Не удалось применить фильтр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            dataGridView1.DataSource = Database.Translate(_filteredData, TableName);
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

        private void addrow_btn_Click(object sender, EventArgs e)
        {
            var columnDefinitions = GetTableColumnDefinitions();
            var form = new UniversalAddEditForm(columnDefinitions, TableName);
            try
            {
                if (form.IsDisposed) throw new Exception("Произошла критическая ошибка формы AddEdit.");
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.GeneratedInsertQuery == null) throw new NullReferenceException("Ошибка формы. Вероятно, введены некорректные значения.");
                    try
                    {
                        string query = form.GeneratedInsertQuery.Replace("%TABLENAME%", TableName);
                        Database.ExecuteNonQuery(query);
                        LoadData();
                        statusLabel.Text = "Запись успешно добавлена";
                    }
                    catch
                    {
                        throw;
                    }
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
                    try
                    {
                        string keyColumn = _originalData.PrimaryKey.Length > 0
                            ? _originalData.PrimaryKey[0].ColumnName
                            : _originalData.Columns[0].ColumnName;
                        string notTranslated = keyColumn;

                        // Получаем оригинальное имя колонки из переведенного
                        if (SystemConfig.ColumnTranslations.TryGetValue(TableName, out var translations))
                        {
                            var originalName = translations.FirstOrDefault(x => x.Value == keyColumn).Key;
                            if (originalName != null)
                            {
                                keyColumn = originalName;
                            }
                        }
                        object keyValue = null;
                        try
                        {
                            keyValue = selectedRow.Cells[keyColumn].Value;
                        }
                        catch
                        {
                            keyValue = selectedRow.Cells[notTranslated].Value;
                        }
                        string formattedValue;
                        if (keyValue is string s)
                        {
                            formattedValue = $"'{s.Replace("'", "''")}'"; // Экранирование одинарных кавычек
                        }
                        else if (keyValue is DateTime dt)
                        {
                            formattedValue = $"'{dt:yyyy-MM-ddTHH:mm:ss}'"; // Формат ISO 8601 для SQL Server
                        }
                        else
                        {
                            formattedValue = keyValue.ToString(); // Другие типы
                        }
                        
                        string query = form.GeneratedUpdateQuery
                            .Replace("%TABLENAME%", TableName)
                            .Replace("%WHERE%", $"{keyColumn} = {formattedValue}");

                        Database.ExecuteNonQuery(query);
                        LoadData();
                        statusLabel.Text = "Запись успешно изменена";
                    }
                    catch
                    {
                        throw;
                    }
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
                reportForm.reportTypeComboBox.SelectedItem = SystemConfig.TranslateComboBox(_tableName);

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
