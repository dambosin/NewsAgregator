using Coypu.Drivers.Selenium;
using Coypu;
using HtmlAgilityPack;
using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NewsAgregator.Buisness.Parsers
{
    public class OnlinerParser : ISiteParser
    {
        public async Task<List<ArticleCreateDto>> Parse(SourceDto source)
        {
            List<ArticleCreateDto> articles = new();
            using (var reader = XmlReader.Create(source.RssUrl))
            {
                var feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    using (var browser = new BrowserSession(new SessionConfiguration
                    {
                        AppHost = "Onliner.by",
                        Browser = Coypu.Drivers.Browser.Edge,
                        SSL = true,
                        Driver = typeof(SeleniumWebDriver),
                        Timeout = TimeSpan.FromSeconds(3),
                        RetryInterval = TimeSpan.FromSeconds(0.1)
                    }))
                    {
                        browser.Visit(item.Id.ToString());
                        var element = browser.FindXPath("//body");
                        var news = element.OuterHTML;
                        var doc = new HtmlDocument();
                        doc.LoadHtml(news);
                        // todo: load title image for detail news from bg-image
                        // todo: manage carousel
                        var content = doc.DocumentNode.SelectSingleNode("//*[@class=\"news-text\"]");
                        var endNode = content.SelectSingleNode("./div[@id=\"news-text-end\"] | ./hr");
                        RemoveEndNodes(endNode);
                        RemoveNodes(content, "./text() | ./script | ./div[not(contains(@class, \"news-media\"))] | ./p[@style]");
                        RemoveNodes(content, "./p/a | ./a", node => node.InnerText.Equals("") || node.InnerText.Equals("\r\n\r\n") || node.HasClass("news-banner"));
                        RemoveNodes(content, "./p", node => node.InnerText.Equals(""));
                        ExtractMedia(content);
                        AddClassesToHtml(content);
                        var header = ExtractHeaderImage(doc, content);
                        var doc2 = new HtmlDocument();
                        doc2.LoadHtml(item.Summary.Text);
                        var description = doc2.DocumentNode;
                        string thumbnail = description.SelectSingleNode("p/a/img").Attributes["src"].Value;
                        RemoveNodes(description, ".//img", node => true);
                        RemoveNodes(description, "./p | ./a", node => node.InnerText.Equals(""));
                        RemoveNodes(description, "./p | ./a", node => node.InnerText.Equals(""));
                        description.LastChild.Remove();
                        var article = new ArticleCreateDto
                        {
                            UrlHeader = header,
                            UrlThumbnail = thumbnail,
                            Title = item.Title.Text,
                            ShortDescription = description.InnerHtml,
                            Content = content.InnerHtml,
                            PositiveIndex = 0,
                            Created = DateTime.Now,
                            LikesCount = 0,
                            SourceId = source.Id,
                            IdOnSite = item.Id
                        };
                        articles.Add(article);
                        Console.Write("adjfdafadf");
                   }
                }
                reader.Close();
            }
            return articles;
        }

        private static string ExtractHeaderImage(HtmlDocument document, HtmlNode content)
        {
            var divWithBg = document.DocumentNode.SelectSingleNode("//div[contains(concat(' ',normalize-space(@class),' '),\" news-header__image \")]");
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
                        node.ParentNode.AddClass("row text-center mb-3 justify-content-center");
                        node.AddClass("col-md-12 col-xl-11 rounded");
                        break;
                    case "p":
                        node.AddClass("col-sm-12 col-md-11 col-xl-10");
                        break;
                    case "a":
                        node.AddClass("link-info link-underline-opacity-25 link-underline-opacity-100-hover");
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
            var nodesWithImage = parentNode.SelectNodes(".//div[contains(concat(\" \", normalize-space(@class), \" \"), \" news-media \")]");
            if (nodesWithImage == null)
            {
                return;
            }
            foreach (var node in nodesWithImage)
            {
                if (node.SelectSingleNode(".//img | .//iframe").ParentNode.Name.Equals("a"))
                {
                    node.Remove();
                    continue;
                }
                if(node.SelectNodes(".//div[contains(concat(\" \", normalize-space(@class),\" \"), \" js-swiper \")]") != null)
                {
                    var images = node.SelectNodes(".//img");
                    images[^1].Remove();
                    images[^1].Remove();
                    var newDiv = "";
                    foreach(var image in images)
                    {
                        newDiv += image.OuterHtml;
                    }
                    node.InnerHtml = newDiv;
                }
                else
                {
                    node.InnerHtml = node.SelectSingleNode(".//img | .//iframe").OuterHtml;
                }
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
            var nodesForDelete = parentNode.SelectNodes(xpath).Where(predicate);
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
