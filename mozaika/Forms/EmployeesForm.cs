using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;
using Mozaika.Models;

namespace Mozaika.Forms
{
    public partial class EmployeesForm : Form
    {
        private DataGridView employeesGrid;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button backButton;
        private TextBox searchTextBox;
        private Label searchLabel;
        private List<Employee> employees;

        public EmployeesForm()
        {
            InitializeComponent();
            SetupForm();
            LoadEmployees();
        }

        private void SetupForm()
        {
            this.Text = "Управление сотрудниками";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            // Заголовок
            var titleLabel = new Label();
            titleLabel.Text = "Управление сотрудниками";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(400, 30);
            this.Controls.Add(titleLabel);

            // Поле поиска
            searchLabel = new Label();
            searchLabel.Text = "Поиск по ФИО:";
            searchLabel.Location = new Point(20, 60);
            searchLabel.Size = new Size(120, 25);
            this.Controls.Add(searchLabel);

            searchTextBox = new TextBox();
            searchTextBox.Location = new Point(140, 60);
            searchTextBox.Size = new Size(250, 25);
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            this.Controls.Add(searchTextBox);

            // Таблица сотрудников
            employeesGrid = new DataGridView();
            employeesGrid.Location = new Point(20, 100);
            employeesGrid.Size = new Size(940, 500);
            employeesGrid.BackgroundColor = Color.White;
            employeesGrid.BorderStyle = BorderStyle.FixedSingle;
            employeesGrid.AllowUserToAddRows = false;
            employeesGrid.AllowUserToDeleteRows = false;
            employeesGrid.ReadOnly = true;
            employeesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeesGrid.MultiSelect = false;
            employeesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(employeesGrid);

            // Кнопки
            addButton = CreateStyledButton("Добавить", 20, 620);
            addButton.Click += AddButton_Click;
            this.Controls.Add(addButton);

            editButton = CreateStyledButton("Редактировать", 130, 620);
            editButton.Click += EditButton_Click;
            this.Controls.Add(editButton);

            deleteButton = CreateStyledButton("Удалить", 240, 620);
            deleteButton.BackColor = Color.FromArgb(171, 207, 206);
            deleteButton.Click += DeleteButton_Click;
            this.Controls.Add(deleteButton);

            backButton = CreateStyledButton("Назад", 850, 620);
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

        private void LoadEmployees()
        {
            employees = GetEmployees();
            UpdateGrid(employees);
        }

        private List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, FullName, BirthDate, PassportData, BankDetails,
                                HasFamily, HealthStatus, Position, HireDate, Salary, EquipmentAccess
                                FROM Employees ORDER BY FullName";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            BirthDate = reader.GetDateTime(2),
                            PassportData = reader.GetString(3),
                            BankDetails = reader.GetString(4),
                            HasFamily = reader.GetBoolean(5),
                            HealthStatus = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            Position = reader.GetString(7),
                            HireDate = reader.GetDateTime(8),
                            Salary = reader.GetDecimal(9),
                            EquipmentAccess = reader.IsDBNull(10) ? "" : reader.GetString(10)
                        });
                    }
                }
            }

            return employees;
        }

        private void UpdateGrid(List<Employee> employeesList)
        {
            employeesGrid.Columns.Clear();
            employeesGrid.Rows.Clear();

            employeesGrid.Columns.Add("Id", "ID");
            employeesGrid.Columns.Add("FullName", "ФИО");
            employeesGrid.Columns.Add("BirthDate", "Дата рождения");
            employeesGrid.Columns.Add("Position", "Должность");
            employeesGrid.Columns.Add("HireDate", "Дата приема");
            employeesGrid.Columns.Add("Salary", "Зарплата");
            employeesGrid.Columns.Add("EquipmentAccess", "Доступ к оборудованию");

            foreach (var employee in employeesList)
            {
                employeesGrid.Rows.Add(
                    employee.Id,
                    employee.FullName,
                    employee.BirthDate.ToShortDateString(),
                    employee.Position,
                    employee.HireDate.ToShortDateString(),
                    employee.Salary.ToString("C"),
                    employee.EquipmentAccess
                );
            }

            employeesGrid.Columns["Id"].Visible = false;
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            var filteredEmployees = employees.FindAll(emp =>
                emp.FullName.ToLower().Contains(searchText) ||
                emp.Position.ToLower().Contains(searchText));

            UpdateGrid(filteredEmployees);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var editForm = new EmployeeEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadEmployees();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (employeesGrid.SelectedRows.Count == 0) return;

            int employeeId = (int)employeesGrid.SelectedRows[0].Cells["Id"].Value;
            var employee = employees.Find(emp => emp.Id == employeeId);

            if (employee != null)
            {
                var editForm = new EmployeeEditForm(employee);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadEmployees();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (employeesGrid.SelectedRows.Count == 0) return;

            int employeeId = (int)employeesGrid.SelectedRows[0].Cells["Id"].Value;

            var result = MessageBox.Show("Вы действительно хотите удалить этого сотрудника?",
                                       "Подтверждение удаления",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                DeleteEmployee(employeeId);
                LoadEmployees();
            }
        }

        private void DeleteEmployee(int employeeId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Employees WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", employeeId);
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



