using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace mqtt_remote_server
{
    // When server processes MQTT transaction he need to use api to work with database
    // For it he need JWT token
    // This class need for control this process
    public class ServerAuth
    {
        static string serverToken;

        public async Task<string> ReturnToken() // return token
        {
            bool check = await CheckToken();
            if (check)
            {
                return serverToken;
            }
            else
            {
                GetToken();
                return serverToken;
            }

        }

        private async Task<bool> CheckToken() // Check token for valid
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri("http://localhost:5240/"),
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + serverToken);

            using HttpResponseMessage response = await httpClient.GetAsync("api/Users");

            return (response.StatusCode == HttpStatusCode.Unauthorized) ? false : true;

        }

        public async void GetToken() // Generate new token
        {
            serverToken = new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: new List<Claim> { new Claim(ClaimTypes.Name, "Server") },
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256))
            );
        }
    }
}