using System;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MusicCollective.Pages
{
    public partial class ReportsPage : Page
    {
        private string connectionString;

        public ReportsPage()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            GenerateReports();
        }

        private void GenerateReports()
        {
            ReportsPanel.Children.Clear();

            ReportsPanel.Children.Add(BuildReportBlock("1. Репетиции за последний месяц", GetRehearsalStats()));
            ReportsPanel.Children.Add(BuildReportBlock("2. Количество концертов по месяцам", GetConcertsByMonth()));
            ReportsPanel.Children.Add(BuildReportBlock("3. Произведения по композиторам", GetRepertoireByComposer()));
        }

        private Border BuildReportBlock(string title, string content)
        {
            var block = new Border
            {
                Background = System.Windows.Media.Brushes.WhiteSmoke,
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(15)
            };

            var panel = new StackPanel();
            panel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            });
            panel.Children.Add(new TextBlock
            {
                Text = content,
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap
            });

            block.Child = panel;
            return block;
        }

        private string GetRehearsalStats()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM rehearsals WHERE date >= CURDATE() - INTERVAL 30 DAY", conn);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return $"Количество репетиций за последние 30 дней: {count}";
            }
        }

        private string GetConcertsByMonth()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"
                    SELECT DATE_FORMAT(date, '%Y-%m') AS month, COUNT(*) AS total
                    FROM concerts
                    GROUP BY month
                    ORDER BY month DESC
                    LIMIT 6", conn);

                var sb = new StringBuilder();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sb.AppendLine($"{reader["month"]}: {reader["total"]} концертов");
                    }
                }

                return sb.Length > 0 ? sb.ToString() : "Нет данных о концертах.";
            }
        }

        private string GetRepertoireByComposer()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"
                    SELECT composer, COUNT(*) AS total
                    FROM repertoire
                    GROUP BY composer
                    ORDER BY total DESC", conn);

                var sb = new StringBuilder();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sb.AppendLine($"{reader["composer"]}: {reader["total"]} произведений");
                    }
                }

                return sb.Length > 0 ? sb.ToString() : "Нет произведений в базе.";
            }
        }
    }
}
