using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mqtt_client
{
    public class UserAuth
    {
        public UserAuth()
        {
            Login = "admin";
            Password = "password";
        }

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
