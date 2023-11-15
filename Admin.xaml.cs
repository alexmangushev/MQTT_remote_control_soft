using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
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
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using System.IO;
using System.Net;

namespace mqtt_client
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        List<UserFromDB>? userFromDBs;
        private readonly ServerRequests serverRequest;

        public Admin(string token)
        {
            serverRequest = new ServerRequests(token);
            InitializeComponent();
            SyncTable();
        }


        public async void SyncTable()
        {
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await serverRequest.GetUsers();

            userFromDBs = JsonConvert.DeserializeObject<List<UserFromDB>>(jsonResponse);

            DBGrid.ItemsSource = userFromDBs;
        }

        private async void ButtonDeleteClick(object sender, RoutedEventArgs e)
        {
            UserFromDB userDelete = (UserFromDB)DBGrid.SelectedItem;

            (string? jsonResponse, HttpStatusCode httpStatusCode) = await serverRequest.DeleteUser(userDelete);

            if (httpStatusCode != HttpStatusCode.NoContent)
                MessageBox.Show(jsonResponse, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                userFromDBs.Remove(userDelete);

            SyncTable();
        }

        private async void ButtonSendClick(object sender, RoutedEventArgs e)
        {
            // Open dialog
            var dialog = new OpenFileDialog()
            {
                Title = "Выберите файл",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                Multiselect = false,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            // Get filename
            string fileName = "";
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }

            if (fileName.Length > 0)
            {
                // Open the file to read from.
                string readText = File.ReadAllText(fileName);

                FileBody file = new();
                file.Data = readText;

                // Send text to server
                HttpStatusCode statusCode = await serverRequest.SendFile(file);
                if (statusCode == HttpStatusCode.OK) 
                {
                    MessageBox.Show("Файл отправлен", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось отправить файл", "Ошибка отправки", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private async void CreateDB(UserFromDB UserFromDB)
        {
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await serverRequest.AddUser(UserFromDB);

            if (httpStatusCode != HttpStatusCode.Created)
                MessageBox.Show(jsonResponse, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);

            SyncTable();
        }

        private async void EditCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            UserFromDB userUpdate = e.Row.Item as UserFromDB; //userFromDBs[DBGrid.SelectedIndex];

            (string? jsonResponse, HttpStatusCode httpStatusCode) = await serverRequest.ModifyUser(userUpdate);


            if (httpStatusCode != System.Net.HttpStatusCode.NotFound && httpStatusCode != HttpStatusCode.NoContent)
                MessageBox.Show(jsonResponse, "Ошибка редактирования", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (httpStatusCode == HttpStatusCode.NotFound)
                CreateDB(userUpdate);

            SyncTable();
        }


        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            System.Environment.Exit(0);
        }

    }
}
