using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class UsersManagementForm : Form
    {
        private DataGridView usersGrid;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button backButton;
        private Label titleLabel;

        public UsersManagementForm()
        {
            InitializeComponent();
            SetupForm();
            LoadUsers();
        }

        private void SetupForm()
        {
            this.Text = "Управление пользователями системы";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            // Заголовок
            titleLabel = new Label();
            titleLabel.Text = "Управление пользователями системы";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(400, 30);
            this.Controls.Add(titleLabel);

            // Таблица пользователей
            usersGrid = new DataGridView();
            usersGrid.Location = new Point(20, 60);
            usersGrid.Size = new Size(940, 500);
            usersGrid.BackgroundColor = Color.White;
            usersGrid.BorderStyle = BorderStyle.FixedSingle;
            usersGrid.AllowUserToAddRows = false;
            usersGrid.AllowUserToDeleteRows = false;
            usersGrid.ReadOnly = true;
            usersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            usersGrid.MultiSelect = false;
            usersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(usersGrid);

            // Кнопки управления
            addButton = CreateStyledButton("Добавить", 20, 580);
            addButton.Click += AddButton_Click;
            this.Controls.Add(addButton);

            editButton = CreateStyledButton("Редактировать", 140, 580);
            editButton.Size = new Size(130, 35); // Увеличиваем ширину кнопки редактирования
            editButton.Click += EditButton_Click;
            this.Controls.Add(editButton);

            deleteButton = CreateStyledButton("Удалить", 290, 580);
            deleteButton.BackColor = Color.FromArgb(171, 207, 206);
            deleteButton.Click += DeleteButton_Click;
            this.Controls.Add(deleteButton);

            // Кнопка назад
            backButton = CreateStyledButton("Назад", 850, 580);
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);
        }

        private Button CreateStyledButton(string text, int x, int y)
        {
            var button = new Button();
            button.Text = text;
            button.Size = new Size(100, 35);
            button.Location = new Point(x, y);
            button.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            button.BackColor = Color.FromArgb(84, 111, 148);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            return button;
        }

        private void LoadUsers()
        {
            usersGrid.Columns.Clear();
            usersGrid.Rows.Clear();

            usersGrid.Columns.Add("Id", "ID");
            usersGrid.Columns.Add("Username", "Логин");
            usersGrid.Columns.Add("Password", "Пароль");
            usersGrid.Columns.Add("FullName", "Полное имя");
            usersGrid.Columns.Add("Role", "Роль");
            usersGrid.Columns.Add("IsActive", "Активен");

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Username, Password, FullName, Role, IsActive
                               FROM Users ORDER BY Username";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string roleName = GetRoleName(reader.GetInt32(4));
                        string activeStatus = reader.GetBoolean(5) ? "Да" : "Нет";

                        usersGrid.Rows.Add(
                            reader.GetInt32(0),    // Id
                            reader.GetString(1),   // Username
                            reader.GetString(2),   // Password (не рекомендуется показывать в реальном приложении)
                            reader.GetString(3),   // FullName
                            roleName,              // Role
                            activeStatus           // IsActive
                        );
                    }
                }
            }

            usersGrid.Columns["Id"].Visible = false;
        }

        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                0 => "Администратор",
                1 => "Менеджер",
                2 => "Мастер",
                _ => "Неизвестно"
            };
        }

        private int GetRoleId(string roleName)
        {
            return roleName switch
            {
                "Администратор" => 0,
                "Менеджер" => 1,
                "Мастер" => 2,
                _ => 0
            };
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var editForm = new UserEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (usersGrid.SelectedRows.Count == 0) return;

            int userId = (int)usersGrid.SelectedRows[0].Cells["Id"].Value;
            var user = GetUserById(userId);
            if (user != null)
            {
                var editForm = new UserEditForm(user);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (usersGrid.SelectedRows.Count == 0) return;

            int userId = (int)usersGrid.SelectedRows[0].Cells["Id"].Value;

            var result = MessageBox.Show("Вы действительно хотите удалить этого пользователя?",
                                       "Подтверждение удаления",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                DeleteUser(userId);
                LoadUsers();
            }
        }

        private Models.User? GetUserById(int userId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Username, Password, FullName, Role, CreatedDate, IsActive
                               FROM Users WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Models.User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                FullName = reader.GetString(3),
                                Role = (Models.UserRole)reader.GetInt32(4),
                                CreatedDate = reader.GetDateTime(5),
                                IsActive = reader.GetBoolean(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void DeleteUser(int userId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Users WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}

