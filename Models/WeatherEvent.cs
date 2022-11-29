using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace WebApplication1.Weather;


public class WeatherEvent
{
    public static readonly Dictionary<string, string> Headers = new Dictionary<string, string>(){
        {"EventId" , "INTEGER"},
        {"TypeID", "INTEGER"},
        {"SeverityID","INTEGER"},
        {"StartTimeUTC","TEXT"},
        {"EndTimeUTC","TEXT"},
        {"Precipitation","TEXT"},
        {"TimeZone","TEXT"},
        {"AirportCode","TEXT"},
        {"LocationLat","REAL"},
        {"LocationLng","REAL"},
        {"City","TEXT"},
        {"County","TEXT"},
        {"State", "TEXT"},
        {"ZipCode","TEXT"},
    };

    public string EventId { get; set; }

    public string Type { get; set; }

    public string Severity { get; set; }

    [Name("StartTime(UTC)")] public string StartTimeUTC { get; set; }

    [Name("EndTime(UTC)")] public string EndTimeUTC { get; set; }

    [Name("Precipitation(in)")] public double Precipitation { get; set; }

    public string TimeZone { get; set; }

    public string AirportCode { get; set; }

    public double LocationLat { get; set; }

    public double LocationLng { get; set; }

    public string City { get; set; }

    public string County { get; set; }

    public string State { get; set; }

    public string ZipCode { get; set; }

    /// <summary>
    /// Represent JSON serialized info
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}