using MusicCollective.Pages;
using System.Windows;

namespace MusicCollective
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Concerts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ConcertsPage());
        }

        private void Rehearsals_Click(object sender, RoutedEventArgs e)
        {
            
MainFrame.Navigate(new Pages.RehearsalsPage());
        }

        private void Members_Click(object sender, RoutedEventArgs e)
        {
            
MainFrame.Navigate(new Pages.MembersPage());
        }

        private void Repertoire_Click(object sender, RoutedEventArgs e)
        {
            
MainFrame.Navigate(new Pages.RepertoirePage());
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ReportsPage());
        }
    }
}
