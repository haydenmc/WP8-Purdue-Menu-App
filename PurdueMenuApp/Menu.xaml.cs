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

namespace PurdueMenuApp
{
    public partial class Menu : PhoneApplicationPage
    {
        ObservableCollection<MenuItem> collection_breakfast;
        ObservableCollection<MenuItem> collection_lunch;
        ObservableCollection<MenuItem> collection_dinner;

        ProgressIndicator progress_menu;

        public Menu()
        {
            InitializeComponent();
            collection_breakfast = new ObservableCollection<MenuItem>();
            collection_lunch = new ObservableCollection<MenuItem>();
            collection_dinner = new ObservableCollection<MenuItem>();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            menu_pivot.Title = App.selected_dc.name.ToUpper();
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

        public void loadMenu_complete(object sender, HtmlDocumentLoadCompleted e)
        {
            progress_menu.IsVisible = false;
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }
            HtmlDocument document = e.Document; //[@class='menu_item']

            //Load breakfast
            HtmlNodeCollection breakfasttags = document.DocumentNode.SelectNodes("//div[@id='Breakfast']//tr[@class=\"menu-item\"]/td/span"); ///td/span
            collection_breakfast.Clear();
            foreach (HtmlNode mi in breakfasttags)
            {
                MenuItem m = new MenuItem
                {
                    name = mi.InnerHtml
                };
                collection_breakfast.Add(m);
            }

            //Load lunch
            HtmlNodeCollection lunchtags = document.DocumentNode.SelectNodes("//div[@id='Lunch']//tr[@class=\"menu-item\"]/td/span"); ///td/span
            collection_lunch.Clear();
            foreach (HtmlNode mi in lunchtags)
            {
                MenuItem m = new MenuItem
                {
                    name = mi.InnerHtml
                };
                collection_lunch.Add(m);
            }

            //Load dinner
            HtmlNodeCollection dinnertags = document.DocumentNode.SelectNodes("//div[@id='Dinner']//tr[@class=\"menu-item\"]/td/span"); ///td/span
            collection_dinner.Clear();
            foreach (HtmlNode mi in dinnertags)
            {
                MenuItem m = new MenuItem
                {
                    name = mi.InnerHtml
                };
                collection_dinner.Add(m);
            }
        }
    }
}