namespace PlacesWF
{
    partial class SearchWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchWindow));
            placesLabel = new Label();
            searchField = new TextBox();
            searchButton = new Button();
            foundPlacesListView = new ListView();
            foundPlacesLabel = new Label();
            showInfoButton = new Button();
            SuspendLayout();
            // 
            // placesLabel
            // 
            placesLabel.AutoSize = true;
            placesLabel.Font = new Font("Segoe UI", 16F);
            placesLabel.Location = new Point(356, 9);
            placesLabel.Name = "placesLabel";
            placesLabel.Size = new Size(90, 37);
            placesLabel.TabIndex = 0;
            placesLabel.Text = "Places";
            // 
            // searchField
            // 
            searchField.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            searchField.Location = new Point(39, 83);
            searchField.Name = "searchField";
            searchField.PlaceholderText = "Type name of the location here";
            searchField.Size = new Size(623, 27);
            searchField.TabIndex = 1;
            // 
            // searchButton
            // 
            searchButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            searchButton.Location = new Point(668, 81);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(94, 29);
            searchButton.TabIndex = 2;
            searchButton.Text = "Search";
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += SearchButton_Click;
            // 
            // foundPlacesListView
            // 
            foundPlacesListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            foundPlacesListView.BorderStyle = BorderStyle.None;
            foundPlacesListView.Location = new Point(39, 153);
            foundPlacesListView.Name = "foundPlacesListView";
            foundPlacesListView.Size = new Size(723, 388);
            foundPlacesListView.TabIndex = 3;
            foundPlacesListView.UseCompatibleStateImageBehavior = false;
            foundPlacesListView.View = View.List;
            foundPlacesListView.Visible = false;
            // 
            // foundPlacesLabel
            // 
            foundPlacesLabel.AutoSize = true;
            foundPlacesLabel.Font = new Font("Segoe UI", 16F);
            foundPlacesLabel.Location = new Point(257, 113);
            foundPlacesLabel.Name = "foundPlacesLabel";
            foundPlacesLabel.Size = new Size(308, 37);
            foundPlacesLabel.TabIndex = 5;
            foundPlacesLabel.Text = "Found places(select one)";
            foundPlacesLabel.TextAlign = ContentAlignment.MiddleCenter;
            foundPlacesLabel.Visible = false;
            // 
            // showInfoButton
            // 
            showInfoButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            showInfoButton.Location = new Point(592, 121);
            showInfoButton.Name = "showInfoButton";
            showInfoButton.Size = new Size(94, 29);
            showInfoButton.TabIndex = 6;
            showInfoButton.Text = "Show Info";
            showInfoButton.UseVisualStyleBackColor = true;
            showInfoButton.Visible = false;
            showInfoButton.Click += ShowInfoButton_Click;
            // 
            // SearchWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(791, 553);
            Controls.Add(showInfoButton);
            Controls.Add(foundPlacesLabel);
            Controls.Add(foundPlacesListView);
            Controls.Add(searchButton);
            Controls.Add(searchField);
            Controls.Add(placesLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SearchWindow";
            Text = "Search";
            Load += SearchWindow_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label placesLabel;
        private TextBox searchField;
        private Button searchButton;
        private ListView foundPlacesListView;
        private Label foundPlacesLabel;
        private Button showInfoButton;
    }
}
