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
using System.Security.Cryptography;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mqtt_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonLoginClick(object sender, RoutedEventArgs e)
        {
            string Login = textBoxLogin.Text;
            string Password = textBoxPassword.Text;

            try
            {
                AuthAnswer authAnswer = await new ServerRequests().AuthorizeUser(Login, Password);

                // Transition to new Window
                if (authAnswer.IsAuth == false)
                {
                    MessageBox.Show("Логин или пароль некорректны", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (authAnswer.IsAdmin == true)
                {
                    Admin AdminWindow = new Admin(authAnswer.JWTtoken);
                    this.Hide();
                    AdminWindow.ShowDialog();
                    this.Show();
                }
                else
                {
                    User UserWindow = new User(authAnswer.JWTtoken);
                    this.Hide();
                    UserWindow.ShowDialog();
                    this.Show();
                }
            }
            catch 
            {
                MessageBox.Show("Ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonRegisterClick(object sender, RoutedEventArgs e)
        {
            Register RegistrationWindow = new Register();
            this.Hide();
            RegistrationWindow.ShowDialog();
            this.Show();
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            System.Environment.Exit(0);
        }
    }
    // Class for getting jwt token and information about status of authorization
    public class AuthAnswer
    {
        public string JWTtoken { get; set; } = null!;

        public bool IsAuth { get; set; } = false;

        public bool IsAdmin { get; set; } = false;
    }
}
