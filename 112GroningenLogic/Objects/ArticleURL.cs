using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace _112GroningenLogic
{
    public sealed class ArticleURL
    {
        public string Title { get; set; }
        public string StatusImageURL { get; set; }
        public string URL { get; set; }

        public ArticleURL(string Title, string StatusImageURL, string URL)
        {
            this.Title = HTMLParserUtil.CleanHTMLString(Title);
            this.StatusImageURL = StatusImageURL;
            this.URL = "http://112groningen.nl/" + URL;
        }
    }
}
