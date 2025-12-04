using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class ReportsForm : Form
    {
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button backButton;
        private Label titleLabel;
        private DataGridView reportGrid;

        public ReportsForm()
        {
            InitializeComponent();
            SetupForm();
            LoadReports();
        }

        private void SetupForm()
        {
            this.Text = "Отчеты";
            this.Size = new Size(1000, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            // Заголовок
            titleLabel = new Label();
            titleLabel.Text = "Аналитические отчеты";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(300, 30);
            this.Controls.Add(titleLabel);

            // Таблица отчетов
            reportGrid = new DataGridView();
            reportGrid.Location = new Point(20, 60);
            reportGrid.Size = new Size(940, 400);
            reportGrid.BackgroundColor = Color.White;
            reportGrid.BorderStyle = BorderStyle.FixedSingle;
            reportGrid.AllowUserToAddRows = false;
            reportGrid.AllowUserToDeleteRows = false;
            reportGrid.ReadOnly = true;
            reportGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            reportGrid.MultiSelect = false;
            reportGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(reportGrid);

            // Кнопки управления
            addButton = CreateStyledButton("Добавить отчет", 20, 470);
            addButton.Click += AddButton_Click;
            this.Controls.Add(addButton);

            editButton = CreateStyledButton("Редактировать", 140, 470);
            editButton.Click += EditButton_Click;
            this.Controls.Add(editButton);

            deleteButton = CreateStyledButton("Удалить", 260, 470);
            deleteButton.BackColor = Color.FromArgb(171, 207, 206);
            deleteButton.Click += DeleteButton_Click;
            this.Controls.Add(deleteButton);

            // Кнопка назад
            backButton = CreateStyledButton("Назад", 850, 470);
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);
        }

        private void LoadReports()
        {
            // Загружаем детальный отчет
            LoadDetailedReport();
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
            MessageBox.Show("Функция создания отчетов находится в разработке", "Информация",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (reportGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите отчет для редактирования", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Функция редактирования отчетов находится в разработке", "Информация",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (reportGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите отчет для удаления", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Функция удаления отчетов находится в разработке", "Информация",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void LoadDetailedReport()
        {
            reportGrid.Columns.Clear();
            reportGrid.Rows.Clear();

            reportGrid.Columns.Add("ReportType", "Тип отчета");
            reportGrid.Columns.Add("Period", "Период");
            reportGrid.Columns.Add("Data", "Данные");
            reportGrid.Columns.Add("CreatedDate", "Дата создания");

            // Добавляем примеры отчетов
            reportGrid.Rows.Add("Отчет по продажам", "Текущий месяц", "Общая сумма: 1,500,000 руб.", DateTime.Now.ToShortDateString());
            reportGrid.Rows.Add("Отчет по партнерам", "Последний квартал", "Активных партнеров: 5", DateTime.Now.AddDays(-7).ToShortDateString());
            reportGrid.Rows.Add("Отчет по сотрудникам", "Текущий год", "Общее количество: 12", DateTime.Now.AddDays(-14).ToShortDateString());
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
