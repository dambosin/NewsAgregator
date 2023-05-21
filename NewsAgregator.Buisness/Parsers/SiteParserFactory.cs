using NewsAgregator.Abstractions;

namespace NewsAgregator.Buisness.Parsers
{
    public class SiteParserFactory : ISiteParserFactory
    {
        private readonly IEnumerable<ISiteParser> _parsers;

        public SiteParserFactory(IEnumerable<ISiteParser> parsers)
        {
            _parsers = parsers;
        }

        public ISiteParser GetInstance(string token)
        {
            return token switch
            {
                "Onliner" => GetInstance(typeof(OnlinerParser)),
                "Tut.by" => GetInstance(typeof(TutbyParser)),
                "Dtf" => GetInstance(typeof(DtfParseer)),
                _ => throw new NotImplementedException()
            };
        }
        public ISiteParser GetInstance(Type type) => _parsers.FirstOrDefault(parser => parser.GetType() == type)!;
    }
}
