using Coypu.Drivers.Selenium;
using Coypu;
using HtmlAgilityPack;
using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.RegularExpressions;

namespace NewsAgregator.Buisness.Parsers
{
    public class OnlinerParser : ISiteParser
    {
        public List<ArticleDto> Parse(SourceDto source)
        {
            List<ArticleDto> articles = new();
            using (var reader = XmlReader.Create(source.RssUrl))
            {
                var feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    using var browser = new BrowserSession(new SessionConfiguration
                    {
                        AppHost = "Onliner.by",
                        Browser = Coypu.Drivers.Browser.Edge,
                        SSL = true,
                        Driver = typeof(SeleniumWebDriver),
                        Timeout = TimeSpan.FromSeconds(3),
                        RetryInterval = TimeSpan.FromSeconds(0.1)
                    });
                    //Manage news page
                    browser.Visit(item.Id.ToString());
                    var element = browser.FindXPath("//body");
                    var news = element.OuterHTML;
                    var doc = new HtmlDocument();
                    doc.LoadHtml(news);
                    var content = SelectNode(doc, "//div[contains(concat(\" \", normalize-space(@class), \" \"), \" news-text \")]");
                    var endNode = SelectNode(content, "./div[@id=\"news-text-end\"] | ./hr");
                    endNode ??= SelectNode(content, "./*[@style = \"text-align: right;\"]")?.PreviousSibling;
                    if (endNode == null) continue;
                    RemoveEndNodes(endNode);
                    RemoveNodes(content, "./text() | ./script | ./div[not(contains(concat(\" \", normalize-space(@class), \" \"), \" news-media \"))] | ./p[@style]");
                    RemoveNodes(content, "./p/a | ./a", node => node.InnerText.Equals("") || node.InnerText.Equals("\r\n\r\n") || node.HasClass("news-banner"));
                    RemoveNodes(content, "./p", node => node.InnerText.Equals("") && !node.HasChildNodes);
                    ExtractMedia(content);
                    AddClassesToHtml(content);
                    var header = ExtractHeaderImage(doc);
                    var TextNodes = content.SelectNodes("//p/text()");
                    var plainText = "";
                    foreach ( var node in TextNodes)
                    {
                        plainText += node.OuterHtml + ". ";
                    }
                    plainText = Regex.Replace(plainText, @"[\r\n\t]", "");


                    //Manage rss item
                    var doc2 = new HtmlDocument();
                    doc2.LoadHtml(item.Summary.Text);
                    var description = doc2.DocumentNode;
                    RemoveNodes(description, ".//img", node => true);
                    RemoveNodes(description, "./p | ./a", node => node.InnerText.Equals(""));
                    RemoveNodes(description, "./p | ./a", node => node.InnerText.Equals(""));
                    description.LastChild.Remove();

                    var article = new ArticleDto
                    {
                        UrlHeader = header,
                        Title = item.Title.Text,
                        ShortDescription = description.FirstChild.InnerHtml,
                        Content = content.InnerHtml,
                        PlainText = plainText,
                        SourceId = source.Id,
                        IdOnSite = item.Id
                    };
                    articles.Add(article);
                }
                reader.Close();
            }
            return articles;
        }

        private static HtmlNode SelectNode(HtmlDocument document, string xPath)
            => document.DocumentNode.SelectSingleNode(xPath);
        private static HtmlNode SelectNode(HtmlNode node, string xPath)
            => node.SelectSingleNode(xPath);

        private static string ExtractHeaderImage(HtmlDocument document)
        {
            var divWithBg = SelectNode(document, "//div[contains(concat(' ',normalize-space(@class),' '),\" news-header__image \")]");
            var style = divWithBg.Attributes["style"].Value;
            if (style.Contains('\''))
            {
                return style.Substring(style.IndexOf('\'') + 1, style.LastIndexOf('\'') - style.IndexOf('\'') - 1);
            }
            return style.Substring(style.IndexOf('(') + 1, style.IndexOf(')') - style.IndexOf('(') - 1);
        }

        private static void AddClassesToHtml(HtmlNode content)
        {
            foreach(var node in content.SelectNodes(".//*")){
                node.RemoveClass();
                if (node.Attributes.Contains("src")) 
                {
                    var src = node.Attributes["src"];
                    node.Attributes.RemoveAll();
                    node.Attributes.Add(src);
                }
                if (node.Attributes.Contains("href"))
                {
                    var href = node.Attributes["href"];
                    node.Attributes.RemoveAll();
                    node.Attributes.Add(href);
                }
                switch (node.Name)
                {
                    case "img":
                        node.AddClass("col-md-12 col-xl-11 rounded");
                        break;
                    case "p":
                        node.AddClass("col-sm-12 col-md-11 col-xl-10");
                        break;
                    case "a":
                        node.AddClass("link-info link-underline-opacity-25 link-underline-opacity-100-hover");
                        break;
                    case "div":
                        node.AddClass("row mb-3 justify-content-center");
                        break;
                    case "iframe":
                        node.AddClass("ratio ratio-16x9");
                        break;
                    default:
                        break;
                }
            }
        }

        private static void RemoveEndNodes(HtmlNode endNode)
        {
            if (endNode == null) return;
            while (endNode.NextSibling != null)
            {
                endNode.NextSibling.Remove();
            }
        }

        private static void ExtractMedia(HtmlNode parentNode)
        {
            var swipers = parentNode.SelectNodes(".//div[contains(concat(\" \", normalize-space(@class), \" \"), \" news-media__gallery \")]");
            if (swipers != null)
            {
                foreach(var swiper in swipers)
                {
                    var swiperImages = swiper.SelectNodes(".//img");
                    var newDiv = "";
                    for(int i = 0; i < swiperImages.Count - 2; i++)
                    {
                        newDiv += swiperImages[i].ParentNode.OuterHtml;
                    }
                    swiper.InnerHtml = newDiv;
                }
            }
            var adds = parentNode.SelectNodes(".//a//img");
            if(adds != null)
            {
                foreach(var add in adds)
                {
                    var pNode = add;
                    while(pNode.ParentNode != parentNode)
                    {
                        pNode = pNode.ParentNode;
                    }
                    pNode.Remove();
                }
            }
            //todo manage subtitles
            //todo too man divs
            var images = parentNode.SelectNodes(".//img[contains(concat(\" \", normalize-space(@class), \" \"), \" news-media__image \")] | .//iframe");
            if(images == null) return;
            foreach(var img in images)
            {
                var pNode = img.ParentNode;
                if (pNode == parentNode) continue;
                while(pNode.ParentNode != parentNode)
                {
                    pNode = pNode.ParentNode; 
                }
                parentNode.RemoveChild(pNode, true);
            }
        }

        private static void RemoveNodes(HtmlNode parentNode, string xpath)
        {
            var nodesForDelete = parentNode.SelectNodes(xpath);
            if (nodesForDelete != null)
            {
                parentNode.RemoveChildren(nodesForDelete);
            }
        }
        private static void RemoveNodes(HtmlNode parentNode, string xpath, Func<HtmlNode, bool> predicate)
        {
            var nodesForDelete = parentNode.SelectNodes(xpath)?.Where(predicate);
            if (nodesForDelete != null)
            {
                foreach(var node in nodesForDelete)
                {
                    node.Remove();
                }
            }
        }
    }
}
