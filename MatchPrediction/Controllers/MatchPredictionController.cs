using MatchPrediction.Services.MatchPredictionServices.ExactResult;
using MatchPrediction.Services.QueryServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MatchPrediction.Controllers
{
    public class MatchPredictionController : Controller
    {
        private readonly ILogger<MatchPredictionController> _logger;
        private readonly QueryService _queryService;
        private readonly IMatchExactResultService _matchExactResultService;

        public MatchPredictionController(
                ILogger<MatchPredictionController> logger,
                QueryService queryService,
                IMatchExactResultService matchExactResultService
            )
        {
            _logger = logger;
            _queryService = queryService;
            _matchExactResultService = matchExactResultService;
        }
        public async Task<IActionResult> InputData()
        {
            ViewBag.Teams = await GetAllTeamsSelectItemList();
            return View();
        }

        private async Task<List<SelectListItem>> GetAllTeamsSelectItemList()
        {
            var teams = await _queryService.GetAllTeams().ToListAsync();
            var listTeamItems = new List<SelectListItem>();
            foreach (var team in teams)
            {
                listTeamItems.Add(new SelectListItem
                {
                    Text = team,
                    Value = team
                });
            }
            return listTeamItems;
        }

        [HttpPost]
        public async Task<IActionResult> CalculatePrediction()
        {
            var home_team_name = Request.Form["homeTeam"].ToString();
            var away_team_name = Request.Form["awayTeam"].ToString();

            var result = await _matchExactResultService.PredictExactResult(home_team_name, away_team_name);

            if (!result.Success)
            {
                ViewBag.Error = result.Errors;
                ViewBag.Teams = await GetAllTeamsSelectItemList();
                return View("InputData");
            }

            var matches = new Dictionary<string, Tuple<double, double>>();
            foreach (var match in result.MatchResults)
            {
                matches[home_team_name + " " + match.HomeScored.ToString() + " - " + match.AwayScored.ToString() + " " + away_team_name] = 
                    new Tuple<double, double>(Math.Round(match.Probability * 100, 2), Math.Round(match.Odd, 2));
            }
            ViewBag.Matches = matches;

            var resultProbs = new Dictionary<string, Tuple<double, double>>
            {
                { home_team_name + " (Home)", new Tuple<double, double>(Math.Round(result.HomeWinsProbability,2 ), Math.Round(result.HomeWinsOdd, 2)) },
                { "Even", new Tuple<double, double>(Math.Round(result.EvenProbability, 2), Math.Round(result.EvenOdd, 2)) },
                { away_team_name + " (Away)", new Tuple<double, double>(Math.Round(result.AwayWinsProbability, 2), Math.Round(result.AwayWinsOdd, 2)) }
            };
            ViewBag.ResultProbs = resultProbs;

            var lambdas = new Dictionary<string, double>
            {
                { home_team_name + " (Home)", Math.Round(result.Home_Lambda_Strength, 4) },
                { away_team_name + " (Away)", Math.Round(result.Away_Lambda_Strength, 4) }
            };
            ViewBag.Lambdas = lambdas;

            //var overGoals = new Dictionary<string, Tuple<double, double>>
            //{
            //    { "Over 0.5", new Tuple<double, double>(0, 0) },
            //    { "Over 1.5", new Tuple<double, double>(0, 0) },
            //    { "Over 2.5", new Tuple<double, double>(0, 0) },
            //    { "Over 3.5", new Tuple<double, double>(0, 0) },
            //    { "Over 4.5", new Tuple<double, double>(0, 0) },
            //    { "Over 5.5", new Tuple<double, double>(0, 0) },
            //    { "Over 6.5", new Tuple<double, double>(0, 0) },
            //    { "Over 7.5", new Tuple<double, double>(0, 0) },
            //};
            //var underGoals = new Dictionary<string, Tuple<double, double>>
            //{
            //    { "Under 0.5", new Tuple<double, double>(0, 0) },
            //    { "Under 1.5", new Tuple<double, double>(0, 0) },
            //    { "Under 2.5", new Tuple<double, double>(0, 0) },
            //    { "Under 3.5", new Tuple<double, double>(0, 0) },
            //    { "Under 4.5", new Tuple<double, double>(0, 0) },
            //    { "Under 5.5", new Tuple<double, double>(0, 0) },
            //    { "Under 6.5", new Tuple<double, double>(0, 0) },
            //    { "Under 7.5", new Tuple<double, double>(0, 0) },
            //};
            var bothTeamsToScore = new Dictionary<string, Tuple<double, double>>
            {
                { "Yes", new Tuple<double, double>(Math.Round(result.BothTeamsToScoreProbability, 2), Math.Round(result.BothTeamsToScoreOdd, 2)) },
                { "No", new Tuple<double, double>(Math.Round(result.BothTeamsNotToScoreProbability, 2), Math.Round(result.BothTeamsNotToScoreOdd, 2)) },
            };
            ViewBag.BothTeamsToScore = bothTeamsToScore;

            return View("OutputData");
        }
    }
}
