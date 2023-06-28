using HtmlAgilityPack;
using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;
using System.ServiceModel.Syndication;
using System.Xml;
using Serilog;

namespace NewsAgregator.Buisness.Parsers
{
    public class DtfParseer : ISiteParser
    {
        private readonly ILogger _logger;

        public DtfParseer(ILogger logger)
        {
            _logger = logger;
        }

        public List<ArticleDto> Parse(SourceDto source)
        {
            try{
                var articles = new List<ArticleDto>();
                using (var reader = XmlReader.Create(source.RssUrl))
                {
                    var feed = SyndicationFeed.Load(reader);
                    foreach (var item in feed.Items)
                    {
                        var idOnSite = item.Id;
                        var urlHeader = item.Summary.Text[item.Summary.Text.IndexOf("src=\"")..item.Summary.Text.IndexOf("\" width")][5..];
                        var title = item.Title.Text;
                        var shortDescription = item.Summary.Text[..item.Summary.Text.IndexOf('<')].Trim();
                        var doc = new HtmlWeb().Load(item.Links[0].Uri);
                        var content = HandleContent(doc.DocumentNode.SelectSingleNode("//div[contains(concat(\" \",normalize-space(@class),\" \"),\" content--full \")]"), item.Links);
                        var plainText = "";
                        foreach (var node in content.SelectNodes(".//p|.//li|.//h4"))
                        {
                            plainText += node.InnerText.Trim() + " ";
                        }
                        articles.Add(new ArticleDto()
                        {
                            IdOnSite = idOnSite,
                            UrlHeader = urlHeader,
                            Title = title,
                            ShortDescription = shortDescription,
                            Content = content.InnerHtml,
                            PlainText = plainText,
                            SourceId = source.Id
                        });
                    }
                };
                return articles;
            }catch(HttpRequestException ex)
            {
                _logger.Error(ex.Message, ex);
                return new List<ArticleDto>();
            }
        }
        private static HtmlNode HandleContent(HtmlNode parentNode, System.Collections.ObjectModel.Collection<SyndicationLink> links) 
        {
            var emptyNodes = parentNode.SelectNodes(".//text()[not(normalize-space())]");
            foreach (HtmlNode emptyNode in emptyNodes)
            {
                emptyNode.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(""), emptyNode);
            }
            parentNode.LastChild.Remove();
            parentNode.LastChild.Remove();
            var currentImage = 0;
            var content = HtmlNode.CreateNode("<div></div>");
            var currentNode = parentNode.FirstChild;
            do {
                switch (currentNode.Name)
                {
                    case "div":
                        if (currentNode.Attributes["class"] != null && currentNode.Attributes["class"].Value.Contains("content-info"))
                        {
                            break;
                        }
                        content.AppendChild(currentNode.FirstChild);
                        break;
                    case "figure":
                        if (currentNode.Attributes["class"] != null && currentNode.Attributes["class"].Value.Contains("figure-image"))
                        {
                            var newNode = HtmlNode.CreateNode($"<img src=\"{links[currentImage + 1].Uri}\">");
                            content.AppendChild(newNode);
                            currentImage++;
                        }
                        else if (currentNode.Attributes["class"] != null && currentNode.Attributes["class"].Value.Contains("figure-video"))
                        {
                            var newNode = HtmlNode.CreateNode($"<div class=\"ratio ratio-16x9\">\r\n<iframe src=\"{links[currentImage + 1].Uri}\" allowfull>\r\n</div>");
                            content.AppendChild(newNode);
                            currentImage++;
                        }
                        else if (currentNode.SelectSingleNode(".//blockquote") != null)
                        {
                            while (currentNode.FirstChild.Name != "p")
                                currentNode.RemoveChild(currentNode.FirstChild, true);
                            var paragrafs = currentNode.SelectNodes(".//p");
                            var text = "\"";
                            foreach (var node2 in paragrafs)
                            {
                                text += node2.InnerText;
                            }
                            text += "\" ";
                            var newNode = HtmlNode.CreateNode($"<blockquote>\r\n<p>{text}</p>\r\n<p class=\"text-end\">{currentNode.LastChild.InnerText.Trim()}</p>\r\n</blockquote>");
                            content.AppendChild(newNode);
                        }
                        else if (currentNode.SelectSingleNode(".//div[@class=\"content content--embed\"]") != null)
                        {
                            var href = currentNode.FirstChild.FirstChild.LastChild.Attributes["href"].Value;
                            var newsTitle = currentNode.FirstChild.FirstChild.ChildNodes[1].FirstChild.FirstChild.InnerText.Trim();
                            var newNode = HtmlNode.CreateNode($"<a href=\"{href}\">{newsTitle}</a>");
                            content.AppendChild(newNode);
                        }
                        else if (currentNode.SelectSingleNode(".//vue[@name=\"booster\"]") != null )
                        {
                            break;
                        }
                        else if(currentNode.SelectSingleNode(".//div[@class=\"andropov_tweet\"]") != null)
                        {
                            var text = currentNode.SelectSingleNode(".//div[@class=\"andropov_tweet__text\"]/p").InnerText;
                            var user = currentNode.SelectSingleNode(".//span[@class=\"andropov_tweet__user__name\"]").InnerText + " "+ currentNode.SelectSingleNode(".//span[@class=\"andropov_tweet__user__nickname\"]").InnerText;
                            var newNode = HtmlNode.CreateNode($"<p>\"{text}\" {user}</p>");
                            content.AppendChild(newNode);
                        }
                        else if (currentNode.HasClass("figure-gallery"))
                        {
                            var divs = currentNode.SelectNodes(".//div[@data-index]");
                            foreach( var div in divs )
                            {
                                var style = div.Attributes["style"].Value;
                                var link = style[(style.IndexOf('(') + 1)..style.IndexOf(')')];
                                var newNode = HtmlNode.CreateNode($"<img src=\"{link}\"");
                                content.AppendChild(newNode);
                            }

                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    case "#text":
                        break;
                    case "h2":
                        {
                            var newNode = HtmlNode.CreateNode($"<h4>{currentNode.FirstChild.InnerText}</h4>");
                            content.AppendChild(newNode);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                currentNode = currentNode.NextSibling;
            } while (currentNode != null);
            return content;
        }
    }
}

