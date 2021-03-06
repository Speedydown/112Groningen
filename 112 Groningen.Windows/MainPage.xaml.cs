﻿using _112_Groningen.Common;
using _112GroningenLogic;
using BaseLogic.ArticleCounter;
using BaseLogic.ClientIDHandler;
using BaseLogic.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
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
                        NotificationHandler.Run("_112GroningenBackGroundTaskW.BackgroundTask", "_112GroningenBackGroundWorker", 15);
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

            await ArticleCounter.AddArticleCount("Wij bieden 112Groningen kostenloos aan en we zouden het op prijs stellen als u de 112Groningen app een positieve review geeft.", "Bedankt");
            Task Notifier = Task.Run(async () => await ClientIDHandler.instance.PostAppStats(ClientIDHandler.AppName._112Groningen));
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            StopRefresh = true;
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
                await Task.Delay(150000);

                List<NewsDay> News = (List<NewsDay>)await Datahandler.GetRegionalNews();

                try
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if ((News.Count > 0 && News.First().Articles.Count > 0 &&
                            News.First().Articles.First().URL != (NewsLV.ItemsSource as List<NewsDay>).First().Articles.First().URL)
                                || (NewsLV.ItemsSource as List<NewsDay>).Count == 0)
                        {
                            NewsLV.ItemsSource = News;
                        }
                    });
                }
                catch
                {

                }
            }
        }
    }
}
