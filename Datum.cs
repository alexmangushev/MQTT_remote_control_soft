using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mqtt_client
{
    public partial class Datum
    {
        public int Id { get; set; }

        public int Temp { get; set; }

        public int Humidity { get; set; }

        public bool Power { get; set; }

        public bool People { get; set; }

        public bool Smoke { get; set; }

        public DateTime GetTime { get; set; }

        public int? DeviceId { get; set; }
    }
}
