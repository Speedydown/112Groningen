using BaseLogic.HtmlUtil;
using System.Collections.Generic;

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
