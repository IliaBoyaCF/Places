namespace PlacesWF
{
    partial class InfoWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoWindow));
            LocationInfoLabel = new Label();
            WeatherLabel = new Label();
            InterestingPlacesLabel = new Label();
            WeatherToFillLabel = new Label();
            InterestingPlacesList = new ListView();
            SuspendLayout();
            // 
            // LocationInfoLabel
            // 
            LocationInfoLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LocationInfoLabel.AutoSize = true;
            LocationInfoLabel.Font = new Font("Segoe UI", 16F);
            LocationInfoLabel.Location = new Point(392, 9);
            LocationInfoLabel.Name = "LocationInfoLabel";
            LocationInfoLabel.Size = new Size(172, 37);
            LocationInfoLabel.TabIndex = 0;
            LocationInfoLabel.Text = "Location info";
            // 
            // WeatherLabel
            // 
            WeatherLabel.AutoSize = true;
            WeatherLabel.Font = new Font("Segoe UI", 16F);
            WeatherLabel.Location = new Point(12, 50);
            WeatherLabel.Name = "WeatherLabel";
            WeatherLabel.Size = new Size(116, 37);
            WeatherLabel.TabIndex = 1;
            WeatherLabel.Text = "Weather";
            // 
            // InterestingPlacesLabel
            // 
            InterestingPlacesLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            InterestingPlacesLabel.AutoSize = true;
            InterestingPlacesLabel.Font = new Font("Segoe UI", 16F);
            InterestingPlacesLabel.Location = new Point(291, 275);
            InterestingPlacesLabel.Name = "InterestingPlacesLabel";
            InterestingPlacesLabel.Size = new Size(356, 37);
            InterestingPlacesLabel.TabIndex = 2;
            InterestingPlacesLabel.Text = "Interesting places in location";
            // 
            // WeatherToFillLabel
            // 
            WeatherToFillLabel.AutoSize = true;
            WeatherToFillLabel.Font = new Font("Segoe UI", 14F);
            WeatherToFillLabel.Location = new Point(12, 98);
            WeatherToFillLabel.Name = "WeatherToFillLabel";
            WeatherToFillLabel.Size = new Size(78, 32);
            WeatherToFillLabel.TabIndex = 3;
            WeatherToFillLabel.Text = "label4";
            // 
            // InterestingPlacesList
            // 
            InterestingPlacesList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            InterestingPlacesList.Location = new Point(10, 315);
            InterestingPlacesList.MultiSelect = false;
            InterestingPlacesList.Name = "InterestingPlacesList";
            InterestingPlacesList.Size = new Size(888, 343);
            InterestingPlacesList.TabIndex = 4;
            InterestingPlacesList.UseCompatibleStateImageBehavior = false;
            InterestingPlacesList.View = View.List;
            // 
            // InfoWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(912, 669);
            Controls.Add(InterestingPlacesList);
            Controls.Add(InterestingPlacesLabel);
            Controls.Add(WeatherLabel);
            Controls.Add(LocationInfoLabel);
            Controls.Add(WeatherToFillLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "InfoWindow";
            Text = "Info";
            Load += InfoWindow_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LocationInfoLabel;
        private Label WeatherLabel;
        private Label InterestingPlacesLabel;
        private Label WeatherToFillLabel;
        private ListView InterestingPlacesList;
    }
}