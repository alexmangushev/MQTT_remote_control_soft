using System;
using System.Collections.Generic;

namespace mqtt_remote_server.Models;

public partial class User
{
    public int? UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public virtual ICollection<DeviceToUser> DeviceToUsers { get; set; } = new List<DeviceToUser>();
}
