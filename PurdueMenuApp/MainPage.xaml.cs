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
                MessageBox.Show("We're having some trouble connecting to Purdue's menu to fetch the latest menu! Please try again later.");
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


                            List<List<DateTime>> hours = new List<List<DateTime>>();

                            //If we're open today, parse the times we're open.
                            if (dc.Attributes["class"].Value.Contains("open"))
                            {
                                //String breakfast_time = dc.SelectNodes("a/div[@class='features-menu']/div[@id='Breakfast']/p").First().InnerText;
                                HtmlNodeCollection times = dc.SelectNodes("a/div[@class='features-menu']/div[@id]/p");
                                foreach (HtmlNode timeNode in times)
                                {
                                    //So sometimes the doofuses maintaining the menu forget to put "pm" or "am" on one side of their time string. Need to check and correct for this.
                                    string timestring = timeNode.InnerHtml.Replace(".","").Replace(" ","");
                                    string open_str = timestring.Substring(0, timestring.IndexOf("-"));
                                    string close_str = timestring.Substring(timestring.IndexOf("-") + 1);

                                    //If we define the open time segment but not the close time segment, copy it.
                                    if (open_str.ToLower().Contains("am") && (!close_str.ToLower().Contains("am") && !close_str.ToLower().Contains("pm")))
                                        close_str += "am";
                                    if (open_str.ToLower().Contains("pm") && (!close_str.ToLower().Contains("am") && !close_str.ToLower().Contains("pm")))
                                        close_str += "pm";

                                    //If we define the close time segment but not the open time segment, copy it.
                                    if (close_str.ToLower().Contains("am") && (!open_str.ToLower().Contains("am") && !open_str.ToLower().Contains("pm")))
                                        open_str += "am";
                                    if (close_str.ToLower().Contains("pm") && (!open_str.ToLower().Contains("am") && !open_str.ToLower().Contains("pm")))
                                        open_str += "pm";

                                    DateTime open_time = DateTime.Parse(open_str);
                                    DateTime close_time = DateTime.Parse(close_str);
                                    List<DateTime> time_range = new List<DateTime>();
                                    time_range.Add(open_time);
                                    time_range.Add(close_time);
                                    hours.Add(time_range);
                                }
                            }

                            DiningCourt d = new DiningCourt {
                                name = dc.Attributes["id"].Value,
                                web_id = webid,
                                open_times = hours
                            };
                            diningcourts.Add(d);
                        }
                    }
                }
            }
        }

        private void list_diningcourts_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
        }

        private void ListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void list_diningcourts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list_diningcourts.SelectedItem is DiningCourt)
            {
                DiningCourt dc = ((list_diningcourts as LongListSelector).SelectedItem as DiningCourt);
                NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.Relative));
                App.selected_dc = dc;
                list_diningcourts.SelectedItem = null;
            }
        }
    }
}