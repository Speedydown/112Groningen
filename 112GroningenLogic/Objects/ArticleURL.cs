using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;
using WRCHelperLibrary;

namespace _112GroningenLogic
{
    public sealed class ArticleURL : INewsLink
    {
        public string Title { get; set; }
        public string StatusImageURL { get; set; }
        public string URL { get; set; }

        public string ImageURL
        {
            get { return string.Empty; }
        }

        public string Content
        {
            get { return string.Empty; }
        }

        public string CommentCount
        {
            get { return string.Empty; }
        }

        public string Time
        {
            get { return string.Empty; }
        }

        public ArticleURL(string Title, string StatusImageURL, string URL)
        {
            this.Title = HTMLParserUtil.CleanHTMLString(Title);
            this.StatusImageURL = StatusImageURL;
            this.URL = "http://112groningen.nl/" + URL;
        }


        
    }
}
