using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mqtt_client
{
    public class RequestData
    {
        public DateTime? Start { get; set; } = null!;

        public DateTime? End { get; set; } = null!;
    }
}
