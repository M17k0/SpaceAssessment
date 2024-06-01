using SpaceAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAssessment
{
    internal static class CsvWriter
    {
        public static string? WriteToCsv(WheatherAnalysisResult analysisResult)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Spaceport, Best day");
            foreach (var spaceportBestDay in analysisResult.SpaceportBestDays)
            {
                csv.AppendLine($"\"{spaceportBestDay.LocationName}\", {spaceportBestDay.Day}");
            }
            var fileName = "LaunchAnalysisReport.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            try
            {
                File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);

                return filePath;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
