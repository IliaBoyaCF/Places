using System.Text.Json.Nodes;

namespace PlacesWF.API_interaction;

public class WeatherForecast
{
    private readonly string _api_key;
    private readonly HttpClient _httpClient;

    public WeatherForecast(string api_key)
    {
        _httpClient = new HttpClient();
        _api_key = api_key;
    }

    public WeatherForecast(string api_key, HttpClient httpClient)
    {
        _api_key = api_key;
        _httpClient = httpClient;
    }

    public record TemperatureInfo(double CurrentTemperature, double FeelsLike, double MinTemp, double MaxTemp);

    public record WindInfo(double WindSpeed, LocationWeather.Direction WindDirection);

    public record WeatherGeneralInfo(string Description, double Pressure, double Humidity);

    public record LocationWeather(WeatherGeneralInfo GeneralInfo, TemperatureInfo TemperatureInfo, WindInfo WindInfo)
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
            return new LocationWeather(
                    new WeatherGeneralInfo(
                        jsonObject["weather"][0]["description"].GetValue<string>(), 
                        main["pressure"].GetValue<double>(), 
                        main["humidity"].GetValue<double>()
                    ), 
                    new TemperatureInfo(
                        main["temp"].GetValue<double>(),
                        main["feels_like"].GetValue<double>(), 
                        main["temp_min"].GetValue<double>(), 
                        main["temp_max"].GetValue<double>()
                    ),
                    new WindInfo(
                        wind["speed"].GetValue<double>(),
                        DirectionFromDeg(wind["deg"].GetValue<int>())
                    )
                );
        }

        private static Direction DirectionFromDeg(int v)
        {
            int directionsCount = Enum.GetNames(typeof(Direction)).Length;
            return (Direction)(((360 / directionsCount / 2 + v) % directionsCount + 1) % directionsCount);
        }
    }

    public LocationWeather GetWeather(double latitude, double longitude)
    {
        Task<LocationWeather> task = GetWeatherAsync(latitude, longitude);
        task.Wait();
        return task.Result;
    }

    public Task<LocationWeather> GetWeatherAsync(double latitude, double longitude)
    {
        return GetWeatherJsonAsync(latitude, longitude).ContinueWith(t => LocationWeather.ParseFromJson(t.Result));
    }

    public JsonObject GetWeatherJson(double latitude, double longitude)
    {
        Task<JsonObject> task = GetWeatherJsonAsync(latitude, longitude);
        task.Wait();
        return task.Result;
    }

    public Task<JsonObject> GetWeatherJsonAsync(double latitude, double longitude)
    {
        string weatherServerUrl = "https://api.openweathermap.org/data/2.5/weather";
        string weatherRequest = "?lat=" + latitude.ToString() + "&lon=" + longitude.ToString() + "&units=metric"
            + "&appid=" + _api_key;

        HttpRequestMessage weather = new HttpRequestMessage(HttpMethod.Get, weatherServerUrl + weatherRequest);

        return _httpClient.SendAsync(weather).ContinueWith(t => JsonNode.Parse(t.Result.Content.ReadAsStream()) as JsonObject);
    }

}
