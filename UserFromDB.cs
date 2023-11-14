using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mqtt_client
{
    public class UserFromDB
    {
        public UserFromDB() {
            UserId = 10000;
            Login = "user";
            Password = "changeme";
            IsAdmin = false;
        }

        public int? UserId { get; set; } = null;
        
        public string Login { get; set; } = null!;
        
        public string Password { get; set; } = null!;

        public bool? IsAdmin { get; set; } = null;
        
    }
}




