using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HW6.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace HW6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private HttpClient _hClient;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _hClient = new HttpClient();
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Weather(string cityName = null)
        {
            ViewBag.HaveData = false;
            if (cityName != null)
            {
                var url =
                $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid=719224426489343967c35e705deb38ca";
            var resString = await _hClient.GetStringAsync(url);
                var resObject = JsonConvert.DeserializeObject<RootObject>(resString);
                ViewBag.Temp = resObject.main.temp.ToString();
                ViewBag.HaveData = true;
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ??
            HttpContext.TraceIdentifier
            });
        }
    }

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
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
    }
    public class Wind
    {
        public double speed { get; set; }
        public int deg { get; set; }
    }
    public class Clouds
    {
        public int all { get; set; }
    }
    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public double message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }
    public class RootObject
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }
}
