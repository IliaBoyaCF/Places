using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlacesWF
{
    public partial class InfoWindow : Form
    {
        private const double PressureConvertFactor = 0.750062;
        private InfoGetter.LocationInfo _locationInfo;

        public InfoWindow(InfoGetter.LocationInfo locationInfo)
        {
            _locationInfo = locationInfo;
            InitializeComponent();
        }

        private void InfoWindow_Load(object sender, EventArgs e)
        {
            WeatherToFillLabel.Text = FormatWeather(_locationInfo.Weather);
            InterestingPlacesList.Items.AddRange(AttractionsAsListViewItems(_locationInfo.Attractions));
        }

        private string FormatWeather(InfoGetter.LocationWeather weather)
        {
            return $"{weather.Description}\n" +
                $"Temperature: {weather.CurrentTemperature}°C\n" +
                $"feels like: {weather.FeelsLike}°C" +
                $" Max: {weather.MaxTemp}°C Min: {weather.MinTemp}°C\n" +
                $"Atmospheric pressure: {weather.Pressure * PressureConvertFactor}mmHg humidity: {weather.Humidity}%\n" +
                $"Wind speed: {weather.WindSpeed}m/s direction: {weather.WindDirection}";
        }

        private static ListViewItem[] AttractionsAsListViewItems(List<InfoGetter.Attraction> attractions)
        {
            ListViewItem[] items = new ListViewItem[attractions.Count];
            for (int i = 0; i < attractions.Count; i++)
            {
                items[i] = new ListViewItem(FormatAttraction(attractions[i]));
            }
            return items;
        }

        private static string FormatAttraction(InfoGetter.Attraction attraction)
        {
            return $"Name: {attraction.Name} rating: {attraction.rate}\nKinds: {attraction.kinds}";
        }
    }
}
