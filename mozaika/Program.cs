using System;
using System.Windows.Forms;
using Mozaika.Database;
using Mozaika.Forms;

namespace Mozaika
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Инициализация базы данных
            DatabaseHelper.InitializeDatabase();

            ApplicationConfiguration.Initialize();

            // Показываем форму авторизации
            var loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Если авторизация успешна, показываем главную форму
                Application.Run(new MainForm());
            }
        }
    }
}
