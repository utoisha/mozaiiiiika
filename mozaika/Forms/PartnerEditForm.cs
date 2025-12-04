using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class PartnerEditForm : Form
    {
        private ComboBox typeComboBox;
        private TextBox companyNameTextBox;
        private TextBox legalAddressTextBox;
        private TextBox innTextBox;
        private TextBox directorNameTextBox;
        private TextBox phoneTextBox;
        private TextBox emailTextBox;
        private Button saveButton;
        private Button cancelButton;
        private Models.Partner? partner;

        public PartnerEditForm(Models.Partner? existingPartner = null)
        {
            partner = existingPartner;
            InitializeComponent();
            SetupForm();
            if (partner != null)
            {
                LoadPartnerData();
            }
        }

        private void SetupForm()
        {
            this.Text = partner == null ? "Добавление партнера" : "Редактирование партнера";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int controlWidth = 300;
            int height = 25;
            int spacing = 35;
            int startY = 20;

            // Тип партнера
            AddLabel("Тип партнера:", 20, startY);
            typeComboBox = new ComboBox();
            typeComboBox.Location = new Point(180, startY);
            typeComboBox.Size = new Size(controlWidth, height);
            typeComboBox.Items.AddRange(new string[] { "Розничный магазин", "Оптовый склад", "Интернет-магазин", "Другое" });
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(typeComboBox);

            // Название компании
            AddLabel("Название компании:", 20, startY + spacing);
            companyNameTextBox = AddTextBox(180, startY + spacing, controlWidth);

            // Юридический адрес
            AddLabel("Юридический адрес:", 20, startY + 2 * spacing);
            legalAddressTextBox = AddTextBox(180, startY + 2 * spacing, controlWidth);

            // ИНН
            AddLabel("ИНН:", 20, startY + 3 * spacing);
            innTextBox = AddTextBox(180, startY + 3 * spacing, controlWidth);

            // ФИО директора
            AddLabel("ФИО директора:", 20, startY + 4 * spacing);
            directorNameTextBox = AddTextBox(180, startY + 4 * spacing, controlWidth);

            // Телефон
            AddLabel("Телефон:", 20, startY + 5 * spacing);
            phoneTextBox = AddTextBox(180, startY + 5 * spacing, controlWidth);

            // Email
            AddLabel("Email:", 20, startY + 6 * spacing);
            emailTextBox = AddTextBox(180, startY + 6 * spacing, controlWidth);

            // Кнопки
            saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Size = new Size(100, 35);
            saveButton.Location = new Point(180, startY + 8 * spacing);
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
            cancelButton.Location = new Point(300, startY + 8 * spacing);
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
            label.Size = new Size(150, 25);
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

        private void LoadPartnerData()
        {
            if (partner == null) return;

            typeComboBox.Text = partner.Type;
            companyNameTextBox.Text = partner.CompanyName;
            legalAddressTextBox.Text = partner.LegalAddress;
            innTextBox.Text = partner.Inn;
            directorNameTextBox.Text = partner.DirectorName;
            phoneTextBox.Text = partner.Phone;
            emailTextBox.Text = partner.Email;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            SavePartner();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (typeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите тип партнера", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(companyNameTextBox.Text))
            {
                MessageBox.Show("Введите название компании", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(innTextBox.Text))
            {
                MessageBox.Show("Введите ИНН", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(directorNameTextBox.Text))
            {
                MessageBox.Show("Введите ФИО директора", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void SavePartner()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query;
                if (partner == null || partner.Id == 0)
                {
                    // Создание нового партнера
                    query = @"INSERT INTO Partners (Type, CompanyName, LegalAddress, Inn, DirectorName, Phone, Email, Rating, SalesLocations, TotalSalesVolume, CreatedDate)
                             VALUES (@Type, @CompanyName, @LegalAddress, @Inn, @DirectorName, @Phone, @Email, 0, '', 0, @CreatedDate)";
                }
                else
                {
                    // Обновление существующего партнера
                    query = @"UPDATE Partners SET Type = @Type, CompanyName = @CompanyName, LegalAddress = @LegalAddress,
                             Inn = @Inn, DirectorName = @DirectorName, Phone = @Phone, Email = @Email
                             WHERE Id = @Id";
                }

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Type", typeComboBox.Text);
                    command.Parameters.AddWithValue("@CompanyName", companyNameTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@LegalAddress", legalAddressTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Inn", innTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@DirectorName", directorNameTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Phone", phoneTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Email", emailTextBox.Text.Trim());

                    if (partner != null && partner.Id != 0)
                    {
                        command.Parameters.AddWithValue("@Id", partner.Id);
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
