using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Windows
{
    public partial class RehearsalFormWindow : Window
    {
        private string connectionString;
        private DataRowView editingRow;
        private DataTable ensembleTable;

        public RehearsalFormWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            LoadEnsembles();
        }

        public RehearsalFormWindow(DataRowView row) : this()
        {
            editingRow = row;
            DatePicker.SelectedDate = Convert.ToDateTime(row["date"]);
            TimeBox.Text = row["time"].ToString();
            LocationBox.Text = row["location"].ToString();

            // Находим id_ensemble по имени
            foreach (DataRow r in ensembleTable.Rows)
            {
                if (r["name"].ToString() == row["ensemble_name"].ToString())
                {
                    EnsembleCombo.SelectedValue = r["id_ensemble"];
                    break;
                }
            }
        }

        private void LoadEnsembles()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT id_ensemble, name FROM ensembles", conn);
                ensembleTable = new DataTable();
                adapter.Fill(ensembleTable);
                EnsembleCombo.ItemsSource = ensembleTable.DefaultView;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate == null || string.IsNullOrWhiteSpace(TimeBox.Text) || EnsembleCombo.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd;

                if (editingRow == null)
                {
                    cmd = new MySqlCommand("INSERT INTO rehearsals (date, time, location, id_ensemble) VALUES (@d, @t, @l, @e)", conn);
                }
                else
                {
                    cmd = new MySqlCommand("UPDATE rehearsals SET date=@d, time=@t, location=@l, id_ensemble=@e WHERE id_rehearsal=@id", conn);
                    cmd.Parameters.AddWithValue("@id", editingRow["id_rehearsal"]);
                }

                cmd.Parameters.AddWithValue("@d", DatePicker.SelectedDate.Value);
                cmd.Parameters.AddWithValue("@t", TimeBox.Text);
                cmd.Parameters.AddWithValue("@l", LocationBox.Text);
                cmd.Parameters.AddWithValue("@e", EnsembleCombo.SelectedValue);
                cmd.ExecuteNonQuery();
            }

            DialogResult = true;
            Close();
        }
    }
}
