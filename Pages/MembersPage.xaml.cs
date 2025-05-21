using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Pages
{
    public partial class MembersPage : Page
    {
        private string connectionString;

        public MembersPage()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            LoadMembers();
        }

        private void LoadMembers()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM members", conn);
                var table = new DataTable();
                adapter.Fill(table);
                MembersGrid.ItemsSource = table.DefaultView;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var form = new Windows.MemberFormWindow();
            if (form.ShowDialog() == true)
                LoadMembers();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var row = MembersGrid.SelectedItem as DataRowView;
            if (row == null) return;

            var form = new Windows.MemberFormWindow(row);
            if (form.ShowDialog() == true)
                LoadMembers();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var row = MembersGrid.SelectedItem as DataRowView;
            if (row == null) return;

            if (MessageBox.Show("Удалить участника?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM members WHERE id_member = @id", conn);
                    cmd.Parameters.AddWithValue("@id", row["id_member"]);
                    cmd.ExecuteNonQuery();
                }
                LoadMembers();
            }
        }
    }
}
