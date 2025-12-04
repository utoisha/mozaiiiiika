using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Services;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Дополнительная настройка после InitializeComponent
            this.AcceptButton = loginButton;
            passwordTextBox.KeyDown += PasswordTextBox_KeyDown;
        }

       

        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (AuthService.Login(username, password))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // Проверяем, существует ли пользователь вообще
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        var count = Convert.ToInt32(command.ExecuteScalar());
                        if (count == 0)
                        {
                            MessageBox.Show($"Пользователь '{username}' не найден в базе данных. Проверьте правильность ввода.", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Неверный пароль", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                passwordTextBox.Clear();
                passwordTextBox.Focus();
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}

