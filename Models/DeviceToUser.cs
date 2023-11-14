using System;
using System.Collections.Generic;

namespace mqtt_remote_server.Models;

public partial class DeviceToUser
{
    public int UserId { get; set; }

    public int DeviceId { get; set; }

    public string? Info { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
