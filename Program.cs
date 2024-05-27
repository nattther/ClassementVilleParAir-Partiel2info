using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Veuillez entrer un code de pays (exemple : FR, US, GB) :");
        string? codePays = Console.ReadLine()?.ToUpper();

        if (!string.IsNullOrEmpty(codePays))
        {
            await PlusGrandesVilles(codePays);
        }
        else
        {
            Console.WriteLine("Entrée invalide. Veuillez entrer un code de pays valide.");
        }
    }

    private static async Task PlusGrandesVilles(string codePays)
    {
        string utilisateur = "natther";
        string url = $"http://api.geonames.org/searchJSON?formatted=true&country={codePays}&maxRows=15&cities=cities15000&orderby=population&username={utilisateur}";

        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);
        string responseBody = await response.Content.ReadAsStringAsync();
        GeoNamesResponse villes = JsonSerializer.Deserialize<GeoNamesResponse>(responseBody);

        Console.WriteLine($"Les plus grandes villes en {codePays} :");

        List<Task<(string, int)>> tasks = new List<Task<(string, int)>>();

        foreach (var ville in villes.geonames)
        {
            Console.WriteLine($"Récupération de l'AQI pour {ville.name}...");
            tasks.Add(GetAirQualityIndex(ville.name, codePays));
        }

        var cityAQIs = await Task.WhenAll(tasks);

        var sortedCityAQIs = cityAQIs.OrderBy(c => c.Item2).ToList();

        Console.WriteLine("Classement des villes par qualité de l'air (AQI) :");

        for (int i = 0; i < sortedCityAQIs.Count; i++)
        {
            Console.WriteLine($"Top {i + 1}: {sortedCityAQIs[i].Item1} - AQI = {sortedCityAQIs[i].Item2}");
        }
    }

    private static async Task<(string, int)> GetAirQualityIndex(string cityName, string countryCode)
    {
        string apiKey = "iEK1eIhnyVkt9WRSOypjGw==qaCqybEnNauXykwC"; 
        string url = $"https://api.api-ninjas.com/v1/airquality?city={Uri.EscapeDataString(cityName)}&country={countryCode}";

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        HttpResponseMessage response = await client.GetAsync(url);
        string responseBody = await response.Content.ReadAsStringAsync();
        AirQualityResponse airQualityResponse = JsonSerializer.Deserialize<AirQualityResponse>(responseBody);

        return (cityName, airQualityResponse.overall_aqi);
    }
}

public class GeoNamesResponse
{
    public GeoName[]? geonames { get; set; }

}

public class GeoName
{
public string? name { get; set; }

    public int population { get; set; }
}

public class AirQualityResponse
{
public AQIInfo? CO { get; set; }
public AQIInfo? NO2 { get; set; }
public AQIInfo? O3 { get; set; }
public AQIInfo? SO2 { get; set; }
public AQIInfo? PM25 { get; set; }
public AQIInfo? PM10 { get; set; }

    public int overall_aqi { get; set; }
}

public class AQIInfo
{
    public double concentration { get; set; }
    public int aqi { get; set; }
}
