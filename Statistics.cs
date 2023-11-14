using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mqtt_client
{
    public class Statistics
    {
        public List<Datum> _listData;


        public double GetAvarageTemp()
        {
            return _listData.Average(r => r.Temp);
        }

        public double GetAvarageHumidity()
        {
            return _listData.Average(r => r.Humidity);
        }

        public double GetMinTemp()
        {
            return _listData.Min(r => r.Temp);
        }

        public double GetMinHumidity()
        {
            return _listData.Min(r => r.Humidity);
        }

        public double GetMaxTemp()
        {
            return _listData.Max(r => r.Temp);
        }

        public double GetMaxHumidity()
        {
            return _listData.Max(r => r.Humidity);
        }

    }
}
