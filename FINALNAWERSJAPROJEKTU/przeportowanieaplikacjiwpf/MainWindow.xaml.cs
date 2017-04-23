using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using MySql.Data;
using MySql.Data.Types;
namespace przeportowanieaplikacjiwpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string conn;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonZaloguj_Click(object sender, RoutedEventArgs e)
        {
            string log;
            string pass;
            oknoGlowne oG = new oknoGlowne(checkBox);
            log = textLog.Text;
            pass = textHaslo.Password;
            conn = "SERVER= 164.132.98.173;DATABASE=projekt;" + "UID=" + log + ";" + "PASSWORD=" + pass + ";";

            using (MySqlConnection con = new MySqlConnection(conn))
            {
                try
                {
                    con.Open();
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        this.Hide();
                        oG.Closed += (s, args) => this.Close();
                        oG.Show();
                    }
                }
                catch
                {
                    MessageBox.Show("Nie udało się połączyć");
                }
            }
        }

        private void zalogujGosc(object sender, RoutedEventArgs e)
        {
            textLog.Text = "Gosc";
            textHaslo.Password = "";
            textLog.IsEnabled = false;
            textHaslo.IsEnabled = false;
        }

        private void zalogujUn(object sender, RoutedEventArgs e)
        {
            textLog.Text = "";
            textHaslo.Password = "";
            textLog.IsEnabled = true;
            textHaslo.IsEnabled = true;
        }
    }
}
