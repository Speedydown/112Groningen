using _112GroningenLogic;
using BaseLogic.Notifications;
using BaseLogic.Xaml_Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            
            if (Content.Count > 0)
            {
                ToastHandler.CreateToast(Content);
                CreateTiles(Content.Cast<INewsLink>().ToList(), Content.Count);
                BadgeHandler.CreateBadge(Content.Count());
            }

            deferral.Complete();
        }

        private void CreateTiles(IList<INewsLink> Content, int Counter)
        {
            XmlDocument RectangleTile = TileXmlHandler.CreateRectangleTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text01), Content, Counter, "ms-appx:///assets/Logo.scale-100.png", "112Groningen.nl", "Laatste nieuws");
            XmlDocument SquareTile = TileXmlHandler.CreateLargeSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310Text01), Content);
            XmlDocument SmallTile = TileXmlHandler.CreateSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText03), Content, "ms-appx:///assets/Logo.scale-100.png", "112Groningen.nl", "Laatste nieuws");

            TileXmlHandler.CreateTileUpdate(new XmlDocument[] { RectangleTile, SquareTile, SmallTile });
        }
    }
}
