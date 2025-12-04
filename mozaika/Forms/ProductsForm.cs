using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class ProductsForm : Form
    {
        private DataGridView productsGrid;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button backButton;
        private Label titleLabel;

        public ProductsForm()
        {
            InitializeComponent();
            SetupForm();
            LoadProducts();
        }

        private void SetupForm()
        {
            this.Text = "Продукты";
            this.Size = new Size(1400, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            // Заголовок
            titleLabel = new Label();
            titleLabel.Text = "Управление продуктами";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(300, 30);
            this.Controls.Add(titleLabel);

            // Таблица продуктов
            productsGrid = new DataGridView();
            productsGrid.Location = new Point(20, 60);
            productsGrid.Size = new Size(1340, 550);
            productsGrid.BackgroundColor = Color.White;
            productsGrid.BorderStyle = BorderStyle.FixedSingle;
            productsGrid.AllowUserToAddRows = false;
            productsGrid.AllowUserToDeleteRows = false;
            productsGrid.ReadOnly = true;
            productsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            productsGrid.MultiSelect = false;
            productsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(productsGrid);

            // Кнопки управления
            addButton = CreateStyledButton("Добавить", 20, 630);
            addButton.Click += AddButton_Click;
            this.Controls.Add(addButton);

            editButton = CreateStyledButton("Редактировать", 130, 630);
            editButton.Click += EditButton_Click;
            this.Controls.Add(editButton);

            deleteButton = CreateStyledButton("Удалить", 260, 630);
            deleteButton.BackColor = Color.FromArgb(171, 207, 206);
            deleteButton.Click += DeleteButton_Click;
            this.Controls.Add(deleteButton);

            // Кнопка назад
            backButton = CreateStyledButton("Назад", 20, 680);
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);
        }

        private void LoadProducts()
        {
            productsGrid.Columns.Clear();
            productsGrid.Rows.Clear();

            productsGrid.Columns.Add("Id", "ID");
            productsGrid.Columns.Add("Article", "Артикул");
            productsGrid.Columns.Add("Name", "Название");
            productsGrid.Columns.Add("Type", "Тип");
            productsGrid.Columns.Add("MinPartnerPrice", "Цена для партнеров");
            productsGrid.Columns.Add("CostPrice", "Себестоимость");
            productsGrid.Columns.Add("StockQuantity", "На складе");
            productsGrid.Columns.Add("WorkshopNumber", "Цех");

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Article, Name, Type, MinPartnerPrice, CostPrice, StockQuantity, WorkshopNumber
                               FROM Products ORDER BY Name";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productsGrid.Rows.Add(
                            reader.GetInt32(0),  // Id
                            reader.GetString(1), // Article
                            reader.GetString(2), // Name
                            reader.GetString(3), // Type
                            reader.GetDecimal(4).ToString("C"), // MinPartnerPrice
                            reader.GetDecimal(5).ToString("C"), // CostPrice
                            reader.GetInt32(6),  // StockQuantity
                            reader.GetInt32(7)   // WorkshopNumber
                        );
                    }
                }
            }

            productsGrid.Columns["Id"].Visible = false;
        }

        private Button CreateStyledButton(string text, int x, int y)
        {
            var button = new Button();
            button.Text = text;
            button.Size = new Size(110, 35);
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
            var editForm = new ProductEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (productsGrid.SelectedRows.Count == 0) return;

            int productId = (int)productsGrid.SelectedRows[0].Cells["Id"].Value;
            var product = GetProductById(productId);
            if (product != null)
            {
                var editForm = new ProductEditForm(product);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (productsGrid.SelectedRows.Count == 0) return;

            int productId = (int)productsGrid.SelectedRows[0].Cells["Id"].Value;

            var result = MessageBox.Show("Вы действительно хотите удалить этот продукт?",
                                       "Подтверждение удаления",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                DeleteProduct(productId);
                LoadProducts();
            }
        }

        private Models.Product? GetProductById(int productId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Article, Type, Name, Description, MinPartnerPrice, CostPrice, StockQuantity, WorkshopNumber
                               FROM Products WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", productId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Models.Product
                            {
                                Id = reader.GetInt32(0),
                                Article = reader.GetString(1),
                                Type = reader.GetString(2),
                                Name = reader.GetString(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                MinPartnerPrice = reader.GetDecimal(5),
                                CostPrice = reader.GetDecimal(6),
                                StockQuantity = reader.GetInt32(7),
                                WorkshopNumber = reader.GetInt32(8)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void DeleteProduct(int productId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Products WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", productId);
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

