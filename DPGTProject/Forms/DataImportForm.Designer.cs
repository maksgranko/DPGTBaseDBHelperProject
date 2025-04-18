namespace DPGTProject
{
    partial class DataImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox tableComboBox;
        private System.Windows.Forms.Button selectFileBtn;
        private System.Windows.Forms.Button previewBtn;
        private System.Windows.Forms.Button importBtn;

        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tableComboBox = new System.Windows.Forms.ComboBox();
            this.selectFileBtn = new System.Windows.Forms.Button();
            this.previewBtn = new System.Windows.Forms.Button();
            this.importBtn = new System.Windows.Forms.Button();
            this.exit_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.Location = new System.Drawing.Point(0, 100);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(562, 300);
            this.dataGridView1.TabIndex = 0;
            // 
            // tableComboBox
            // 
            this.tableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tableComboBox.Location = new System.Drawing.Point(10, 10);
            this.tableComboBox.Name = "tableComboBox";
            this.tableComboBox.Size = new System.Drawing.Size(200, 21);
            this.tableComboBox.TabIndex = 1;
            // 
            // selectFileBtn
            // 
            this.selectFileBtn.Location = new System.Drawing.Point(216, 8);
            this.selectFileBtn.Name = "selectFileBtn";
            this.selectFileBtn.Size = new System.Drawing.Size(100, 23);
            this.selectFileBtn.TabIndex = 2;
            this.selectFileBtn.Text = "Выбрать файл";
            this.selectFileBtn.Click += new System.EventHandler(this.SelectFile);
            // 
            // previewBtn
            // 
            this.previewBtn.Location = new System.Drawing.Point(10, 40);
            this.previewBtn.Name = "previewBtn";
            this.previewBtn.Size = new System.Drawing.Size(100, 23);
            this.previewBtn.TabIndex = 3;
            this.previewBtn.Text = "Просмотр";
            this.previewBtn.Click += new System.EventHandler(this.PreviewData);
            // 
            // importBtn
            // 
            this.importBtn.Enabled = false;
            this.importBtn.Location = new System.Drawing.Point(120, 40);
            this.importBtn.Name = "importBtn";
            this.importBtn.Size = new System.Drawing.Size(100, 23);
            this.importBtn.TabIndex = 4;
            this.importBtn.Text = "Импорт";
            this.importBtn.Click += new System.EventHandler(this.ImportData);
            // 
            // exit_btn
            // 
            this.exit_btn.Location = new System.Drawing.Point(10, 69);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(210, 23);
            this.exit_btn.TabIndex = 5;
            this.exit_btn.Text = "Выход";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // DataImportForm
            // 
            this.ClientSize = new System.Drawing.Size(562, 400);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tableComboBox);
            this.Controls.Add(this.selectFileBtn);
            this.Controls.Add(this.previewBtn);
            this.Controls.Add(this.importBtn);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(9999, 439);
            this.MinimumSize = new System.Drawing.Size(343, 439);
            this.Name = "DataImportForm";
            this.Text = "Импорт данных";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button exit_btn;
    }
}
