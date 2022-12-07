using MatchPrediction.Models;
using MatchPrediction.Services.QueryService;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using static System.Linq.Enumerable;

namespace MatchPrediction.Controllers
{
    public class MatchPredictionController : Controller
    {
        private readonly ILogger<MatchPredictionController> _logger;
        private readonly QueryService _queryService;

        public MatchPredictionController(
                ILogger<MatchPredictionController> logger,
                QueryService queryService
            )
        {
            _logger = logger;
            _queryService = queryService;
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

            var home_team = await _queryService.GetTeamStrength().Where(x => x.Team == home_team_name).FirstOrDefaultAsync();
            var away_team = await _queryService.GetTeamStrength().Where(x => x.Team == away_team_name).FirstOrDefaultAsync();
            if(home_team == null || away_team == null)
            {
                var errors = new List<string>();
                if (home_team == null)
                {
                    _logger.LogError("Can't retrieve home team stats");
                    errors.Add("Please select a valid home team");
                }
                if (away_team == null)
                {
                    _logger.LogError("Can't retrieve away team stats");
                    errors.Add("Please select a valid away team");
                }

                ViewBag.Error = errors;
                ViewBag.Teams = await GetAllTeamsSelectItemList();
                return View("InputData");
            }

            var hg = home_team.HomeGoalAvgWeighted;
            var hgc = home_team.HomeGoalsConceededAvgWeighted;
            var ag = away_team.AwayGoalAvgWeighted;
            var agc = away_team.AwayGoalsConceededAvgWeighted;

            var home_lambda = hg * agc;
            var away_lambda = ag * hgc;

            var p1 = new Poisson(home_lambda);
            var p2 = new Poisson(away_lambda);

            var matches = new Dictionary<string, Tuple<double, double>>();
            double phome = 0;
            double peven = 0;
            double paway = 0;

            foreach (var h in Range(0, 10))
            {
                foreach (var a in Range(0, 10))
                {
                    var p = p1.Probability(h) * p2.Probability(a);
                    matches[home_team_name + " " + h.ToString() + " - " + a.ToString() + " " + away_team_name] = new Tuple<double, double>(Math.Round(p*100, 2), GetOddFromProb(p));
                    if (h > a) phome += p;
                    if (h < a) paway += p;
                    if (h == a) peven += p;
                }
            }
            var ordered = matches.OrderByDescending(x => x.Value);
            var sum = matches.Sum(x => x.Value.Item1);
            ViewBag.Matches = ordered;

            var resultProbs = new Dictionary<string, Tuple<double, double>>();
            var phome_rounded = Math.Round(phome * 100, 2);
            var peven_rounded = Math.Round(peven * 100, 2);
            var paway_rounded = Math.Round(paway * 100, 2);

            resultProbs.Add(home_team_name + " (Home)", new Tuple<double, double>(phome_rounded, GetOddFromProb(phome)));
            resultProbs.Add("Even", new Tuple<double, double>(peven_rounded, GetOddFromProb(peven)));
            resultProbs.Add(away_team_name + " (Away)", new Tuple<double, double>(paway_rounded, GetOddFromProb(paway)));
            ViewBag.ResultProbs = resultProbs;

            var lambdas = new Dictionary<string, double>();
            lambdas.Add(home_team_name + " (Home)", Math.Round(home_lambda, 4));
            lambdas.Add(away_team_name + " (Away)", Math.Round(away_lambda, 4));
            ViewBag.Lambdas = lambdas;

            return View("OutputData");
        }

        private double GetOddFromProb(double prob)
        {
            return Math.Round(1/prob, 2);
        }
    }
}
