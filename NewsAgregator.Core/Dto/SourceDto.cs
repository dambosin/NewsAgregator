namespace NewsAgregator.Core.Dto
{
    public class SourceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string RssUrl { get; set; }
    }
}
