using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class ArrivalsForm : Form
    {
        private DataGridView arrivalsGrid;
        private Button addButton;
        private Button backButton;
        private Label titleLabel;

        public ArrivalsForm()
        {
            InitializeComponent();
            SetupForm();
            LoadArrivals();
        }

        private void SetupForm()
        {
            this.Text = "Поступления";
            this.Size = new Size(1200, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            // Заголовок
            titleLabel = new Label();
            titleLabel.Text = "Управление поступлениями материалов";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(400, 30);
            this.Controls.Add(titleLabel);

            // Таблица поступлений
            arrivalsGrid = new DataGridView();
            arrivalsGrid.Location = new Point(20, 60);
            arrivalsGrid.Size = new Size(1140, 450);
            arrivalsGrid.BackgroundColor = Color.White;
            arrivalsGrid.BorderStyle = BorderStyle.FixedSingle;
            arrivalsGrid.AllowUserToAddRows = false;
            arrivalsGrid.AllowUserToDeleteRows = false;
            arrivalsGrid.ReadOnly = true;
            arrivalsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            arrivalsGrid.MultiSelect = false;
            arrivalsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(arrivalsGrid);

            // Кнопка добавить поступление
            addButton = CreateStyledButton("Добавить поступление", 20, 520);
            addButton.Click += AddButton_Click;
            this.Controls.Add(addButton);

            // Кнопка назад
            backButton = CreateStyledButton("Назад", 20, 570);
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);
        }

        private void LoadArrivals()
        {
            arrivalsGrid.Columns.Clear();
            arrivalsGrid.Rows.Clear();

            arrivalsGrid.Columns.Add("MaterialName", "Материал");
            arrivalsGrid.Columns.Add("SupplierName", "Поставщик");
            arrivalsGrid.Columns.Add("Quantity", "Количество");
            arrivalsGrid.Columns.Add("Unit", "Единица");
            arrivalsGrid.Columns.Add("Cost", "Стоимость");
            arrivalsGrid.Columns.Add("LastUpdated", "Последнее обновление");

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT m.Name, s.Name, m.StockQuantity, m.Unit, m.Cost, m.LastUpdated
                               FROM Materials m
                               JOIN Suppliers s ON m.SupplierId = s.Id
                               ORDER BY m.LastUpdated DESC";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        arrivalsGrid.Rows.Add(
                            reader.GetString(0), // Material name
                            reader.GetString(1), // Supplier name
                            reader.GetInt32(2),  // Quantity
                            reader.GetString(3), // Unit
                            reader.GetDecimal(4).ToString("C"), // Cost
                            reader.GetDateTime(5).ToShortDateString() // Last updated
                        );
                    }
                }
            }
        }

        private Button CreateStyledButton(string text, int x, int y)
        {
            var button = new Button();
            button.Text = text;
            button.Size = new Size(150, 35);
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
            var editForm = new ArrivalEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadArrivals();
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
