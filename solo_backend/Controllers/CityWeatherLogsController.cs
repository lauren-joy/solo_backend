using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaurensProjects.Models1;
using LaurensProjects.Controllers.DTO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text;
using System.Json;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using LaurensProjects.Models1;

namespace LaurensProjects.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityWeatherLogsController : ControllerBase
    {
        private readonly airportdbContext _context;

        public CityWeatherLogsController()
        {
            airportdbContext context = new airportdbContext();
            _context = context;
        }

        // GET: api/CityWeatherLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWeatherLog>>> GetCityWeatherLogs()
        {
            return await _context.CityWeatherLogs.ToListAsync();
        }

        // GET: api/CityWeatherLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CityWeatherLog>> GetCityWeatherLog(int id)
        {
            var cityWeatherLog = await _context.CityWeatherLogs.FindAsync(id);

            if (cityWeatherLog == null)
            {
                return NotFound();
            }

            return cityWeatherLog;
        }
        public class paramatersPassFromId
        {
            public int id { get; set; }
            public string appid { get; set; }
        }
        public class paramatersPassFromName
        {
            public string name { get; set; }
            public string appid { get; set; }
        }
        [HttpGet]
        [Route("GetAirportNames")]
        public async Task<ActionResult> OnPageLoadAirPortNames()
        {
            try
            {
                string path = "airports.csv";
                FileStream uploadFileStream = System.IO.File.OpenRead(path);

                var results = System.IO.File.ReadAllLines(path);

                var result = System.IO.File.ReadAllLines("airports.csv")
                              .Select(line => line.Split(','))
                              .Select(x => new AirportDTO
                              {
                                  id = x[0],
                                  ident = (x[1].Replace(@"\", "").Replace(@"/", "")).Replace("\"", ""),
                                  type = x[2],

                                  name = (x[3].Replace(@"\", "").Replace(@"/", "")).Replace("\"", ""), // removing unecessary characters
                                  latitude_deg = x[4],
                                  longitude_deg = x[5],
                              })
                              .ToArray();
                return Ok(result);

            }
            catch (Exception ex)
            {
                Conflict("the exception is " + ex);
            }



            return Ok("no return");
        }
        [HttpGet]
        [Route("GetAirportRunways")]
        public async Task<ActionResult> OnPageLoadAirPortRunways()
        {
            try
            {
                string path = "runways.csv";
                FileStream uploadFileStream = System.IO.File.OpenRead(path);


                var results = System.IO.File.ReadAllLines(path)
                    .Select(line => line.Split(','))
                    .Select(x => new RunwaysDTO
                    {
                        id = x[0],
                        airport_ref = x[1],
                        length_ft = x[4],
                        width_ft = x[5],
                        le_ident = x[9],
                        le_latitude_deg = x[10],
                        le_longitude_deg = x[11],
                        he_ident = x[16],
                        he_latitude_deg = x[17],
                        he_longitude_deg = x[18]


                    }).ToArray();
                return Ok(results);
            }
            catch (Exception ex)
            {
                Conflict("the issue is" + ex);
            }

            return Ok(1);

        }






        /*
         * 
         * on page load method to retrieve all the key value pairs of id and names 
         * have a drop down list that maps the two together in react
         * whenever we use the drop downlist  2988507.. Paris
         *                              
         * 
         * **/
        /***
         * When ever we Get the data from drop down list and then calls the api to the weather service
         * and gives us information to show status, wind, names and map them to the database**/
        [HttpPost]
        [Route("PassFromSearchMenu")]
        public async Task<ActionResult> FindAirportInformation([FromBody] Root x)
        {
            if (x.id != null)
            {
                HttpClient client = new HttpClient();


                var url = "http://api.openweathermap.org/data/2.5/weather";

                paramatersPassFromId passParamaters = new paramatersPassFromId();
                passParamaters.id = x.id;
                passParamaters.appid = "ae5e42da1282ca3833870ec8216e25df";
                var json = JsonSerializer.Serialize(passParamaters);
                var uri = "http://api.openweathermap.org/data/2.5/weather";
                string urlParameters = $"?q={x.city}&appid=6c6ae12dffb287467b35e194927944ef";
                var stringcontent = new StringContent(json, Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseMessage = client.GetAsync(urlParameters).Result;

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                string replacedCharacter = @"\";
                string parsedResponse = " ";
                var responseReplace = responseBody.Replace(replacedCharacter, parsedResponse);
                var jsonObj = JsonSerializer.Deserialize<Object>(responseBody);

                try
                {
                    if (responseMessage.IsSuccessStatusCode)
                    {

                        // Parse the response body.

                        CityWeatherLog cityInstance = new CityWeatherLog();
                        cityInstance = (CityWeatherLog)jsonObj;

                        _context.CityWeatherLogs.Add(cityInstance);

                        return Ok(cityInstance);

                    }
                    else
                    {
                        Conflict("response was not obtained");
                    }


                }
                catch (Exception ex)
                {
                    Conflict("issue is" + ex);
                }


                CityWeatherLog weatherLog = new CityWeatherLog();


                _context.CityWeatherLogs.Add(weatherLog);


                return Ok(responseMessage.Content);


            }
            else if (x.name != null)
            {

            }
            return Ok(0);
        }


        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult> GetDataFromDB()
        {

            var contents = await (
                from WeatherLog in _context.CityWeatherLogs
                select WeatherLog).ToListAsync();
            return Ok(contents);
        }
        class City
        {
            public string CityName { get; set; }
        }
        [HttpPost]
        [Route("GetCityName")]
        public async Task<ActionResult> GetCityFromLonLat([FromBody] GrabCityDTO instance)
        {
            if (instance.latlong != null)
            {
                HttpClient client = new HttpClient();
                var url = "https://maps.googleapis.com/maps/api/geocode/json";
                paramatersPassFromId passParamaters = new paramatersPassFromId();
                passParamaters.appid = "AIzaSyAol82SlkS90aK6Jgllo_Uzo4HavbQUzdQ";
                var json = JsonSerializer.Serialize(passParamaters);
                string latlon = instance.latlong;
                string urlParameters = $"?latlng={latlon}&key=AIzaSyAol82SlkS90aK6Jgllo_Uzo4HavbQUzdQ";
                var stringcontent = new StringContent(json, Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseMessage = client.GetAsync(urlParameters).Result;

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                string replacedCharacter = @"\";
                string parsedResponse = " ";
                var responseReplace = responseBody.Replace(replacedCharacter, parsedResponse);
                RootGoogleDecode jsonObj = JsonSerializer.Deserialize<RootGoogleDecode>(responseBody);
                try
                {
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        AddressComponent addr = new AddressComponent();
                        List<Result> res = new List<Result>();
                        res = jsonObj.results;

                        string city = res[0].address_components[3].short_name;
                        City newCity = new City();
                        newCity.CityName = city;
                        return Ok(city);
                    }
                    else
                    {
                        Conflict("issue has occured");

                    }
                }
                catch (Exception ex)
                {
                    Conflict("Exception has occured" + ex);
                }





                return Ok();
            }
            else
            {
                return Conflict("issue is code");
            }
        }

        public class WeatherPrediction
        {
            public string weatherstatus { get; set; }
            public double temp { get; set; }
            public string dt_txt { get; set; }

        }
        public class ListOfWeatherPredictions
        {
            public List<WeatherPrediction> list { get; set; }
            public string GoodDayForPicnic { get; set; }
        }



        [HttpPost]
        [Route("CalculateWeatherPredictions")]
        public async Task<ActionResult> CalculateWindPredictions([FromBody] Root instance)
        {
            try
            {
                //look at the data 
                var previousDates = _context.CityWeatherLogs.OrderByDescending(p => p.CityId).Take(5);

                //  api.openweathermap.org/data/2.5/forecast?q=Houston&appid=6c6ae12dffb287467b35e194927944ef

                HttpClient client = new HttpClient();
                var url = "http://api.openweathermap.org/data/2.5/weather";

                paramatersPassFromId passParamaters = new paramatersPassFromId();
                passParamaters.id = Convert.ToInt32(instance.airportid);
                passParamaters.appid = "ae5e42da1282ca3833870ec8216e25df";
                var json = JsonSerializer.Serialize(passParamaters);
                var uri = "http://api.openweathermap.org/data/2.5/forecast";
                string urlParameters = $"?q={instance.name}&appid=6c6ae12dffb287467b35e194927944ef";
                var stringcontent = new StringContent(json, Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseMessage = client.GetAsync(urlParameters).Result;

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                string replacedCharacter = @"\";
                string parsedResponse = " ";
                var responseReplace = responseBody.Replace(replacedCharacter, parsedResponse);
                RootForecast jsonObj = JsonSerializer.Deserialize<RootForecast>(responseBody);
                RootForecast instanceObj = new RootForecast();
                string weatherStatus = "";
                int incrementer = 8;
                int counter = 0;
                //5 
                List<CityWeatherLog> city = new List<CityWeatherLog>();
                ListOfWeatherPredictions i = new ListOfWeatherPredictions();
                WeatherPrediction w = new WeatherPrediction();
                int instanceCounter = 1;
                int eightvalue = 8;
                List<WeatherPrediction> weatherPrediction = new List<WeatherPrediction>();

                //add wind speed to algorithm 
                int totalCount = jsonObj.list.Count();
                for (int j = 0; j < totalCount; j++)
                {
                    counter = j;

                    if (counter + 1 == eightvalue || counter == 0)
                    {
                        var CityId = _context.CityWeatherLogs.OrderByDescending(p => p.CityId).Take(1);
                        var cityContainer = CityId.ToList();
                        //primary key is set manually 
                        CityWeatherLog instanceCity = new CityWeatherLog();
                        if (cityContainer.Count == 0)
                        {

                            Random r = new Random();
                            instanceCity.CityId = r.Next();

                        }
                        else
                        {
                            instanceCity.CityId = cityContainer[0].CityId + 1;
                        }
                        instanceCity.CityName = instance.name;
                        instanceCity.FromDate = Convert.ToDateTime(jsonObj.list[counter].dt_txt);
                        instanceCity.ToDate = Convert.ToDateTime(jsonObj.list[counter].dt_txt);

                        instanceCity.WindSpeed = Convert.ToDecimal(jsonObj.list[counter].wind.gust);
                        instanceCity.PlaneFrequency = Convert.ToInt32(jsonObj.list[counter].wind.speed);

                        if (jsonObj.list[counter].wind.speed > 0)
                        {
                            //the real reason flights from west to east are quicker is down to jet streams.

                            if (jsonObj.list[counter].wind.deg > 240 & jsonObj.list[counter].wind.deg <= 360)
                            {
                                if (jsonObj.list[counter].wind.gust > 3)
                                {
                                    weatherStatus = "Moderate";
                                }
                                else
                                {
                                    weatherStatus = "Low";

                                }
                            }
                            if (jsonObj.list[counter].wind.deg > 45 & jsonObj.list[counter].wind.deg <= 126)
                            {

                                weatherStatus = "High";


                            }
                            if (jsonObj.list[counter].wind.deg >= 0 & jsonObj.list[counter].wind.deg < 45)
                            {
                                if (jsonObj.list[counter].wind.deg < 3)
                                {
                                    weatherStatus = "Moderate";
                                }
                                else
                                {
                                    weatherStatus = "High";
                                }
                            }
                            if (jsonObj.list[counter].wind.deg > 126 & jsonObj.list[counter].wind.deg < 200)
                            {
                                if (jsonObj.list[counter].wind.gust < 3)
                                    weatherStatus = "Moderate";
                                else
                                {
                                    weatherStatus = "High";
                                }
                            }
                            if (weatherStatus == "Low")
                            {
                                i.GoodDayForPicnic = "Great day for a picnic tomorrow";
                            }
                            if (weatherStatus == "Moderate")
                            {
                                i.GoodDayForPicnic = "Will be an alright day for a picnic tomorrow";
                            }
                            if (weatherStatus == "High")
                            {
                                i.GoodDayForPicnic = "Wouldn't advise for a picnic tomorrow";
                            }

                            jsonObj.list[counter].weatherstatus = weatherStatus;
                            instanceCity.Status = weatherStatus;
                            DateTime time = Convert.ToDateTime(jsonObj.list[counter].dt_txt);
                            weatherPrediction.Add(new WeatherPrediction
                            {

                                weatherstatus = weatherStatus,
                                temp = jsonObj.list[counter].main.temp,
                                dt_txt = time.ToString("yyyy-MM-dd"),


                            });
                            i.list = weatherPrediction;
                            try
                            {
                                city.Add(instanceCity);
                                instanceCounter += 1;

                                eightvalue = instanceCounter * 7;
                                await _context.CityWeatherLogs.AddAsync(instanceCity);
                                await _context.SaveChangesAsync();

                            }
                            catch (Exception e)
                            {
                                Conflict("The exception is" + e);
                            }

                        }

                    }


                }


                return Ok(i);


            }

            catch (Exception e)
            {
                Conflict("the issue is" + e);
            }





            return Ok();
        }
        [HttpPost]
        [Route("PredictTraffic")]
        public async Task<ActionResult> GetWindDirection([FromBody] Root instance)
        {
            string weatherStatus = "";

            for (int i = 0; i < instance.runways.Count; i++)
            {
                if (instance.runways[i].he_latitude_deg != "" && instance.runways[i].le_latitude_deg != "")
                {
                    double lelat = Convert.ToDouble(instance.runways[i].le_latitude_deg);
                    double lelon = Convert.ToDouble(instance.runways[i].le_longitude_deg);
                    double helat = Convert.ToDouble(instance.runways[i].he_latitude_deg);
                    double helon = Convert.ToDouble(instance.runways[i].he_longitude_deg);
                    double x = Math.Sin(Convert.ToDouble(instance.runways[i].he_latitude_deg) * (helon - lelon));

                    double y = Math.Cos(helat) * Math.Sin(lelat) - Math.Sin(helat) * Math.Cos(lelat) * Math.Cos(helon - lelon);
                    // x= sin(longitude) * cos(latittude) 
                    // y = cos θa * sin θb – sin θa * cos θb * cos ∆L   
                    double angle = Math.Atan2(x, y);
                    double degrees = angle * Math.PI / 180;
                    int directionofRunway = 0;
                    int weatherDirection = 0;
                    if (degrees == 0 && degrees < 90)
                    {
                        directionofRunway = 1;
                    }
                    if (degrees >= 90 && degrees < 180)
                    {
                        directionofRunway = 2;
                    }
                    if (degrees >= 180 && degrees < 270)
                    {
                        directionofRunway = 3;
                    }
                    if (degrees >= 270 && degrees < 360)
                    {
                        directionofRunway = 4;
                    }

                    if (instance.wind.deg == 0 && instance.wind.deg < 90)
                    {
                        weatherDirection = 1;
                    }
                    if (instance.wind.deg >= 90 && instance.wind.deg < 180)
                    {
                        weatherDirection = 2;
                    }
                    if (instance.wind.deg >= 180 && instance.wind.deg < 270)
                    {
                        weatherDirection = 3;
                    }
                    if (instance.wind.deg >= 270 && instance.wind.deg < 360)
                    {
                        weatherDirection = 4;
                    }

                    if (weatherDirection == directionofRunway)
                    {
                        weatherStatus = "Low";

                    }

                    else
                    {
                        if (directionofRunway - 1 == weatherDirection)
                        {
                            weatherStatus = "Moderate";
                        }
                        if (directionofRunway - 2 == weatherDirection)
                        {
                            weatherStatus = "High";
                        }
                        if (directionofRunway - 3 == weatherDirection)
                        {
                            weatherStatus = "Moderate";
                        }


                        CityWeatherLog cityInstance = new CityWeatherLog();
                        cityInstance.CityId = instance.id;
                        cityInstance.CityName = instance.name;
                        cityInstance.Status = weatherStatus;

                        Wind wind = new Wind();
                        wind.speed = instance.wind.speed;
                        wind.gust = instance.wind.gust;
                        Decimal xs = 23.885m;

                        Main main = new Main();
                        main.temp = instance.main.temp;
                        main.pressure = instance.main.pressure;
                        main.humidity = instance.main.humidity;
                        cityInstance.WindSpeed = xs;
                        cityInstance.FromDate = DateTime.Now;
                        cityInstance.ToDate = DateTime.Now;

                        try
                        {
                            await _context.CityWeatherLogs.AddAsync(cityInstance);

                        }
                        catch (Exception e)
                        {
                            Conflict("issue is" + e);
                        }

                        //degree 140 

                        return Ok(weatherStatus);
                    }


                }
                if (instance.wind.deg >= 0)
                {
                    //The real reason flights from west to east are quicker is down to jet streams.

                    if (instance.wind.deg > 240 & instance.wind.deg <= 360)
                    {
                        weatherStatus = "Low";
                        if (instance.wind.gust > 3)
                        {
                            weatherStatus = "Moderate";
                        }
                    }
                    if (instance.wind.deg > 45 & instance.wind.deg <= 126)
                    {

                        weatherStatus = "High";


                    }
                    if (instance.wind.deg >= 0 & instance.wind.deg < 45)
                    {
                        if (instance.wind.deg < 3)
                        {
                            weatherStatus = "Moderate";
                        }
                        else
                        {
                            weatherStatus = "High";
                        }
                    }
                    if (instance.wind.deg > 126 & instance.wind.deg < 200)
                    {
                        if (instance.wind.gust < 3)
                            weatherStatus = "Moderate";
                        else
                        {
                            weatherStatus = "High";
                        }
                    }


                    CityWeatherLog cityInstance = new CityWeatherLog();
                    cityInstance.CityId = instance.id;
                    cityInstance.CityName = instance.name;
                    cityInstance.Status = weatherStatus;

                    Wind wind = new Wind();
                    wind.speed = instance.wind.speed;
                    wind.gust = instance.wind.gust;
                    Decimal xs = 23.885m;

                    Main main = new Main();
                    main.temp = instance.main.temp;
                    main.pressure = instance.main.pressure;
                    main.humidity = instance.main.humidity;
                    cityInstance.WindSpeed = xs;
                    cityInstance.FromDate = DateTime.Now;
                    cityInstance.ToDate = DateTime.Now;

                    try
                    {
                        await _context.CityWeatherLogs.AddAsync(cityInstance);
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception e)
                    {
                        Conflict("issue is" + e);
                    }

                    //degree 140 

                    return Ok(weatherStatus);

                }
                else
                {

                    CityWeatherLog cityInstance = new CityWeatherLog();
                    cityInstance.CityId = instance.id;
                    cityInstance.CityName = instance.name;
                    cityInstance.Status = weatherStatus;

                    Wind wind = new Wind();
                    wind.speed = instance.wind.speed;
                    wind.gust = instance.wind.gust;
                    Decimal xs = 23.885m;

                    Main main = new Main();
                    main.temp = instance.main.temp;
                    main.pressure = instance.main.pressure;
                    main.humidity = instance.main.humidity;
                    cityInstance.WindSpeed = xs;
                    cityInstance.FromDate = DateTime.Now;
                    cityInstance.ToDate = DateTime.Now;

                    try
                    {
                        await _context.CityWeatherLogs.AddAsync(cityInstance);

                        await _context.SaveChangesAsync();

                    }
                    catch (Exception e)
                    {
                        Conflict("issue is" + e);
                    }

                    //degree 140 

                    return Ok(weatherStatus);
                }
            }
            object obj = new object();
            return Ok(obj);
        }
        [HttpPost]
        [Route("PassFromDropDownList")]
        public async Task<ActionResult> MapData([FromBody] PassFromDDLDTO instance)

        {
            if (instance.airportid != null)
            {
                //check for the date
                HttpClient client = new HttpClient();
                var url = "http://api.openweathermap.org/data/2.5/weather";

                paramatersPassFromId passParamaters = new paramatersPassFromId();
                passParamaters.id = Convert.ToInt32(instance.airportid);
                passParamaters.appid = "ae5e42da1282ca3833870ec8216e25df";
                var json = JsonSerializer.Serialize(passParamaters);
                var uri = "http://api.openweathermap.org/data/2.5/weather";
                string urlParameters = $"?q={instance.city}&appid=6c6ae12dffb287467b35e194927944ef";
                var stringcontent = new StringContent(json, Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseMessage = client.GetAsync(urlParameters).Result;

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                string replacedCharacter = @"\";
                string parsedResponse = " ";
                var responseReplace = responseBody.Replace(replacedCharacter, parsedResponse);
                Root jsonObj = JsonSerializer.Deserialize<Root>(responseBody);

                try
                {
                    if (responseMessage.IsSuccessStatusCode)
                    {

                        // Parse the response body.


                        var CityId = _context.CityWeatherLogs.OrderByDescending(p => p.CityId).Take(1);




                        var cityContainer = CityId.ToList();


                        CityWeatherLog cityInstance = new CityWeatherLog();
                        cityInstance.Status = " ";
                        if (CityId.Count() == 0)
                        {
                            Random r = new Random();
                            cityInstance.CityId = r.Next();


                        }
                        else
                        {
                            cityInstance.CityId = Convert.ToInt32(cityContainer[0].CityId + 1);

                        }
                        cityInstance.CityName = instance.city;
                        Wind wind = new Wind();
                        wind.speed = jsonObj.wind.speed;
                        wind.gust = jsonObj.wind.gust;
                        Decimal x = 23.885m;
                        Main main = new Main();
                        main.temp = jsonObj.main.temp;
                        main.pressure = jsonObj.main.pressure;
                        main.humidity = jsonObj.main.humidity;
                        jsonObj.airportid = instance.airportid;
                        cityInstance.WindSpeed = x;
                        cityInstance.FromDate = DateTime.Now;
                        cityInstance.ToDate = DateTime.Now;
                        try
                        {
                            string path = "runways.csv";
                            FileStream uploadFileStream = System.IO.File.OpenRead(path);


                            var results = System.IO.File.ReadAllLines(path)
                                .Select(line => line.Split(','))

                                .Select(x => new RunwaysDTO
                                {
                                    id = x[0],
                                    airport_ref = x[1],
                                    length_ft = x[4],
                                    width_ft = x[5],
                                    le_ident = x[9],
                                    le_latitude_deg = x[10],
                                    le_longitude_deg = x[11],
                                    he_ident = x[16],
                                    he_latitude_deg = x[17],
                                    he_longitude_deg = x[18]


                                }).Where(p => p.airport_ref == instance.airportid).ToList();


                            Root r = jsonObj;

                            r.runways = results;
                            try
                            {
                                _context.CityWeatherLogs.Add(cityInstance);
                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                Conflict("The exception is " + ex);
                            }
                            return Ok(r);

                        }
                        catch (Exception ex)
                        {
                            Conflict("the issue is" + ex);
                        }
                        if (wind.speed > 10)
                        {
                            cityInstance.Status = "Bad";

                        }
                        else
                        {
                            cityInstance.Status = "Good";
                        }
                        try
                        {
                            _context.CityWeatherLogs.Add(cityInstance);

                            return Ok(jsonObj);
                        }
                        catch (Exception ex)
                        {
                            Conflict("The exception is " + ex);
                        }

                        return Ok(jsonObj);

                    }
                    else
                    {
                        Conflict("The response was not recieved");
                    }


                }
                catch (Exception ex)
                {
                    Conflict("issue is" + ex);
                }


                CityWeatherLog weatherLog = new CityWeatherLog();


                _context.CityWeatherLogs.Add(weatherLog);


                return Ok(responseMessage.Content);

            }

            else
            { // hit other endpoint api.openweathermap.org/data/2.5/weather?q={city name},{state code},{country code}&appid={API key}
                if (instance.city != null) // city name is exist 
                {
                    if (instance.airportid != null)
                    {
                        var url = "http://api.openweathermap.org/data/2.5/weather?";
                        HttpClient client = new HttpClient();
                        paramatersPassFromName passParamaters = new paramatersPassFromName();
                        passParamaters.name = instance.city;
                        passParamaters.appid = "6c6ae12dffb287467b35e194927944ef";
                        var json = JsonSerializer.Serialize(passParamaters);
                        var uri = "http://api.openweathermap.org/data/2.5/weather";


                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(uri, content).ConfigureAwait(false);

                    }

                }
            }
            return Ok(1);
        }






        // PUT: api/CityWeatherLogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCityWeatherLog(int id, CityWeatherLog cityWeatherLog)
        {
            if (id != cityWeatherLog.CityId)
            {
                return BadRequest();
            }

            _context.Entry(cityWeatherLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityWeatherLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CityWeatherLogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CityWeatherLog>> PostCityWeatherLog(CityWeatherLog cityWeatherLog)
        {
            _context.CityWeatherLogs.Add(cityWeatherLog);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }

            return CreatedAtAction("GetCityWeatherLog", new { id = cityWeatherLog.CityId }, cityWeatherLog);
        }

        // DELETE: api/CityWeatherLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCityWeatherLog(int id)
        {
            var cityWeatherLog = await _context.CityWeatherLogs.FindAsync(id);
            if (cityWeatherLog == null)
            {
                return NotFound();
            }

            _context.CityWeatherLogs.Remove(cityWeatherLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityWeatherLogExists(int id)
        {
            return _context.CityWeatherLogs.Any(e => e.CityId == id);
        }
    }
}