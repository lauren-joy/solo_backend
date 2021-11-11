using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaurensProjects.Controllers.DTO
{
    // main page object passed 

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double feels_like { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public int deg { get; set; }
        public double gust { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }
    public class RunwaysDTO // data transfer object  
                            //csv file -> to C# object 
    {
        public String id { get; set; } //0
        public string Status { get; set; }
        public string airport_ref { get; set; } //1 air port to one run  1:M
        public string airport_ident { get; set; } //

        public string length_ft { get; set; }

        public string width_ft { get; set; }

        public string surface { get; set; }
        public string lighted { get; set; }

        public int closed { get; set; }
        public string le_ident { get; set; }


        public string? le_latitude_deg { get; set; }

        public string? le_longitude_deg { get; set; }

        public string he_ident { get; set; } //16
        public string he_latitude_deg { get; set; }

        public string he_longitude_deg { get; set; }

    }
    public class Root
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public string airportid { get; set; }

        public List<RunwaysDTO> runways { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
        public string city { get; set; }
    }


}