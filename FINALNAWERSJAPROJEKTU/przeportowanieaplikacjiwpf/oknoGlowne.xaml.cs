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
using Microsoft.Win32;

namespace przeportowanieaplikacjiwpf
{
    /// <summary>
    /// Interaction logic for oknoGlowne.xaml
    /// </summary>
    public partial class oknoGlowne : Window
    {

        static string log = "Pracownik", pass = "projekt", conn = "SERVER= 164.132.98.173;DATABASE=projekt;" + "UID=" + log + ";" + "PASSWORD=" + pass + ";character set=utf8";
        MySqlConnection connection = new MySqlConnection(conn);
        public oknoGlowne(CheckBox gosc)
        {
            InitializeComponent();
            urzWyswietl();
            zdjWyswietl();
            prWyswietl();
            lokWyswietl();
            inwWyswietl();
            amoWyswietl();
            histWyswietl();
            if (gosc.IsChecked == true)
            {
                log = "Gosc";
                pass = "";
                buttonDodaj.IsEnabled = false;
                buttonUsun.IsEnabled = false;
                buttonZdjeciaDodaj.IsEnabled = false;
                buttonZdjeciaUsun.IsEnabled = false;
                buttonZdjecieWybierz.IsEnabled = false;
                button.IsEnabled = false;
                button2.IsEnabled = false;
                buttonDodajLokal.IsEnabled = false;
                buttonUsunLokal.IsEnabled = false;
                buttonInwentaryzacjaDodaj.IsEnabled = false;
                buttonDodajLokal_Copy.IsEnabled = false;
                buttonUsunLokal_Copy.IsEnabled = false;
                buttonAmortyzacja.IsEnabled = false;
            }
            if (gosc.IsChecked == false)
            {
                log = "Pracownik";
                pass = "projekt";
            }
        }
        private void oknoGlowne_Loaded(object sender, RoutedEventArgs e)
        {
            string query1 = "call wyswietl_zdjecia_nie();";
            connection.Open();
            MySqlCommand cmd1 = new MySqlCommand(query1, connection);
            MySqlDataReader mdr1 = cmd1.ExecuteReader();
            while (mdr1.Read())
            {
                comboZdjecia1.Items.Add(mdr1.GetString("ID"));
            }
            connection.Close();
            string query2 = "call wyswietl_zdjecia_tak();";
            connection.Open();
            MySqlCommand cmd2 = new MySqlCommand(query2, connection);
            MySqlDataReader mdr2 = cmd2.ExecuteReader();
            while (mdr2.Read())
            {
                comboZdjecia2.Items.Add(mdr2.GetString("ID"));
                comboZdjecia3.Items.Add(mdr2.GetString("ID"));
            }
            connection.Close();
            string query3 = "call wyswietl_urzadzenia();";
            connection.Open();
            MySqlCommand cmd3 = new MySqlCommand(query3, connection);
            MySqlDataReader mdr3 = cmd3.ExecuteReader();
            while (mdr3.Read())
            {
                comboAktualneID.Items.Add(mdr3.GetString("ID"));
                comboInwentaryzacja.Items.Add(mdr3.GetString("ID"));
            }
            connection.Close();
            string query4 = "select * from pracownicy";
            connection.Open();
            MySqlCommand cmd4 = new MySqlCommand(query4, connection);
            MySqlDataReader mdr4 = cmd4.ExecuteReader();
            string imie, nazwisko, wspolne;
            textBoxUrzadzenie5.IsEnabled = false;
            textBoxUrzadzenie6.IsEnabled = false;
            while (mdr4.Read())
            {
                imie = mdr4.GetString("imie");
                nazwisko = mdr4.GetString("nazwisko");
                wspolne = imie + " " + nazwisko;
                comboBoxUrzadzeniaImieNazwisko.Items.Add(wspolne);
            }
            connection.Close();
        }

        private void comboBoxUrzadzeniaImieNazwisko_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            connection.Open();
            string wspolne;
            wspolne = comboBoxUrzadzeniaImieNazwisko.SelectedItem.ToString();
            string[] sp = wspolne.Split(' ');
            textBoxUrzadzenie5.Text = sp[0];
            textBoxUrzadzenie6.Text = sp[1];
            connection.Close();
        }
        private void buttonDodaj_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            try
            {
                string query = "call dodaj_urzadzenie('" + textBoxUrzadzenie1.Text + "', '" + textBoxUrzadzenie2.Text + "', '" + textBoxUrzadzenie3.Text + "', " + textBoxUrzadzenie4.Text + ", '" + textBoxUrzadzenie5.Text + "', '" + textBoxUrzadzenie6.Text + "', " + textBoxUrzadzenie7.Text + ", " + textBoxUrzadzenie8.Text + ")";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Jedna z wartosci jest zle podana", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
            urzWyswietl();
        }

        private void buttonUsun_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            try
            {
                string query = "call usun_urzadzenia(" + textBoxUrzadzenie9.Text + ")";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Niepoprawne ID", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
            urzWyswietl();
        }

        private void buttonZdjeciaPokaz_Click(object sender, RoutedEventArgs e)
        {
            Zdjecia zdj = new Zdjecia(comboZdjecia2.Text);
            zdj.Show();   
        }

        private void buttonZdjecieWybierz_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                textSciezka.Text = openFileDialog.FileName;
        }

        private void buttonZdjeciaDodaj_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = new MySqlConnection(conn);
            MySqlCommand cmd;
            FileStream fs;
            BinaryReader br;
            try
            {
                if (textSciezka.Text.Length > 0)
                {
                    string FileName = textSciezka.Text;
                    byte[] ImageData;
                    fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                    br = new BinaryReader(fs);
                    ImageData = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();
                    string CmdString = "call dodaj_zdjęcie(@idurzadzenie, @Image)";
                    cmd = new MySqlCommand(CmdString, con);
                    cmd.Parameters.Add("@idurzadzenie", MySqlDbType.VarChar, 45);
                    cmd.Parameters.Add("@Image", MySqlDbType.Blob);
                    string combo = comboZdjecia1.Text;
                    cmd.Parameters["@idurzadzenie"].Value = comboZdjecia1.Text;
                    cmd.Parameters["@Image"].Value = ImageData;
                    con.Open();
                    int RowsAffected = cmd.ExecuteNonQuery();
                    if (RowsAffected > 0)
                    {
                        MessageBox.Show("Zdjecie zapisane poprawnie!");
                        zdjWyswietl();
                        RefreshCombo1();
                        RefreshCombo2();
                        RefreshCombo3();
                    }
                    con.Close();
                }
                else
                {
                    MessageBox.Show("Niekompletne dane!");
                }
            }
            catch (MySqlException mex)
            {
                MessageBox.Show(mex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    zdjWyswietl();
                }
            }
        }

        private void buttonZdjeciaUsun_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            try
            {
                string query = "call usun_zdjecia(" + comboZdjecia3.Text + ")";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Zdjecie badz urzadzenie juz nie istnieje", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
            comboZdjecia3.Text = "";
            RefreshCombo1();
            RefreshCombo2();
            RefreshCombo3();
            zdjWyswietl();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string imie = textPracownik1.Text;
            string nazwisko = textPracownik2.Text;
            if (imie.Length > 0 && nazwisko.Length > 0)
            {
                connection.Open();
                try
                {
                    string query = "call dodaj_pracownika('" + imie + "', '" + nazwisko + "')";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    MessageBox.Show("Blednie podane dane!", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            } else {
                MessageBox.Show("Probujesz wyslac puste wartosci");
                     }
            connection.Close();
            prWyswietl();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            try
            {
                string query = "call usun_pracownika(" + textUsunPrac.Text + ")";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Pracownik juz nie istnieje!", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
            prWyswietl();
        }
        public void urzWyswietl()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call wyswietl_urzadzenia()";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridUrzadzenia.ItemsSource = dt.DefaultView;
        }

        private void button_ClickLokal(object sender, RoutedEventArgs e)
        {
            connection.Open();
            try
            {
                string numerpietrasl = textLokalizacje1.Text;
                int numerpietrail = Convert.ToInt32(numerpietrasl);
                string numerpokojusl = textLokalizacje2.Text;
                int numerpokojuil = Convert.ToInt32(numerpokojusl);
                string query = "call dodaj_lokalizacje(" + numerpietrail + ", " + numerpokojuil + ")";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Blednie podane dane!", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
            lokWyswietl();
        }

        private void button2_ClickLokal(object sender, RoutedEventArgs e)
        {
            connection.Open();
            try
            {
                string query = "call usun_lokalizacje(" + textLokalizacje3.Text + ")";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Aktualizacja juz nie istnieje!", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
            lokWyswietl();
        }
        public void zdjWyswietl()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call wyswietl_zdjecia()";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridZdjecia.ItemsSource = dt.DefaultView;
        }
        public void prWyswietl()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call wyswietl_baza_pracownicy();";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridPracownicy.ItemsSource = dt.DefaultView;
        }

        private void buttonAktLokalizacjeDodaj(object sender, RoutedEventArgs e)
        {
            connection.Open();
            try
            {
                string query = "call aktualizuj_lokalizacje (" + comboAktualneID.Text + ", " + textAktualnePietro.Text + ", " + textAktualnyPokoj.Text + ")";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Blednie podane dane!", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
            aktWyswietlLokal();
        }

        private void buttonAktualnePracDodaj(object sender, RoutedEventArgs e)
        {
            connection.Open();
            try
            {
                string query = "call aktualizuj_pracownika (" + comboAktualneID.Text + ", '" + textAktualneImie.Text + "', '" + textAktualneNazwisko.Text + "')";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Blednie podane dane!", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
            aktWyswietlPrac();
        }

        public void lokWyswietl()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call wyswietl_baza_lokalizacje();";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridLokalizacje.ItemsSource = dt.DefaultView;

        }
        public void inwWyswietl()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call wyswietl_urzadzenia()";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridInwentaryzacja.ItemsSource = dt.DefaultView;
        }

        private void AktPrac(object sender, RoutedEventArgs e)
        {
            aktWyswietlPrac();
            textAktualnePietro.IsEnabled = false;
            textAktualnyPokoj.IsEnabled = false;
            textAktualneImie.IsEnabled = true;
            textAktualneNazwisko.IsEnabled = true;
        }

        private void AktLokal(object sender, RoutedEventArgs e)
        {
            aktWyswietlLokal();
            textAktualnePietro.IsEnabled = true;
            textAktualnyPokoj.IsEnabled = true;
            textAktualneImie.IsEnabled = false;
            textAktualneNazwisko.IsEnabled = false;
        }

        private void amorRecz(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query = "call sprawdz_amortyzacja()";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            amoWyswietl();
        }

        public void aktWyswietlPrac()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call aktualna_pracownicy();";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridAktualne.ItemsSource = dt.DefaultView;
        }

        public void aktWyswietlLokal()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call aktualna_lokalizacje();";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridAktualne.ItemsSource = dt.DefaultView;
        }

        public void amoWyswietl()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call wyswietl_zamortyzowane();";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridAmortyzacja.ItemsSource = dt.DefaultView;
        }

        private void textBoxWyszukaj_KeyUp(object sender, KeyEventArgs e)
        {
            if (comboBoxWyszukaj.Text == "Pracownik; Nazwisko")
            {
                connection.Open();
                DataTable dt = new DataTable();
                using (MySqlDataAdapter da = new MySqlDataAdapter("call szukaj_pracownicy_nazwisko('" + textBoxWyszukaj.Text + "')", connection))
                    da.Fill(dt);
                dataGridWyszukaj.ItemsSource = dt.DefaultView;
                connection.Close();
            }

            else if (comboBoxWyszukaj.Text == "Pracownik; Imie")
            {
                connection.Open();
                DataTable dt = new DataTable();
                using (MySqlDataAdapter da = new MySqlDataAdapter("call szukaj_pracownicy_imie('" + textBoxWyszukaj.Text + "')", connection))
                    da.Fill(dt);
                dataGridWyszukaj.ItemsSource = dt.DefaultView;
                connection.Close();
            }
            else if (comboBoxWyszukaj.Text == "Urzadzenie; Nazwa/Opis")
            {
                connection.Open();
                DataTable dt = new DataTable();
                using (MySqlDataAdapter da = new MySqlDataAdapter("call szukaj_urzadzenia_nazwa_opis('" + textBoxWyszukaj.Text + "')", connection))
                    da.Fill(dt);
                dataGridWyszukaj.ItemsSource = dt.DefaultView;
                connection.Close();
            }
            else if (comboBoxWyszukaj.Text == "Urzadzenie; Kod")
            {
                connection.Open();
                DataTable dt = new DataTable();
                using (MySqlDataAdapter da = new MySqlDataAdapter("call szukaj_urzadzenia_kod('" + textBoxWyszukaj.Text + "')", connection))
                    da.Fill(dt);
                dataGridWyszukaj.ItemsSource = dt.DefaultView;
                connection.Close();
            }
        }

        private void buttonInwentaryzacjaPokaz_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call wyswietl_inwentaryzacje(" + comboInwentaryzacja.Text + ");";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridInwentaryzacja2.ItemsSource = dt.DefaultView;
            connection.Close();
        }

        private void buttonInwentaryzacjaDodaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                string query = "call dodaj_inwentaryzacja(" + comboInwentaryzacja.Text + ")";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
                DataTable dt = new DataTable();
                using (connection)
                {
                    connection.Open();
                    string query2 = "call wyswietl_inwentaryzacje(" + comboInwentaryzacja.Text + ");";
                    using (MySqlDataAdapter da = new MySqlDataAdapter(query2, connection))
                        da.Fill(dt);
                }
                dataGridInwentaryzacja2.ItemsSource = dt.DefaultView;
            }
            catch (Exception)
            {
                MessageBox.Show("Blad!", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();
        }

        private void formatuj(object sender, DataGridRowEventArgs e)
        {
            DataGridRow row = e.Row;
            DataRowView rView = row.Item as DataRowView;
            if (rView != null && rView.Row.ItemArray[6].ToString().Contains("TAK"))
            {
                Color col = (Color)ColorConverter.ConvertFromString("Red");
                e.Row.Background = new SolidColorBrush(col);
            }
            else
            {
                Color col = (Color)ColorConverter.ConvertFromString("White");
                e.Row.Background = new SolidColorBrush(col);
            }
        }

        public void histWyswietl()
        {
            DataTable dt = new DataTable();
            using (connection)
            {
                connection.Open();
                string query = "call wyswietl_historia();";
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                    da.Fill(dt);
            }
            dataGridHistoria1.ItemsSource = dt.DefaultView;
            connection.Close();
        }

        private void PobierzIdHistoria(object sender, MouseButtonEventArgs e)
        {
            try
            {
                object item = dataGridHistoria1.SelectedItem;
                string ID = (dataGridHistoria1.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                if (radioButtonHistoriaPracownikow.IsChecked == true)
                {
                    DataTable dt = new DataTable();
                    using (connection)
                    {
                        connection.Open();
                        string query = "call historia_pracownicy(" + ID + ")";
                        using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                            da.Fill(dt);
                    }
                    dataGridHistoria2.ItemsSource = dt.DefaultView;
                }
                else if (radioButtonHistoriaLokalizacji.IsChecked == true)
                {
                    DataTable dt = new DataTable();
                    using (connection)
                    {
                        connection.Open();
                        string query = "call historia_lokalizacje(" + ID + ")";
                        using (MySqlDataAdapter da = new MySqlDataAdapter(query, connection))
                            da.Fill(dt);
                    }
                    dataGridHistoria2.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Kliknietą zła komórkę", "Blad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        void  RefreshCombo1()
        {
            string query1 = "call wyswietl_zdjecia_nie();";
            connection.Open();
            comboZdjecia1.Items.Clear();
            connection.Close();
            connection.Open();
            MySqlCommand cmd1 = new MySqlCommand(query1, connection);
            MySqlDataReader mdr1= cmd1.ExecuteReader();
            while (mdr1.Read())
            {
                comboZdjecia1.Items.Add(mdr1.GetString("ID"));
            }
            connection.Close();
        }
        void RefreshCombo2()
        {
            string query2 = "call wyswietl_zdjecia_tak();";
            connection.Open();
            comboZdjecia2.Items.Clear();            
            connection.Close();
            connection.Open();
            MySqlCommand cmd3 = new MySqlCommand(query2, connection);
            MySqlDataReader mdr3 = cmd3.ExecuteReader();
            while (mdr3.Read())
            {
                comboZdjecia2.Items.Add(mdr3.GetString("ID"));
            }
            connection.Close();
        }

        void RefreshCombo3()
        {
            string query3 = "call wyswietl_zdjecia_tak();";
            connection.Open();
            comboZdjecia3.Items.Clear();
            connection.Close();
            connection.Open();
            MySqlCommand cmd3 = new MySqlCommand(query3, connection);
            MySqlDataReader mdr3 = cmd3.ExecuteReader();
            while (mdr3.Read())
            {
                comboZdjecia3.Items.Add(mdr3.GetString("ID"));
            }
            connection.Close();
        }
    }
}

