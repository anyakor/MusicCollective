using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Pages
{
    public partial class ConcertsPage : Page
    {
        private string connectionString;

        public ConcertsPage()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            LoadConcerts();
        }

        private void LoadConcerts()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM concerts", conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                ConcertsGrid.ItemsSource = table.DefaultView;
            }
            finally
            {
                conn.Close();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var form = new Windows.ConcertFormWindow();
            if (form.ShowDialog() == true)
                LoadConcerts();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var row = ConcertsGrid.SelectedItem as DataRowView;
            if (row == null) return;

            var form = new Windows.ConcertFormWindow(row);
            if (form.ShowDialog() == true)
                LoadConcerts();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var row = ConcertsGrid.SelectedItem as DataRowView;
            if (row == null) return;

            if (MessageBox.Show("Удалить выбранный концерт?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM concerts WHERE id_concert = @id", conn);
                    cmd.Parameters.AddWithValue("@id", row["id_concert"]);
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }

                LoadConcerts();
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchBox.Text.Trim();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM concerts";
                if (!string.IsNullOrEmpty(keyword))
                {
                    query += " WHERE name LIKE @keyword";
                }

                var cmd = new MySqlCommand(query, conn);
                if (!string.IsNullOrEmpty(keyword))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                }

                var adapter = new MySqlDataAdapter(cmd);
                var table = new DataTable();
                adapter.Fill(table);
                ConcertsGrid.ItemsSource = table.DefaultView;
            }
        }

    }
}
