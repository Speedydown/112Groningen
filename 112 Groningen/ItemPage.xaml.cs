﻿using _112_Groningen.Common;
using _112GroningenLogic;
using BaseLogic.ArticleCounter;
using BaseLogic.ClientIDHandler;
using BaseLogic.Utils;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace _112_Groningen
{
    public sealed partial class ItemPage : Page
    {
        private string CurrentURL = string.Empty;
        RelayCommand _checkedGoBackCommand;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ItemPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            _checkedGoBackCommand = new RelayCommand(
                                    () => this.CheckGoBack(),
                                    () => this.CanCheckGoBack()
                                );

            navigationHelper.GoBackCommand = _checkedGoBackCommand;
        }

        private bool CanCheckGoBack()
        {
            return true;
        }

        private void CheckGoBack()
        {
            if (NewsItemControl.CanGoBack())
            {
                NavigationHelper.GoBack();
            }
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
            try
            {
                LoadingControl.SetLoadingStatus(true);

                if (e.NavigationParameter != null)
                {
                    CurrentURL = (string)e.NavigationParameter;
                    Article article = await Datahandler.GetArticleByURL((string)e.NavigationParameter);
                    this.DataContext = article;
                }
                else
                {
                    LoadingControl.DisplayLoadingError(true);
                }
            }
            catch
            {
                LoadingControl.DisplayLoadingError(true);
            }
            finally
            {
                LoadingControl.SetLoadingStatus(false);
            }
            
            await ArticleCounter.AddArticleCount("Wij bieden 112Groningen kostenloos aan en we zouden het op prijs stellen als u de 112Groningen app een positieve review geeft.", "Bedankt");

            if (e.NavigationParameter != null)
            {
                Task Notifier = Task.Run(async () => await ClientIDHandler.instance.PostAppStats(ClientIDHandler.AppName._112Groningen));
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            ShareToolkit.ShareContentAsString(CurrentURL, "Gedeeld met 112Groningen voor Windows Phone", string.Empty);
        }

    }
}


    