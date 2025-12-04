using System;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;
using Mozaika.Services;
using Mozaika.Models;

namespace Mozaika.Forms
{
    public partial class MainForm : Form
    {
        private PictureBox logoPictureBox;
        private Label titleLabel;
        private Label userInfoLabel;
        private Panel menuPanel;
        private Button usersButton;
        private Button reportsButton;
        private Button partnersButton;
        private Button ordersButton;
        private Button suppliersButton;
        private Button arrivalsButton;
        private Button productsButton;
        private Button logoutButton;

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateButtonAccess();
        }

        private void SetupForm()
        {
            this.Text = "Система управления производственной компанией «Мозаика»";
            this.Size = new Size(400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            // Логотип вверху по центру
            logoPictureBox = new PictureBox();
            logoPictureBox.Size = new Size(120, 120);
            logoPictureBox.Location = new Point(140, 20);
            logoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            logoPictureBox.BackColor = Color.LightGray;
            logoPictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(logoPictureBox);

            // Заголовок под логотипом
            titleLabel = new Label();
            titleLabel.Text = "Система управления\nпроизводственной компанией\n«Мозаика»";
            titleLabel.Font = new Font("Comic Sans MS", 12, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(50, 150);
            titleLabel.Size = new Size(300, 80);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(titleLabel);

            // Информация о пользователе
            userInfoLabel = new Label();
            userInfoLabel.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            userInfoLabel.ForeColor = Color.FromArgb(84, 111, 148);
            userInfoLabel.Location = new Point(50, 240);
            userInfoLabel.Size = new Size(300, 25);
            userInfoLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(userInfoLabel);

            // Панель меню
            menuPanel = new Panel();
            menuPanel.Location = new Point(100, 280);
            menuPanel.Size = new Size(200, 430);
            menuPanel.BackColor = Color.FromArgb(240, 240, 240);
            menuPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(menuPanel);

            // Создание кнопок меню
            CreateMenuButtons();

            // Кнопка выхода
            logoutButton = CreateStyledButton("Выход", 145, 730);
            logoutButton.BackColor = Color.FromArgb(171, 207, 206);
            logoutButton.Click += LogoutButton_Click;
            this.Controls.Add(logoutButton);
        }

        private void CreateMenuButtons()
        {
            int buttonWidth = 160;
            int buttonHeight = 50;
            int startY = 20;
            int spacing = 8;

            // Пользователи
            usersButton = CreateMenuButton("Пользователи", 10, startY, buttonWidth, buttonHeight);
            usersButton.Click += UsersButton_Click;
            menuPanel.Controls.Add(usersButton);

            // Отчеты
            reportsButton = CreateMenuButton("Отчеты", 10, startY + (buttonHeight + spacing), buttonWidth, buttonHeight);
            reportsButton.Click += ReportsButton_Click;
            menuPanel.Controls.Add(reportsButton);

            // Партнеры
            partnersButton = CreateMenuButton("Партнеры", 10, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight);
            partnersButton.Click += PartnersButton_Click;
            menuPanel.Controls.Add(partnersButton);

            // Заявки
            ordersButton = CreateMenuButton("Заявки", 10, startY + 3 * (buttonHeight + spacing), buttonWidth, buttonHeight);
            ordersButton.Click += OrdersButton_Click;
            menuPanel.Controls.Add(ordersButton);

            // Поставщики
            suppliersButton = CreateMenuButton("Поставщики", 10, startY + 4 * (buttonHeight + spacing), buttonWidth, buttonHeight);
            suppliersButton.Click += SuppliersButton_Click;
            menuPanel.Controls.Add(suppliersButton);

            // Поступления
            arrivalsButton = CreateMenuButton("Поступления", 10, startY + 5 * (buttonHeight + spacing), buttonWidth, buttonHeight);
            arrivalsButton.Click += ArrivalsButton_Click;
            menuPanel.Controls.Add(arrivalsButton);

            // Продукты
            productsButton = CreateMenuButton("Продукты", 10, startY + 6 * (buttonHeight + spacing), buttonWidth, buttonHeight);
            productsButton.Click += ProductsButton_Click;
            menuPanel.Controls.Add(productsButton);
        }

        private Button CreateMenuButton(string text, int x, int y, int width, int height)
        {
            var button = new Button();
            button.Text = text;
            button.Size = new Size(width, height);
            button.Location = new Point(x, y);
            button.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            button.BackColor = Color.FromArgb(84, 111, 148);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            return button;
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

        private bool IsButtonAccessible(Button button)
        {
            if (AuthService.CurrentUser == null) return false;

            string buttonText = button.Text;

            if (AuthService.IsAdmin)
            {
                return buttonText.Contains("Пользователи") || buttonText.Contains("Отчеты");
            }
            else if (AuthService.IsManager)
            {
                return buttonText.Contains("Партнеры") || buttonText.Contains("Заявки") || buttonText.Contains("Продукты");
            }
            else if (AuthService.IsMaster)
            {
                return buttonText.Contains("Поставщики") || buttonText.Contains("Поступления");
            }

            return false;
        }

        private void UpdateButtonAccess()
        {
            if (AuthService.CurrentUser == null) return;

            // Обновляем информацию о пользователе
            string roleName = AuthService.CurrentUser.Role switch
            {
                UserRole.Admin => "Администратор",
                UserRole.Manager => "Менеджер",
                UserRole.Master => "Мастер",
                _ => "Неизвестно"
            };
            userInfoLabel.Text = $"Пользователь: {AuthService.CurrentUser.FullName} ({roleName})";

            // Включаем/отключаем кнопки в зависимости от роли
            bool isAdmin = AuthService.IsAdmin;
            bool isManager = AuthService.IsManager;
            bool isMaster = AuthService.IsMaster;

            usersButton.Enabled = isAdmin;
            usersButton.BackColor = isAdmin ? Color.FromArgb(84, 111, 148) : Color.FromArgb(200, 200, 200);

            reportsButton.Enabled = isAdmin;
            reportsButton.BackColor = isAdmin ? Color.FromArgb(84, 111, 148) : Color.FromArgb(200, 200, 200);

            partnersButton.Enabled = isManager;
            partnersButton.BackColor = isManager ? Color.FromArgb(84, 111, 148) : Color.FromArgb(200, 200, 200);

            ordersButton.Enabled = isManager;
            ordersButton.BackColor = isManager ? Color.FromArgb(84, 111, 148) : Color.FromArgb(200, 200, 200);

            productsButton.Enabled = isManager;
            productsButton.BackColor = isManager ? Color.FromArgb(84, 111, 148) : Color.FromArgb(200, 200, 200);

            suppliersButton.Enabled = isMaster;
            suppliersButton.BackColor = isMaster ? Color.FromArgb(84, 111, 148) : Color.FromArgb(200, 200, 200);

            arrivalsButton.Enabled = isMaster;
            arrivalsButton.BackColor = isMaster ? Color.FromArgb(84, 111, 148) : Color.FromArgb(200, 200, 200);
        }

        private void UsersButton_Click(object sender, EventArgs e)
        {
            var usersForm = new UsersManagementForm();
            usersForm.ShowDialog();
        }

        private void ReportsButton_Click(object sender, EventArgs e)
        {
            var reportsForm = new ReportsForm();
            reportsForm.ShowDialog();
        }

        private void PartnersButton_Click(object sender, EventArgs e)
        {
            var partnersForm = new PartnersForm();
            partnersForm.ShowDialog();
        }

        private void OrdersButton_Click(object sender, EventArgs e)
        {
            var ordersForm = new OrdersForm();
            ordersForm.ShowDialog();
        }

        private void SuppliersButton_Click(object sender, EventArgs e)
        {
            var suppliersForm = new SuppliersForm();
            suppliersForm.ShowDialog();
        }

        private void ArrivalsButton_Click(object sender, EventArgs e)
        {
            var arrivalsForm = new ArrivalsForm();
            arrivalsForm.ShowDialog();
        }

        private void ProductsButton_Click(object sender, EventArgs e)
        {
            var productsForm = new ProductsForm();
            productsForm.ShowDialog();
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            AuthService.Logout();
            this.Hide();

            var loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                this.Show();
                UpdateButtonAccess();
            }
            else
            {
                Application.Exit();
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MainForm
            // 
            ClientSize = new Size(302, 452);
            Name = "MainForm";
            ResumeLayout(false);
        }
    }
}