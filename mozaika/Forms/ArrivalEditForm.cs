using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;
using Mozaika.Models;

namespace Mozaika.Forms
{
    public partial class ArrivalEditForm : Form
    {
        private ComboBox? materialComboBox;
        private TextBox? quantityTextBox;
        private TextBox? supplierTextBox;
        private DateTimePicker? arrivalDatePicker;
        private Button? saveButton;
        private Button? cancelButton;

        public ArrivalEditForm()
        {
            InitializeComponent();
            SetupForm();
            LoadMaterials();
        }

        private void SetupForm()
        {
            this.Text = "Регистрация поступления";
            this.Size = new Size(450, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int controlWidth = 250;
            int height = 25;
            int spacing = 35;
            int startY = 20;

            // Материал
            AddLabel("Материал:", 20, startY);
            materialComboBox = new ComboBox();
            materialComboBox.Location = new Point(150, startY);
            materialComboBox.Size = new Size(controlWidth, height);
            materialComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            materialComboBox.SelectedIndexChanged += MaterialComboBox_SelectedIndexChanged;
            this.Controls.Add(materialComboBox);

            // Количество
            AddLabel("Количество:", 20, startY + spacing);
            quantityTextBox = AddTextBox(150, startY + spacing, controlWidth);

            // Поставщик
            AddLabel("Поставщик:", 20, startY + 2 * spacing);
            supplierTextBox = AddTextBox(150, startY + 2 * spacing, controlWidth);
            supplierTextBox.ReadOnly = true;

            // Дата поступления
            AddLabel("Дата поступления:", 20, startY + 3 * spacing);
            arrivalDatePicker = new DateTimePicker();
            arrivalDatePicker.Location = new Point(150, startY + 3 * spacing);
            arrivalDatePicker.Size = new Size(controlWidth, height);
            arrivalDatePicker.Format = DateTimePickerFormat.Short;
            arrivalDatePicker.Value = DateTime.Now;
            this.Controls.Add(arrivalDatePicker);

            // Кнопки
            saveButton = new Button();
            saveButton.Text = "Зарегистрировать";
            saveButton.Size = new Size(120, 35);
            saveButton.Location = new Point(150, startY + 5 * spacing);
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
            cancelButton.Location = new Point(280, startY + 5 * spacing);
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

        private void LoadMaterials()
        {
            if (materialComboBox == null) return;

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT m.Id, m.Name || ' (' || s.Name || ')' as DisplayName, s.Name as SupplierName
                               FROM Materials m
                               JOIN Suppliers s ON m.SupplierId = s.Id
                               ORDER BY m.Name";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        materialComboBox.Items.Add(new MaterialItem
                        {
                            Id = reader.GetInt32(0),
                            DisplayName = reader.GetString(1),
                            SupplierName = reader.GetString(2)
                        });
                    }
                }
            }

            materialComboBox.DisplayMember = "DisplayName";
            materialComboBox.ValueMember = "Id";

            if (materialComboBox.Items.Count > 0)
            {
                materialComboBox.SelectedIndex = 0;
                UpdateSupplierInfo();
            }
        }

        private void UpdateSupplierInfo()
        {
            if (materialComboBox?.SelectedItem is MaterialItem item && supplierTextBox != null)
            {
                supplierTextBox.Text = item.SupplierName;
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            RegisterArrival();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (materialComboBox?.SelectedItem == null)
            {
                MessageBox.Show("Выберите материал", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(quantityTextBox?.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void RegisterArrival()
        {
            if (materialComboBox?.SelectedItem is not MaterialItem selectedItem)
                return;
            if (quantityTextBox?.Text == null || arrivalDatePicker == null)
                return;

            int quantity = int.Parse(quantityTextBox.Text);

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                // Обновляем количество материала на складе
                string updateQuery = "UPDATE Materials SET StockQuantity = StockQuantity + @Quantity, LastUpdated = @LastUpdated WHERE Id = @Id";

                using (var command = new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@LastUpdated", arrivalDatePicker.Value);
                    command.Parameters.AddWithValue("@Id", selectedItem.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void MaterialComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateSupplierInfo();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // ArrivalEditForm
            // 
            ClientSize = new Size(282, 253);
            Name = "ArrivalEditForm";
            ResumeLayout(false);
        }

        private class MaterialItem
        {
            public int Id { get; set; }
            public string? DisplayName { get; set; }
            public string? SupplierName { get; set; }
        }
    }
}
