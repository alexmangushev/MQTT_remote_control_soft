using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

namespace mqtt_client
{
    /// <summary>
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class User : Window
    {
        private readonly Statistics statisctics;
        private readonly ServerRequests serverRequest;
        public User(string token)
        {
            statisctics = new Statistics();
            serverRequest = new ServerRequests(token);
            InitializeComponent();
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            System.Environment.Exit(0);
        }

        public void SyncTable()
        {
            List<ShowNumberData> showNumberData = new();
            showNumberData.Add(new ShowNumberData());
            showNumberData.Add(new ShowNumberData());

            showNumberData[0].Name = "Температура";
            showNumberData[0].Min = statisctics.GetMinTemp();
            showNumberData[0].Avg = statisctics.GetAvarageTemp();
            showNumberData[0].Max = statisctics.GetMaxTemp();

            showNumberData[1].Name = "Влажность";
            showNumberData[1].Min = statisctics.GetMinHumidity();
            showNumberData[1].Avg = statisctics.GetAvarageHumidity();
            showNumberData[1].Max = statisctics.GetMaxHumidity();

            dataGrid.ItemsSource = showNumberData;

        }
        
        private async void ButtonGetDataClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime? startDate = DatePickerStart.SelectedDate.Value.ToLocalTime();
                DateTime? endDate = DatePickerEnd.SelectedDate.Value.ToLocalTime();

                if (startDate <= endDate && endDate <= DateTime.Now)
                {

                    RequestData date = new RequestData();
                    date.Start = startDate; date.End = endDate;

                    (string? jsonResponse, HttpStatusCode httpStatusCode) = await serverRequest.GetData(date, "api/Data/for_user");

                    List<Datum> listData = JsonConvert.DeserializeObject<List<Datum>>(jsonResponse);
                    statisctics._listData = listData;
                    SyncTable();
                }
                else
                {
                    MessageBox.Show("Укажите корректные даты", "Ошибка запроса", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка запроса", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }

    // Class for show data in table
    public class ShowNumberData
    {
        public string Name { get; set; }
        public double Min { get; set; }
        public double Avg { get; set; }
        public double Max { get; set; }
    }
}
