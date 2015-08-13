using _112GroningenLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using WRCHelperLibrary;

namespace _112GroningenBackGroundTaskWP
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            IList<ArticleURL> Content = await NotificationDataHandler.GenerateNotifications();

            if (Content.Count > 0)
            {
                CreateTiles(Content.Cast<INewsLink>().ToList(), Content.Count);
                BadgeHandler.CreateBadge(Content.Count());
            }

            deferral.Complete();
        }

        private void CreateTiles(IList<INewsLink> Content, int Counter)
        {
            XmlDocument RectangleTile = TileXmlHandler.CreateRectangleTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150IconWithBadgeAndText), Content, Counter, "ms-appx:///assets/71x71Badge.png", "112Groningen");
            XmlDocument SquareTile = TileXmlHandler.CreateSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150IconWithBadge), Content, "ms-appx:///assets/71x71Badge.png", "112Groningen");
            XmlDocument SmallTile = TileXmlHandler.CreateSmallSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare71x71IconWithBadge), "ms-appx:///assets/71x71Badge.png", "112Groningen");

            TileXmlHandler.CreateTileUpdate(new XmlDocument[] { RectangleTile, SquareTile, SmallTile });
        }
    }
}
