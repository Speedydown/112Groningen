using _112_Groningen.Common;
using _112GroningenLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace _112_Groningen
{
    public sealed partial class ItemPage : Page
    {
        RelayCommand _checkedGoBackCommand;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private bool FullsizeImage = false;

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
            if (this.FullsizeImage)
            {
                this.FullsizeImage = false;
                ContentScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                FullImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                FullImageScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
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
            ErrorGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            string URL = (string)e.NavigationParameter;

            try
            {
                Article article = await Datahandler.GetArticleByURL(URL);
                LayoutRoot.DataContext = article;
                ArticleCounter.AddArticleCount();
            }
            catch (Exception)
            {
                ErrorGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            LoadingBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Task t = Task.Run(() => Datahandler.PostArticle("http://speedydown-001-site2.smarterasp.net/api.ashx?Groningen=" + URL));
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

        private void ImagesListview_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.FullsizeImage = true;
            ContentScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            FullImage.Source = new BitmapImage(new Uri(e.ClickedItem as string));
            FullImageScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            FullImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void FullImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.FullsizeImage = false;
            ContentScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            FullImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            FullImageScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}


    