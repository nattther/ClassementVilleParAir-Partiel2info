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
        Console.WriteLine("Veuillez entrer un code de pays, (exemple FR) :");
        string codePays = Console.ReadLine();

        if (!string.IsNullOrEmpty(codePays))
        {
            await PlusGrandVille(codePays.ToUpper());
        }
        else
        {
            Console.WriteLine("Entrée invalide. Veuillez entrer un code de pays valide.");
        }
    }

   private static async Task PlusGrandVille(string codePays)
{
    string utilisateur = "natther";
    string url = $"http://api.geonames.org/searchJSON?formatted=true&country={codePays}&maxRows=15&cities=cities15000&orderby=population&username={utilisateur}";

    using HttpClient client = new HttpClient();
    HttpResponseMessage response = await client.GetAsync(url);
    string responseBody = await response.Content.ReadAsStringAsync();
    GeoNamesResponse villes = JsonSerializer.Deserialize<GeoNamesResponse>(responseBody);

    Console.WriteLine($"Les plus grandes villes en {codePays} :");
    List<(string, int)> cityAQIs = new List<(string, int)>();

    foreach (var ville in villes.geonames)
    {
        int aqi = await GetAirQualityIndex(ville.name, codePays);
        cityAQIs.Add((ville.name, aqi));
    }

    var sortedCityAQIs = cityAQIs.OrderBy(c => c.Item2).ToList();

    Console.WriteLine("Villes classées par qualité de l'air (AQI) :");
    foreach (var cityAQI in sortedCityAQIs)
    {
        Console.WriteLine($"{cityAQI.Item1}: AQI = {cityAQI.Item2}");
    }
}

    private static async Task<int> GetAirQualityIndex(string cityName, string countryCode)
    {
        string apiKey = "iEK1eIhnyVkt9WRSOypjGw==qaCqybEnNauXykwC"; // Remplacez par votre clé API API Ninjas
        string url = $"https://api.api-ninjas.com/v1/airquality?city={Uri.EscapeDataString(cityName)}&country={countryCode}";

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        HttpResponseMessage response = await client.GetAsync(url);
        string responseBody = await response.Content.ReadAsStringAsync();
        AirQualityResponse airQualityResponse = JsonSerializer.Deserialize<AirQualityResponse>(responseBody);

        return airQualityResponse.overall_aqi;
    }
}

public class GeoNamesResponse
{
    public GeoName[] geonames { get; set; }
}

public class GeoName
{
    public string name { get; set; }
    public int population { get; set; }
}

public class AirQualityResponse
{
    public AQIInfo CO { get; set; }
    public AQIInfo NO2 { get; set; }
    public AQIInfo O3 { get; set; }
    public AQIInfo SO2 { get; set; }
    public AQIInfo PM25 { get; set; }
    public AQIInfo PM10 { get; set; }
    public int overall_aqi { get; set; }
}

public class AQIInfo
{
    public double concentration { get; set; }
    public int aqi { get; set; }
}

public class CityAQI
{
    public string Name { get; set; }
    public int AQI { get; set; }
}
