using System;
using System.Collections.Generic;

namespace mqtt_remote_server.Models;

public partial class Device
{
    public int DeviceId { get; set; }

    public string Name { get; set; } = null!;

    public int? ChannelId { get; set; }

    public virtual Channel? Channel { get; set; }

    public virtual ICollection<Datum> Data { get; set; } = new List<Datum>();

    public virtual ICollection<DeviceToUser> DeviceToUsers { get; set; } = new List<DeviceToUser>();
}
