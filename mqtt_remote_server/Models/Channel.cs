using System;
using System.Collections.Generic;

namespace mqtt_remote_server.Models;

public partial class Channel
{
    public int ChannelId { get; set; }

    public string Phone { get; set; } = null!;

    public string Operator { get; set; } = null!;

    public string Tariff { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
