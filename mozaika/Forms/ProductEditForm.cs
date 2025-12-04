using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;
using Mozaika.Database;

namespace Mozaika.Forms
{
    public partial class ProductEditForm : Form
    {
        private TextBox articleTextBox;
        private ComboBox typeComboBox;
        private TextBox nameTextBox;
        private TextBox descriptionTextBox;
        private TextBox minPartnerPriceTextBox;
        private TextBox costPriceTextBox;
        private TextBox stockQuantityTextBox;
        private TextBox workshopNumberTextBox;
        private Button saveButton;
        private Button cancelButton;
        private Models.Product? product;

        public ProductEditForm(Models.Product? existingProduct = null)
        {
            product = existingProduct;
            InitializeComponent();
            SetupForm();
            if (product != null)
            {
                LoadProductData();
            }
        }

        private void SetupForm()
        {
            this.Text = product == null ? "Добавление продукта" : "Редактирование продукта";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int controlWidth = 200;
            int height = 25;
            int spacing = 35;
            int startY = 20;

            // Артикул
            AddLabel("Артикул:", 20, startY);
            articleTextBox = AddTextBox(180, startY, controlWidth);

            // Тип
            AddLabel("Тип:", 20, startY + spacing);
            typeComboBox = new ComboBox();
            typeComboBox.Location = new Point(180, startY + spacing);
            typeComboBox.Size = new Size(controlWidth, height);
            typeComboBox.Items.AddRange(new string[] { "Керамическая плитка", "Керамогранит", "Декоративные элементы" });
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(typeComboBox);

            // Название
            AddLabel("Название:", 20, startY + 2 * spacing);
            nameTextBox = AddTextBox(180, startY + 2 * spacing, controlWidth);

            // Описание
            AddLabel("Описание:", 20, startY + 3 * spacing);
            descriptionTextBox = new TextBox();
            descriptionTextBox.Location = new Point(180, startY + 3 * spacing);
            descriptionTextBox.Size = new Size(controlWidth, 60);
            descriptionTextBox.Font = new Font("Comic Sans MS", 10, FontStyle.Regular);
            descriptionTextBox.Multiline = true;
            this.Controls.Add(descriptionTextBox);

            // Цена для партнеров
            AddLabel("Цена для партнеров:", 20, startY + 5 * spacing);
            minPartnerPriceTextBox = AddTextBox(180, startY + 5 * spacing, controlWidth);

            // Себестоимость
            AddLabel("Себестоимость:", 20, startY + 6 * spacing);
            costPriceTextBox = AddTextBox(180, startY + 6 * spacing, controlWidth);

            // Количество на складе
            AddLabel("На складе (шт.):", 20, startY + 7 * spacing);
            stockQuantityTextBox = AddTextBox(180, startY + 7 * spacing, controlWidth);

            // Номер цеха
            AddLabel("Номер цеха:", 20, startY + 8 * spacing);
            workshopNumberTextBox = AddTextBox(180, startY + 8 * spacing, controlWidth);

            // Кнопки
            saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Size = new Size(100, 35);
            saveButton.Location = new Point(180, startY + 10 * spacing);
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
            cancelButton.Location = new Point(300, startY + 10 * spacing);
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

        private void LoadProductData()
        {
            if (product == null) return;

            articleTextBox.Text = product.Article;
            typeComboBox.Text = product.Type;
            nameTextBox.Text = product.Name;
            descriptionTextBox.Text = product.Description;
            minPartnerPriceTextBox.Text = product.MinPartnerPrice.ToString();
            costPriceTextBox.Text = product.CostPrice.ToString();
            stockQuantityTextBox.Text = product.StockQuantity.ToString();
            workshopNumberTextBox.Text = product.WorkshopNumber.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            SaveProduct();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(articleTextBox.Text))
            {
                MessageBox.Show("Введите артикул продукта", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (typeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите тип продукта", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Введите название продукта", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!decimal.TryParse(minPartnerPriceTextBox.Text, out _))
            {
                MessageBox.Show("Введите корректную цену для партнеров", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!decimal.TryParse(costPriceTextBox.Text, out _))
            {
                MessageBox.Show("Введите корректную себестоимость", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(stockQuantityTextBox.Text, out _))
            {
                MessageBox.Show("Введите корректное количество на складе", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(workshopNumberTextBox.Text, out _))
            {
                MessageBox.Show("Введите корректный номер цеха", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void SaveProduct()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query;
                if (product == null || product.Id == 0)
                {
                    // Создание нового продукта
                    query = @"INSERT INTO Products (Article, Type, Name, Description, MinPartnerPrice, CostPrice, StockQuantity, WorkshopNumber, ProductionTime, RequiredMaterials)
                             VALUES (@Article, @Type, @Name, @Description, @MinPartnerPrice, @CostPrice, @StockQuantity, @WorkshopNumber, '02:00:00', '')";
                }
                else
                {
                    // Обновление существующего продукта
                    query = @"UPDATE Products SET Article = @Article, Type = @Type, Name = @Name, Description = @Description,
                             MinPartnerPrice = @MinPartnerPrice, CostPrice = @CostPrice, StockQuantity = @StockQuantity, WorkshopNumber = @WorkshopNumber
                             WHERE Id = @Id";
                }

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Article", articleTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Type", typeComboBox.Text);
                    command.Parameters.AddWithValue("@Name", nameTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Description", descriptionTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@MinPartnerPrice", decimal.Parse(minPartnerPriceTextBox.Text));
                    command.Parameters.AddWithValue("@CostPrice", decimal.Parse(costPriceTextBox.Text));
                    command.Parameters.AddWithValue("@StockQuantity", int.Parse(stockQuantityTextBox.Text));
                    command.Parameters.AddWithValue("@WorkshopNumber", int.Parse(workshopNumberTextBox.Text));

                    if (product != null && product.Id != 0)
                    {
                        command.Parameters.AddWithValue("@Id", product.Id);
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
