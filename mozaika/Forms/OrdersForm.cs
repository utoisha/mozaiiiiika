using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;
using Mozaika.Models;
using Mozaika.Services;

namespace Mozaika.Forms
{
    public partial class OrdersForm : Form
    {
        private DataGridView ordersGrid;
        private Button backButton;
        private Label titleLabel;

        public OrdersForm()
        {
            InitializeComponent();
            SetupForm();
            LoadOrders();
        }

        private void SetupForm()
        {
            this.Text = "Заявки";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            // Заголовок
            titleLabel = new Label();
            titleLabel.Text = "Управление заявками";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(300, 30);
            this.Controls.Add(titleLabel);

            // Таблица заказов
            ordersGrid = new DataGridView();
            ordersGrid.Location = new Point(20, 60);
            ordersGrid.Size = new Size(940, 450);
            ordersGrid.BackgroundColor = Color.White;
            ordersGrid.BorderStyle = BorderStyle.FixedSingle;
            ordersGrid.AllowUserToAddRows = false;
            ordersGrid.AllowUserToDeleteRows = false;
            ordersGrid.ReadOnly = true;
            ordersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ordersGrid.MultiSelect = false;
            ordersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(ordersGrid);

            // Кнопка назад
            backButton = new Button();
            backButton.Text = "Назад";
            backButton.Size = new Size(100, 35);
            backButton.Location = new Point(20, 520);
            backButton.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            backButton.BackColor = Color.FromArgb(84, 111, 148);
            backButton.ForeColor = Color.White;
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Cursor = Cursors.Hand;
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);
        }

        private void LoadOrders()
        {
            ordersGrid.Columns.Clear();
            ordersGrid.Rows.Clear();

            ordersGrid.Columns.Add("Id", "ID");
            ordersGrid.Columns.Add("PartnerName", "Партнер");
            ordersGrid.Columns.Add("CreatedDate", "Дата создания");
            ordersGrid.Columns.Add("Status", "Статус");
            ordersGrid.Columns.Add("TotalAmount", "Сумма");

            // Все авторизованные пользователи видят все заказы
            string query = @"SELECT o.Id, p.CompanyName, o.CreatedDate, o.Status, o.TotalAmount
                           FROM Orders o
                           JOIN Partners p ON o.PartnerId = p.Id
                           ORDER BY o.CreatedDate DESC";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string statusText = GetStatusText((OrderStatus)reader.GetInt32(3));

                            ordersGrid.Rows.Add(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetDateTime(2).ToShortDateString(),
                                statusText,
                                reader.GetDecimal(4).ToString("C")
                            );
                        }
                    }
                }
            }

            ordersGrid.Columns["Id"].Visible = false;
        }

        private string GetStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Created => "Создан",
                OrderStatus.PrepaymentPending => "Ожидает предоплаты",
                OrderStatus.PrepaymentReceived => "Предоплата получена",
                OrderStatus.InProduction => "В производстве",
                OrderStatus.ReadyForDelivery => "Готов к доставке",
                OrderStatus.Delivered => "Доставлен",
                OrderStatus.Completed => "Выполнен",
                OrderStatus.Cancelled => "Отменен",
                _ => "Неизвестно"
            };
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

