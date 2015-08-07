using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace _112GroningenLogic
{
    public static class NotificationDataHandler
    {
        public static IAsyncOperation<IList<ArticleURL>> GenerateNotifications()
        {
            return GenerateNotificationsHelper().AsAsyncOperation();
        }

        private static async Task<IList<ArticleURL>> GenerateNotificationsHelper()
        {
            try
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;
                IList<NewsDay> News = await Datahandler.GetRegionalNews();

                IList<ArticleURL> ArticleURL = new List<ArticleURL>();

                string LastURL = string.Empty;

                if (localSettings.Values["LastNewsItem"] != null)
                {
                    LastURL = localSettings.Values["LastNewsItem"].ToString();
                }
                else
                {
                    return ArticleURL;
                }

                int NotificationCounter = 0;

                foreach (NewsDay n in News)
                {
                    foreach (ArticleURL au in n.Articles)
                    {
                        if (au.URL == LastURL)
                        {
                            if (NotificationCounter > 0)
                            {
                                return (ArticleURL);
                            }
                        }

                        ArticleURL.Add(au);
                        NotificationCounter++;
                    }
                }
            }
            catch (Exception)
            {

            }

            return new List<ArticleURL>();
        }
    }
}
