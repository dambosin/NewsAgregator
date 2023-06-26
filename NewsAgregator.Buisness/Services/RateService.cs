using Microsoft.Extensions.Configuration;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Lemma;
using System.Net;
using Serilog;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace NewsAgregator.Buisness.Services
{
    public class RateService : IRateService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public RateService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public double ConvertToLocalRate(double rate)
        {
            int.TryParse(_configuration["Rating:MaxRate"], out var MaxRate);
            int.TryParse(_configuration["Rating:MinRate"], out var MinRate);
            int.TryParse(_configuration["Rating:MaxInputRate"], out var MaxInputRate);
            int.TryParse(_configuration["Rating:MinInputRate"], out var MinInputRate);
            int.TryParse(_configuration["Rating:RateRank"], out var RateRank);

            if (rate >= MaxInputRate) return MaxRate;
            if (rate <= MinInputRate) return MinRate;

            var result = (rate - MinInputRate);//to start from zero
            result /= (MaxInputRate - MinInputRate);//percantage of value
            result *= (MaxRate - MinRate);//convert to new rate system
            result *= Math.Pow(10, RateRank);//save RateRank numbers after dot as integer
            result = Math.Ceiling(result) / Math.Pow(10, RateRank);//rounding value and restoring it's view

            return result;
        }

        public async Task<List<Lemma>> DoLemmatization(string text)
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7288/")
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            text = text.Replace("\"", "\\\"").Replace(Environment.NewLine, "");
            _logger.Information(text);
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"http://api.ispras.ru/texterra/v1/nlp?targetType=lemma&apikey={_configuration["Secrets:TextErra:ApiKey"]}")
            {
                Content = new StringContent("[{\"text\":\"" + text + "\"}]",
                            Encoding.UTF8,
                            "application/json")
            };
            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new WebException($"Request was unsuccessfull. Status code {response.StatusCode}. {response.Content}");
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<LemmaResponse>>(responseString)[0].Annotations.Lemma;
        }

        public async Task<Dictionary<string, int>> GetLemmaRates()
        {
            using StreamReader reader = new("bin/Debug/net6.0/AFINN-ru.json");
            string json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
        }

        public async Task<double> Rate(string text)
        {
            try
            {
                var lemmas = await DoLemmatization(text);
                var lemmaRates = await GetLemmaRates();
                double rate = 0;
                var lemmasCount = 0;
                foreach (var lemma in lemmas)
                {
                    if (!lemmaRates.ContainsKey(lemma.Value!)) continue;
                    rate += lemmaRates[lemma.Value!];
                    lemmasCount++;
                }
                if (lemmasCount == 0) return 0;
                return ConvertToLocalRate(rate / lemmasCount);
            }
            catch (WebException ex)
            {
                _logger.Warning(ex.Message);
                return -10.0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}
