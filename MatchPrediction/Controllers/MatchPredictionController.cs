using MatchPrediction.Data.Contexts;
using MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers;
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
        private readonly MatchPredictionContext _db;
        private readonly PredictionResponse_PoissonExactResult_TeamWinner _predictionResponse_PoissonExactResult_TeamWinner;
        private readonly PredictionResponse_PoissonExactResult_BothTeamsToScore _predictionResponse_PoissonExactResult_BothTeamsToScore;
        private readonly PredictionResponse_PoissonExactResult_ExactResult _predictionResponse_PoissonExactResult_ExactResult;

        public MatchPredictionController(
                ILogger<MatchPredictionController> logger,
                QueryService queryService,
                IMatchExactResultService matchExactResultService,
                MatchPredictionContext db,
                PredictionResponse_PoissonExactResult_TeamWinner predictionResponse_PoissonExactResult_TeamWinner,
                PredictionResponse_PoissonExactResult_BothTeamsToScore predictionResponse_PoissonExactResult_BothTeamsToScore,
                PredictionResponse_PoissonExactResult_ExactResult predictionResponse_PoissonExactResult_ExactResult
            )
        {
            _logger = logger;
            _queryService = queryService;
            _matchExactResultService = matchExactResultService;
            _db = db;
            _predictionResponse_PoissonExactResult_TeamWinner = predictionResponse_PoissonExactResult_TeamWinner;
            _predictionResponse_PoissonExactResult_BothTeamsToScore = predictionResponse_PoissonExactResult_BothTeamsToScore;
            _predictionResponse_PoissonExactResult_ExactResult = predictionResponse_PoissonExactResult_ExactResult;
        }
        public async Task<IActionResult> InputData()
        {
            ViewBag.Teams = await GetAllTeamsSelectItemList();
            ViewBag.YearsBack = await GenerateYearsBackDropdownList();
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
        private async Task<List<SelectListItem>> GenerateYearsBackDropdownList()
        {
            var lastMatch = await _db.Matches.MaxAsync(x => x.Date);
            var firstMatch = await _db.Matches.MinAsync(x => x.Date);

            var lastYear = lastMatch.Date.Year;
            var firstYear = firstMatch.Date.Year;

            var listTeamItems = new List<SelectListItem>();

            for (int i = firstYear; i <= lastYear; i++)
            {
                listTeamItems.Add(new SelectListItem()
                {
                    Text = "From 01/01/" + i + " to 31/12/" + lastYear,
                    Value = i.ToString()
                });
            }
            return listTeamItems;
        }

        [HttpPost]
        public async Task<IActionResult> CalculatePrediction()
        {
            var home_team_name = Request.Form["homeTeam"].ToString();
            var away_team_name = Request.Form["awayTeam"].ToString();
            var fromYear = 1900;
            try
            {
                fromYear = Int32.Parse(Request.Form["fromYear"].ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while parsing fromDate");
                ViewBag.Error = new List<string> { "Please select a valid dataset period." };
                ViewBag.Teams = await GetAllTeamsSelectItemList();
                ViewBag.YearsBack = await GenerateYearsBackDropdownList();
                return View("InputData");
            }

            var dateFrom = new DateTime(fromYear,1,1,0,0,0);
            var lastYear = _db.Matches.Max(x => x.Date).Date.Year;
            var dateTo = new DateTime(lastYear, 12, 31, 23, 59, 59);

            var result = await _matchExactResultService.PredictExactResult(home_team_name, away_team_name, dateFrom, dateTo);

            if (!result.ExecutedWithoutErrors)
            {
                ViewBag.Error = result.Errors;
                ViewBag.Teams = await GetAllTeamsSelectItemList();
                return View("InputData");
            }

            var exactResultReader = _predictionResponse_PoissonExactResult_ExactResult.CreateReader(result);
            var matches = _predictionResponse_PoissonExactResult_ExactResult.GetExactResultProbabilities();
            ViewBag.Matches = matches;

            var teamWinnerReader = _predictionResponse_PoissonExactResult_TeamWinner.CreateReader(result);
            var resultProbs = teamWinnerReader.GetTeamWinnerProbabilities();
            ViewBag.ResultProbs = resultProbs;

            var lambdas = new Dictionary<string, double>
            {
                { home_team_name + " (Home)", Math.Round(result.Home_Lambda_Strength, 4) },
                { away_team_name + " (Away)", Math.Round(result.Away_Lambda_Strength, 4) }
            };
            ViewBag.Lambdas = lambdas;

            var bothTeamsToScoreReader = _predictionResponse_PoissonExactResult_BothTeamsToScore.CreateReader(result);
            var bothTeamsToScore = bothTeamsToScoreReader.GetBothTeamsToScoreProbabilities();
            ViewBag.BothTeamsToScore = bothTeamsToScore;

            return View("OutputData");
        }
    }
}
