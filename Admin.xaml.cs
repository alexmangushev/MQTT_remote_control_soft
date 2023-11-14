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

namespace mqtt_client
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        List<UserFromDB>? userFromDBs;
        static string? JWTToken;
        static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:5240/"),
        };

        public Admin(string token)
        {
            JWTToken = token;
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            InitializeComponent();
            SyncTable();
        }


        public async void SyncTable()
        {
            using HttpResponseMessage response = await httpClient.GetAsync("api/Users");

            string? jsonResponse = await response.Content.ReadAsStringAsync();

            userFromDBs = JsonConvert.DeserializeObject<List<UserFromDB>>(jsonResponse);

            DBGrid.ItemsSource = userFromDBs;
        }

        private async void ButtonDeleteClick(object sender, RoutedEventArgs e)
        {
            UserFromDB userDelete = (UserFromDB)DBGrid.SelectedItem;

            using HttpResponseMessage response = await httpClient.DeleteAsync(
                   String.Format("api/Users/{0}", userDelete.UserId));

            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                MessageBox.Show(jsonResponse, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                userFromDBs.Remove(userDelete);

            SyncTable();
        }

        private async void CreateDB(UserFromDB UserFromDB)
        {
            using StringContent jsonContent = new(
                    JsonConvert.SerializeObject(UserFromDB),
                    Encoding.UTF8,
                    "application/json");

            using HttpResponseMessage response = await httpClient.PostAsync(
                String.Format("api/Users"), jsonContent);

            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                MessageBox.Show(jsonResponse, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);

            SyncTable();
        }

        private async void EditCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            UserFromDB userUpdate = e.Row.Item as UserFromDB; //userFromDBs[DBGrid.SelectedIndex];

            using StringContent jsonContent = new(
                    JsonConvert.SerializeObject(userUpdate),
                    Encoding.UTF8,
                    "application/json");

            using HttpResponseMessage response = await httpClient.PutAsync(
                String.Format("api/Users/{0}", userUpdate.UserId), jsonContent);

            var jsonResponse = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
                MessageBox.Show(jsonResponse, "Ошибка редактирования", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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
