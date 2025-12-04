using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class UserEditForm : Form
    {
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private TextBox fullNameTextBox;
        private ComboBox roleComboBox;
        private CheckBox isActiveCheckBox;
        private Button saveButton;
        private Button cancelButton;
        private Models.User? user;

        public UserEditForm(Models.User? existingUser = null)
        {
            user = existingUser;
            InitializeComponent();
            SetupForm();
            if (user != null)
            {
                LoadUserData();
            }
        }

        private void SetupForm()
        {
            this.Text = user == null ? "Добавление пользователя" : "Редактирование пользователя";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int controlWidth = 200;
            int height = 25;
            int spacing = 35;
            int startY = 20;

            // Логин
            AddLabel("Логин:", 20, startY);
            usernameTextBox = AddTextBox(150, startY, controlWidth);

            // Пароль
            AddLabel("Пароль:", 20, startY + spacing);
            passwordTextBox = AddTextBox(150, startY + spacing, controlWidth);
            passwordTextBox.PasswordChar = '*';

            // Полное имя
            AddLabel("Полное имя:", 20, startY + 2 * spacing);
            fullNameTextBox = AddTextBox(150, startY + 2 * spacing, controlWidth);

            // Роль
            AddLabel("Роль:", 20, startY + 3 * spacing);
            roleComboBox = new ComboBox();
            roleComboBox.Location = new Point(150, startY + 3 * spacing);
            roleComboBox.Size = new Size(controlWidth, height);
            roleComboBox.Items.AddRange(new string[] { "Администратор", "Менеджер", "Мастер" });
            roleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(roleComboBox);

            // Активен
            AddLabel("Активен:", 20, startY + 4 * spacing);
            isActiveCheckBox = new CheckBox();
            isActiveCheckBox.Location = new Point(150, startY + 4 * spacing);
            isActiveCheckBox.Size = new Size(20, height);
            isActiveCheckBox.Checked = true;
            this.Controls.Add(isActiveCheckBox);

            // Кнопки
            saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Size = new Size(100, 35);
            saveButton.Location = new Point(100, startY + 6 * spacing);
            saveButton.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            saveButton.BackColor = Color.FromArgb(84, 111, 148);
            saveButton.ForeColor = Color.White;
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Cursor = Cursors.Hand;
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.Size = new Size(100, 35);
            cancelButton.Location = new Point(210, startY + 6 * spacing);
            cancelButton.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            cancelButton.BackColor = Color.FromArgb(171, 207, 206);
            cancelButton.ForeColor = Color.Black;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Cursor = Cursors.Hand;
            cancelButton.Click += CancelButton_Click;
            this.Controls.Add(cancelButton);
        }

        private Label AddLabel(string text, int x, int y)
        {
            var label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Size = new Size(120, 25);
            label.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            this.Controls.Add(label);
            return label;
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            var textBox = new TextBox();
            textBox.Location = new Point(x, y);
            textBox.Size = new Size(width, 25);
            textBox.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            this.Controls.Add(textBox);
            return textBox;
        }

        private void LoadUserData()
        {
            if (user == null) return;

            usernameTextBox.Text = user.Username;
            passwordTextBox.Text = user.Password;
            fullNameTextBox.Text = user.FullName;
            roleComboBox.Text = GetRoleName(user.Role);
            isActiveCheckBox.Checked = user.IsActive;
        }

        private string GetRoleName(Models.UserRole role)
        {
            return role switch
            {
                Models.UserRole.Admin => "Администратор",
                Models.UserRole.Manager => "Менеджер",
                Models.UserRole.Master => "Мастер",
                _ => "Администратор"
            };
        }

        private Models.UserRole GetRoleFromName(string roleName)
        {
            return roleName switch
            {
                "Администратор" => Models.UserRole.Admin,
                "Менеджер" => Models.UserRole.Manager,
                "Мастер" => Models.UserRole.Master,
                _ => Models.UserRole.Admin
            };
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            SaveUser();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                MessageBox.Show("Введите логин", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(fullNameTextBox.Text))
            {
                MessageBox.Show("Введите полное имя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (roleComboBox.SelectedIndex == -1 && string.IsNullOrEmpty(roleComboBox.Text))
            {
                MessageBox.Show("Выберите роль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Проверка уникальности логина
            if (!IsUsernameUnique(usernameTextBox.Text.Trim(), user?.Id ?? 0))
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool IsUsernameUnique(string username, int excludeUserId = 0)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Id != @ExcludeId";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@ExcludeId", excludeUserId);

                    var count = Convert.ToInt32(command.ExecuteScalar());
                    return count == 0;
                }
            }
        }

        private void SaveUser()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query;
                if (user == null || user.Id == 0)
                {
                    // Создание нового пользователя
                    query = @"INSERT INTO Users (Username, Password, FullName, Role, CreatedDate, IsActive)
                             VALUES (@Username, @Password, @FullName, @Role, @CreatedDate, @IsActive)";
                }
                else
                {
                    // Обновление существующего пользователя
                    query = @"UPDATE Users SET Username = @Username, Password = @Password, FullName = @FullName,
                             Role = @Role, IsActive = @IsActive WHERE Id = @Id";
                }

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", usernameTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Password", passwordTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@FullName", fullNameTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Role", (int)GetRoleFromName(roleComboBox.Text));
                    command.Parameters.AddWithValue("@IsActive", isActiveCheckBox.Checked);

                    if (user != null && user.Id != 0)
                    {
                        command.Parameters.AddWithValue("@Id", user.Id);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}

