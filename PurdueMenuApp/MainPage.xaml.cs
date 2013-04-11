using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HtmlAgilityPack;
using System.Collections.ObjectModel;

namespace PurdueMenuApp
{
    public partial class MainPage : PhoneApplicationPage
    {

        ObservableCollection<DiningCourt> diningcourts;
        ProgressIndicator progress_courts;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            diningcourts = new ObservableCollection<DiningCourt>();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            list_diningcourts.ItemsSource = diningcourts;
            LoadLocationList();
        }

        // Load dining court listing
        private void LoadLocationList()
        {
            //Show progress indicator
            progress_courts = new ProgressIndicator
            {
                IsVisible = true,
                IsIndeterminate = true,
                Text = "Fetching Dining Courts..."
            };
            SystemTray.SetProgressIndicator(this, progress_courts);
            progress_courts.IsVisible = true;
            HtmlWeb.LoadAsync("http://www.housing.purdue.edu/Menus/",LoadLocationList_complete);
        }

        public void LoadLocationList_complete(object sender, HtmlDocumentLoadCompleted e)
        {
            progress_courts.IsVisible = false;
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }
            HtmlDocument document = e.Document;
            var ulTags = document.DocumentNode.SelectNodes("//ul");
            HtmlNode courts_tag = null;
            if (ulTags != null)
            {
                foreach (HtmlNode tag in ulTags)
                {
                    if (tag.Attributes["class"] != null && tag.Attributes["class"].Value.Equals("kwicks"))
                    {
                        System.Diagnostics.Debug.WriteLine(tag.Attributes["class"].Value);
                        courts_tag = tag;
                        break;
                    }
                }

                // Build a list of dining courts
                if (courts_tag != null)
                {
                    diningcourts.Clear();
                    HtmlNodeCollection dcs = courts_tag.SelectNodes("li[@class]");
                    foreach (HtmlNode dc in dcs)
                    {
                        if (dc.Attributes["class"] != null && dc.Attributes["class"].Value.Contains("location-kwick"))
                        {
                            //Find the ID
                            HtmlNode id_link = dc.ChildNodes.FindFirst("a");
                            String webid = id_link.Attributes["href"].Value;
                            webid = webid.Substring(webid.LastIndexOf('/') + 1, webid.Length - webid.LastIndexOf('/') - 1); 
                            DiningCourt d = new DiningCourt {
                                name = dc.Attributes["id"].Value,
                                web_id = webid,
                                open = dc.Attributes["class"].Value.Contains("open")
                            };
                            diningcourts.Add(d);
                        }
                    }
                }
            }
        }

        private void list_diningcourts_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DiningCourt dc = ((list_diningcourts as LongListSelector).SelectedItem as DiningCourt);
            NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.Relative));
            App.selected_dc = dc;
        }
    }
}