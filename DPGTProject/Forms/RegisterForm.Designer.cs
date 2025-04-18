namespace DPGTProject
{
    partial class RegisterForm
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
            this.register_btn = new System.Windows.Forms.Button();
            this.login_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.password_tb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.exit_btn = new System.Windows.Forms.Button();
            this.dev_btn = new System.Windows.Forms.Button();
            this.login_btn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.password_verify_tb = new System.Windows.Forms.TextBox();
            this.hide_password_btn = new System.Windows.Forms.Button();
            this.roles_cb = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // register_btn
            // 
            this.register_btn.Location = new System.Drawing.Point(21, 199);
            this.register_btn.Name = "register_btn";
            this.register_btn.Size = new System.Drawing.Size(279, 23);
            this.register_btn.TabIndex = 5;
            this.register_btn.Text = "Зарегистрироваться";
            this.register_btn.UseVisualStyleBackColor = true;
            this.register_btn.Click += new System.EventHandler(this.register_btn_Click);
            // 
            // login_tb
            // 
            this.login_tb.Location = new System.Drawing.Point(21, 62);
            this.login_tb.Name = "login_tb";
            this.login_tb.Size = new System.Drawing.Size(279, 20);
            this.login_tb.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(18, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Логин:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(18, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(282, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Пароль:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // password_tb
            // 
            this.password_tb.Location = new System.Drawing.Point(21, 105);
            this.password_tb.Name = "password_tb";
            this.password_tb.Size = new System.Drawing.Size(279, 20);
            this.password_tb.TabIndex = 1;
            this.password_tb.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(21, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(279, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Регистрация";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // exit_btn
            // 
            this.exit_btn.Location = new System.Drawing.Point(24, 301);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(279, 23);
            this.exit_btn.TabIndex = 8;
            this.exit_btn.Text = "Выход";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // dev_btn
            // 
            this.dev_btn.Location = new System.Drawing.Point(24, 272);
            this.dev_btn.Name = "dev_btn";
            this.dev_btn.Size = new System.Drawing.Size(279, 23);
            this.dev_btn.TabIndex = 7;
            this.dev_btn.Text = "Разработчики";
            this.dev_btn.UseVisualStyleBackColor = true;
            this.dev_btn.Click += new System.EventHandler(this.dev_btn_Click);
            // 
            // login_btn
            // 
            this.login_btn.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.login_btn.Location = new System.Drawing.Point(24, 243);
            this.login_btn.Name = "login_btn";
            this.login_btn.Size = new System.Drawing.Size(279, 23);
            this.login_btn.TabIndex = 6;
            this.login_btn.Text = "У вас уже есть аккаунт? Нажмите сюда!";
            this.login_btn.UseVisualStyleBackColor = true;
            this.login_btn.Click += new System.EventHandler(this.login_btn_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(16, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(282, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Подтверждение пароля:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // password_verify_tb
            // 
            this.password_verify_tb.Location = new System.Drawing.Point(21, 146);
            this.password_verify_tb.Name = "password_verify_tb";
            this.password_verify_tb.Size = new System.Drawing.Size(279, 20);
            this.password_verify_tb.TabIndex = 3;
            this.password_verify_tb.UseSystemPasswordChar = true;
            // 
            // hide_password_btn
            // 
            this.hide_password_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hide_password_btn.Location = new System.Drawing.Point(236, 126);
            this.hide_password_btn.Name = "hide_password_btn";
            this.hide_password_btn.Size = new System.Drawing.Size(64, 20);
            this.hide_password_btn.TabIndex = 2;
            this.hide_password_btn.Text = "показать";
            this.hide_password_btn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.hide_password_btn.UseVisualStyleBackColor = true;
            this.hide_password_btn.Click += new System.EventHandler(this.hide_password_btn_Click);
            // 
            // roles_cb
            // 
            this.roles_cb.FormattingEnabled = true;
            this.roles_cb.Location = new System.Drawing.Point(139, 172);
            this.roles_cb.Name = "roles_cb";
            this.roles_cb.Size = new System.Drawing.Size(161, 21);
            this.roles_cb.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(24, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 21);
            this.label5.TabIndex = 13;
            this.label5.Text = "Выбор роли:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 347);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.roles_cb);
            this.Controls.Add(this.hide_password_btn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.password_verify_tb);
            this.Controls.Add(this.login_btn);
            this.Controls.Add(this.dev_btn);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.password_tb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.login_tb);
            this.Controls.Add(this.register_btn);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Регистрация";
            this.LocationChanged += new System.EventHandler(this.RegisterForm_LocationChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button register_btn;
        private System.Windows.Forms.TextBox login_tb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox password_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button exit_btn;
        private System.Windows.Forms.Button dev_btn;
        private System.Windows.Forms.Button login_btn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox password_verify_tb;
        private System.Windows.Forms.Button hide_password_btn;
        private System.Windows.Forms.ComboBox roles_cb;
        private System.Windows.Forms.Label label5;
    }
}

