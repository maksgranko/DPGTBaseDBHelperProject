using DPGTProject.Configs;
using DPGTProject.Forms;
using System;
using System.Windows.Forms;

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
            if (!RoleManager.IsRoleExists(roles_cb.Text))
            {
                MessageBox.Show("Указанная роль не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Auth.RegisterUser(login_tb.Text, password_tb.Text, roles_cb.Text))
            {
                UserConfig.Login(login_tb.Text);
                MessageBox.Show("Успешная регистрация!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SystemConfig.mainForm = new MainForm();
                this.Hide();
                SystemConfig.mainForm.Location = this.Location;
                SystemConfig.mainForm.ShowDialog();
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
            SystemConfig.LastLocation = this.Location;
        }
    }
}
