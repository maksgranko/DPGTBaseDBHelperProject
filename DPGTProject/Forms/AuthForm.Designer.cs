namespace DPGTProject
{
    partial class AuthForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.auth_btn = new System.Windows.Forms.Button();
            this.login_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.password_tb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.exit_btn = new System.Windows.Forms.Button();
            this.dev_btn = new System.Windows.Forms.Button();
            this.register_btn = new System.Windows.Forms.Button();
            this.hide_password_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // auth_btn
            // 
            this.auth_btn.Location = new System.Drawing.Point(21, 151);
            this.auth_btn.Name = "auth_btn";
            this.auth_btn.Size = new System.Drawing.Size(279, 23);
            this.auth_btn.TabIndex = 2;
            this.auth_btn.Text = "Войти";
            this.auth_btn.UseVisualStyleBackColor = true;
            this.auth_btn.Click += new System.EventHandler(this.auth_btn_Click);
            // 
            // login_tb
            // 
            this.login_tb.Location = new System.Drawing.Point(21, 64);
            this.login_tb.Name = "login_tb";
            this.login_tb.Size = new System.Drawing.Size(279, 20);
            this.login_tb.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(18, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Логин:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(18, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(282, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Пароль:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // password_tb
            // 
            this.password_tb.Location = new System.Drawing.Point(21, 107);
            this.password_tb.Name = "password_tb";
            this.password_tb.Size = new System.Drawing.Size(279, 20);
            this.password_tb.TabIndex = 1;
            this.password_tb.UseSystemPasswordChar = true;
            this.password_tb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.password_tb_KeyPress);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(21, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(279, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Авторизация";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // exit_btn
            // 
            this.exit_btn.Location = new System.Drawing.Point(24, 296);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(279, 23);
            this.exit_btn.TabIndex = 6;
            this.exit_btn.Text = "Выход";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // dev_btn
            // 
            this.dev_btn.Location = new System.Drawing.Point(24, 267);
            this.dev_btn.Name = "dev_btn";
            this.dev_btn.Size = new System.Drawing.Size(279, 23);
            this.dev_btn.TabIndex = 5;
            this.dev_btn.Text = "Разработчики";
            this.dev_btn.UseVisualStyleBackColor = true;
            this.dev_btn.Click += new System.EventHandler(this.dev_btn_Click);
            // 
            // register_btn
            // 
            this.register_btn.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.register_btn.Location = new System.Drawing.Point(24, 238);
            this.register_btn.Name = "register_btn";
            this.register_btn.Size = new System.Drawing.Size(279, 23);
            this.register_btn.TabIndex = 4;
            this.register_btn.Text = "У вас нет аккаунта? Нажмите сюда!";
            this.register_btn.UseVisualStyleBackColor = true;
            this.register_btn.Click += new System.EventHandler(this.register_btn_Click);
            // 
            // hide_password_btn
            // 
            this.hide_password_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hide_password_btn.Location = new System.Drawing.Point(236, 127);
            this.hide_password_btn.Name = "hide_password_btn";
            this.hide_password_btn.Size = new System.Drawing.Size(64, 20);
            this.hide_password_btn.TabIndex = 3;
            this.hide_password_btn.Text = "показать";
            this.hide_password_btn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.hide_password_btn.UseVisualStyleBackColor = true;
            this.hide_password_btn.Click += new System.EventHandler(this.hide_password_btn_Click);
            // 
            // AuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 347);
            this.Controls.Add(this.hide_password_btn);
            this.Controls.Add(this.register_btn);
            this.Controls.Add(this.dev_btn);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.password_tb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.login_tb);
            this.Controls.Add(this.auth_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AuthForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Авторизация";
            this.LocationChanged += new System.EventHandler(this.AuthForm_LocationChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button auth_btn;
        private System.Windows.Forms.TextBox login_tb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox password_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button exit_btn;
        private System.Windows.Forms.Button dev_btn;
        private System.Windows.Forms.Button register_btn;
        private System.Windows.Forms.Button hide_password_btn;
    }
}

