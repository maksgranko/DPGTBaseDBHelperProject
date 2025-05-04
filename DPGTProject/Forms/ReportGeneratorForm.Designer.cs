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
            this.radioButtonExportTables = new System.Windows.Forms.RadioButton();
            this.start_dtp = new System.Windows.Forms.DateTimePicker();
            this.end_dtp = new System.Windows.Forms.DateTimePicker();
            this.from_lb = new System.Windows.Forms.Label();
            this.to_lb = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.Location = new System.Drawing.Point(0, 116);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(536, 284);
            this.dataGridView1.TabIndex = 0;
            // 
            // radioPredefinedReport
            // 
            this.radioPredefinedReport.AutoSize = true;
            this.radioPredefinedReport.Checked = true;
            this.radioPredefinedReport.Location = new System.Drawing.Point(319, 9);
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
            this.radioNormalTable.Location = new System.Drawing.Point(319, 32);
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
            this.reportTypeComboBox.Size = new System.Drawing.Size(300, 21);
            this.reportTypeComboBox.TabIndex = 3;
            this.reportTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.reportTypeComboBox_SelectedIndexChanged);
            // 
            // generate_btn
            // 
            this.generate_btn.Location = new System.Drawing.Point(12, 32);
            this.generate_btn.Name = "generate_btn";
            this.generate_btn.Size = new System.Drawing.Size(144, 23);
            this.generate_btn.TabIndex = 4;
            this.generate_btn.Text = "Сформировать";
            this.generate_btn.Click += new System.EventHandler(this.GenerateReport);
            // 
            // export_btn
            // 
            this.export_btn.Location = new System.Drawing.Point(162, 32);
            this.export_btn.Name = "export_btn";
            this.export_btn.Size = new System.Drawing.Size(149, 23);
            this.export_btn.TabIndex = 5;
            this.export_btn.Text = "Экспорт";
            this.export_btn.Click += new System.EventHandler(this.ExportReport);
            // 
            // exit_btn
            // 
            this.exit_btn.Location = new System.Drawing.Point(12, 87);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(300, 23);
            this.exit_btn.TabIndex = 6;
            this.exit_btn.Text = "Выход";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // radioButtonExportTables
            // 
            this.radioButtonExportTables.AutoSize = true;
            this.radioButtonExportTables.Location = new System.Drawing.Point(319, 55);
            this.radioButtonExportTables.Name = "radioButtonExportTables";
            this.radioButtonExportTables.Size = new System.Drawing.Size(202, 17);
            this.radioButtonExportTables.TabIndex = 7;
            this.radioButtonExportTables.Text = "Экспорт базы данных в виде XLSX";
            this.radioButtonExportTables.UseVisualStyleBackColor = true;
            this.radioButtonExportTables.CheckedChanged += new System.EventHandler(this.ReportTypeChanged);
            // 
            // start_dtp
            // 
            this.start_dtp.Location = new System.Drawing.Point(29, 61);
            this.start_dtp.Name = "start_dtp";
            this.start_dtp.Size = new System.Drawing.Size(126, 20);
            this.start_dtp.TabIndex = 8;
            // 
            // end_dtp
            // 
            this.end_dtp.Location = new System.Drawing.Point(184, 61);
            this.end_dtp.Name = "end_dtp";
            this.end_dtp.Size = new System.Drawing.Size(127, 20);
            this.end_dtp.TabIndex = 9;
            // 
            // from_lb
            // 
            this.from_lb.AutoSize = true;
            this.from_lb.Location = new System.Drawing.Point(11, 64);
            this.from_lb.Name = "from_lb";
            this.from_lb.Size = new System.Drawing.Size(17, 13);
            this.from_lb.TabIndex = 10;
            this.from_lb.Text = "С:";
            // 
            // to_lb
            // 
            this.to_lb.AutoSize = true;
            this.to_lb.Location = new System.Drawing.Point(158, 64);
            this.to_lb.Name = "to_lb";
            this.to_lb.Size = new System.Drawing.Size(24, 13);
            this.to_lb.TabIndex = 11;
            this.to_lb.Text = "По:";
            // 
            // ReportGeneratorForm
            // 
            this.ClientSize = new System.Drawing.Size(536, 400);
            this.Controls.Add(this.to_lb);
            this.Controls.Add(this.from_lb);
            this.Controls.Add(this.end_dtp);
            this.Controls.Add(this.start_dtp);
            this.Controls.Add(this.radioButtonExportTables);
            this.Controls.Add(this.radioPredefinedReport);
            this.Controls.Add(this.radioNormalTable);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.reportTypeComboBox);
            this.Controls.Add(this.generate_btn);
            this.Controls.Add(this.export_btn);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(900, 439);
            this.MinimumSize = new System.Drawing.Size(397, 439);
            this.Name = "ReportGeneratorForm";
            this.Text = "Генератор отчётов";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button export_btn;
        private System.Windows.Forms.Button exit_btn;
        public System.Windows.Forms.RadioButton radioPredefinedReport;
        public System.Windows.Forms.RadioButton radioNormalTable;
        public System.Windows.Forms.ComboBox reportTypeComboBox;
        public System.Windows.Forms.Button generate_btn;
        public System.Windows.Forms.RadioButton radioButtonExportTables;
        private System.Windows.Forms.DateTimePicker start_dtp;
        private System.Windows.Forms.DateTimePicker end_dtp;
        private System.Windows.Forms.Label from_lb;
        private System.Windows.Forms.Label to_lb;
    }
}
