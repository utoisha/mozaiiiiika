using System;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;

namespace Mozaika
{
    public class TestForm : Form
    {
        public TestForm()
        {
            this.Text = "Test";
            this.Size = new Size(200, 200);
            var button = new Button();
            button.Text = "Test SQLite";
            button.Click += (s, e) =>
            {
                try
                {
                    using (var conn = new SqliteConnection("Data Source=:memory:"))
                    {
                        conn.Open();
                        MessageBox.Show("SQLite works!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            };
            this.Controls.Add(button);
        }
    }
}
