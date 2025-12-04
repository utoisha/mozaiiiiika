using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaika.Forms
{
    public partial class ProductionForm : Form
    {
        public ProductionForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Управление производством";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);

            var titleLabel = new Label();
            titleLabel.Text = "Управление производством";
            titleLabel.Font = new Font("Comic Sans MS", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(84, 111, 148);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(400, 30);
            this.Controls.Add(titleLabel);

            var messageLabel = new Label();
            messageLabel.Text = "Модуль управления производством находится в разработке.\nЗдесь будет отображаться информация о заказах,\nпроизводственных процессах, контроле качества\nи готовности продукции.";
            messageLabel.Font = new Font("Comic Sans MS", 12, FontStyle.Regular);
            messageLabel.Location = new Point(20, 80);
            messageLabel.Size = new Size(700, 100);
            this.Controls.Add(messageLabel);

            var backButton = new Button();
            backButton.Text = "Назад";
            backButton.Size = new Size(100, 35);
            backButton.Location = new Point(20, 500);
            backButton.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            backButton.BackColor = Color.FromArgb(84, 111, 148);
            backButton.ForeColor = Color.White;
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Cursor = Cursors.Hand;
            backButton.Click += (s, e) => this.Close();
            this.Controls.Add(backButton);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}





