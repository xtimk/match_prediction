using CsvHelper.Configuration.Attributes;
using MatchPrediction.Helpers.CsvHelper;
using MatchPrediction.Models.MatchPrediction;
using System.Globalization;
using static System.Linq.Enumerable;

namespace MatchPrediction.Services.MatchStatsGetterService.Impl
{
    public class FootballDataCoUkGetter : IMatchStatsGetterService
    {
        private readonly ILogger<FootballDataCoUkGetter> _logger;
        private readonly ICsvHelper<MatchDto> _csvreader;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly List<string> _leagues;
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
            // Only from 2015 to 2021
            _fromYear = 15;
            _toYear= 21;
        }

        public async Task<IEnumerable<Match>> GetMatchesStats()
        {
            _logger.LogInformation("Getting matches stats from football-data.co.uk.");
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
    public sealed class MatchDto
    {
        public string Div { get; set; } = default!;
        public string Date { get; set; } = default!;

        [Optional]
        public string Time { get; set; } = default!;
        public string HomeTeam { get; set; } = default!;
        public string AwayTeam { get; set; } = default!;
        public int FTHG { get; set; }
        public int FTAG { get; set; }

        public Match ToMatch()
        {
            string[] formats = { "dd/MM/yyyy", "dd/MM/yy", "dd/MM/yyyy HH:mm", "dd/MM/yy HH:mm" };
            var match = new Match()
            {
                Div = Div,
                Date = Time == null ? DateTime.ParseExact(Date, formats, CultureInfo.InvariantCulture) : DateTime.ParseExact(Date + " " + Time, formats, CultureInfo.InvariantCulture),
                HomeTeam = HomeTeam,
                AwayTeam = AwayTeam,
                FTHG = FTHG,
                FTAG = FTAG
            };
            return match;
        }
    }
}
