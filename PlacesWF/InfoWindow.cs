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
            SetListViewSchema();
            PopulateListView();
        }

        private void SetListViewSchema()
        {
            InterestingPlacesList.View = View.Details;
            InterestingPlacesList.Columns.Add("Name");
            InterestingPlacesList.Columns.Add("Rating");
            InterestingPlacesList.Columns.Add("Kinds");
        }

        private void PopulateListView()
        {
            if (_locationInfo.Attractions.Count == 0)
            {
                return;
            }
            InterestingPlacesList.Items.AddRange(AttractionsAsListViewItems(_locationInfo.Attractions));
            InterestingPlacesList.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            InterestingPlacesList.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            InterestingPlacesList.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private string FormatWeather(InfoGetter.LocationWeather weather)
        {
            return $"{char.ToUpper(weather.Description[0]) + weather.Description.Substring(1)}\n" +
                $"Temperature: {weather.CurrentTemperature}°C\n" +
                $"Feels like: {weather.FeelsLike}°C" +
                $" Max: {weather.MaxTemp}°C Min: {weather.MinTemp}°C\n" +
                $"Atmospheric pressure: {weather.Pressure * PressureConvertFactor}mmHg Humidity: {weather.Humidity}%\n" +
                $"Wind speed: {weather.WindSpeed}m/s direction: {weather.WindDirection.ToString().ToLower()}";
        }

        private static ListViewItem[] AttractionsAsListViewItems(List<InfoGetter.Attraction> attractions)
        {
            ListViewItem[] items = new ListViewItem[attractions.Count];
            for (int i = 0; i < attractions.Count; i++)
            {
                items[i] = AttractionToListItem(attractions[i]);
            }
            return items;
        }

        private static ListViewItem AttractionToListItem(InfoGetter.Attraction attraction)
        {
            ListViewItem row = new ListViewItem(attraction.Name);
            row.SubItems.Add(ValueOrNotStated(attraction.rate.ToString()));
            row.SubItems.Add(ValueOrNotStated(attraction.kinds));
            return row;
        }

        private static string ValueOrNotStated(string? state)
        {
            return state ?? "no info";
        }
    }
}
