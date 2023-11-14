using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace mqtt_client
{
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private async void ButtonRegisterClick(object sender, RoutedEventArgs e)
        {
            string Login = textBoxLogin.Text;
            string Password = textBoxPassword.Text;

            try
            {
                (string? jsonResponse, HttpStatusCode httpStatusCode) = await ServerRequests.RegisterUser(Login, Password);

                if (httpStatusCode != HttpStatusCode.Created)
                { 
                    MessageBox.Show(jsonResponse, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Регистрация успешна", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Hide();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
