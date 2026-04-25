using DPGTProject.Forms;
using System;
using System.Data;
using System.Windows.Forms;
using Scraps.Security;

namespace DPGTProject
{
    public partial class AuthForm : BaseForm
    {
        public AuthForm()
        {
            InitializeComponent();
            if (Test.Initialized)
            {
                login_tb.Text = Test.login;
                password_tb.Text = Test.password;
            }
        }

        private void dev_btn_Click(object sender, EventArgs e)
        {
            Functions.Developers();
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            Functions.Exit();
        }

        private void hide_password_btn_Click(object sender, EventArgs e)
        {
            if (hide_password_btn.Text == "скрыть")
            {
                hide_password_btn.Text = "показать";
                password_tb.UseSystemPasswordChar = true;
            }
            else
            {
                hide_password_btn.Text = "скрыть";
                password_tb.UseSystemPasswordChar = false;
            }
        }

        private void auth_btn_Click(object sender, EventArgs e)
        {
            AuthUser();
        }

        private void AuthUser()
        {
            if (string.IsNullOrEmpty(login_tb.Text) || string.IsNullOrEmpty(password_tb.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool validCredentials;
            try
            {
                validCredentials = UserSession.CheckIsUserValid(login_tb.Text, password_tb.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить вход. Обратитесь к администратору.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!validCredentials)
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UserSession.LoginByName(login_tb.Text);

            var mainForm = new MainForm();
            this.Hide();
            mainForm.Location = this.Location;
            if (mainForm.ShowDialog() != DialogResult.Retry) { Functions.Exit(); }
            if (!this.IsDisposed) this.Show();
        }

        private void register_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            RegisterForm rf = new RegisterForm();
            rf.Location = this.Location;
            rf.ShowDialog();
            if (!this.IsDisposed) this.Show();
        }

        private void AuthForm_LocationChanged(object sender, EventArgs e)
        {
            // Reserved for future UI state persistence.
        }

        private void password_tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                AuthUser();
            }
        }
    }
}
