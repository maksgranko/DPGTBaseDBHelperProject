using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DPGTProject
{
    public partial class UniversalTableViewerForm : Form
    {
        private string _tableName;
        private DataTable _originalData;
        private List<DataGridViewCell> _searchResults = new List<DataGridViewCell>();
        private int _currentSearchIndex = -1;

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
        }

        private void LoadData()
        {
            try
            {
                _originalData = Database.GetAll(TableName);
                dataGridView1.DataSource = Database.Translate(_originalData, TableName);
                statusLabel.Text = $"Загружено записей: {_originalData.Rows.Count}";
            }
            catch
            {
                MessageBox.Show("Не удалось загрузить данные. Обратитесь к администратору.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var changedData = (DataTable)dataGridView1.DataSource;
                var untranslated = Database.Untranslate(changedData, TableName);
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
            if (dataGridView1.SelectedRows.Count == 0) { MessageBox.Show("Выберите один или более колон!","Ошибка!",MessageBoxButtons.OK,MessageBoxIcon.Error); return; }
            if (MessageBox.Show("Вы точно желаете удалить запись(и)?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) { return; }
            for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--)
            {
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[i]);
            }
        }

        private void help_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Вы можете использовать кнопки на верхней панели.\n" +
                "Дополнительные подсказки:\n" +
                "1. Для фильтрации данных используйте кнопку 'Фильтр'\n" +
                "2. Для экспорта нажмите 'Экспорт'\n" +
                "3. Редактируйте напрямую в полях\n" +
                "4. Для удаления выделите строки и нажмите 'Удалить'\n" +
                "5. Для поиска введите текст в поле 'Найти' и используйте кнопки ↑↓",
                "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FindNext_Click(object sender, EventArgs e)
        {
            SearchAllColumns();
            if (_searchResults.Count == 0) return;
            
            _currentSearchIndex = (_currentSearchIndex + 1) % _searchResults.Count;
            NavigateToResult();
        }

        private void FindPrevious_Click(object sender, EventArgs e)
        {
            SearchAllColumns();
            if (_searchResults.Count == 0) return;
            
            _currentSearchIndex = (_currentSearchIndex - 1 + _searchResults.Count) % _searchResults.Count;
            NavigateToResult();
        }

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
                _currentSearchIndex = 0;
                statusLabel.Text = $"Найдено: {_searchResults.Count}";
            }
            else
            {
                statusLabel.Text = "Ничего не найдено";
            }
        }

        private void NavigateToResult()
        {
            if (_currentSearchIndex < 0 || _currentSearchIndex >= _searchResults.Count) return;

            var cell = _searchResults[_currentSearchIndex];
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = cell;
            dataGridView1.Rows[cell.RowIndex].Selected = true;
            dataGridView1.FirstDisplayedScrollingRowIndex = cell.RowIndex;
        }
    }
}
