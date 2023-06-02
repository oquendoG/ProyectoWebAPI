using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class WeatherForecastController : BaseApiController
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        _logger.LogInformation("Ejecutando weatherForecast");
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [Route("get/weather")]
    [Route("get/forecast")]
    [Route("[action]")]
    public IEnumerable<WeatherForecast> Get()
    {
        //throw new Exception("Excepción no controlada");

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
