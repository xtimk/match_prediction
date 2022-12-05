using CsvHelper;
using CsvHelper.Configuration;
using MatchPrediction.Helpers.CsvHelper;
using MatchPrediction.Models.MatchPrediction;
using System.Globalization;
using System.IO;

namespace MatchPrediction.Services.MatchStatsGetterService.Impl
{
    public class FootballDataCoUkGetter : IMatchStatsGetterService
    {
        private readonly ILogger<FootballDataCoUkGetter> _logger;
        private readonly ICsvHelper<MatchDto> _csvreader;
        private readonly IHttpClientFactory _httpClientFactory;

        public FootballDataCoUkGetter(
                ILogger<FootballDataCoUkGetter> logger, 
                ICsvHelper<MatchDto> csvreader,
                IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _csvreader = csvreader;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Match>> GetMatchesStats()
        {
            var http = _httpClientFactory.CreateClient();

            var response = await http.GetAsync("https://www.football-data.co.uk/mmz4281/2021/I1.csv");
            response.EnsureSuccessStatusCode();

            var responseContentStream = await response.Content.ReadAsStreamAsync();

            var matchesdto = _csvreader.Read(responseContentStream);

            var matches = new List<Match>();
            foreach (var item in matchesdto)
            {
                matches.Add(item.ToMatch());
            }

            return matches;
        }
    }
}

public sealed class MatchDto
{
    public string Div { get; set; }
    public string Date { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public int FTHG { get; set; }
    public int FTAG { get; set; }

    public Match ToMatch()
    {
        var match = new Match()
        {
            Div = Div,
            Date = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture),
            HomeTeam = HomeTeam,
            AwayTeam = AwayTeam,
            FTHG = FTHG,
            FTAG = FTAG
        };
        return match;
    }
}
