using MQTTnet.Server;
using MQTTnet.Client;
using MQTTnet;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Protocol;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.DotNet.MSIdentity.Shared;
using mqtt_remote_server.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace mqtt_remote_server
{
    public class Mqtt
    {
        private static string Topic;
        private static string Broker;
        private static int Port;

        public Mqtt()
        {
            StreamReader strReader = new StreamReader("ConnectionMQTT.txt"); // get Reader
            Mqtt.Topic = strReader.ReadLine();
            Mqtt.Broker = strReader.ReadLine();
            Mqtt.Port = Int32.Parse(strReader.ReadLine());
        }

        public Mqtt(string Topic, string Broker, int Port)
        {
            Mqtt.Topic = Topic;
            Mqtt.Broker = Broker;
            Mqtt.Port = Port;
        }


        // Create a MQTT client factory
        public static MqttFactory mqttFactory = new MqttFactory();

        // Create a MQTT client instance
        public static IMqttClient client = mqttFactory.CreateMqttClient();

        // Create keep alive timer
        static TimeSpan keepAlive = TimeSpan.FromSeconds(60);

        // Create MQTT client options
        static MqttClientOptions mqttClientOptions;

        static MqttClientConnectResult? connectResult;

        public async Task Connect()
        {
            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(Broker, Port)
                .WithKeepAlivePeriod(keepAlive)
                .WithCleanSession()
                .Build();

            connectResult = await client.ConnectAsync(mqttClientOptions);

            Console.WriteLine("The MQTT client is connected.");
        }

        public async Task Disconnect()
        {
            await client.DisconnectAsync();
            Console.WriteLine("The MQTT client disconnected.");
        }

        public async Task Subscribe()
        {
            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");

                // Subscribe to a topic
                await client.SubscribeAsync(Topic);

                // Callback function when a message is received
                client.ApplicationMessageReceivedAsync += async e =>
                {
                    CallbackMessageReceived(e);
                };
            }
            else
            {
                Console.WriteLine("Connected to MQTT broker fail.");
            }
        }

        public async Task Unsubscribe()
        {
            await client.UnsubscribeAsync(Topic);
        }

        public async void CallbackMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            // {"Temp":25,"Humidity":54,"Power":false,"People":false,"Smoke":true, "DeviceId":0}
            string data_str = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            Console.WriteLine($"Received message: {data_str}");
            Datum mQTTData = JsonConvert.DeserializeObject<Datum>(data_str);

            mQTTData.GetTime = DateTime.Now;

            using StringContent jsonContent = new(
                JsonConvert.SerializeObject(mQTTData),
                Encoding.UTF8,
                "application/json");

            HttpClient httpClient = new()
            {
                BaseAddress = new Uri("http://localhost:5240/"),
            };

            ServerAuth serverAuth = ServerAuth.getInstance();

            string token = await serverAuth.ReturnToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            using HttpResponseMessage response = await httpClient.PostAsync(
                String.Format("api/Data"), jsonContent);

            var jsonResponse = await response.Content.ReadAsStringAsync();
        }
    }
}
