using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class SupplierEditForm : Form
    {
        private ComboBox typeComboBox;
        private TextBox nameTextBox;
        private TextBox innTextBox;
        private TextBox contactInfoTextBox;
        private CheckBox isActiveCheckBox;
        private Button saveButton;
        private Button cancelButton;
        private Models.Supplier? supplier;

        public SupplierEditForm(Models.Supplier? existingSupplier = null)
        {
            supplier = existingSupplier;
            InitializeComponent();
            SetupForm();
            if (supplier != null)
            {
                LoadSupplierData();
            }
        }

        private void SetupForm()
        {
            this.Text = supplier == null ? "Добавление поставщика" : "Редактирование поставщика";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int controlWidth = 250;
            int height = 25;
            int spacing = 35;
            int startY = 20;

            // Тип поставщика
            AddLabel("Тип поставщика:", 20, startY);
            typeComboBox = new ComboBox();
            typeComboBox.Location = new Point(150, startY);
            typeComboBox.Size = new Size(controlWidth, height);
            typeComboBox.Items.AddRange(new string[] { "Производитель", "Поставщик сырья", "Дистрибьютор" });
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(typeComboBox);

            // Название
            AddLabel("Название:", 20, startY + spacing);
            nameTextBox = AddTextBox(150, startY + spacing, controlWidth);

            // ИНН
            AddLabel("ИНН:", 20, startY + 2 * spacing);
            innTextBox = AddTextBox(150, startY + 2 * spacing, controlWidth);

            // Контактная информация
            AddLabel("Контакты:", 20, startY + 3 * spacing);
            contactInfoTextBox = AddTextBox(150, startY + 3 * spacing, controlWidth);

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
            saveButton.Location = new Point(150, startY + 6 * spacing);
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
            cancelButton.Location = new Point(270, startY + 6 * spacing);
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

        private void LoadSupplierData()
        {
            if (supplier == null) return;

            typeComboBox.Text = supplier.Type;
            nameTextBox.Text = supplier.Name;
            innTextBox.Text = supplier.Inn;
            contactInfoTextBox.Text = supplier.ContactInfo;
            isActiveCheckBox.Checked = supplier.IsActive;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            SaveSupplier();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (typeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите тип поставщика", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Введите название поставщика", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(innTextBox.Text))
            {
                MessageBox.Show("Введите ИНН", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void SaveSupplier()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query;
                if (supplier == null || supplier.Id == 0)
                {
                    // Создание нового поставщика
                    query = @"INSERT INTO Suppliers (Type, Name, Inn, ContactInfo, CreatedDate, IsActive)
                             VALUES (@Type, @Name, @Inn, @ContactInfo, @CreatedDate, @IsActive)";
                }
                else
                {
                    // Обновление существующего поставщика
                    query = @"UPDATE Suppliers SET Type = @Type, Name = @Name, Inn = @Inn, ContactInfo = @ContactInfo, IsActive = @IsActive
                             WHERE Id = @Id";
                }

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Type", typeComboBox.Text);
                    command.Parameters.AddWithValue("@Name", nameTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Inn", innTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@ContactInfo", contactInfoTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@IsActive", isActiveCheckBox.Checked);

                    if (supplier != null && supplier.Id != 0)
                    {
                        command.Parameters.AddWithValue("@Id", supplier.Id);
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
