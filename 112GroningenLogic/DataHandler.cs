using BaseLogic.HtmlUtil;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace _112GroningenLogic
{
    public sealed class Datahandler
    {
        public static IAsyncOperation<IList<NewsDay>> GetRegionalNews()
        {
            return LatestArticles.Instance.GetLatestArticles().AsAsyncOperation();
        }

        public static IAsyncOperation<Article> GetArticleByURL(string URL)
        {
            return ArticleHandler.Instance.GetArticleFromURL(URL).AsAsyncOperation();
        }

        public static IAsyncAction PostArticle(string URL)
        {
            return HTTPGetUtil.GetDataAsStringFromURL(URL).AsAsyncAction();
        }
    }
}
