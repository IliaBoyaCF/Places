namespace PlacesWF
{
    public partial class SearchWindow : Form
    {

        private List<InfoGetter.LocationPreview> _locationPreviews = [];

        public SearchWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string typedText = searchField.Text;
            if (string.IsNullOrWhiteSpace(typedText))
            {
                MessageBox.Show("Type the name of the location in text field before pressing search button", 
                    "No input", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            _locationPreviews =  InfoGetter.GetLocationByName(searchField.Text);
            if (_locationPreviews.Count == 0)
            {
                MessageBox.Show("No results for location name.",
                    "No results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foundPlacesLabel.Visible = true;
            foundPlacesListView.Visible = true;
            showInfoButton.Visible = true;
            foundPlacesListView.Items.Clear();
            foundPlacesListView.Items.AddRange(LocationPreviewsToListItem(_locationPreviews));
        }

        private static ListViewItem[] LocationPreviewsToListItem(List<InfoGetter.LocationPreview> locationPreviews)
        {
            ListViewItem[] listViewItems = new ListViewItem[locationPreviews.Count];
            for (int i = 0; i < locationPreviews.Count; i++)
            {
                listViewItems[i] =  new ListViewItem(FormatLocationPreview(locationPreviews[i]));
            }
            return listViewItems;
        }

        private static string FormatLocationPreview(InfoGetter.LocationPreview locationPreview)
        {
            return $"Name: {locationPreview.Name} Country: {locationPreview.Country}";
        }


        private void ShowInfoButton_Click(object sender, EventArgs e)
        {
            InfoGetter.LocationInfo locationInfo;
            try
            { 
                if (foundPlacesListView.SelectedIndices.Count == 0)
                {
                    throw new InvalidOperationException("Select location first.");
                }
                locationInfo = InfoGetter.GetLocationInfo(_locationPreviews[foundPlacesListView.SelectedIndices[0]]);
            }
            catch (Exception exc) when (exc is InfoGetter.InfoGetterException || exc is InvalidOperationException)
            {
                MessageBox.Show($"Some error occurred. Try again.\n(Error info: {exc.Message})", "Error executing request", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            InfoWindow infoWindow = new InfoWindow(locationInfo);
            infoWindow.Show();
        }
    }
}
