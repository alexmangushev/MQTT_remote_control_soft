using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mqtt_remote_server.Controllers;
using mqtt_remote_server.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using Xunit;
namespace mqtt_remote_server_test
{
    public class UnitTest1
    {
        [Fact]
        public void DataControllerGetDataReturn1()
        {
            var options = new DbContextOptionsBuilder<TelemetryContext>()
                .UseInMemoryDatabase(databaseName: "UnitTests1")
                .Options;

            // Add one user, one device and one data log
            // Insert seed data into the database using one instance of the context
            using (var context = new TelemetryContext(options))
            {

                context.Users.Add(new User
                {
                    UserId = 1,
                    Login = "login",
                    Password = "password",
                    IsAdmin = true
                });

                context.DeviceToUsers.Add(new DeviceToUser
                {
                    UserId = 1,
                    DeviceId = 1
                });

                context.Data.Add(new Datum
                {
                    Id = 1,
                    Temp = 23,
                    Humidity = 23,
                    Power = true,
                    People = true,
                    Smoke = true,
                    GetTime = DateTime.Parse("2023-01-10"),
                    DeviceId = 1
                });

                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new TelemetryContext(options))
            {
                DataController dataController = new DataController(context);

                // Add login to claim in http context
                var response = new HttpResponseFeature();
                var features = new FeatureCollection();
                features.Set<IHttpResponseFeature>(response);
                dataController.ControllerContext.HttpContext = new DefaultHttpContext(features)
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "login") // set username
                    }))
                };

                // Do test
                Task<ActionResult<IEnumerable<Datum>>>? data_dirty = dataController.GetData(new RequestDate { Start = DateTime.Parse("2023-01-01"), End = DateTime.Now });
                List<Datum>? data = data_dirty.Result.Value.ToList();
                Assert.Equal(1, data.Count);
            }
        }

        [Fact]
        public void DataControllerloginTrue()
        {
            var options = new DbContextOptionsBuilder<TelemetryContext>()
                .UseInMemoryDatabase(databaseName: "UnitTests2")
                .Options;

            // Add one user
            // Insert seed data into the database using one instance of the context
            using (var context = new TelemetryContext(options))
            {
                context.Users.Add(new User
                {
                    UserId = 1,
                    Login = "login",
                    Password = "password",
                    IsAdmin = true
                });
                context.SaveChanges();
            }

            using (var context = new TelemetryContext(options))
            {
                DataController dataController = new DataController(context);

                // Do test
                Task<string> answer = dataController.GetToken(new UserAuth { Login = "login", Password = "password" });
                AuthAnswer ansObj = JsonConvert.DeserializeObject<AuthAnswer>(answer.Result);
                Assert.True(ansObj.IsAuth);
            }
        }

        [Fact]
        public void Get3UsersDevice()
        {
            var options = new DbContextOptionsBuilder<TelemetryContext>()
                .UseInMemoryDatabase(databaseName: "UnitTests3")
                .Options;

            // Add one user
            // Insert seed data into the database using one instance of the context
            using (var context = new TelemetryContext(options))
            {
                context.Users.Add(new User
                {
                    UserId = 1,
                    Login = "login",
                    Password = "password",
                    IsAdmin = true
                });

                context.Devices.Add(new Device
                {
                    DeviceId = 1,
                    Name = "Test1"
                });
                context.Devices.Add(new Device
                {
                    DeviceId = 2,
                    Name = "Test2"
                });
                context.Devices.Add(new Device
                {
                    DeviceId = 3,
                    Name = "Test3"
                });

                context.DeviceToUsers.Add(new DeviceToUser
                {
                    UserId = 1,
                    DeviceId = 1
                });
                context.DeviceToUsers.Add(new DeviceToUser
                {
                    UserId = 1,
                    DeviceId = 2
                });
                context.DeviceToUsers.Add(new DeviceToUser
                {
                    UserId = 1,
                    DeviceId = 3
                });

                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new TelemetryContext(options))
            {
                DataController dataController = new DataController(context);

                // Add login to claim in http context
                var response = new HttpResponseFeature();
                var features = new FeatureCollection();
                features.Set<IHttpResponseFeature>(response);
                dataController.ControllerContext.HttpContext = new DefaultHttpContext(features)
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "login") // set username
                    }))
                };

                // Do test
                Task<ActionResult<IEnumerable<Device>>>? data_dirty = dataController.GetUsersDevice();
                List<Device>? data = data_dirty.Result.Value.ToList();
                Assert.Equal(3, data.Count);
            }
        }
    }
}