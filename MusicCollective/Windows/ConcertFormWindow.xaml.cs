using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Windows
{
    public partial class ConcertFormWindow : Window
    {
        private string connectionString;
        private DataRowView editingRow;

        public ConcertFormWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        }

        public ConcertFormWindow(DataRowView row) : this()
        {
            editingRow = row;
            TitleBox.Text = row["name"].ToString();
            DatePicker.SelectedDate = Convert.ToDateTime(row["date"]);
            LocationBox.Text = row["location"].ToString();
            ProgramBox.Text = row["program"].ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text) || DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd;

                if (editingRow == null)
                {
                    cmd = new MySqlCommand("INSERT INTO concerts (name, date, location, program) VALUES (@n, @d, @l, @p)", conn);
                }
                else
                {
                    cmd = new MySqlCommand("UPDATE concerts SET name=@n, date=@d, location=@l, program=@p WHERE id_concert=@id", conn);
                    cmd.Parameters.AddWithValue("@id", editingRow["id_concert"]);
                }

                cmd.Parameters.AddWithValue("@n", TitleBox.Text);
                cmd.Parameters.AddWithValue("@d", DatePicker.SelectedDate.Value);
                cmd.Parameters.AddWithValue("@l", LocationBox.Text);
                cmd.Parameters.AddWithValue("@p", ProgramBox.Text);

                cmd.ExecuteNonQuery();
                DialogResult = true;
                Close();
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
