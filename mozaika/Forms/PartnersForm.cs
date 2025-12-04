using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;
using Mozaika.Models;

namespace Mozaika.Forms
{
    public partial class PartnersForm : Form
    {
        private DataGridView partnersGrid;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button backButton;
        private TextBox searchTextBox;
        private Label searchLabel;

        public PartnersForm()
        {
            InitializeComponent();
            SetupForm();
            LoadPartners();
        }

        private void SetupForm()
        {
            this.Text = "Управление партнерами";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            var titleLabel = new Label();
            titleLabel.Text = "Управление партнерами";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(400, 30);
            this.Controls.Add(titleLabel);

            searchLabel = new Label();
            searchLabel.Text = "Поиск по названию:";
            searchLabel.Location = new Point(20, 60);
            searchLabel.Size = new Size(140, 25);
            this.Controls.Add(searchLabel);

            searchTextBox = new TextBox();
            searchTextBox.Location = new Point(160, 60);
            searchTextBox.Size = new Size(250, 25);
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            this.Controls.Add(searchTextBox);

            partnersGrid = new DataGridView();
            partnersGrid.Location = new Point(20, 100);
            partnersGrid.Size = new Size(1140, 500);
            partnersGrid.BackgroundColor = Color.White;
            partnersGrid.BorderStyle = BorderStyle.FixedSingle;
            partnersGrid.AllowUserToAddRows = false;
            partnersGrid.AllowUserToDeleteRows = false;
            partnersGrid.ReadOnly = true;
            partnersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            partnersGrid.MultiSelect = false;
            partnersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(partnersGrid);

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

            backButton = CreateStyledButton("Назад", 1050, 620);
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);

            LoadPartners();
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

        private void LoadPartners()
        {
            partnersGrid.Columns.Clear();
            partnersGrid.Rows.Clear();

            partnersGrid.Columns.Add("Id", "ID");
            partnersGrid.Columns.Add("Type", "Тип");
            partnersGrid.Columns.Add("CompanyName", "Название компании");
            partnersGrid.Columns.Add("DirectorName", "Директор");
            partnersGrid.Columns.Add("Phone", "Телефон");
            partnersGrid.Columns.Add("Email", "Email");
            partnersGrid.Columns.Add("Rating", "Рейтинг");
            partnersGrid.Columns.Add("TotalSalesVolume", "Объем продаж");

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Type, CompanyName, DirectorName, Phone, Email, Rating, TotalSalesVolume
                                FROM Partners ORDER BY CompanyName";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        partnersGrid.Rows.Add(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetString(5),
                            reader.GetInt32(6),
                            reader.GetDecimal(7).ToString("C")
                        );
                    }
                }
            }

            partnersGrid.Columns["Id"].Visible = false;
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            // Фильтрация по названию компании
            string searchText = searchTextBox.Text.ToLower();
            foreach (DataGridViewRow row in partnersGrid.Rows)
            {
                if (row.Cells["CompanyName"].Value != null)
                {
                    row.Visible = row.Cells["CompanyName"].Value.ToString().ToLower().Contains(searchText);
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var editForm = new PartnerEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadPartners();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (partnersGrid.SelectedRows.Count == 0) return;

            int partnerId = (int)partnersGrid.SelectedRows[0].Cells["Id"].Value;
            var partner = GetPartnerById(partnerId);
            if (partner != null)
            {
                var editForm = new PartnerEditForm(partner);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadPartners();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (partnersGrid.SelectedRows.Count == 0) return;

            int partnerId = (int)partnersGrid.SelectedRows[0].Cells["Id"].Value;

            var result = MessageBox.Show("Вы действительно хотите удалить этого партнера?",
                                       "Подтверждение удаления",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                DeletePartner(partnerId);
                LoadPartners();
            }
        }

        private Models.Partner? GetPartnerById(int partnerId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Type, CompanyName, LegalAddress, Inn, DirectorName, Phone, Email, Logo, Rating, SalesLocations, TotalSalesVolume, CreatedDate
                               FROM Partners WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", partnerId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Models.Partner
                            {
                                Id = reader.GetInt32(0),
                                Type = reader.GetString(1),
                                CompanyName = reader.GetString(2),
                                LegalAddress = reader.GetString(3),
                                Inn = reader.GetString(4),
                                DirectorName = reader.GetString(5),
                                Phone = reader.GetString(6),
                                Email = reader.GetString(7),
                                Rating = reader.GetInt32(9),
                                SalesLocations = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                TotalSalesVolume = reader.GetDecimal(11),
                                CreatedDate = reader.GetDateTime(12)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void DeletePartner(int partnerId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Partners WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", partnerId);
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

