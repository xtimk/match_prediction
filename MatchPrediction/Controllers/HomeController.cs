using MatchPrediction.Data.Contexts;
using MatchPrediction.Helpers.DataTables;
using MatchPrediction.Models;
using MatchPrediction.Models.MatchPrediction;
using MatchPrediction.Services.MatchStatsGetterService;
using MatchPrediction.Services.QueryService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MathNet.Numerics.Distributions;
using static System.Linq.Enumerable;

namespace MatchPrediction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMatchStatsGetterService _matchStatsGetterService;
        private readonly MatchPredictionContext _db;
        private readonly DataTablesService<Match> _dataTablesService;
        private readonly QueryService _queryService;

        public HomeController(
                ILogger<HomeController> logger,
                IMatchStatsGetterService matchStatsGetterService,
                MatchPredictionContext db,
                DataTablesService<Match> dataTablesService,
                QueryService queryService)
        {
            _logger = logger;
            _matchStatsGetterService = matchStatsGetterService;
            _db = db;
            _dataTablesService = dataTablesService;
            _queryService = queryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetMatches(DataTableAjaxPostModel model)
        {
            _logger.LogInformation("Getting matches from db");
            var query = _db.Matches.AsQueryable();
            var dataTableResponse = await _dataTablesService.GenerateDataTableResponse(model, query);
            return Json(dataTableResponse);
        }

        public IActionResult TeamStrength()
        {
            var home_team = _queryService.GetTeamStrength().Where(x => x.Team == "Napoli").First();
            var hg = home_team.HomeGoalAvgWeighted;
            var hgc = home_team.HomeGoalsConceededAvgWeighted;

            var away_team = _queryService.GetTeamStrength().Where(x => x.Team == "Juventus").First();
            var ag = away_team.AwayGoalAvgWeighted;
            var agc = away_team.AwayGoalsConceededAvgWeighted;

            var p1 = new Poisson(hg * agc);
            var p2 = new Poisson(ag * hgc);

            var matches = new Dictionary<string, double>();
            double phome = 0;
            double peven = 0;
            double paway = 0;

            foreach (var h in Range(0, 10))
            {
                foreach (var a in Range(0, 10))
                {
                    var p = p1.Probability(h) * p2.Probability(a);
                    matches[h.ToString() + "-" + a.ToString()] = p;
                    if (h > a) phome += p;
                    if (h < a) paway += p;
                    if (h == a) peven += p;
                }
            }

            var ordered = matches.OrderByDescending(x => x.Value);

            var sum = matches.Sum(x => x.Value);


            var q = _queryService.GetTeamStrength();
            return View();
        }

        public async Task<JsonResult> GetDivisionGoalAvg(DataTableAjaxPostModel model)
        {
            var queryDatatables = _queryService.GetDivisionGoalsAverage();
            var dataTableResponse = await _dataTablesService.GenerateDataTableResponse(model, queryDatatables);
            return Json(dataTableResponse);
        }
        public async Task<JsonResult> GetTeamStregths(DataTableAjaxPostModel model)
        {
            _logger.LogInformation("Calculating team strenghts.");
            var queryDatatables = _queryService.GetTeamStrength();
            var dataTableResponse = await _dataTablesService.GenerateDataTableResponse(model, queryDatatables);
            return Json(dataTableResponse);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}