using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;
using Mozaika.Models;

namespace Mozaika.Forms
{
    public partial class SuppliersForm : Form
    {
        private DataGridView suppliersGrid;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button backButton;
        private Label titleLabel;

        public SuppliersForm()
        {
            InitializeComponent();
            SetupForm();
            LoadSuppliers();
        }

        private void SetupForm()
        {
            this.Text = "Поставщики";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            // Заголовок
            titleLabel = new Label();
            titleLabel.Text = "Управление поставщиками";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(300, 30);
            this.Controls.Add(titleLabel);

            // Таблица поставщиков
            suppliersGrid = new DataGridView();
            suppliersGrid.Location = new Point(20, 60);
            suppliersGrid.Size = new Size(940, 450);
            suppliersGrid.BackgroundColor = Color.White;
            suppliersGrid.BorderStyle = BorderStyle.FixedSingle;
            suppliersGrid.AllowUserToAddRows = false;
            suppliersGrid.AllowUserToDeleteRows = false;
            suppliersGrid.ReadOnly = true;
            suppliersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            suppliersGrid.MultiSelect = false;
            suppliersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(suppliersGrid);

            // Кнопки управления
            addButton = CreateStyledButton("Добавить", 20, 520);
            addButton.Click += AddButton_Click;
            this.Controls.Add(addButton);

            editButton = CreateStyledButton("Редактировать", 130, 520);
            editButton.Click += EditButton_Click;
            this.Controls.Add(editButton);

            deleteButton = CreateStyledButton("Удалить", 260, 520);
            deleteButton.BackColor = Color.FromArgb(171, 207, 206);
            deleteButton.Click += DeleteButton_Click;
            this.Controls.Add(deleteButton);

            // Кнопка назад
            backButton = CreateStyledButton("Назад", 20, 570);
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);
        }

        private void LoadSuppliers()
        {
            suppliersGrid.Columns.Clear();
            suppliersGrid.Rows.Clear();

            suppliersGrid.Columns.Add("Id", "ID");
            suppliersGrid.Columns.Add("Type", "Тип");
            suppliersGrid.Columns.Add("Name", "Название");
            suppliersGrid.Columns.Add("Inn", "ИНН");
            suppliersGrid.Columns.Add("ContactInfo", "Контакты");
            suppliersGrid.Columns.Add("IsActive", "Активен");

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Type, Name, Inn, ContactInfo, IsActive
                               FROM Suppliers ORDER BY Name";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string activeStatus = reader.GetBoolean(5) ? "Да" : "Нет";

                        suppliersGrid.Rows.Add(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.IsDBNull(4) ? "" : reader.GetString(4),
                            activeStatus
                        );
                    }
                }
            }

            suppliersGrid.Columns["Id"].Visible = false;
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

        private void AddButton_Click(object sender, EventArgs e)
        {
            var editForm = new SupplierEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadSuppliers();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (suppliersGrid.SelectedRows.Count == 0) return;

            int supplierId = (int)suppliersGrid.SelectedRows[0].Cells["Id"].Value;
            var supplier = GetSupplierById(supplierId);
            if (supplier != null)
            {
                var editForm = new SupplierEditForm(supplier);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadSuppliers();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (suppliersGrid.SelectedRows.Count == 0) return;

            int supplierId = (int)suppliersGrid.SelectedRows[0].Cells["Id"].Value;

            var result = MessageBox.Show("Вы действительно хотите удалить этого поставщика?",
                                       "Подтверждение удаления",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                DeleteSupplier(supplierId);
                LoadSuppliers();
            }
        }

        private Models.Supplier? GetSupplierById(int supplierId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Type, Name, Inn, ContactInfo, CreatedDate, IsActive
                               FROM Suppliers WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", supplierId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Models.Supplier
                            {
                                Id = reader.GetInt32(0),
                                Type = reader.GetString(1),
                                Name = reader.GetString(2),
                                Inn = reader.GetString(3),
                                ContactInfo = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                CreatedDate = reader.GetDateTime(5),
                                IsActive = reader.GetBoolean(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void DeleteSupplier(int supplierId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Suppliers WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", supplierId);
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
