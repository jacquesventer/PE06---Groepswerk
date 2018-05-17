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
using System.Data.OleDb;
using MailingList.Lib.Entities;
using MailingList.Lib.Services;

namespace MailingList.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string bestandsPad;
        DeelnemerServices beheerDeelnemers = new DeelnemerServices();
        Deelnemer deelnemer_Sel;
        //TextBoxControl beheerControls = new TextBoxControl();

        public MainWindow()
        {
            InitializeComponent();
            DataOphalen();
            this.WindowState = WindowState.Maximized;
        }

        void VulList()
        {
            lstMailingList.ItemsSource = beheerDeelnemers.deelnemers;
            lstMailingList.Items.Refresh();
        }

        void DataOphalen()
        {
            beheerDeelnemers.deelnemers = new List<Deelnemer>();
            beheerDeelnemers.ImportData();
            VulList();
        }

        void MaakVeldenDeelnemersLeeg()
        {
            lstMailingList.SelectedIndex = -1;
            lblId.Content = "";
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtStreet.Clear();
            txtStreetNumber.Clear();
            txtCity.Clear();
            txtPostalCode.Clear();
            VulList();
        }

        private void lstPersoneel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMailingList.SelectedIndex >= 0)
            {
                deelnemer_Sel = (Deelnemer)lstMailingList.SelectedItem;
                txtFirstName.Text = deelnemer_Sel.FirstName;
                txtLastName.Text = deelnemer_Sel.LastName;
                txtEmail.Text = deelnemer_Sel.Email;
                txtPhone.Text = deelnemer_Sel.Phone.ToString();
                txtStreet.Text = deelnemer_Sel.Street;
                txtStreetNumber.Text = deelnemer_Sel.StreetNumber.ToString();
                txtCity.Text = deelnemer_Sel.City;
                txtPostalCode.Text = deelnemer_Sel.PostalCode.ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Deelnemer participant = new Deelnemer(0, txtFirstName.Text, txtLastName.Text, txtEmail.Text, Int32.Parse(txtPhone.Text),
                txtStreet.Text, Int32.Parse(txtStreetNumber.Text), txtCity.Text, Int32.Parse(txtPostalCode.Text));
            if (!beheerDeelnemers.NieuwDeelnemer(participant))
            {
                MessageBox.Show("De wijzigingen zijn niet opgeslagen");
            }
            else
            {
                MaakVeldenDeelnemersLeeg();
                VulList();
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            MaakVeldenDeelnemersLeeg();
            txtFirstName.Focus();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lblId.Content.ToString() != "")
            {
                if (!beheerDeelnemers.VerwijderDeelnemer(deelnemer_Sel))
                {
                    MessageBox.Show("De wijzigingen zijn niet opgeslagen");
                }
                else
                {
                    VulList();
                    MaakVeldenDeelnemersLeeg();
                }
            }
        }

        private void btnSendNewsLetter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChooseWinner_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSendWinnersMail_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
