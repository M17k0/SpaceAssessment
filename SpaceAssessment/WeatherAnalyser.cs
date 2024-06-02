using SpaceAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAssessment
{
    internal static class WeatherAnalyser
    {
        public static WheatherAnalysisResult MakeAnalysis(List<SpaceportForecast> spaceportsForecasts)
        {
            var maxWindSpeed = 11;
            var maxHumidity = 55;

            var spaceportsBestDays = spaceportsForecasts.Select(sf =>
            {
                var bestDay = sf.DayForecasts.Where(f => (f.Temperature >= 1 && f.Temperature <= 32)
                                                         && f.WindSpeed <= maxWindSpeed
                                                         && f.Humidity <= maxHumidity
                                                         && f.Precipitation == 0
                                                         && !f.HasLightnings
                                                         && (f.CloudsType != WeatherCloudsType.Cumulus
                                                             && f.CloudsType != WeatherCloudsType.Nimbus))
                                              .OrderBy(f => ((decimal)f.WindSpeed / maxWindSpeed + (decimal)f.Humidity / maxHumidity))
                                              .FirstOrDefault();
                return new
                {
                    Spaceport = sf,
                    DayForecast = bestDay
                };
            }).ToList();

            return new WheatherAnalysisResult
            {
                SpaceportBestDays = spaceportsBestDays.Select(x => new SpaceportBestDay
                {
                    LocationName = x.Spaceport.LocationName,
                    Day = x.DayForecast?.Day
                }).ToList(),
                BestSpaceport = spaceportsBestDays.Where(x => x.DayForecast != null)
                                                  .OrderBy(x => ((decimal)x.DayForecast!.WindSpeed / maxWindSpeed + (decimal)x.DayForecast.Humidity / maxHumidity))
                                                  .ThenBy(x => x.Spaceport.DistanceToEquator)
                                                  .Select(x => new SpaceportBestDay
                                                  {
                                                      LocationName = x.Spaceport.LocationName,
                                                      Day = x.DayForecast!.Day,
                                                  }).FirstOrDefault(),
            };
        }
    }
}
