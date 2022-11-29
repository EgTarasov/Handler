using System.Data;
using System.Globalization;
using CsvHelper;
using Dapper;
using Microsoft.Data.Sqlite;
using WebApplication1.Weather;
namespace WebApplication1;

public class WeatherEventRepository
{

    private List<WeatherEvent>? _list;
    private const string ConnectionString = $"Data Source = WeatherInfo.db";
    public bool IsConnected { get; private set; } = false;

    /// <summary>
    /// Open csv file or throw exception if cant open csv file
    /// </summary>
    /// <param name="path"></param>
    public WeatherEventRepository(string path, int quantity)
    {
        CreateDb(path, quantity);
    }

/// <summary>
/// Show all data in WeatherEvents table
/// </summary>
/// <returns></returns>
    public List<WeatherEvent>? GetInformationAboutEvents()
    {
        if (IsConnected is false)
            return new List<WeatherEvent>();
        using var conn = new SqliteConnection(ConnectionString);
        var response = conn.Query<WeatherEvent>("Select * from WeatherEvents").ToList();
        return response;
    }
    
    /// <summary>
    /// Pu info from csv file in the list
    /// </summary>
    /// <param name="path">path to csv file</param>
    /// <exception cref="ArgumentException">File not found</exception>
    public  void ReadCsv(string path)
    {
        try
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            _list = 
                 csv
                    .GetRecords<WeatherEvent>()
                    .ToList();
        }
        catch(Exception)
        {
            throw new ArgumentException("File not found");
        }
    }
    

    /// <summary>
    /// Fill table with data from csv file
    /// </summary>
    /// <param name="path">path to csv file</param>
    /// <param name="quantity">amount of rows to be read</param>
    private void FillTable(string path, long quantity = 100)
    {
        if (_list is null || _list.Count == 0)
        {
            ReadCsv(path);
        }
        using var conn = new SqliteConnection(ConnectionString);
        var counter = 0l;
        foreach (var day in _list)
        {
            if (counter++ == quantity)
            {
                break;
            }
            //Console.WriteLine(day);
            var TypeOfEvent = conn.Query<int>($"SELECT TypeID FROM Type WHERE TypeName = '{day.Type}'").ToArray()[0];
            var SeverityOfEvent = conn.Query<int>($"SELECT SeverityID FROM Severity WHERE SeverityName = '{day.Severity}'").ToArray()[0];
            var sql = $"INSERT INTO WeatherEvents " +
                      $" (TypeID," +
                      $"SeverityID," +
                      $"StartTimeUTC," +
                      $"EndTimeUTC," +
                      $" Precipitation," +
                      $"TimeZone," +
                      $"AirportCode," +
                      $"LocationLat," +
                      $"LocationLng," +
                      $"City," +
                      $"County," +
                      $"State," +
                      $"ZipCode) VALUES ({TypeOfEvent}," +
                      $"{SeverityOfEvent},'{day.StartTimeUTC}','{day.EndTimeUTC}'," +
                      $"'{day.Precipitation}','{day.TimeZone}','{day.AirportCode}'," +
                      $"{day.LocationLat},{day.LocationLng},'{day.City}'," +
                      $"'{day.County}','{day.State}','{day.ZipCode}');";
            conn.Execute(sql);
        }
        
        _list.Clear();
    }

    /// <summary>
    /// Check whether table with name tableName exists in connection 
    /// </summary>
    /// <param name="tableName">table name</param>
    /// <param name="connection">Database info</param>
    /// <returns>return true if exist else false</returns>
    public bool IsTableExists(String tableName, SqliteConnection connection)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name = @name";
        cmd.Parameters.Add("@name", SqliteType.Text).Value = tableName;
        return (cmd.ExecuteScalar() != null);
    }

    /// <summary>
    /// Create database to store WeatherEvents and additional databases for type and severity
    /// </summary>
    /// <param name="pathToCsv">location of csv file on the computer</param>
    public void CreateDb(string pathToCsv, int quantity)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        if (IsTableExists("WeatherEvents", conn))
        {
            conn.Execute("DROP TABLE WeatherEvents");
        }

        var createCommandText = new List<string>() { $"CREATE TABLE 'WeatherEvents' (" };
        foreach (var header in WeatherEvent.Headers.Keys)
        {
            createCommandText.Add($"{header}  {WeatherEvent.Headers[header]}, ");
        }

        createCommandText.Add("PRIMARY KEY('EventId' AUTOINCREMENT));");

        conn.Execute(string.Join("", createCommandText));
        ReadCsv(pathToCsv);
        CreateDictionary("Severity", conn, x => x.Severity);
        CreateDictionary("Type", conn, x => x.Type);

        FillTable(pathToCsv, quantity);
        IsConnected = true;
        //Console.WriteLine(_list.Count);
    }

    private void CreateDictionary(string name, SqliteConnection connection, Func<WeatherEvent, string> selectFunc)
    {
        if (IsTableExists(name, connection))
        {
            connection.Execute($"DROP TABLE {name}");
        }
        connection.Execute($"CREATE TABLE '{name}' (" +
                           $"'{name}ID' INTEGER," +
                           $"'{name}Name' TEXT," +
                           $"PRIMARY KEY('{name}ID' AUTOINCREMENT));");
        foreach (var cls in _list.Select(selectFunc).Distinct())
        {
            connection.Execute($"INSERT INTO {name} ({name}Name) VALUES ('{cls}')");
        }
    }
}