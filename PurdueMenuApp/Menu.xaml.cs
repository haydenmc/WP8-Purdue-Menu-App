using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;

namespace PurdueMenuApp
{
    public partial class Menu : PhoneApplicationPage
    {
        ObservableCollection<FoodStation<MenuItem>> collection_breakfast;
        ObservableCollection<FoodStation<MenuItem>> collection_lunch;
        ObservableCollection<FoodStation<MenuItem>> collection_dinner;

        ProgressIndicator progress_menu;

        public Menu()
        {
            InitializeComponent();
            collection_breakfast = new ObservableCollection<FoodStation<MenuItem>>();
            collection_lunch = new ObservableCollection<FoodStation<MenuItem>>();
            collection_dinner = new ObservableCollection<FoodStation<MenuItem>>();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Set up title
            menu_pivot.Title = App.selected_dc.name.ToUpper();
            //Set up details view
            BitmapImage img = new BitmapImage();
            img.SetSource(Application.GetResourceStream(new Uri(@"Images/dining_courts/" + App.selected_dc.name.ToLower() + ".jpg", UriKind.Relative)).Stream);
            image.Source = img;
            title.Text = char.ToUpper(App.selected_dc.name[0]) + App.selected_dc.name.Substring(1).ToLower() + " Dining Court";
            //Set hours
            hours_listing.Text = "";
            foreach (List<DateTime> t in App.selected_dc.open_times)
            {
                hours_listing.Text += t.First().ToShortTimeString() + " - " + t.Last().ToShortTimeString() + "\n";
            }
            if (App.selected_dc.open_times.Count() <= 0)
                hours_listing.Text = "not serving today";
            open_indicator.DataContext = App.selected_dc;
            //Load / populate the menu
            loadMenu();
            list_breakfast.ItemsSource = collection_breakfast;
            list_lunch.ItemsSource = collection_lunch;
            list_dinner.ItemsSource = collection_dinner;
        }

        private void loadMenu()
        {
            //Show progress indicator
            progress_menu = new ProgressIndicator
            {
                IsVisible = true,
                IsIndeterminate = true,
                Text = "Fetching Menu..."
            };
            SystemTray.SetProgressIndicator(this, progress_menu);
            progress_menu.IsVisible = true;
            HtmlWeb.LoadAsync("http://www.housing.purdue.edu/Menus/"+App.selected_dc.web_id,loadMenu_complete);
        }

        private ObservableCollection<FoodStation<MenuItem>> parseMeal(string mealid,HtmlDocument document)
        {
            HtmlNodeCollection stations = document.DocumentNode.SelectNodes("//div[@id='"+mealid+"']//tr");
            ObservableCollection<FoodStation<MenuItem>> menuCollection = new ObservableCollection<FoodStation<MenuItem>>();
            if (stations != null)
            {
                FoodStation<MenuItem> station = null;
                foreach (HtmlNode mi in stations)
                {
                    HtmlNodeCollection checkstation = mi.SelectNodes("th[@class='station-name']/h4");
                    if (checkstation != null)
                    {
                        if (station != null)
                            menuCollection.Add(station);
                        station = new FoodStation<MenuItem>(HtmlEntity.DeEntitize(checkstation.First().InnerText), new List<MenuItem>());
                        continue;
                    }
                    HtmlNode itemnode = mi.SelectSingleNode("td/span");
                    if (itemnode == null) continue;
                    MenuItem m = new MenuItem
                    {
                        name = HtmlEntity.DeEntitize(itemnode.InnerText)
                    };
                    if (station != null)
                        station.Add(m);
                }
                if (station == null)
                    return null;
                menuCollection.Add(station);
                return menuCollection;
            }
            return null;
        }

        public void loadMenu_complete(object sender, HtmlDocumentLoadCompleted e)
        {
            progress_menu.IsVisible = false;
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }
            HtmlDocument document = e.Document; //[@class='menu_item']

            //Get the address
            HtmlNode addresstag = document.DocumentNode.SelectSingleNode("//span[@class='address']");
            String address = HtmlEntity.DeEntitize(addresstag.InnerHtml.Replace("<br>", ", "));
            address_listing.Text = address;

            //Load breakfast
            list_breakfast.ItemsSource = parseMeal("Breakfast", document);
            if (list_breakfast.ItemsSource == null)
            {
                menu_pivot.Items.Remove(pivot_breakfast);
            }
            //Load lunch
            list_lunch.ItemsSource = parseMeal("Lunch", document);
            if (list_lunch.ItemsSource == null)
            {
                menu_pivot.Items.Remove(pivot_lunch);
            }
            //Load dinner
            list_dinner.ItemsSource = parseMeal("Dinner", document);
            if (list_dinner.ItemsSource == null)
            {
                menu_pivot.Items.Remove(pivot_dinner);
            }
        }

        private void listbox_address_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MapsTask mapsTask = new MapsTask();
            mapsTask.SearchTerm = address_listing.Text;
            mapsTask.ZoomLevel = 2;
            mapsTask.Show();
        }
    }
}