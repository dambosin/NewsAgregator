using HtmlAgilityPack;

namespace WebApi
{
    public class Class1
    {
        public void GetRss(string url)
        {
            var htmlWeb = new HtmlWeb();
            var doc = htmlWeb.Load(url);
            Console.WriteLine(doc.ToString());
        }
    }
}