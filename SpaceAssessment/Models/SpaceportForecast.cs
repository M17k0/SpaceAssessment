using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAssessment.Models
{
    internal class SpaceportForecast
    {
        public string LocationName { get; set; } = "";
        public int DistanceToEquator { get; set; }
        public List<WeatherDayForecast> DayForecasts { get; set; } = new List<WeatherDayForecast>();
    }
}
