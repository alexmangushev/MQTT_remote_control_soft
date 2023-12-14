using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mqtt_remote_server.Models;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;

namespace mqtt_remote_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly TelemetryContext _context;

        public DataController(TelemetryContext context)
        {
            _context = context;
        }

        // GET: api/Data/users_device
        // Self write
        [HttpGet("users_device")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Device>>> GetUsersDevice()
        {
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var claim = principal.Claims.FirstOrDefault();
                string login = claim.Value; // get user's login from jwt token

                // Get list of user's devices
                List<User> user = _context.Users.Where(r => r.Login == login).ToList();
                List<DeviceToUser> devtouser = _context.DeviceToUsers.Include(x => x.Device).Where(r => r.UserId == user[0].UserId).ToList();

                List<Device> Device = new ();
                foreach (var i in devtouser)
                {
                    Device.Add(i.Device);
                }
                //return list of user's devices
                return Device.ToImmutableList();
            }

            return NoContent();
        }

        // POST: api/Data/for_user
        // Self write
        [HttpPost("for_user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Datum>>> GetData(RequestDate date)
        {
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var claim = principal.Claims.FirstOrDefault();
                string login = claim.Value; // get user's login from jwt token

                // Get list of user's devices
                int? userid = _context.Users.Where(r => r.Login == login).ToList().FirstOrDefault().UserId;
                List<DeviceToUser> devtouser = _context.DeviceToUsers.Include(x => x.User).Where(r => r.UserId == userid).ToList();

                // return list of data beetween date.Start and date.End for all devices of this user
                return _context.Data.Where(r => r.GetTime > date.Start && r.GetTime < date.End && r.DeviceId == devtouser[0].DeviceId).ToImmutableList();

             }

            return NoContent();
        }

        // POST: api/Data/file
        // Self write
        [HttpPost("file")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Datum>>> GetFile(FileBody file)
        {
            // Send file to handler 
            // Do RPC to RabbitMQ
            using var rpcClient = new RpcClient("FTPQueue");
            string response = rpcClient.CallAsync(file);

            if (response == "Ok")
                return Ok();
            else
                return BadRequest();
        }

        // POST: api/Data/login
        // Self write
        // Get requests for authorization
        [HttpPost("login")] 
        public async Task<string> GetToken(UserAuth a)
        {
            string username = a.Login;
            string password = a.Password;

            IEnumerable<User> userReq = _context.Users.Where(r => r.Login == username); // find user with username
            List<User> userList = userReq.ToList();

            string? passwordHash = userList[0].Password;

            AuthAnswer answer = new AuthAnswer();

            if (passwordHash != null && passwordHash == password) // if password OK make token
            {
                if (userList[0].IsAdmin == true)
                {
                    answer.IsAdmin = true;
                    answer.IsAuth = true;
                }
                else
                {
                    answer.IsAdmin = false;
                    answer.IsAuth = true;
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

        // GET: api/Data
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Datum>>> GetData()
        {
            if (_context.Data == null)
            {
                return NotFound();
            }
            return await _context.Data.ToListAsync();
        }

        // GET: api/Data/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Datum>> GetDatum(int id)
        {
            if (_context.Data == null)
            {
                return NotFound();
            }
            var datum = await _context.Data.FindAsync(id);

            if (datum == null)
            {
                return NotFound();
            }

            return datum;
        }

        // PUT: api/Data/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutDatum(int id, Datum datum)
        {
            if (id != datum.Id)
            {
                return BadRequest();
            }

            _context.Entry(datum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DatumExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/

        // POST: api/Data
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Datum>> PostDatum(Datum datum)
        {
            if (_context.Data == null)
            {
                return Problem("Entity set 'TelemetryContext.Data'  is null.");
            }
            _context.Data.Add(datum);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDatum", new { id = datum.Id }, datum);
        }

        // DELETE: api/Data/5
        /*[HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDatum(int id)
        {
            if (_context.Data == null)
            {
                return NotFound();
            }
            var datum = await _context.Data.FindAsync(id);
            if (datum == null)
            {
                return NotFound();
            }

            _context.Data.Remove(datum);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/

        private bool DatumExists(int id)
        {
            return (_context.Data?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    // Class for getting information from user during data request
    public class RequestDate
    {
        public DateTime? Start { get; set; } = null!;

        public DateTime? End { get; set; } = null!;
    }

    // Class for sending to user jwt token and information about status of authorization
    public class AuthAnswer
    {
        public string? JWTtoken { get; set; } = null!;

        public bool IsAuth { get; set; } = false;

        public bool IsAdmin { get; set; } = false;
    }

    // Class for getting information from user during authorization
    public class UserAuth
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    // Class for getting file from user
    public class FileBody
    {
        public string? Data { get; set; } = null!;
    }
}
