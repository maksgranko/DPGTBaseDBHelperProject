namespace DPGTProject
{
    partial class UniversalTableViewerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.save_btn = new System.Windows.Forms.ToolStripButton();
            this.refresh_btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.removerow_btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.find_label = new System.Windows.Forms.ToolStripLabel();
            this.find_tb = new System.Windows.Forms.ToolStripTextBox();
            this.find_next_btn = new System.Windows.Forms.ToolStripButton();
            this.find_previous_btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.filter_label = new System.Windows.Forms.ToolStripLabel();
            this.filter_tb = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.help_btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exit_btn = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(821, 367);
            this.dataGridView1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.save_btn,
            this.refresh_btn,
            this.toolStripSeparator3,
            this.removerow_btn,
            this.toolStripSeparator1,
            this.find_label,
            this.find_tb,
            this.find_next_btn,
            this.find_previous_btn,
            this.toolStripSeparator4,
            this.filter_label,
            this.filter_tb,
            this.toolStripSeparator2,
            this.help_btn,
            this.toolStripSeparator5,
            this.exit_btn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(821, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // save_btn
            // 
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(70, 22);
            this.save_btn.Text = "Сохранить";
            this.save_btn.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // refresh_btn
            // 
            this.refresh_btn.Name = "refresh_btn";
            this.refresh_btn.Size = new System.Drawing.Size(65, 22);
            this.refresh_btn.Text = "Обновить";
            this.refresh_btn.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // removerow_btn
            // 
            this.removerow_btn.Name = "removerow_btn";
            this.removerow_btn.Size = new System.Drawing.Size(95, 22);
            this.removerow_btn.Text = "Удалить запись";
            this.removerow_btn.Click += new System.EventHandler(this.removerow_btn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // find_label
            // 
            this.find_label.Name = "find_label";
            this.find_label.Size = new System.Drawing.Size(44, 22);
            this.find_label.Text = "Найти:";
            // 
            // find_tb
            // 
            this.find_tb.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.find_tb.Name = "find_tb";
            this.find_tb.Size = new System.Drawing.Size(100, 25);
            // 
            // find_next_btn
            // 
            this.find_next_btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.find_next_btn.Name = "find_next_btn";
            this.find_next_btn.Size = new System.Drawing.Size(23, 22);
            this.find_next_btn.Text = "↑";
            this.find_next_btn.ToolTipText = "Далее";
            this.find_next_btn.Click += new System.EventHandler(this.FindNext_Click);
            // 
            // find_previous_btn
            // 
            this.find_previous_btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.find_previous_btn.Name = "find_previous_btn";
            this.find_previous_btn.Size = new System.Drawing.Size(23, 22);
            this.find_previous_btn.Text = "↓";
            this.find_previous_btn.ToolTipText = "Назад";
            this.find_previous_btn.Click += new System.EventHandler(this.FindPrevious_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // filter_label
            // 
            this.filter_label.Name = "filter_label";
            this.filter_label.Size = new System.Drawing.Size(51, 22);
            this.filter_label.Text = "Фильтр:";
            // 
            // filter_tb
            // 
            this.filter_tb.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.filter_tb.Name = "filter_tb";
            this.filter_tb.Size = new System.Drawing.Size(100, 25);
            this.filter_tb.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Filter_tb_KeyDown);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // help_btn
            // 
            this.help_btn.Name = "help_btn";
            this.help_btn.Size = new System.Drawing.Size(57, 22);
            this.help_btn.Text = "Справка";
            this.help_btn.Click += new System.EventHandler(this.help_btn_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // exit_btn
            // 
            this.exit_btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exit_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(46, 22);
            this.exit_btn.Text = "Выход";
            this.exit_btn.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 392);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(821, 22);
            this.statusStrip1.TabIndex = 2;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // UniversalTableViewerForm
            // 
            this.ClientSize = new System.Drawing.Size(821, 414);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "UniversalTableViewerForm";
            this.Text = "Универсальный просмотр таблиц";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton save_btn;
        private System.Windows.Forms.ToolStripButton find_previous_btn;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripButton refresh_btn;
        private System.Windows.Forms.ToolStripButton removerow_btn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel find_label;
        private System.Windows.Forms.ToolStripTextBox find_tb;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton help_btn;
        private System.Windows.Forms.ToolStripButton find_next_btn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel filter_label;
        private System.Windows.Forms.ToolStripTextBox filter_tb;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton exit_btn;
    }
}
