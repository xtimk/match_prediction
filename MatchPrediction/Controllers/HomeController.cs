using MatchPrediction.Data.Contexts;
using MatchPrediction.Helpers.DataTables;
using MatchPrediction.Models;
using MatchPrediction.Models.MatchPrediction;
using MatchPrediction.Services.MatchStatsGetterService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MatchPrediction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMatchStatsGetterService _matchStatsGetterService;
        private readonly MatchPredictionContext _db;
        private readonly DataTablesService<Match> _dataTablesService;

        public HomeController(
                ILogger<HomeController> logger,
                IMatchStatsGetterService matchStatsGetterService,
                MatchPredictionContext db,
                DataTablesService<Match> dataTablesService)
        {
            _logger = logger;
            _matchStatsGetterService = matchStatsGetterService;
            _db = db;
            _dataTablesService = dataTablesService;
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
            return View();
        }

        public async Task<JsonResult> GetTeamStregths(DataTableAjaxPostModel model)
        {
            _logger.LogInformation("Calculating team strenghts.");
            var query = _db.Matches.AsQueryable();
            var HomeGoalsAvg = query.GroupBy(x => x.HomeTeam).Select(x => new { Key = x.Key, HomeTeamGoalsAvg = x.Average(s => s.FTHG) });
            var AwayGoalsAvg = query.GroupBy(x => x.AwayTeam).Select(x => new { Key = x.Key, AwayTeamGoalsAvg = x.Average(s => s.FTAG) });
            var HomeGoalsConceededAvg = query.GroupBy(x => x.HomeTeam).Select(x => new { Key = x.Key, HomeTeamConceededGoalsAvg = x.Average(s => s.FTAG) });
            var AwayGoalsConceededAvg = query.GroupBy(x => x.AwayTeam).Select(x => new { Key = x.Key, AwayTeamConceededGoalsAvg = x.Average(s => s.FTHG) });

            var TeamAvgGoals = HomeGoalsAvg.Join(
                    AwayGoalsAvg,
                    h => h.Key,
                    a => a.Key,
                    (h, a) => new
                    {
                        Team = h.Key,
                        HomeGoalsAvg = h.HomeTeamGoalsAvg,
                        AwayGoalsAvg = a.AwayTeamGoalsAvg
                    }
                )
                .Join(
                    HomeGoalsConceededAvg, 
                    h => h.Team,
                    a => a.Key,
                    (h, a) => new
                    {
                        Team = h.Team,
                        HomeGoalsAvg = h.HomeGoalsAvg,
                        AwayGoalsAvg = h.AwayGoalsAvg,
                        HomeGoalsConceededAvg = a.HomeTeamConceededGoalsAvg
                    }
                )
                .Join(
                    AwayGoalsConceededAvg,
                    h => h.Team,
                    a => a.Key,
                    (h, a) => new
                    {
                        Team = h.Team,
                        HomeGoalsAvg = h.HomeGoalsAvg,
                        AwayGoalsAvg = h.AwayGoalsAvg,
                        HomeGoalsConceededAvg = h.HomeGoalsConceededAvg,
                        AwayGoalsConceededAvg = a.AwayTeamConceededGoalsAvg
                    }
                );

            var queryDatatables = TeamAvgGoals;
            var dataTableResponse = await _dataTablesService.GenerateDataTableResponse(model, queryDatatables);
            return Json(dataTableResponse);

            //var total = await queryDatatables.CountAsync();

            //queryDatatables = _dataTablesService.Search(queryDatatables, model.search.value);
            //var filtered_total = await queryDatatables.CountAsync();

            //queryDatatables = _dataTablesService.Paginate(queryDatatables, model.start, model.length);

            //var result = await queryDatatables.ToListAsync();

            //return Json(new
            //{
            //    draw = model.draw,
            //    recordsTotal = total,
            //    recordsFiltered = filtered_total,
            //    data = result
            //});
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