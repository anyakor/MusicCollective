using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Pages
{
    public partial class RehearsalsPage : Page
    {
        private string connectionString;

        public RehearsalsPage()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            LoadRehearsals();
        }

        private void LoadRehearsals()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var query = @"SELECT r.id_rehearsal, r.date, r.time, r.location, e.name AS ensemble_name
                              FROM rehearsals r
                              JOIN ensembles e ON r.id_ensemble = e.id_ensemble";
                var adapter = new MySqlDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                RehearsalsGrid.ItemsSource = table.DefaultView;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var form = new Windows.RehearsalFormWindow();
            if (form.ShowDialog() == true) LoadRehearsals();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var row = RehearsalsGrid.SelectedItem as DataRowView;
            if (row == null) return;
            var form = new Windows.RehearsalFormWindow(row);
            if (form.ShowDialog() == true) LoadRehearsals();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var row = RehearsalsGrid.SelectedItem as DataRowView;
            if (row == null) return;

            if (MessageBox.Show("Удалить выбранную репетицию?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM rehearsals WHERE id_rehearsal = @id", conn);
                    cmd.Parameters.AddWithValue("@id", row["id_rehearsal"]);
                    cmd.ExecuteNonQuery();
                }
                LoadRehearsals();
            }
        }
    }
}
