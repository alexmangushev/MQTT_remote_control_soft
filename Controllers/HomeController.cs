using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mqtt_remote_server.Models;
using Newtonsoft.Json;
using NuGet.Common;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace mqtt_remote_server.Controllers
{
    [Route("/login")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ServerAuth serverAuth;
        public AuthController()
        {
            serverAuth = new();
        }

        [HttpPost]
        public async Task<string> GetToken(UserAuth a)
        {
            string username = a.Login;
            string password = a.Password;

            HttpClient httpClient = new()
            {
                BaseAddress = new Uri("http://localhost:5240/")
            };

            string token = await serverAuth.ReturnToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            using HttpResponseMessage response = await httpClient.GetAsync("api/Users");

            string? jsonResponse = await response.Content.ReadAsStringAsync();

            List<User> users = JsonConvert.DeserializeObject<List<User>>(jsonResponse);

            IEnumerable <User> userReq = users.Where(r => r.Login == username);
            List <User> userList = userReq.ToList();

            string? passwordHash = userList[0].Password;

            AuthAnswer answer = new AuthAnswer();

            if (passwordHash != null && passwordHash == password) 
            { 
                if (userList[0].IsAdmin == true)
                {
                    answer.IsAdmin = true;
                }
                else
                {
                    answer.IsAdmin = false;
                }

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };

                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)), 
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                answer.JWTtoken = new JwtSecurityTokenHandler().WriteToken(jwt);

            }

            string jsonContent = JsonConvert.SerializeObject(answer);
            return jsonContent;
        }
        /*
        // GET: HomeController
        public ActionResult Index()
        {
            return View();
        }

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
    }

    public class AuthAnswer
    {
        public string? JWTtoken { get; set; } = null!;

        public bool? IsAdmin { get; set; } = null!;
    }
}
