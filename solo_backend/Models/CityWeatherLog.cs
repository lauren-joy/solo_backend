using System;
using System.Collections.Generic;

#nullable disable

namespace LaurensProjects.Models1
{
    public partial class CityWeatherLog
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int? PlaneFrequency { get; set; }
        public decimal? WindSpeed { get; set; }
        public string Status { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime? GeneratedAt { get; set; }
    }
}