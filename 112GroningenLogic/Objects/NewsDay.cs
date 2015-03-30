using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace _112GroningenLogic
{
    public sealed class NewsDay
    {
        public string Header { get; private set; }
        public IList<ArticleURL> Articles { get; set; }
        
        public NewsDay(string Header, IList<ArticleURL> Articles)
        {
            this.Header = HTMLParserUtil.CleanHTMLString(Header);
            this.Articles = Articles;
        }
    }
}
