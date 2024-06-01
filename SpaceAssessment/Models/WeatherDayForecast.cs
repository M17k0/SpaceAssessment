using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAssessment.Models
{
    internal class WeatherDayForecast
    {
        public int Day { get; set; }
        public int Temperature { get; set; }
        public int WindSpeed { get; set; }
        public int Humidity { get; set; }
        public int Precipitation { get; set; }
        public bool HasLightnings { get; set; }
        public WeatherCloudsType CloudsType { get; set; }
    }
}
