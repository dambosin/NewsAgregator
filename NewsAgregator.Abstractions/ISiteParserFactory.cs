namespace NewsAgregator.Abstractions
{
    public interface ISiteParserFactory
    {
        ISiteParser GetInstance(string token);
    }
}
