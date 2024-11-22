using System.Text.Json.Nodes;

namespace PlacesWF.API_interaction;

public class LocationByNameSearcher
{
    public record SpecificLocation(string? State, string? City, string? Street, string? HouseNumber);
    public record LocationPreviewDescription(string Name, string Country, string? OSM_key, SpecificLocation SpecificLocation);

    private readonly HttpClient _httpClient;
    private readonly string _api_key;

    public LocationByNameSearcher(string api_key, HttpClient httpClient)
    {
        _api_key = api_key;
        _httpClient = httpClient;
    }

    public LocationByNameSearcher(string api_key)
    {
        _api_key = api_key;
        _httpClient = new HttpClient();
    }

    public record LocationPreview(LocationPreviewDescription Description, double Latitude, double Longitude,
        double MinLongitude, double MaxLongitude, double MinLatitude, double MaxLatitude)
    {
        public static LocationPreview ParseFromJson(JsonObject jsonObject)
        {
            string name = jsonObject["name"].GetValue<string>();
            string country = jsonObject["country"].GetValue<string>();
            JsonNode point;
            double latitude;
            double longitude;
            if (!jsonObject.TryGetPropertyValue("point", out point))
            {
                latitude = longitude = double.NaN;
            }
            else
            {
                latitude = point["lat"].GetValue<double>();
                longitude = point["lng"].GetValue<double>();
            }
            JsonNode extent;
            double minLongitude;
            double maxLongitude;
            double minLatitude;
            double maxLatitude;
            if (!jsonObject.TryGetPropertyValue("extent", out extent))
            {
                minLongitude = maxLongitude = minLatitude = maxLatitude = double.NaN;
            }
            else
            {
                minLongitude = extent[0].GetValue<double>();
                maxLongitude = extent[2].GetValue<double>();
                minLatitude = extent[1].GetValue<double>();
                maxLatitude = extent[3].GetValue<double>();
            }

            string? osmKey = jsonObject["osm_key"]?.GetValue<string>();
            string? state = jsonObject["state"]?.GetValue<string>();
            string? city = jsonObject["city"]?.GetValue<string>();
            string? street = jsonObject["street"]?.GetValue<string>();
            string? housenumber = jsonObject["housenumber"]?.GetValue<string>();

            LocationPreviewDescription description = new LocationPreviewDescription(name, country, osmKey, new SpecificLocation(state,
                                                                                    city, street, housenumber));
            return new LocationPreview(description, latitude, longitude, minLongitude, maxLongitude, minLatitude, maxLatitude);
        }

        public static List<LocationPreview> ParsePreviewsFromJson(JsonObject jsonObject)
        {
            List<LocationPreview> locations = [];
            JsonNode hits;
            if (!jsonObject.TryGetPropertyValue("hits", out hits))
            {
                return locations;
            }
            foreach (JsonObject item in hits.AsArray())
            {
                locations.Add(ParseFromJson(item));
            }
            return locations;
        }
    }

    public JsonObject GetLocationByNameJson(string name)
    {
        Task<JsonObject> task = GetLocationByNameJsonAsync(name);
        task.Wait();
        return task.Result;
    }

    public Task<JsonObject> GetLocationByNameJsonAsync(string name)
    {
        string serverUrl = "https://graphhopper.com/api/1/geocode";
        string locationRequest = "?q=" + FormatNameForRequest(name) + "&key=" + _api_key;
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serverUrl + locationRequest);
        Task<JsonObject> locationPreviewTask = _httpClient.SendAsync(request)
            .ContinueWith((t) => JsonNode.Parse(t.Result.Content.ReadAsStream()) as JsonObject);
        return locationPreviewTask;
    }

    public Task<List<LocationPreview>> GetLocationByNameAsync(string name)
    {
        return GetLocationByNameJsonAsync(name).ContinueWith(t => LocationPreview.ParsePreviewsFromJson(t.Result));
    }

    public List<LocationPreview> GetLocationByName(string Name)
    {

        //string serverUrl = "https://graphhopper.com/api/1/geocode";
        //string locationRequest = "?q=" + FormatNameForRequest(Name) + "&key=" + _api_key;
        //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serverUrl + locationRequest);
        //Task<HttpResponseMessage> locationPreviewTask = _httpClient.SendAsync(request);
        //locationPreviewTask.Wait();
        //HttpResponseMessage response = locationPreviewTask.Result;
        //JsonObject jsonResponse = JsonNode.Parse(response.Content.ReadAsStream()) as JsonObject;
        return LocationPreview.ParsePreviewsFromJson(GetLocationByNameJson(Name));
    }

    private static string FormatNameForRequest(string name)
    {
        return name.Trim().Replace(" ", "+");
    }
}
