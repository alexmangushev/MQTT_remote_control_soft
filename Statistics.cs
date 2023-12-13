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

        public (List<double>, List<DateTime>) DataForPlot(string dataType)
        {
            List<double> values = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            foreach (var i in _listData)
            {
                dates.Add(i.GetTime);

                switch (dataType)
                {
                    case "Температура":
                        values.Add(i.Temp);
                        break;

                    case "Влажность":
                        values.Add(i.Humidity);
                        break;

                    case "Наличие питания":
                        values.Add(i.Power ? 1 : 0);
                        break;

                    case "Наличие людей":
                        values.Add(i.People ? 1 : 0);
                        break;

                    case "Наличие дыма":
                        values.Add(i.Smoke ? 1 : 0);
                        break;
                }
            }

            for (int i = 0; i < dates.Count() - 1; i++)
            {
                for (int k = 0; k < dates.Count() - 1; k++)
                {
                    
                    if (DateTime.Compare(dates[k], dates[k + 1]) > 0)
                    {
                        DateTime tmp = dates[k];
                        dates[k] = dates[k + 1];
                        dates[k + 1] = tmp;

                        double tmpD = values[k];
                        values[k] = values[k + 1];
                        values[k + 1] = tmpD;
                    }
                }
            }

            return (values, dates);
            
        }
    }
}
