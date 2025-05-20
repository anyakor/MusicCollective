using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Windows
{
    public partial class RepertoireFormWindow : Window
    {
        private string connectionString;
        private DataRowView editingRow;

        public RepertoireFormWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        }

        public RepertoireFormWindow(DataRowView row) : this()
        {
            editingRow = row;
            TitleBox.Text = row["title"].ToString();
            ComposerBox.Text = row["composer"].ToString();
            DurationBox.Text = row["duration"].ToString();
            CommentBox.Text = row["comment"].ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Название обязательно.");
                return;
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd;

                if (editingRow == null)
                {
                    cmd = new MySqlCommand("INSERT INTO repertoire (title, composer, duration, comment) VALUES (@t, @c, @d, @m)", conn);
                }
                else
                {
                    cmd = new MySqlCommand("UPDATE repertoire SET title=@t, composer=@c, duration=@d, comment=@m WHERE id_piece=@id", conn);
                    cmd.Parameters.AddWithValue("@id", editingRow["id_piece"]);
                }

                cmd.Parameters.AddWithValue("@t", TitleBox.Text);
                cmd.Parameters.AddWithValue("@c", ComposerBox.Text);
                cmd.Parameters.AddWithValue("@d", DurationBox.Text);
                cmd.Parameters.AddWithValue("@m", CommentBox.Text);

                cmd.ExecuteNonQuery();
            }

            DialogResult = true;
            Close();
        }
    }
}
