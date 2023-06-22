using NewsAgregator.Core.Lemma;
namespace NewsAgregator.Abstractions.Services
{
    public interface IRateService
    {
        Task<double> Rate(string text);
        Task<List<Lemma>> DoLemmatization(string text);
        Task<Dictionary<string, int>> GetLemmaRates();
        double ConvertToLocalRate(double rate);
    }
}
