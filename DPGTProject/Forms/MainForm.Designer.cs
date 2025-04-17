﻿namespace DPGTProject
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button exit_btn;
        private System.Windows.Forms.Button dev_btn;
        private System.Windows.Forms.Button unlogin_btn;
        private System.Windows.Forms.Label role_lb;
        private System.Windows.Forms.Label hello_lb;

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
            this.exit_btn = new System.Windows.Forms.Button();
            this.dev_btn = new System.Windows.Forms.Button();
            this.unlogin_btn = new System.Windows.Forms.Button();
            this.role_lb = new System.Windows.Forms.Label();
            this.hello_lb = new System.Windows.Forms.Label();
            this.open_table_btn = new System.Windows.Forms.Button();
            this.gen_btn = new System.Windows.Forms.Button();
            this.table_cb = new System.Windows.Forms.ComboBox();
            this.import_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // exit_btn
            // 
            this.exit_btn.Location = new System.Drawing.Point(12, 347);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(274, 23);
            this.exit_btn.TabIndex = 0;
            this.exit_btn.Text = "Выход";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // dev_btn
            // 
            this.dev_btn.Location = new System.Drawing.Point(12, 318);
            this.dev_btn.Name = "dev_btn";
            this.dev_btn.Size = new System.Drawing.Size(274, 23);
            this.dev_btn.TabIndex = 11;
            this.dev_btn.Text = "Разработчики";
            this.dev_btn.Click += new System.EventHandler(this.dev_btn_Click);
            // 
            // unlogin_btn
            // 
            this.unlogin_btn.Location = new System.Drawing.Point(12, 289);
            this.unlogin_btn.Name = "unlogin_btn";
            this.unlogin_btn.Size = new System.Drawing.Size(274, 23);
            this.unlogin_btn.TabIndex = 10;
            this.unlogin_btn.Text = "Выйти из аккаунта";
            this.unlogin_btn.Click += new System.EventHandler(this.unlogin_btn_Click);
            // 
            // role_lb
            // 
            this.role_lb.Location = new System.Drawing.Point(12, 263);
            this.role_lb.Name = "role_lb";
            this.role_lb.Size = new System.Drawing.Size(274, 23);
            this.role_lb.TabIndex = 9;
            this.role_lb.Text = "Ваша роль: ";
            this.role_lb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // hello_lb
            // 
            this.hello_lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hello_lb.Location = new System.Drawing.Point(12, 9);
            this.hello_lb.Name = "hello_lb";
            this.hello_lb.Size = new System.Drawing.Size(274, 23);
            this.hello_lb.TabIndex = 8;
            this.hello_lb.Text = "Здравствуйте!";
            this.hello_lb.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // open_table_btn
            // 
            this.open_table_btn.Location = new System.Drawing.Point(199, 35);
            this.open_table_btn.Name = "open_table_btn";
            this.open_table_btn.Size = new System.Drawing.Size(87, 23);
            this.open_table_btn.TabIndex = 7;
            this.open_table_btn.Text = "Открыть";
            this.open_table_btn.Click += new System.EventHandler(this.tables_Click);
            // 
            // gen_btn
            // 
            this.gen_btn.Location = new System.Drawing.Point(12, 64);
            this.gen_btn.Name = "gen_btn";
            this.gen_btn.Size = new System.Drawing.Size(274, 23);
            this.gen_btn.TabIndex = 12;
            this.gen_btn.Text = "Генерация отчёта";
            this.gen_btn.Click += new System.EventHandler(this.reports_Click);
            // 
            // table_cb
            // 
            this.table_cb.FormattingEnabled = true;
            this.table_cb.Location = new System.Drawing.Point(15, 35);
            this.table_cb.Name = "table_cb";
            this.table_cb.Size = new System.Drawing.Size(167, 21);
            this.table_cb.TabIndex = 15;
            // 
            // import_btn
            // 
            this.import_btn.Location = new System.Drawing.Point(12, 93);
            this.import_btn.Name = "import_btn";
            this.import_btn.Size = new System.Drawing.Size(274, 23);
            this.import_btn.TabIndex = 17;
            this.import_btn.Text = "Импорт данных";
            this.import_btn.Click += new System.EventHandler(this.import_btn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 382);
            this.Controls.Add(this.import_btn);
            this.Controls.Add(this.table_cb);
            this.Controls.Add(this.gen_btn);
            this.Controls.Add(this.open_table_btn);
            this.Controls.Add(this.hello_lb);
            this.Controls.Add(this.role_lb);
            this.Controls.Add(this.unlogin_btn);
            this.Controls.Add(this.dev_btn);
            this.Controls.Add(this.exit_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Главное меню";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button open_table_btn;
        private System.Windows.Forms.Button gen_btn;
        private System.Windows.Forms.ComboBox table_cb;
        private System.Windows.Forms.Button import_btn;
    }
}
