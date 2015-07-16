﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using WRCHelperLibrary;

namespace _112GroningenLogic
{
    public sealed class Article : INewsItem
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public string Author { get; private set; }
        public IList<string> Body { get; private set; }
        public IList<string> ImageList { get; set; }
        public Uri MediaFile { get; private set; }
        public string Added { get; private set; }
        public string Updated { get; private set; }

        public string TimeStamp
        {
            get
            {
                return string.Empty;
            }
        }

        public Brush TitleColor
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb((byte)255, (byte)89, (byte)126, (byte)170));
            }
        }

        public Visibility MediaVisibilty
        {
            get
            {
                return MediaFile == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility TimeStampVisibilty
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        public Visibility SummaryVisibilty
        {
            get
            {
                return Visibility.Visible;
            }
        }

        public Thickness ContentMargins
        {
            get
            {
                return new Thickness(4, 15, 4, 5);
            }
        }

        public Uri YoutubeURL { get; private set; }
        public Visibility DisplayWebView
        {
            get
            {
                return YoutubeURL == null ? Visibility.Collapsed : Visibility.Visible;
            }

        }

        public string ContentSummary { get; private set; }

        public Article(string Title, string Date, string Author, string Location, IList<string> Body, IList<string> ImageList, string MediaFile)
        {
            this.Author = (HTMLParserUtil.CleanHTMLString(Date).Trim() + ", " + HTMLParserUtil.CleanHTMLString(Author).Trim());
            this.Title = HTMLParserUtil.CleanHTMLString(Title);
            this.Location = Location;

            if (Body.Count > 0)
            {
                Body[0] = Location + " " + Body[0];
            }

            if (Body.Count == 2)
            {
                this.ContentSummary = Body.First();
                Body.RemoveAt(0);
            }

            this.Body = Body;
            this.ImageList = ImageList;

            if (MediaFile.Length != 0)
            {
                this.YoutubeURL = new Uri(MediaFile);
            }
        }
    }
}
