using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.Json.Nodes;

namespace PlacesWF.API_interaction;

public class InfoGetter
{

    public class InfoGetterException : Exception
    {
        public InfoGetterException() : base() { }

        public InfoGetterException(string message) : base(message) { }

        public InfoGetterException(string message, Exception innerException) : base(message, innerException) { }
    }

    private static HttpClient _httpClient;

    private static Attractions _attractions;
    private static WeatherForecast _weatherForecast;
    private static LocationByNameSearcher _locationByNameSearcher;

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
        HttpClient httpClient = GetHttpClient();
        _locationByNameSearcher = new LocationByNameSearcher(s_location_api_key, httpClient);
        _weatherForecast = new WeatherForecast(s_weather_api_key, httpClient);
        _attractions = new Attractions(s_places_api_key, httpClient);
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

    public record LocationInfo(WeatherForecast.LocationWeather Weather, List<Attractions.Attraction> Attractions);

    public static List<LocationByNameSearcher.LocationPreview> GetLocationByName(string name)
    {
        return _locationByNameSearcher.GetLocationByName(name);
    }

    public static LocationInfo GetLocationInfo(LocationByNameSearcher.LocationPreview location)
    {
        
        Task<WeatherForecast.LocationWeather> weatherForecastTask = _weatherForecast.GetWeatherAsync(location.Latitude, location.Longitude);
        Task<List<Attractions.Attraction>> attractionsTask = GetAttractionsInLocation(location);
        bool weatherRequestStatus = weatherForecastTask.Wait(10_000);
        bool attractionsRequestStatus = attractionsTask.Wait(10_000);
        if (!(weatherRequestStatus && attractionsRequestStatus))
        {
            if (weatherRequestStatus)
            {
                return new LocationInfo(weatherForecastTask.Result, []);
            }
            throw new InfoGetterException("One of API is not responding.");
        }
        return new LocationInfo(weatherForecastTask.Result, attractionsTask.Result);
    }

    private static Task<List<Attractions.Attraction>> GetAttractionsInLocation(LocationByNameSearcher.LocationPreview location)
    {
        if (double.IsNaN(location.MinLatitude))
        {
            return _attractions.GetAttractionFromPointAsync(location.Longitude, location.Latitude, 1_000);
        }
        return _attractions.GetAttractionsInLocationExtentAsync(location.MaxLongitude, location.MinLongitude, location.MaxLatitude, location.MinLatitude);
    }
}
