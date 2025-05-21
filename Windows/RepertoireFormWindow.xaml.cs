using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Configuration;
using System;

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
            string title = TitleBox.Text.Trim();
            string composer = ComposerBox.Text.Trim();
            string durationText = DurationBox.Text.Trim();
            string comment = CommentBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите название произведения.");
                return;
            }

            if (string.IsNullOrWhiteSpace(composer))
            {
                MessageBox.Show("Введите имя композитора.");
                return;
            }

            if (string.IsNullOrWhiteSpace(durationText))
            {
                MessageBox.Show("Введите длительность произведения.");
                return;
            }

            if (!TimeSpan.TryParse(durationText, out _))
            {
                MessageBox.Show("Длительность должна быть в формате ЧЧ:ММ:СС, например 00:04:30.");
                return;
            }

            if (comment.Length > 500)
            {
                MessageBox.Show("Комментарий не должен превышать 500 символов.");
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

                cmd.Parameters.AddWithValue("@t", title);
                cmd.Parameters.AddWithValue("@c", composer);
                cmd.Parameters.AddWithValue("@d", durationText); 
                cmd.Parameters.AddWithValue("@m", comment);

                cmd.ExecuteNonQuery();
            }

            DialogResult = true;
            Close();
        }

    }
}
