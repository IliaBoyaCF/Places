using System.Globalization;
using System.Text.Json.Nodes;

namespace PlacesWF.API_interaction;

public class Attractions
{
    private readonly string _api_key;
    private readonly HttpClient _httpClient;

    public Attractions(string api_key)
    {
        _api_key = api_key;
        _httpClient = new HttpClient();
    }

    public Attractions(string api_key, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _api_key = api_key;
    }

    public record Attraction(string Name, int rate, string kinds)
    {
        public static Attraction? ParseFromJson(JsonObject jsonObject)
        {
            JsonObject properties = jsonObject["properties"] as JsonObject;
            string name = properties["name"].GetValue<string>();
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            return new Attraction(name, properties["rate"].GetValue<int>(),
                properties["kinds"].GetValue<string>());
        }
        public static List<Attraction> ParseAttractionsFromJson(JsonObject jsonObject)
        {
            List<Attraction> attractions = [];
            JsonArray features = jsonObject["features"] as JsonArray;
            if (features == null)
            {
                return attractions;
            }
            foreach (JsonObject feature in features)
            {
                Attraction? attraction = ParseFromJson(feature);
                if (attraction == null)
                {
                    continue;
                }
                attractions.Add(attraction);
            }
            return attractions;
        }
    }

    public List<Attraction> GetAttractionsInLocationExtent(double maxLongitude, double minLongitude,
        double maxLatitude, double minLatitude)
    {
        return Utils.FromTask(GetAttractionsInLocationExtentJsonAsync(maxLongitude, minLongitude, maxLatitude, minLatitude), 
            Attraction.ParseAttractionsFromJson);
    }

    public Task<List<Attraction>> GetAttractionsInLocationExtentAsync(double maxLongitude, double minLongitude,
        double maxLatitude, double minLatitude)
    {
        return GetAttractionsInLocationExtentJsonAsync(maxLongitude, minLongitude, maxLatitude, minLatitude)
            .ContinueWith(t => Attraction.ParseAttractionsFromJson(t.Result));
    }

    public JsonObject GetAttractionsInLocationExtentJson(double maxLongitude, double minLongitude,
        double maxLatitude, double minLatitude)
    {
        return Utils.WaitTask(GetAttractionsInLocationExtentJsonAsync(maxLongitude, minLongitude, maxLatitude, minLatitude));
    }

    public Task<JsonObject> GetAttractionsInLocationExtentJsonAsync(double maxLongitude, double minLongitude, 
        double maxLatitude, double minLatitude)
    {
        string placesServerUrl = "http://api.opentripmap.com/0.1/ru/places/bbox";
        string placesRequest = "?lang=ru&lon_min=" + minLongitude.ToString(CultureInfo.InvariantCulture)
            + "&lon_max=" + maxLongitude.ToString(CultureInfo.InvariantCulture) +
                "&lat_min=" + minLatitude.ToString(CultureInfo.InvariantCulture)
                + "&lat_max=" + maxLatitude.ToString(CultureInfo.InvariantCulture)
                + "&apikey=" + _api_key;

        HttpRequestMessage places = new HttpRequestMessage(HttpMethod.Get, placesServerUrl + placesRequest);

        return _httpClient.SendAsync(places).ContinueWith(t => JsonNode.Parse(t.Result.Content.ReadAsStream()) as JsonObject);
    }

    public List<Attraction> GetAttractionFromPoint(double longitude, double latitude, double radius)
    {
        return Utils.FromTask(GetAttractionsFromPointJsonAsync(longitude, latitude, radius), Attraction.ParseAttractionsFromJson);
    }

    public Task<List<Attraction>> GetAttractionFromPointAsync(double longitude, double latitude, double radius)
    {
        return GetAttractionsFromPointJsonAsync(longitude, latitude, radius).ContinueWith(t => Attraction.ParseAttractionsFromJson(t.Result));
    }

    public JsonObject GetAttractionFromPointJson(double longitude, double latitude, double radius)
    {
        return Utils.WaitTask(GetAttractionsFromPointJsonAsync(longitude, latitude, radius));
    }

    public Task<JsonObject> GetAttractionsFromPointJsonAsync(double longitude, double latitude, double radius)
    {
        string placesServerUrl = "http://api.opentripmap.com/0.1/ru/places/radius";
        string placesRequest = "?lang=ru&lon=" + longitude.ToString() + "&lat=" + latitude.ToString() +
            "&radius=" + radius.ToString()
        + "&apikey=" + _api_key;

        HttpRequestMessage places = new HttpRequestMessage(HttpMethod.Get, placesServerUrl + placesRequest);

        return _httpClient.SendAsync(places).ContinueWith(t => JsonNode.Parse(t.Result.Content.ReadAsStream()) as JsonObject);
    }
}
