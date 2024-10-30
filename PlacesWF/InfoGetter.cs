using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.Json.Nodes;

namespace PlacesWF;

public class InfoGetter
{

    public class InfoGetterException : Exception
    {
        public InfoGetterException() : base() { }
        
        public InfoGetterException(string message) : base(message) { }

        public InfoGetterException(string message,  Exception innerException) : base(message, innerException) { }
    }

    private static HttpClient _httpClient;

    private static string s_location_api_key;
    private static string s_places_api_key;
    private static string s_weather_api_key;

    public static void SetConfig(IConfigurationRoot configuration)
    {
        s_location_api_key = configuration["LocationApiKey"];
        s_places_api_key = configuration["PlacesApiKey"];
        s_weather_api_key = configuration["WeatherApiKey"];
        if (IsConfigSet())
        {
            throw new ArgumentException("Provided configuration does not contain the necessary information.");
        }
    }

    public static bool IsConfigSet()
    {
        return s_location_api_key == null || s_places_api_key == null || s_weather_api_key == null;
    }

    private static HttpClient GetHttpClient()
    {
        if (_httpClient == null)
        {
            _httpClient = new HttpClient();
        }
        return _httpClient;
    }

    public record LocationWeather(string Description, double CurrentTemperature, double FeelsLike, double MinTemp, double MaxTemp,
      double Pressure, double Humidity, double WindSpeed, LocationWeather.Direction WindDirection)
    {

        public enum Direction : int
        {
            NORTH,
            NORTHWEST,
            WEST,
            SOUTHWEST,
            SOUTH,
            SOUTHEAST,
            EAST,
            NORTHEAST,
        }
        public static LocationWeather ParseFromJson(JsonObject jsonObject)
        {
            JsonObject main = jsonObject["main"] as JsonObject;
            JsonObject wind = jsonObject["wind"] as JsonObject;
            return new LocationWeather(jsonObject["weather"][0]["description"].GetValue<string>(), main["temp"].GetValue<double>(), 
                main["feels_like"].GetValue<double>(), main["temp_min"].GetValue<double>(), main["temp_max"].GetValue<double>(), 
                main["pressure"].GetValue<double>(), main["humidity"].GetValue<double>(), wind["speed"].GetValue<double>(), 
                DirectionFromDeg(wind["deg"].GetValue<int>()));
        }

        private static Direction DirectionFromDeg(int v)
        {
            int directionsCount = Enum.GetNames(typeof(Direction)).Length;
            return (Direction)(((360 / directionsCount / 2 + v) % directionsCount + 1) % directionsCount);
        }
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

    public record LocationPreviewDescription(string Name, string Country, string? OSM_key, string? State, 
                                            string? City, string? Street, string? HouseNumber) { }

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

            LocationPreviewDescription description = new LocationPreviewDescription(name, country, osmKey, state,
                                                                                    city, street, housenumber);
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

    public record LocationInfo(LocationWeather Weather, List<Attraction> Attractions);


    public static List<LocationPreview> GetLocationByName(string Name)
    {

        string serverUrl = "https://graphhopper.com/api/1/geocode";
        string locationRequest = "?q=" + FormatNameForRequest(Name) + "&key=" + s_location_api_key;
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serverUrl + locationRequest);
        Task<HttpResponseMessage> locationPreviewTask = GetHttpClient().SendAsync(request);
        locationPreviewTask.Wait();
        HttpResponseMessage response = locationPreviewTask.Result;
        JsonObject jsonResponse = JsonNode.Parse(response.Content.ReadAsStream()) as JsonObject;
        return LocationPreview.ParsePreviewsFromJson(jsonResponse);
    }

    private static string FormatNameForRequest(string name)
    {
        return name.Trim().Replace(" ", "+");
    }

    public static LocationInfo GetLocationInfo(LocationPreview location)
    {
        Task<HttpResponseMessage> weatherResponseTask = GetWeatherInLocation(location);
        Task<HttpResponseMessage> attractionsResponseTask = GetAttractionsInLocation(location);
        Task<LocationWeather> weatherTask = weatherResponseTask.ContinueWith((task, state) =>
        {
            if (task.IsFaulted)
            {
                return null;
            }
            JsonObject response = JsonNode.Parse(task.Result.Content.ReadAsStream()) as JsonObject;
            return LocationWeather.ParseFromJson(response);
        }, null);
        Task<List<Attraction>> attractionsTask = attractionsResponseTask.ContinueWith((task, state) =>
        {
            if (task.IsFaulted)
            {
                return null;
            }
            JsonObject response = JsonNode.Parse(task.Result.Content.ReadAsStream()) as JsonObject;
            return Attraction.ParseAttractionsFromJson(response);
        }, null);
        bool weatherRequestStatus = weatherTask.Wait(10_000);
        bool attractionsRequestStatus = attractionsTask.Wait(10_000);
        if (!(weatherRequestStatus && attractionsRequestStatus))
        {
            if (weatherRequestStatus)
            {
                return new LocationInfo(weatherTask.Result, []);
            }
            throw new InfoGetterException("One of API is not responding.");
        }
        return new LocationInfo(weatherTask.Result, attractionsTask.Result);
    }

    private static Task<HttpResponseMessage> GetAttractionsInLocation(LocationPreview location)
    {
        if (double.IsNaN(location.MinLatitude))
        {
            return GetAttractionsFromPoint(location);
        }
        return GetAttractionsInLocationExtent(location);
    }

    private static Task<HttpResponseMessage> GetAttractionsInLocationExtent(LocationPreview location)
    {
        string placesServerUrl = "http://api.opentripmap.com/0.1/ru/places/bbox";
        string placesRequest = "?lang=ru&lon_min=" + location.MinLongitude.ToString(CultureInfo.InvariantCulture)
            + "&lon_max=" + location.MaxLongitude.ToString(CultureInfo.InvariantCulture) +
                "&lat_min=" + location.MinLatitude.ToString(CultureInfo.InvariantCulture)
                + "&lat_max=" + location.MaxLatitude.ToString(CultureInfo.InvariantCulture)
                + "&apikey=" + s_places_api_key;

        HttpRequestMessage places = new HttpRequestMessage(HttpMethod.Get, placesServerUrl + placesRequest);

        return GetHttpClient().SendAsync(places);
    }

    private static Task<HttpResponseMessage> GetWeatherInLocation(LocationPreview location)
    {
        string weatherServerUrl = "https://api.openweathermap.org/data/2.5/weather";
        string weatherRequest = "?lat=" + location.Latitude.ToString() + "&lon=" + location.Longitude.ToString() + "&units=metric"
            + "&appid=" + s_weather_api_key;

        HttpRequestMessage weather = new HttpRequestMessage(HttpMethod.Get, weatherServerUrl + weatherRequest);

        return GetHttpClient().SendAsync(weather);
    }

    private static Task<HttpResponseMessage> GetAttractionsFromPoint(LocationPreview location)
    {
        string placesServerUrl = "http://api.opentripmap.com/0.1/ru/places/radius";
        string radius = "1000";
        string placesRequest = "?lang=ru&lon=" + location.Longitude.ToString() + "&lat=" + location.Latitude.ToString() + 
            "&radius=" + radius
        + "&apikey=" + s_places_api_key;

        HttpRequestMessage places = new HttpRequestMessage(HttpMethod.Get, placesServerUrl +  placesRequest);

        return GetHttpClient().SendAsync(places);
    }
}
