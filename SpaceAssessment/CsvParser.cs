using SpaceAssessment.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAssessment
{
    internal static class CsvParser
    {
        public static List<SpaceportForecast> ReadSpaceportData(string folderPath, Dictionary<string, int> spaceportDistances)
        {
            var spaceports = new List<SpaceportForecast>();

            if (!Directory.Exists(folderPath))
                return new List<SpaceportForecast>();

            string[] fileNames;
            try
            {
                fileNames = Directory.GetFiles(folderPath);
            }
            catch (Exception)
            {
                return new List<SpaceportForecast>();
            }

            foreach (var spaceport in spaceportDistances)
            {
                var spaceportForecast = new SpaceportForecast
                {
                    LocationName = spaceport.Key,
                    DistanceToEquator = spaceport.Value
                };

                var fileToRead = fileNames.FirstOrDefault(f => f.EndsWith($"{spaceportForecast.LocationName}.csv", StringComparison.OrdinalIgnoreCase));
                if (fileToRead == null)
                    return new List<SpaceportForecast>();
        
                string[] lines;
                try
                {
                    lines = File.ReadAllLines(fileToRead);
                }
                catch (IOException)
                {
                    return new List<SpaceportForecast>();
                }

                var weatherDayForecasts = ParseForecastData(lines);
                if (weatherDayForecasts == null
                    || weatherDayForecasts.Count == 0)
                    return new List<SpaceportForecast>();

                spaceportForecast.DayForecasts = weatherDayForecasts;
                spaceports.Add(spaceportForecast);
            }

            return spaceports;
        }

        private static List<WeatherDayForecast>? ParseForecastData(string[] lines)
        {
            try
            {
                var weatherDayForecasts = new Dictionary<int, WeatherDayForecast>();

                for (int row = 0; row < lines.Length; row++)
                {
                    string line = lines[row];
                    var columns = line.Split(',');

                    // when on first row(header) build the forecast models with their day
                    if (row == 0)
                    {
                        for (int col = 1; col < columns.Length; col++)
                        {
                            if (!int.TryParse(columns[col], out int day))
                                return null;

                            weatherDayForecasts[col] = new WeatherDayForecast
                            {
                                Day = day
                            };
                        }
                    }
                    else // update weather forecast data for each day
                    {
                        for (int col = 1; col < columns.Length; col++)
                        {
                            if (columns[0].Contains("Temperature", StringComparison.OrdinalIgnoreCase))
                            {
                                weatherDayForecasts[col].Temperature = int.Parse(columns[col]);
                            }
                            else if (columns[0].Contains("Wind", StringComparison.OrdinalIgnoreCase))
                            {
                                weatherDayForecasts[col].WindSpeed = int.Parse(columns[col]);
                            }
                            else if (columns[0].Contains("Humidity", StringComparison.OrdinalIgnoreCase))
                            {
                                weatherDayForecasts[col].Humidity = int.Parse(columns[col]);
                            }
                            else if (columns[0].Contains("Precipitation", StringComparison.OrdinalIgnoreCase))
                            {
                                weatherDayForecasts[col].Precipitation = int.Parse(columns[col]);
                            }
                            else if (columns[0].Contains("Lightning", StringComparison.OrdinalIgnoreCase))
                            {
                                weatherDayForecasts[col].HasLightnings = "yes".Equals(columns[col], StringComparison.OrdinalIgnoreCase);
                            }
                            else if (columns[0].Contains("Clouds", StringComparison.OrdinalIgnoreCase))
                            {
                                if (Enum.TryParse<WeatherCloudsType>(columns[col], true, out var parsedType))
                                    weatherDayForecasts[col].CloudsType = parsedType;
                            }
                        }
                    }
                }
                return weatherDayForecasts.Values.OrderBy(x => x.Day).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
