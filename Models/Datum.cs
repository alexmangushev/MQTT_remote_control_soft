using System;
using System.Collections.Generic;

namespace mqtt_remote_server.Models;

public partial class Datum
{
    public int? Id { get; set; }

    public int Temp { get; set; }

    public int Humidity { get; set; }

    public bool Power { get; set; }

    public bool People { get; set; }

    public bool Smoke { get; set; }

    public DateTime? GetTime { get; set; }

    public int DeviceId { get; set; }

    //public virtual Device Device { get; set; } = null!;
}
