using _112_Groningen.Common;
using _112GroningenLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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

        private string CurrentURL = string.Empty;
        private bool StopRefresh = false;

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

            if (e.NavigationParameter != null && e.NavigationParameter.ToString() != "")
            {
                try
                {
                    await this.OpenNewsItem(e.NavigationParameter.ToString());

                    return;
                }
                catch
                {

                }
            }
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
                        //NotificationHandler.Run();
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

                    if (News.Count > 0 && News.First().Articles.Count > 0)
                    {
                        await this.OpenNewsItem(News.First().Articles.First().URL);
                    }

                    Task RefreshTask = Task.Run(() => this.RefreshData());

                    LastLoadedDT = DateTime.Now;
                }
                catch (Exception)
                {
                    LoadingControl.DisplayLoadingError(true);
                }
            }

            LoadingControl.SetLoadingStatus(false);
        }

        private async Task OpenNewsItem(string URL)
        {
            try
            {
                NewsItemControl.DataContext = null;
                NewsItemLoadingControl.DisplayLoadingError(false);
                NewsItemLoadingControl.SetLoadingStatus(true);

                if (URL != null)
                {
                    CurrentURL = URL;
                    Article newsItem = await Datahandler.GetArticleByURL(URL);
                    NewsItemControl.DataContext = newsItem;
                }
            }
            catch
            {
                NewsItemLoadingControl.DisplayLoadingError(true);
            }
            finally
            {
                NewsItemLoadingControl.SetLoadingStatus(false);
            }

            ArticleCounter.AddArticleCount();
            Task t = Task.Run(() => Datahandler.PostArticle(URL));
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

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await this.OpenNewsItem((e.ClickedItem as ArticleURL).URL);
        }

        private async Task RefreshData()
        {
            while (!StopRefresh)
            {
                await Task.Delay(300000);
                
                List<NewsDay> News = (List<NewsDay>)await Datahandler.GetRegionalNews();

                if ((News.Count > 0 && News.First().Articles.Count > 0 &&
                    News.First().Articles.First().URL != (NewsLV.ItemsSource as List<NewsDay>).First().Articles.First().URL)
                        || (NewsLV.ItemsSource as List<NewsDay>).Count == 0)
                {
                    NewsLV.ItemsSource = News;
                }
            }
        }
    }
}
