using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace _112GroningenLogic
{
    internal class ArticleHandler
    {
        public readonly static ArticleHandler Instance = new ArticleHandler();

        private ArticleHandler()
        {

        }

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
                    Content.Add(this.CleanContent(HTMLParserUtil.GetContentAndSubstringInput("<div class=\"artikel_tekst\">", "</div>", Source, out Source)));
                }
                catch (Exception)
                {
                    break;
                }
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
            if (Content.Contains("<strong>"))
            {
                Content = Content.Insert(Content.IndexOf("<strong>"), "\n\n");
            }


            Content = HTMLParserUtil.CleanHTMLString(Content);
            Content = Content.Replace("<p>", "");
            Content = Content.Replace("</p>", "");
            Content = Content.Replace(";", "");

            string TransFormedContent = string.Empty;

            try
            {
                TransFormedContent += Content.Substring(0, Content.IndexOf("<"));
                Content = Content.Substring(Content.IndexOf("<") + 1);

                while (true)
                {
                    try
                    {
                        if (Content.Contains("<"))
                        {
                            if (!(TransFormedContent.Last() == '\n'))
                            {
                                TransFormedContent += HTMLParserUtil.GetContentAndSubstringInput(">", "<", Content, out Content).Trim() + "\n";
                            }
                            else
                            {
                                TransFormedContent += HTMLParserUtil.GetContentAndSubstringInput(">", "<", Content, out Content);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        TransFormedContent += Content;
                        break;
                    }
                }
            }

            catch
            {
                TransFormedContent += Content;
            }

            while (TransFormedContent.Contains("http"))
            {
                string Temp = TransFormedContent.Substring(0, TransFormedContent.IndexOf("http"));
                string TailOfArticle = TransFormedContent.Substring(TransFormedContent.IndexOf("http"));
                Temp += TailOfArticle.Substring(TailOfArticle.IndexOf(" "));
                TransFormedContent = Temp;
            }

            TransFormedContent = TransFormedContent.Trim().Replace("      ", "\n\n").Replace("\r", "").Replace("\n ", "\n\n").Replace(".\n", "\n\n");

            while (TransFormedContent.Contains("\n\n\n"))
            {
                TransFormedContent = TransFormedContent.Replace("\n\n\n", "\n\n");
            }

            return TransFormedContent;
        }
    }
}
