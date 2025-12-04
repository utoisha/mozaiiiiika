using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;
using Mozaika.Models;

namespace Mozaika.Forms
{
    public partial class EmployeeEditForm : Form
    {
        private Employee? employee;
        private TextBox fullNameTextBox;
        private DateTimePicker birthDatePicker;
        private TextBox passportTextBox;
        private TextBox bankDetailsTextBox;
        private CheckBox hasFamilyCheckBox;
        private TextBox healthStatusTextBox;
        private TextBox positionTextBox;
        private DateTimePicker hireDatePicker;
        private TextBox salaryTextBox;
        private TextBox equipmentAccessTextBox;
        private Button saveButton;
        private Button cancelButton;

        public EmployeeEditForm(Employee? emp = null)
        {
            employee = emp;
            InitializeComponent();
            SetupForm();
            if (employee != null)
            {
                LoadEmployeeData();
            }
        }

        private void SetupForm()
        {
            this.Text = employee == null ? "Добавление сотрудника" : "Редактирование сотрудника";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int controlWidth = 300;
            int height = 25;
            int spacing = 35;
            int startY = 20;

            // ФИО
            AddLabel("ФИО:", 20, startY);
            fullNameTextBox = AddTextBox(180, startY, controlWidth);

            // Дата рождения
            AddLabel("Дата рождения:", 20, startY + spacing);
            birthDatePicker = new DateTimePicker();
            birthDatePicker.Location = new Point(180, startY + spacing);
            birthDatePicker.Size = new Size(controlWidth, height);
            birthDatePicker.Format = DateTimePickerFormat.Short;
            this.Controls.Add(birthDatePicker);

            // Паспортные данные
            AddLabel("Паспортные данные:", 20, startY + 2 * spacing);
            passportTextBox = AddTextBox(180, startY + 2 * spacing, controlWidth);

            // Банковские реквизиты
            AddLabel("Банковские реквизиты:", 20, startY + 3 * spacing);
            bankDetailsTextBox = AddTextBox(180, startY + 3 * spacing, controlWidth);

            // Наличие семьи
            AddLabel("Наличие семьи:", 20, startY + 4 * spacing);
            hasFamilyCheckBox = new CheckBox();
            hasFamilyCheckBox.Location = new Point(180, startY + 4 * spacing);
            hasFamilyCheckBox.Size = new Size(20, height);
            this.Controls.Add(hasFamilyCheckBox);

            // Состояние здоровья
            AddLabel("Состояние здоровья:", 20, startY + 5 * spacing);
            healthStatusTextBox = AddTextBox(180, startY + 5 * spacing, controlWidth);

            // Должность
            AddLabel("Должность:", 20, startY + 6 * spacing);
            positionTextBox = AddTextBox(180, startY + 6 * spacing, controlWidth);

            // Дата приема на работу
            AddLabel("Дата приема:", 20, startY + 7 * spacing);
            hireDatePicker = new DateTimePicker();
            hireDatePicker.Location = new Point(180, startY + 7 * spacing);
            hireDatePicker.Size = new Size(controlWidth, height);
            hireDatePicker.Format = DateTimePickerFormat.Short;
            this.Controls.Add(hireDatePicker);

            // Зарплата
            AddLabel("Зарплата (руб.):", 20, startY + 8 * spacing);
            salaryTextBox = AddTextBox(180, startY + 8 * spacing, controlWidth);

            // Доступ к оборудованию
            AddLabel("Доступ к оборудованию:", 20, startY + 9 * spacing);
            equipmentAccessTextBox = AddTextBox(180, startY + 9 * spacing, controlWidth);
            equipmentAccessTextBox.Multiline = true;
            equipmentAccessTextBox.Height = 60;

            // Кнопки
            saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Size = new Size(100, 35);
            saveButton.Location = new Point(180, startY + 12 * spacing);
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
            cancelButton.Location = new Point(300, startY + 12 * spacing);
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
            label.Size = new Size(150, 25);
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

        private void LoadEmployeeData()
        {
            if (employee == null) return;

            fullNameTextBox.Text = employee.FullName;
            birthDatePicker.Value = employee.BirthDate;
            passportTextBox.Text = employee.PassportData;
            bankDetailsTextBox.Text = employee.BankDetails;
            hasFamilyCheckBox.Checked = employee.HasFamily;
            healthStatusTextBox.Text = employee.HealthStatus;
            positionTextBox.Text = employee.Position;
            hireDatePicker.Value = employee.HireDate;
            salaryTextBox.Text = employee.Salary.ToString();
            equipmentAccessTextBox.Text = employee.EquipmentAccess;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var emp = employee ?? new Employee();

            emp.FullName = fullNameTextBox.Text.Trim();
            emp.BirthDate = birthDatePicker.Value;
            emp.PassportData = passportTextBox.Text.Trim();
            emp.BankDetails = bankDetailsTextBox.Text.Trim();
            emp.HasFamily = hasFamilyCheckBox.Checked;
            emp.HealthStatus = healthStatusTextBox.Text.Trim();
            emp.Position = positionTextBox.Text.Trim();
            emp.HireDate = hireDatePicker.Value;
            emp.Salary = decimal.Parse(salaryTextBox.Text);
            emp.EquipmentAccess = equipmentAccessTextBox.Text.Trim();

            SaveEmployee(emp);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(fullNameTextBox.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(passportTextBox.Text))
            {
                MessageBox.Show("Введите паспортные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(positionTextBox.Text))
            {
                MessageBox.Show("Введите должность", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!decimal.TryParse(salaryTextBox.Text, out _))
            {
                MessageBox.Show("Введите корректную зарплату", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void SaveEmployee(Employee emp)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query;
                if (emp.Id == 0)
                {
                    query = @"INSERT INTO Employees (FullName, BirthDate, PassportData, BankDetails,
                              HasFamily, HealthStatus, Position, HireDate, Salary, EquipmentAccess)
                              VALUES (@FullName, @BirthDate, @PassportData, @BankDetails,
                              @HasFamily, @HealthStatus, @Position, @HireDate, @Salary, @EquipmentAccess)";
                }
                else
                {
                    query = @"UPDATE Employees SET FullName = @FullName, BirthDate = @BirthDate,
                              PassportData = @PassportData, BankDetails = @BankDetails,
                              HasFamily = @HasFamily, HealthStatus = @HealthStatus,
                              Position = @Position, HireDate = @HireDate, Salary = @Salary,
                              EquipmentAccess = @EquipmentAccess WHERE Id = @Id";
                }

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", emp.FullName);
                    command.Parameters.AddWithValue("@BirthDate", emp.BirthDate);
                    command.Parameters.AddWithValue("@PassportData", emp.PassportData);
                    command.Parameters.AddWithValue("@BankDetails", emp.BankDetails);
                    command.Parameters.AddWithValue("@HasFamily", emp.HasFamily);
                    command.Parameters.AddWithValue("@HealthStatus", emp.HealthStatus);
                    command.Parameters.AddWithValue("@Position", emp.Position);
                    command.Parameters.AddWithValue("@HireDate", emp.HireDate);
                    command.Parameters.AddWithValue("@Salary", emp.Salary);
                    command.Parameters.AddWithValue("@EquipmentAccess", emp.EquipmentAccess);

                    if (emp.Id != 0)
                    {
                        command.Parameters.AddWithValue("@Id", emp.Id);
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
