using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Windows
{
    public partial class MemberFormWindow : Window
    {
        private string connectionString;
        private DataRowView editingRow;

        public MemberFormWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        }

        public MemberFormWindow(DataRowView row) : this()
        {
            editingRow = row;
            LastNameBox.Text = row["last_name"].ToString();
            FirstNameBox.Text = row["first_name"].ToString();
            PatronymicBox.Text = row["patronymic"].ToString();
            InstrumentBox.Text = row["instrument"].ToString();
            PhoneBox.Text = row["phone"].ToString();
            EmailBox.Text = row["email"].ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Фамилия обязательна.");
                return;
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd;

                if (editingRow == null)
                {
                    cmd = new MySqlCommand("INSERT INTO members (last_name, first_name, patronymic, instrument, phone, email) VALUES (@l, @f, @p, @i, @ph, @e)", conn);
                }
                else
                {
                    cmd = new MySqlCommand("UPDATE members SET last_name=@l, first_name=@f, patronymic=@p, instrument=@i, phone=@ph, email=@e WHERE id_member=@id", conn);
                    cmd.Parameters.AddWithValue("@id", editingRow["id_member"]);
                }

                cmd.Parameters.AddWithValue("@l", LastNameBox.Text);
                cmd.Parameters.AddWithValue("@f", FirstNameBox.Text);
                cmd.Parameters.AddWithValue("@p", PatronymicBox.Text);
                cmd.Parameters.AddWithValue("@i", InstrumentBox.Text);
                cmd.Parameters.AddWithValue("@ph", PhoneBox.Text);
                cmd.Parameters.AddWithValue("@e", EmailBox.Text);

                cmd.ExecuteNonQuery();
            }

            DialogResult = true;
            Close();
        }
    }
}
