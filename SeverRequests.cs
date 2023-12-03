using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Security.Policy;

namespace mqtt_client
{
    public class ServerRequests : IDoRequests
    {
        private string url = "http://localhost:5240/";
        string? JWTToken;

        public ServerRequests(string token) { JWTToken = token; }
        public ServerRequests() { JWTToken = ""; }
        public async Task<(string?, HttpStatusCode)> RegisterUser(string login, string password)
        {

            // Create object User
            UserFromDB newUser = new UserFromDB();
            newUser.Login = login;
            newUser.Password = GetHash(password);

            // Try to register User
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await DoPost(newUser, "api/Users", false);

            return (jsonResponse, httpStatusCode);
        }

        public async Task<(string?, HttpStatusCode)> GetData(RequestData date, string path)
        {
            // Try to get data
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await DoPost(date, path, true);

            return (jsonResponse, httpStatusCode);
        }

        public async Task<(string?, HttpStatusCode)> GetUsers()
        {
            // Try to get Users
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await DoGet("api/Users", true);

            return (jsonResponse, httpStatusCode);
        }

        public async Task<(string?, HttpStatusCode)> AddUser(UserFromDB user)
        {
            // Try to create User
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await DoPost(user, "api/Users", false);

            return (jsonResponse, httpStatusCode);
        }

        public async Task<(string?, HttpStatusCode)> ModifyUser(UserFromDB user)
        {
            // Try to modify User
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await DoPut(user, String.Format("api/Users/{0}", user.UserId), true);

            return (jsonResponse, httpStatusCode);
        }

        public async Task<(string?, HttpStatusCode)> DeleteUser(UserFromDB user)
        {
            
            // Try to delete user
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await DoDelete(String.Format("api/Users/{0}", user.UserId), true);

            return (jsonResponse, httpStatusCode);
        }

        public async Task<HttpStatusCode> SendFile(FileBody file)
        {
            // Try to send file
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await DoPost(file, "api/Data/file", true);

            return httpStatusCode;
        }

        // Return Token
        public async Task<AuthAnswer> AuthorizeUser(string login, string password) 
        {

            // Create User
            UserAuth auth = new UserAuth();
            auth.Login = login;
            auth.Password = GetHash(password);

            // Try to authorize user
            (string? jsonResponse, HttpStatusCode httpStatusCode) = await DoPost(auth, "api/data/login", false);

            if (httpStatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show(jsonResponse, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new AuthAnswer();
            }
            else
            {
                return JsonConvert.DeserializeObject<AuthAnswer>(jsonResponse);
            }
        }

        public async Task<(string, HttpStatusCode)> DoPost<T>(T obj, string path, bool isAuth)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(url)
            };

            if (isAuth) { httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JWTToken); }

            using StringContent jsonContent = new(
                JsonConvert.SerializeObject(obj),
                Encoding.UTF8,
                "application/json");

            using HttpResponseMessage response = await httpClient.PostAsync(
                String.Format(path), jsonContent);

            string? jsonResponse = await response.Content.ReadAsStringAsync();

            return (jsonResponse, response.StatusCode);
        }

        public async Task<(string, HttpStatusCode)> DoGet(string path, bool isAuth)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(url)
            };

            if (isAuth) { httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JWTToken); }


            using HttpResponseMessage response = await httpClient.GetAsync(path);

            string? jsonResponse = await response.Content.ReadAsStringAsync();

            return (jsonResponse, response.StatusCode);
        }

        public async Task<(string, HttpStatusCode)> DoDelete(string obj, bool isAuth)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(url)
            };

            if (isAuth) { httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JWTToken); }

            
            using HttpResponseMessage response = await httpClient.DeleteAsync(obj);

            string? jsonResponse = await response.Content.ReadAsStringAsync();

            return (jsonResponse, response.StatusCode);
        }

        public async Task<(string, HttpStatusCode)> DoPut<T>(T obj, string path, bool isAuth)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(url)
            };

            if (isAuth) { httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + JWTToken); }

            using StringContent jsonContent = new(
                JsonConvert.SerializeObject(obj),
                Encoding.UTF8,
                "application/json");

            using HttpResponseMessage response = await httpClient.PutAsync(
                String.Format(path), jsonContent);

            string? jsonResponse = await response.Content.ReadAsStringAsync();

            return (jsonResponse, response.StatusCode);

        }

        private string GetHash(string password)
        {
            // Create a SHA256
            SHA256 sha256Hash = SHA256.Create();

            // ComputeHash - returns byte array
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public interface IDoRequests
    {
        public Task<(string, HttpStatusCode)> DoPost<T>(T obj, string path, bool isAuth);
        public Task<(string, HttpStatusCode)> DoGet(string path, bool isAuth);
        public Task<(string, HttpStatusCode)> DoDelete(string obj, bool isAuth);
        public Task<(string, HttpStatusCode)> DoPut<T>(T obj, string path, bool isAuth);
    }
}
