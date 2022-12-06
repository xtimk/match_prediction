using MatchPrediction.Data.Contexts;
using MatchPrediction.Helpers.DataTables;
using MatchPrediction.Models;
using MatchPrediction.Models.MatchPrediction;
using MatchPrediction.Services.MatchStatsGetterService;
using MatchPrediction.Services.QueryService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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