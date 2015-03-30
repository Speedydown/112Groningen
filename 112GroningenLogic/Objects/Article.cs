using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace _112GroningenLogic
{
    public sealed class Article
    {
        public string Title { get; set; }
        public string AuthorDate { get; set; }
        public string Location { get; set; }
        public IList<string> Content { get; set; }
        public IList<string> ImageList { get; set; }

        public Article(string Title, string AuthorDate, string Location, IList<string> Content, IList<string> ImageList)
        {
            this.Title = HTMLParserUtil.CleanHTMLString(Title);
            this.AuthorDate = HTMLParserUtil.CleanHTMLString(AuthorDate).Trim();
            this.Location = Location;
            this.Content = Content;
            this.ImageList = ImageList;

            if (Content.Count >0)
            {
                Content[0] = Location + " " + Content[0];
            }
        }
    }
}
