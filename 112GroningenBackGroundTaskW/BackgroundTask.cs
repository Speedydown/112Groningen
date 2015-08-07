﻿using _112GroningenLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace _112GroningenBackGroundTaskW
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            IList<ArticleURL> Content = await NotificationDataHandler.GenerateNotifications();
            ToastHandler.CreateToast(Content);

            if (Content.Count > 0)
            {
                CreateTile(Content, Content.Count);
            }

            deferral.Complete();
        }

        private void CreateTile(IList<ArticleURL> Content, int Counter)
        {
            //LargeTile
            XmlDocument RectangleTile = CreateRectangleTile(Content, Counter);
            XmlDocument SquareTile = CreateSquareTile(Content);
            XmlDocument SmallTile = CreateSmallTile(Content);

            //Badges
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);
            XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
            badgeElement.SetAttribute("value", Counter.ToString());

            BadgeNotification badge = new BadgeNotification(badgeXml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

            //Add tiles together
            IXmlNode node = RectangleTile.ImportNode(SquareTile.GetElementsByTagName("binding").Item(0), true);
            RectangleTile.GetElementsByTagName("visual").Item(0).AppendChild(node);

            node = RectangleTile.ImportNode(SmallTile.GetElementsByTagName("binding").Item(0), true);
            RectangleTile.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(RectangleTile);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        private XmlDocument CreateRectangleTile(IList<ArticleURL> Content, int Counter)
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text01);
            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");


            try
            {
                tileTextAttributes[0].InnerText = "Laatste nieuws;";
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[1].InnerText = Content[0].Title;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[2].InnerText = Content[1].Title;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[3].InnerText = Content[2].Title;
            }
            catch
            {

            }

            return tileXml;
        }

        private XmlDocument CreateSquareTile(IList<ArticleURL> Content)
        {
            int ContentCounter = 9;

            if (Content.Count < 9)
            {
                ContentCounter = Content.Count;
            }

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310Text01);

            XmlNodeList tileTextAttributes = squareTileXml.GetElementsByTagName("text");


            try
            {
                tileTextAttributes[0].InnerText = "Laatste nieuws:";
            }
            catch
            {

            }

            for (int i = 0; i < ContentCounter; i++)
            {
                try
                {
                    tileTextAttributes[i + 1].InnerText = Content[i].Title;
                }
                catch
                {

                }
            }



            return squareTileXml;
        }

        private XmlDocument CreateSmallTile(IList<ArticleURL> Content)
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText03);
            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/Logo.scale-100.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "112Groningen.nl");

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");

            try
            {
                tileTextAttributes[0].InnerText = "Laatste nieuws:";
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[1].InnerText = Content[0].Title;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[2].InnerText = Content[1].Title;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[3].InnerText = Content[2].Title;
            }
            catch
            {

            }

            return tileXml;
        }
    }
}
