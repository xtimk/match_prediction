using CsvHelper.Configuration.Attributes;
using MatchPrediction.Helpers.CsvHelper;
using MatchPrediction.Models.MatchPrediction;
using System.Globalization;
using static System.Linq.Enumerable;
using static System.Net.WebRequestMethods;

namespace MatchPrediction.Services.MatchStatsGetterService.Impl
{
    public class FootballDataCoUkGetter : IMatchStatsGetterService
    {
        private readonly ILogger<FootballDataCoUkGetter> _logger;
        private readonly ICsvHelper<MatchDto> _csvreader;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly List<string> _leagues = new List<string>
        {
            "I1", // Italy serie A
            "E0", // England 
            "D1"  // Germany
        };
        private readonly int _fromYear;
        private readonly int _toYear;

        public FootballDataCoUkGetter(
                ILogger<FootballDataCoUkGetter> logger, 
                ICsvHelper<MatchDto> csvreader,
                IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _csvreader = csvreader;
            _httpClientFactory = httpClientFactory;
            
            // Leagues I want to import
            _leagues = new List<string>
            {
                "I1", // Italy serie A
                "E0", // England 
                "D1"  // Germany
            };
            // Only from 2018 to 2022
            _fromYear = 18;
            _toYear= 22;
        }

        public async Task<IEnumerable<Match>> GetMatchesStats()
        {
            var http = _httpClientFactory.CreateClient();
            var matches = new List<Match>();

            foreach (var league in _leagues)
            {
                foreach (var year in Range(_fromYear, (_toYear - _fromYear)))
                {
                    var urlToCall = "https://www.football-data.co.uk/mmz4281/" + year.ToString() + (year + 1).ToString() + "/" + league + ".csv";
                    var response = await http.GetAsync(urlToCall);
                    response.EnsureSuccessStatusCode();

                    var responseContentStream = await response.Content.ReadAsStreamAsync();

                    var matchesdto = _csvreader.Read(responseContentStream);
                    foreach (var item in matchesdto)
                    {
                        matches.Add(item.ToMatch());
                    }
                }
            }
            return matches;
        }
    }
}

public sealed class MatchDto
{
    public string Div { get; set; }
    public string Date { get; set; }

    [Optional]
    public string Time { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public int FTHG { get; set; }
    public int FTAG { get; set; }

    public Match ToMatch()
    {
        var match = new Match()
        {
            Div = Div,
            Date = Time == null ? DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.ParseExact(Date + " " + Time, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
            HomeTeam = HomeTeam,
            AwayTeam = AwayTeam,
            FTHG = FTHG,
            FTAG = FTAG
        };
        return match;
    }
}
