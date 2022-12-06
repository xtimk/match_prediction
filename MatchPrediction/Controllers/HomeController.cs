using MatchPrediction.Data.Contexts;
using MatchPrediction.Helpers.DataTables;
using MatchPrediction.Models;
using MatchPrediction.Models.MatchPrediction;
using MatchPrediction.Services.MatchStatsGetterService;
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

        public async Task<JsonResult> GetDivisionGoalAvg(DataTableAjaxPostModel model)
        {
            var divisionAvgHomeGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueHomeGoalsAvg = x.Average(s => s.FTHG) });
            var divisionAvgAwayGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueAwayGoalsAvg = x.Average(s => s.FTAG) });
            var divisionAvgGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueTotalGoalsAvg = x.Average(s => s.FTHG + s.FTAG) });
            var queryDatatables = divisionAvgHomeGoals.Join(
                divisionAvgAwayGoals,
                dahg => dahg.Key,
                daag => daag.Key,
                (dahg, daag) => new
                {
                    dahg.Key,
                    dahg.LeagueHomeGoalsAvg,
                    daag.LeagueAwayGoalsAvg
                })
                .Join(
                    divisionAvgGoals,
                    l => l.Key,
                    dag => dag.Key,
                    (l, r) => new
                    {
                        Div = l.Key,
                        l.LeagueHomeGoalsAvg,
                        l.LeagueAwayGoalsAvg,
                        r.LeagueTotalGoalsAvg
                    });
            var dataTableResponse = await _dataTablesService.GenerateDataTableResponse(model, queryDatatables);
            return Json(dataTableResponse);
        }
        public async Task<JsonResult> GetTeamStregths(DataTableAjaxPostModel model)
        {
            _logger.LogInformation("Calculating team strenghts.");
            var query = _db.Matches.AsQueryable();
            var HomeGoalsAvg = query.GroupBy(x => x.HomeTeam).Select(x => new { Key = x.Key, HomeTeamGoalsAvg = x.Average(s => s.FTHG) });
            var AwayGoalsAvg = query.GroupBy(x => x.AwayTeam).Select(x => new { Key = x.Key, AwayTeamGoalsAvg = x.Average(s => s.FTAG) });
            var HomeGoalsConceededAvg = query.GroupBy(x => x.HomeTeam).Select(x => new { Key = x.Key, HomeTeamConceededGoalsAvg = x.Average(s => s.FTAG) });
            var AwayGoalsConceededAvg = query.GroupBy(x => x.AwayTeam).Select(x => new { Key = x.Key, AwayTeamConceededGoalsAvg = x.Average(s => s.FTHG) });
            var DivTeam = query.Select(x => new { x.Div, Team = x.HomeTeam }).Distinct();

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
                )
                .Join(
                    DivTeam,
                    h => h.Team,
                    a => a.Team,
                    (h, a) => new
                    {
                        Div = a.Div,
                        Team = h.Team,
                        HomeGoalsAvg = h.HomeGoalsAvg,
                        AwayGoalsAvg = h.AwayGoalsAvg,
                        HomeGoalsConceededAvg = h.HomeGoalsConceededAvg,
                        AwayGoalsConceededAvg = h.AwayGoalsConceededAvg,
                    }
                );

            var divisionAvgHomeGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueHomeGoalsAvg = x.Average(s => s.FTHG) });
            var divisionAvgAwayGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueAwayGoalsAvg = x.Average(s => s.FTAG) });
            var divisionAvgTotalGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueTotalGoalsAvg = x.Average(s => s.FTHG + s.FTAG) });
            var divisionAvgGoals = divisionAvgHomeGoals.Join(
                divisionAvgAwayGoals,
                dahg => dahg.Key,
                daag => daag.Key,
                (dahg, daag) => new
                {
                    dahg.Key,
                    dahg.LeagueHomeGoalsAvg,
                    daag.LeagueAwayGoalsAvg
                })
                .Join(
                    divisionAvgTotalGoals,
                    l => l.Key,
                    dag => dag.Key,
                    (l, r) => new
                    {
                        Div = l.Key,
                        l.LeagueHomeGoalsAvg,
                        l.LeagueAwayGoalsAvg,
                        r.LeagueTotalGoalsAvg
                    });

            var finalTable = TeamAvgGoals.Join(
                divisionAvgGoals,
                l => l.Div,
                dag => dag.Div,
                (l, dag) => new
                {
                    Div = l.Div,
                    Team = l.Team,
                    HomeStrenght = (l.HomeGoalsAvg / dag.LeagueHomeGoalsAvg) * (l.HomeGoalsConceededAvg / dag.LeagueAwayGoalsAvg),
                    AwayStrength = (l.AwayGoalsAvg / dag.LeagueAwayGoalsAvg) * (l.AwayGoalsConceededAvg / dag.LeagueHomeGoalsAvg),
                    HomeGoalsAvg = l.HomeGoalsAvg,
                    HomeGoalAvgWeighted = l.HomeGoalsAvg / dag.LeagueHomeGoalsAvg,
                    AwayGoalsAvg = l.AwayGoalsAvg,
                    AwayGoalAvgWeighted = l.HomeGoalsAvg / dag.LeagueAwayGoalsAvg,
                    HomeGoalsConceededAvg = l.HomeGoalsConceededAvg,
                    HomeGoalsConceededAvgWeighted = l.HomeGoalsConceededAvg/ dag.LeagueAwayGoalsAvg,
                    AwayGoalsConceededAvg = l.AwayGoalsConceededAvg,
                    AwayGoalsConceededAvgWeighted = l.AwayGoalsConceededAvg / dag.LeagueHomeGoalsAvg,
                });

            //var queryDatatables = TeamAvgGoals;
            var queryDatatables = finalTable;
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