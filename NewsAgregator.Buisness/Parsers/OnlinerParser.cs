using HtmlAgilityPack;
using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using Microsoft.Identity.Client;

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
                    var idOnsite = item.Id;
                    var title = item.Title.Text;
                    var shortDescription = item.Summary.Text[(item.Summary.Text.IndexOf("</p><p>") + 7)..(item.Summary.Text.LastIndexOf("</p><p>"))];
                    var sourceId = source.Id;

                    var doc = new HtmlWeb().Load(idOnsite);
                    var style = doc.DocumentNode.SelectSingleNode(".//div[@class=\"news-header__image\"]").Attributes["style"].Value;
                    string urlHeader = "";
                    if (style.Contains("('"))
                    {
                        urlHeader = style[(style.IndexOf("('") + 2)..style.IndexOf("')")];
                    }
                    else
                    {
                        urlHeader = style[(style.IndexOf("(") + 1)..style.IndexOf(")")];
                    }
                    var content = HandleContent(doc.DocumentNode.SelectSingleNode(".//div[@class=\"news-text\"]"));
                    var plainText = "";
                    foreach (var node in content.SelectNodes(".//p|.//li|.//h4"))
                    {
                        plainText += node.InnerText.Trim() + " ";
                    }
                    articles.Add(new ArticleDto()
                    {
                        IdOnSite = idOnsite,
                        Title = title,
                        UrlHeader = urlHeader,
                        ShortDescription = shortDescription,
                        SourceId = sourceId,
                        Content = content.InnerHtml,
                        PlainText = plainText
                    });
                }
            }
            return articles;
        }

        private HtmlNode HandleContent(HtmlNode parentNode)
        {
            var emptyNodes = parentNode.SelectNodes(".//text()[not(normalize-space())]");
            foreach (HtmlNode emptyNode in emptyNodes)
            {
                emptyNode.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(""), emptyNode);
            }
            var currentNode = parentNode.FirstChild;
            var content = HtmlNode.CreateNode("<div></div>");
            while(currentNode != null)
            {
                switch(currentNode.Name) 
                {
                    case "p":
                        if ((currentNode.FirstChild != null && currentNode.FirstChild.HasClass("attachment"))
                            || (currentNode.Attributes["style"] != null)
                            || (currentNode.ChildNodes.Count == 1 && currentNode.FirstChild!.Name.Equals("a") && !currentNode.FirstChild.HasChildNodes))
                        {
                        }
                        else if(currentNode.ChildNodes.Count == 1 && currentNode.FirstChild!.Name.Equals("strong"))
                        {
                            var newNode = HtmlNode.CreateNode($"<h4>{currentNode.FirstChild.InnerHtml}</h4>");
                            content.AppendChild(newNode);
                        }
                        else if (currentNode.SelectNodes(".//img[contains(concat(\" \",normalize-space(@class),\" \"),\" news-media__image \")]") != null)
                        {
                            var imgs = currentNode.SelectNodes(".//img[contains(concat(\" \",normalize-space(@class),\" \"),\" news-media__image \")]");
                            foreach (var img in imgs)
                            {
                                if (img.Attributes["data-src"] == null)
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{img.Attributes["src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                                else
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{img.Attributes["data-src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                            }
                        }
                        else
                        {
                            var newNode = HtmlNode.CreateNode($"<p>{currentNode.InnerHtml}</p>");
                            content.AppendChild(newNode);
                        }
                        break;
                    case "div":
                        if ((currentNode.Id != null && !currentNode.Id.Equals("news-text-end") && !currentNode.Id.Equals("")) 
                            || currentNode.SelectSingleNode(".//a[@href=\"https://t.me/autoonliner\"]") != null
                            || currentNode.HasClass("news-reference")
                            || currentNode.HasClass("news-widget")
                            || currentNode.HasClass("news-promo")
                            || currentNode.HasClass("news-header")
                            || currentNode.HasClass("news-entry")
                            || currentNode.HasClass("news-banner")
                            || currentNode.HasClass("twitter-media")
                            || currentNode.HasClass("news-incut"))
                        {
                        }
                        else if (currentNode.Id != null && currentNode.Id.Equals("news-text-end"))
                        {
                            while(currentNode.NextSibling != null)
                            {
                                currentNode.NextSibling.Remove();
                            }
                        }else if(currentNode.SelectSingleNode(".//div[contains(concat(\" \",normalize-space(@class),\" \"),\" js-swiper \")]") !=null)
                        {
                            var imgs = currentNode.SelectNodes(".//img[contains(concat(\" \",normalize-space(@class),\" \"),\" news-media__image \")]");
                            for(int i = 0;i < imgs.Count - 2; i++)
                            {
                                if (imgs[i].Attributes["data-src"] == null)
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{imgs[i].Attributes["src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                                else
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{imgs[i].Attributes["data-src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                            }
                        }
                        /*else if (currentNode.SelectNodes(".//img[contains(concat(\" \",normalize-space(@class),\" \"),\" news-media__image \")]") != null)
                        {
                            var imgs = currentNode.SelectNodes(".//img[contains(concat(\" \",normalize-space(@class),\" \"),\" news-media__image \")]");
                            foreach(var img in imgs) 
                            {
                                if (img.Attributes["data-src"] == null)
                                {
                                      var newNode = HtmlNode.CreateNode($"<img src=\"{img.Attributes["src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                                    else
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{img.Attributes["data-src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                            }
                        }*/
                        else if (currentNode.SelectNodes(".//img") != null)
                        {
                            var imgs = currentNode.SelectNodes(".//img");
                            foreach (var img in imgs)
                            {
                                if (img.Attributes["data-src"] == null)
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{img.Attributes["src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                                else
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{img.Attributes["data-src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                            }
                        }
                        else if (currentNode.SelectNodes(".//img[contains(concat(\" \",normalize-space(@class),\" \"),\" aligncenter \")]") != null)
                        {
                            var imgs = currentNode.SelectNodes(".//img[contains(concat(\" \",normalize-space(@class),\" \"),\" aligncenter \")]");
                            foreach (var img in imgs)
                            {
                                if (img.Attributes["data-src"] == null)
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{img.Attributes["src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                                else
                                {
                                    var newNode = HtmlNode.CreateNode($"<img src=\"{img.Attributes["data-src"].Value}\">");
                                    content.AppendChild(newNode);
                                }
                            }
                        }
                        else if(currentNode.SelectSingleNode(".//iframe") != null)
                        {
                            var newNode = HtmlNode.CreateNode($"<div class=\"ratio ratio-16x9\">\r\n<iframe src=\"{currentNode.SelectSingleNode(".//iframe").Attributes["src"].Value}\" allowfull>\r\n</div>");
                            content.AppendChild(newNode);
                        }
                        else
                        {
                            break;
                        }
                        break;
                    case "a":
                        if(currentNode.HasClass("news-banner") || currentNode.HasClass("attachment"))
                        {
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case "blockquote":
                        if (!currentNode.HasClass("instagram-media"))
                        {
                            if (!currentNode.HasAttributes)
                            {
                                var newNode = HtmlNode.CreateNode($"<p>{currentNode.InnerHtml}</p>");
                                content.AppendChild(newNode);
                                break;
                            }
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case "ul":
                        {
                            var newNode = HtmlNode.CreateNode($"<ul>{currentNode.InnerHtml}</ul>");
                            content.AppendChild(newNode);
                            break;
                        }
                    case "script":
                    case "hr":
                    case "h6":
                    case "h3":
                        break;
                    case "h2":
                        if (currentNode.FirstChild != null && currentNode.FirstChild.Name.Equals("strong"))
                        {
                            var newNode = HtmlNode.CreateNode($"<h4>{currentNode.FirstChild.InnerHtml}</h4>");
                            content.AppendChild(newNode);
                        }
                        else
                        {
                            var newNode = HtmlNode.CreateNode($"<h4>{currentNode.InnerHtml}</h4>");
                            content.AppendChild(newNode);
                        }
                        break;
                    default:
                        break;
                }
                currentNode = currentNode.NextSibling;
            }
            return content;
        }
    }
}
