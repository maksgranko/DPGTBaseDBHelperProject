using DPGTProject.Configs;
using DPGTProject.Forms;
using System;
using System.Windows.Forms;
using Scraps.Security;

namespace DPGTProject
{
    public partial class RegisterForm : BaseForm
    {
        public RegisterForm()
        {
            InitializeComponent();
            this.roles_cb.Items.AddRange(SystemConfig.roles);
            if (!SystemConfig.addRolesWhenRegistering) { roles_cb.Visible = false; roles_label.Visible = false; }
            roles_cb.SelectedIndex = roles_cb.Items.Count - 1; // выбор самой мелкой(последней) роли по умолчанию 
        }

        private void register_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(login_tb.Text) ||
                string.IsNullOrEmpty(password_tb.Text) ||
                string.IsNullOrEmpty(password_verify_tb.Text) ||
                string.IsNullOrEmpty(roles_cb.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool userExists;
            try
            {
                userExists = UserSession.CheckIsUserExists(login_tb.Text);
            }
            catch (Exception ex)
            {
                SystemConfig.lastError = ex.Message;
                MessageBox.Show("Не удалось проверить логин. Обратитесь к администратору.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (userExists)
            {
                MessageBox.Show("Пользователь уже зарегистрирован!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (password_tb.Text != password_verify_tb.Text)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Array.IndexOf(SystemConfig.roles, roles_cb.Text) < 0)
            {
                MessageBox.Show("Указанная роль не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool registered;
            try
            {
                if (!UserSession.Utilities.IsPasswordValid(password_tb.Text))
                {
                    MessageBox.Show("Пароль не подходит по условиям!\nУсловия:\nСодержится хотя-бы одна заглавная латинская буква\nСодержится спец. символ\nКоличество символов не менее 8-ми.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                UserSession.Register(login_tb.Text, password_tb.Text, roles_cb.Text, loginAfterRegistration: false);
                registered = true;
            }
            catch (Exception ex)
            {
                SystemConfig.lastError = ex.Message;
                registered = false;
            }

            if (registered)
            {
                UserConfig.Login(login_tb.Text);
                MessageBox.Show("Успешная регистрация!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var mainForm = new MainForm();
                this.Hide();
                mainForm.Location = this.Location;
                mainForm.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Возникла ошибка при регистрации аккаунта!\nОшибка: " + SystemConfig.lastError, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            Functions.Exit();
        }

        private void dev_btn_Click(object sender, EventArgs e)
        {
            Functions.Developers();
        }

        private void hide_password_btn_Click(object sender, EventArgs e)
        {

            if (hide_password_btn.Text == "скрыть")
            {
                hide_password_btn.Text = "показать";
                password_tb.UseSystemPasswordChar = true;
                password_verify_tb.UseSystemPasswordChar = true;
            }
            else
            {
                hide_password_btn.Text = "скрыть";
                password_tb.UseSystemPasswordChar = false;
                password_verify_tb.UseSystemPasswordChar = false;
            }
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
        }

        private void RegisterForm_LocationChanged(object sender, EventArgs e)
        {
            // Reserved for future UI state persistence.
        }
    }
}
