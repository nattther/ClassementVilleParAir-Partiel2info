using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var codepays = "FR";
        var utilisateur = "natther";
        var url = $"http://api.geonames.org/searchJSON?formatted=true&country={codepays}&maxRows=15&cities=cities15000&orderby=population&username={utilisateur}";

        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var cities = JsonSerializer.Deserialize<GeoNamesResponse>(responseBody);

        foreach (var city in cities.geonames)
        {
            Console.WriteLine($"{city.name}");
        }
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
