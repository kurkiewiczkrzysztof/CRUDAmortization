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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using MySql.Data;
using MySql.Data.Types;
using System.IO;
using System.Data;
using System.Timers;
using System.Threading;
namespace przeportowanieaplikacjiwpf
{
    /// <summary>
    /// Interaction logic for Zdjecia.xaml
    /// </summary>
    public partial class Zdjecia : Window
    {

        static string log = "projekt", pass = "projekt", conn = "SERVER= 164.132.98.173;DATABASE=projekt;" + "UID=" + log + ";" + "PASSWORD=" + pass + ";character set=utf8";
        MySqlConnection connection = new MySqlConnection(conn);
        public Zdjecia(string qs)
        {
            InitializeComponent();
            try
            {
                string query = "Select * from zdjecia where idurzadzenie=" + qs;
                DataTable dt = new DataTable();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                byte[] img = (byte[])dt.Rows[0][1];
                MemoryStream ms = new MemoryStream(img);
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = ms;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
                image.Source = imageSource;
                da.Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show("Nie ma takiego zdjecia!", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
