namespace DPGTProject
{
    partial class ReportGeneratorForm
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
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.radioPredefinedReport = new System.Windows.Forms.RadioButton();
            this.radioNormalTable = new System.Windows.Forms.RadioButton();
            this.reportTypeComboBox = new System.Windows.Forms.ComboBox();
            this.generate_btn = new System.Windows.Forms.Button();
            this.export_btn = new System.Windows.Forms.Button();
            this.exit_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.Location = new System.Drawing.Point(0, 88);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(500, 312);
            this.dataGridView1.TabIndex = 0;
            // 
            // radioPredefinedReport
            // 
            this.radioPredefinedReport.AutoSize = true;
            this.radioPredefinedReport.Checked = true;
            this.radioPredefinedReport.Location = new System.Drawing.Point(258, 11);
            this.radioPredefinedReport.Name = "radioPredefinedReport";
            this.radioPredefinedReport.Size = new System.Drawing.Size(104, 17);
            this.radioPredefinedReport.TabIndex = 1;
            this.radioPredefinedReport.TabStop = true;
            this.radioPredefinedReport.Text = "Заготовленные";
            this.radioPredefinedReport.UseVisualStyleBackColor = true;
            this.radioPredefinedReport.CheckedChanged += new System.EventHandler(this.ReportTypeChanged);
            // 
            // radioNormalTable
            // 
            this.radioNormalTable.AutoSize = true;
            this.radioNormalTable.Location = new System.Drawing.Point(258, 34);
            this.radioNormalTable.Name = "radioNormalTable";
            this.radioNormalTable.Size = new System.Drawing.Size(118, 17);
            this.radioNormalTable.TabIndex = 2;
            this.radioNormalTable.Text = "Обычные таблицы";
            this.radioNormalTable.UseVisualStyleBackColor = true;
            this.radioNormalTable.CheckedChanged += new System.EventHandler(this.ReportTypeChanged);
            // 
            // reportTypeComboBox
            // 
            this.reportTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.reportTypeComboBox.Location = new System.Drawing.Point(12, 8);
            this.reportTypeComboBox.Name = "reportTypeComboBox";
            this.reportTypeComboBox.Size = new System.Drawing.Size(240, 21);
            this.reportTypeComboBox.TabIndex = 3;
            // 
            // generate_btn
            // 
            this.generate_btn.Location = new System.Drawing.Point(12, 31);
            this.generate_btn.Name = "generate_btn";
            this.generate_btn.Size = new System.Drawing.Size(117, 23);
            this.generate_btn.TabIndex = 4;
            this.generate_btn.Text = "Сформировать";
            this.generate_btn.Click += new System.EventHandler(this.GenerateReport);
            // 
            // export_btn
            // 
            this.export_btn.Location = new System.Drawing.Point(135, 31);
            this.export_btn.Name = "export_btn";
            this.export_btn.Size = new System.Drawing.Size(117, 23);
            this.export_btn.TabIndex = 5;
            this.export_btn.Text = "Экспорт";
            this.export_btn.Click += new System.EventHandler(this.ExportReport);
            // 
            // exit_btn
            // 
            this.exit_btn.Location = new System.Drawing.Point(12, 59);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(240, 23);
            this.exit_btn.TabIndex = 6;
            this.exit_btn.Text = "Выход";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // ReportGeneratorForm
            // 
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.Controls.Add(this.radioPredefinedReport);
            this.Controls.Add(this.radioNormalTable);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.reportTypeComboBox);
            this.Controls.Add(this.generate_btn);
            this.Controls.Add(this.export_btn);
            this.Name = "ReportGeneratorForm";
            this.Text = "Генератор отчётов";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox reportTypeComboBox;
        private System.Windows.Forms.Button generate_btn;
        private System.Windows.Forms.Button export_btn;
        private System.Windows.Forms.Button exit_btn;
    }
}
