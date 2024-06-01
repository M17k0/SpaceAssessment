using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAssessment.Models
{
    internal class WheatherAnalysisResult
    {
        public List<SpaceportBestDay> SpaceportBestDays { get; set; }
        public SpaceportBestDay? BestSpaceport { get; set; }
    }
}
