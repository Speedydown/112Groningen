using BaseLogic.HtmlUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _112GroningenLogic
{
    internal class ArticleHandler
    {
        public readonly static ArticleHandler Instance = new ArticleHandler();

        public async Task<Article> GetArticleFromURL(string URL)
        {
            try
            {
                string Source = await HTTPGetUtil.GetDataAsStringFromURL(URL);
                Source = this.StripHeader(Source, "<div id=\"artikel\">");
                return this.GetArticle(Source);

            }
            catch
            {
                return null;
            }

        }

        private string StripHeader(string Source, string Header)
        {
            return Source.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource(Header, Source, false));
        }

        private Article GetArticle(string Source)
        {
            List<string> Images = new List<string>();

            Images.Add(HTMLParserUtil.GetContentAndSubstringInput("<a href=\"", "\" class=", Source, out Source));
            string Author = HTMLParserUtil.GetContentAndSubstringInput("<div id=\"article_info\">", "</div>", Source, out Source);
            string Header = HTMLParserUtil.GetContentAndSubstringInput("<h2>", "</h2>", Source, out Source);
            string Location = HTMLParserUtil.GetContentAndSubstringInput("<strong>", "</strong>", Source, out Source, string.Empty, false);

            List<string> Content = new List<string>();

            Content.Add(this.CleanContent(HTMLParserUtil.GetContentAndSubstringInput("</strong>", "</div>", Source, out Source)));

            while (true)
            {
                try
                {
                    string NewsParagraph = this.CleanContent(HTMLParserUtil.GetContentAndSubstringInput("<div class=\"artikel_tekst\">", "</div>", Source, out Source));

                    if (NewsParagraph.Length > 0)
                    {
                        Content.Add(NewsParagraph);
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        Source = Source.Substring(Source.IndexOf("<p class=\"introductie\">"));
                        string NewsParagraph = this.CleanContent(HTMLParserUtil.GetContentAndSubstringInput("<p class=\"introductie\">", "</p>", Source, out Source));

                        if (NewsParagraph.Length > 0)
                        {
                            Content.Add(NewsParagraph);
                        }

                        NewsParagraph = this.CleanContent(HTMLParserUtil.GetContentAndSubstringInput("<div>", "</div>", Source, out Source));

                        if (NewsParagraph.Length > 0)
                        {
                            Content.Add(NewsParagraph);
                        }
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }

            try
            {
                string NewsParagraph = this.CleanContent(HTMLParserUtil.GetContentAndSubstringInput("<p align=\"left\">", "<div id=\"artikel_videos\">", Source, out Source));

                    if (NewsParagraph.Length > 0)
                    {
                        Content.Add(NewsParagraph);
                    }
                
            }
            catch
            {

            }

            string Youtube = "";

            try
            {
                Youtube = "https://www.youtube.com/embed/" + HTMLParserUtil.GetContentAndSubstringInput("https://www.youtube.com/embed/", "\" frameborder", Source, out Source);
            }
            catch
            {

            }

            Source = this.StripHeader(Source, "<div id=\"artikel_fotos\">");

            while (true)
            {
                try
                {
                    Images.Add(HTMLParserUtil.GetContentAndSubstringInput("<a href=\"", "\" class=\"lightbox\"", Source, out Source));
                }
                catch (Exception)
                {
                    break;
                }
            }

            string[] SplittedTimeStamp = Author.Split(',');

            return new Article(Header, SplittedTimeStamp.First(), SplittedTimeStamp.Last(), Location, Content, Images, Youtube);
        }

        private string CleanContent(string Content)
        {
            Content = Content.Replace("</p><p>", " ");
            Content = Content.Replace(" &nbsp; ", "\n\n");
            Content = Content.Replace("&nbsp;", " ");
            Content = Content.Replace("&nbsp; ", " ");
            Content = Content.Replace("<p><strong>", " ");
            Content = Content.Replace("</strong></p>", " ");
            Content = Content.Replace("   ", " ");
            Content = Content.Replace("  ", " ");

            Content = HTMLParserUtil.CleanHTMLString(Content);
            Content = HTMLParserUtil.CleanHTMLTagsFromString(Content);
            Content = HTMLParserUtil.CleanHTTPTagsFromInput(Content);
            Content = Content.Trim().Replace("      ", "\n\n").Replace("\n ", "\n\n").Replace(".\n", "\n\n");
            Content = HTMLParserUtil.CleanDoubleBreakLinesFromInput(Content);

            return Content;
        }
    }
}
