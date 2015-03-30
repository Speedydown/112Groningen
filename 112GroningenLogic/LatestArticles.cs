using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace _112GroningenLogic
{
    internal class LatestArticles
    {
        public readonly static LatestArticles Instance = new LatestArticles();

        private LatestArticles()
        {

        }

        public async Task<IList<NewsDay>> GetLatestArticles()
        {
            List<NewsDay> Articles = new List<NewsDay>();

            try
            {
                string Source = await HTTPGetUtil.GetDataAsStringFromURL("http://112groningen.nl/");
                Source = this.StripHeader(Source);
                return this.GetNews(Source);

            }
            catch
            {
                return Articles;
            }

        }

        private string StripHeader(string Source)
        {
            return Source.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"articleblox\">", Source));
        }

        private List<NewsDay> GetNews(string Source)
        {
            List<NewsDay> NewsDays = new List<NewsDay>();

            while (true)
            {
                int PositionOfTable = HTMLParserUtil.GetPositionOfStringInHTMLSource("<table class=\"overig\" align=\"left\" cellspacing=\"0\" cellpadding=\"0\"", Source);

                if (PositionOfTable == -1)
                {
                    break;
                }

                //Cut to start of table
                Source = Source.Substring(PositionOfTable);

                try
                {
                    string Header = HTMLParserUtil.GetContentAndSubstringInput("<thead><tr><td colspan=\"3\">", "</td></tr></thead>", Source, out Source);

                    int EndIndexOfDay = HTMLParserUtil.GetPositionOfStringInHTMLSource("<table class=\"overig\" align=\"left\" cellspacing=\"0\" cellpadding=\"0\"", Source, false);

                    string DaySource = string.Empty;

                    try
                    {
                        DaySource = Source.Substring(0, EndIndexOfDay);
                    }
                    catch
                    {
                        DaySource = Source;
                    }

                    List<ArticleURL> Articles = this.GetArticleUrlsFromDaySource(DaySource);

                    NewsDays.Add(new NewsDay(Header, Articles));
                }
                catch
                {
                    break;
                }
            }

            return NewsDays;
        }

        private List<ArticleURL> GetArticleUrlsFromDaySource(string DaySource)
        {
            List<ArticleURL> Articles = new List<ArticleURL>();

            while (true)
            {
                try
                {
                    string Header = HTMLParserUtil.GetContentAndSubstringInput("<tr title=\"", "\"><td>", DaySource, out DaySource);
                    string CategoryImage = HTMLParserUtil.GetContentAndSubstringInput("<img src=\"", "\" />", DaySource, out DaySource);
                    DaySource = DaySource.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<a href=\"", DaySource, false));
                    string URL = HTMLParserUtil.GetContentAndSubstringInput("<a href=\"", "\">", DaySource, out DaySource);

                    Articles.Add(new ArticleURL(Header, CategoryImage, URL));
                }
                catch (Exception)
                {
                    break;
                }
            }

            return Articles;
        }
    }
}
