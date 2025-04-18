using DPGTProject.Configs;
using System;
using System.Windows.Forms;

namespace DPGTProject
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
            DesignConfig.ApplyTheme(SystemConfig.applicationTheme, this.Controls);
            if (Test.Initialized)
            {
                login_tb.Text = "www";
                password_tb.Text = "TestData1234@";
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
            if(hide_password_btn.Text == "скрыть")
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
            if (!Auth.CheckIsUserValid(login_tb.Text, password_tb.Text))
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            UserConfig.userLogin = login_tb.Text;
            UserConfig.userRole = Auth.GetUserStatus(login_tb.Text);
            MainForm mf = new MainForm();
            this.Hide();
            mf.Location = this.Location;
            if (mf.ShowDialog() != DialogResult.Retry) { Functions.Exit(); }
            if(!this.IsDisposed) this.Show();
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
            SystemConfig.LastLocation = this.Location;
        }

        private void password_tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                AuthUser();
            }
        }
    }
}


