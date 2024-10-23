using System.Collections.Generic;

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
            _locationPreviews = InfoGetter.GetLocationByName(searchField.Text);
            if (_locationPreviews.Count == 0)
            {
                MessageBox.Show("No results for location name.",
                    "No results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foundPlacesLabel.Visible = true;
            foundPlacesListView.Visible = true;
            showInfoButton.Visible = true;
            PopulateList();
        }

        private void PopulateList()
        {
            foundPlacesListView.Items.Clear();
            foundPlacesListView.Items.AddRange(LocationPreviewsToListItem(_locationPreviews));
            foreach (ColumnHeader column in foundPlacesListView.Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private static ListViewItem[] LocationPreviewsToListItem(List<InfoGetter.LocationPreview> locationPreviews)
        {
            ListViewItem[] listViewItems = new ListViewItem[locationPreviews.Count];
            for (int i = 0; i < locationPreviews.Count; i++)
            {
                //listViewItems[i] =  new ListViewItem(FormatLocationPreview(locationPreviews[i]));
                listViewItems[i] = LocationPreviewToListItem(locationPreviews[i]);
            }
            return listViewItems;
        }

        private static ListViewItem LocationPreviewToListItem(InfoGetter.LocationPreview locationPreview)
        {
            ListViewItem row = new ListViewItem($"{locationPreview.Description.Name}");
            row.SubItems.Add(ValueOrNotStated(locationPreview.Description.OSM_key));
            row.SubItems.Add($"{locationPreview.Description.Country}");
            row.SubItems.Add(ValueOrNotStated(locationPreview.Description.State));
            row.SubItems.Add(ValueOrNotStated(locationPreview.Description.City));
            row.SubItems.Add(ValueOrNotStated(locationPreview.Description.Street));
            row.SubItems.Add(ValueOrNotStated(locationPreview.Description.HouseNumber));

            return row;
        }

        private static string ValueOrNotStated(string? state)
        {
            return state ?? "no info";
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

        private void SearchWindow_Load(object sender, EventArgs e)
        {
            SetListViewSchema(foundPlacesListView);
        }

        private void SetListViewSchema(ListView foundPlacesListView)
        {
            foundPlacesListView.View = View.Details;
            foundPlacesListView.Columns.Add("Name");
            foundPlacesListView.Columns.Add("Type");
            foundPlacesListView.Columns.Add("Country");
            foundPlacesListView.Columns.Add("State");
            foundPlacesListView.Columns.Add("City");
            foundPlacesListView.Columns.Add("Street");
            foundPlacesListView.Columns.Add("House number");
        }
    }
}
