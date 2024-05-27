using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Veuillez entrer un code de pays, (exemple FR) :");
        string codePays = Console.ReadLine();

        if (!string.IsNullOrEmpty(codePays))
        {
            await PlusGrandVille(codePays);
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
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        GeoNamesResponse villes = JsonSerializer.Deserialize<GeoNamesResponse>(responseBody);

        Console.WriteLine($"Les plus grandes villes en {codePays} :");
        foreach (var ville in villes.geonames)
        {
            Console.WriteLine($" {ville.name} ");
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
