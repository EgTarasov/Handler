using System.Globalization;
using CsvHelper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using WebApplication1.Weather;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{

    private static WeatherEventRepository _controller;
    
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(ILogger<WeatherController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// return all data from db
    /// </summary>
    /// <returns></returns>
    [HttpGet("WeatherEventsData")]
    public List<WeatherEvent> GetDataFRomDb()
    {
        return _controller.GetInformationAboutEvents();
    }

    /// <summary>
    /// Create database according to csv file which lay in C:/WeatherEvent.csv
    /// </summary>
    [HttpPost("Initialise")]
    public ActionResult Initialisation(int quantity = 100)
    {
        _controller = new WeatherEventRepository("C:/WeatherEvent.csv", quantity);
        return Ok();
    }
}