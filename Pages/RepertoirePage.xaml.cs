using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Pages
{
    public partial class RepertoirePage : Page
    {
        private string connectionString;

        public RepertoirePage()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            LoadRepertoire();
        }

        private void LoadRepertoire()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM repertoire", conn);
                var table = new DataTable();
                adapter.Fill(table);
                RepertoireGrid.ItemsSource = table.DefaultView;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var form = new Windows.RepertoireFormWindow();
            if (form.ShowDialog() == true)
                LoadRepertoire();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var row = RepertoireGrid.SelectedItem as DataRowView;
            if (row == null) return;

            var form = new Windows.RepertoireFormWindow(row);
            if (form.ShowDialog() == true)
                LoadRepertoire();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var row = RepertoireGrid.SelectedItem as DataRowView;
            if (row == null) return;

            if (MessageBox.Show("Удалить произведение?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM repertoire WHERE id_piece = @id", conn);
                    cmd.Parameters.AddWithValue("@id", row["id_piece"]);
                    cmd.ExecuteNonQuery();
                }
                LoadRepertoire();
            }
        }
    }
}
