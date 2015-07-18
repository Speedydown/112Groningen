using _112_Groningen.Common;
using _112GroningenLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace _112_Groningen
{
    public sealed partial class MainPage : Page
    {
        private static DateTime? LastLoadedDT = null;
        public static MainPage Instance { get; private set; }
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            Instance = this;
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            await this.HandleData();
        }

        public async Task HandleData(bool OverrideTimer = false)
        {
            LoadingControl.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true);

            if (OverrideTimer)
            {
                LastLoadedDT = DateTime.Now.AddHours(-1);
            }

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            this.NewsLV.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if (LastLoadedDT == null || DateTime.Now.Subtract((DateTime)LastLoadedDT).TotalMinutes > 5)
            {
                try
                {
                    List<NewsDay> News = (List<NewsDay>)await Datahandler.GetRegionalNews();

                    if (News.Count == 0)
                    {
                        throw new Exception("No items");
                    }

                    NewsLV.ItemsSource = News;

                    if (LastLoadedDT == null)
                    {
                        NotificationHandler.Run();
                    }

                    ApplicationData applicationData = ApplicationData.Current;
                    ApplicationDataContainer localSettings = applicationData.LocalSettings;

                    try
                    {
                        localSettings.Values["LastNewsItem"] = News.First().Articles.First().URL;
                    }
                    catch
                    {

                    }

                    LastLoadedDT = DateTime.Now;
                }
                catch (Exception)
                {
                    LoadingControl.DisplayLoadingError(true);
                }
            }

            LoadingControl.SetLoadingStatus(false);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
           
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!Frame.Navigate(typeof(ItemPage), (e.ClickedItem as ArticleURL).URL))
            {

            }
        }

        private async void GroningenButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://112groningen.nl"));
        }

        private async void PrivacyPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://wiezitwaarvandaag.nl/privacypolicy.aspx"));
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshButton.IsEnabled = false;
            this.NewsLV.ItemsSource = null;
            LastLoadedDT = null;
            await this.HandleData();
            this.RefreshButton.IsEnabled = true;
        }
    }
}
