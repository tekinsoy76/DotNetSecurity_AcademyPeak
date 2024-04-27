using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademyPeak_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet(Name = "GetWeatherForecast")]
        [Authorize(Policy = "Yetkili")]
        public IEnumerable<WeatherForecast> Get()
        {
            List<WeatherForecast> list = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-19, 40) //
            })
            .ToList();
            list.ForEach(t => t.Summary = Summaries[(t.TemperatureC + 20) / 6]);// burada 20 ile toplamamizin sebebi index yapisina uydurmak
            return list;
        }
    }
}
