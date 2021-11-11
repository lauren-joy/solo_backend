using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaurensProjects.Controllers.DTO
{
    public class GrabCityDTO
    {
        public string latlong { get; set; }
        public int id { get; set; }
        public int airportid { get; set; }
        public string name { get; set; }
    }
}