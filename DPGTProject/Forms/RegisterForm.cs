using DPGTProject.Configs;
using System;
using System.Windows.Forms;

namespace DPGTProject
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            DesignConfig.ApplyTheme(SystemConfig.applicationTheme, this);
            this.roles_cb.Items.AddRange(SystemConfig.roles);
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
            if (Auth.CheckIsUserValid(login_tb.Text, password_tb.Text))
            {
                MessageBox.Show("Пользователь уже зарегистрирован!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (password_tb.Text != password_verify_tb.Text)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Functions.IsRoleExists(roles_cb.Text))
            {
                MessageBox.Show("Указанная роль не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Auth.RegisterUser(login_tb.Text, password_tb.Text, roles_cb.Text))
            {
                UserConfig.userLogin = login_tb.Text;
                UserConfig.userRole = Auth.GetUserStatus(login_tb.Text);
                MessageBox.Show("Успешная регистрация!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MainForm mf = new MainForm();
                this.Hide();
                mf.Location = this.Location;
                mf.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Возникла ошибка при регистрации аккаунта!\nОшибка: "+SystemConfig.lastError, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            SystemConfig.LastLocation = this.Location;
        }
    }
}


