using MatchPrediction.Models.PredictionResults.ExactResult;
using MatchPrediction.Services.QueryServices;
using MathNet.Numerics.Distributions;
using Microsoft.EntityFrameworkCore;
using static System.Linq.Enumerable;

namespace MatchPrediction.Services.MatchPredictionServices.ExactResult.Impl
{
    public class MatchExactResultService : IMatchExactResultService
    {
        private readonly ILogger<MatchExactResult> _logger;
        private readonly QueryService _queryService;
        private int MAX_GOALS;

        public MatchExactResultService(ILogger<MatchExactResult> logger, QueryService queryService)
        {
            _logger = logger;
            _queryService = queryService;
            MAX_GOALS= 10;
        }

        public async Task<Prediction_ExactResult_Response> PredictExactResult(string home_team_name, string away_team_name)
        {
            var result = new Prediction_ExactResult_Response();
            result.Success = true;

            var home_team = await _queryService.GetTeamStrength().Where(x => x.Team == home_team_name).FirstOrDefaultAsync();
            var away_team = await _queryService.GetTeamStrength().Where(x => x.Team == away_team_name).FirstOrDefaultAsync();
            if (home_team == null || away_team == null)
            {
                result.Success = false;
                if (home_team == null)
                {
                    _logger.LogError("Can't retrieve home team stats");
                    result.Errors.Add("Please select a valid home team");
                }
                if (away_team == null)
                {
                    _logger.LogError("Can't retrieve away team stats");
                    result.Errors.Add("Please select a valid away team");
                }
                return result;
            }

            result.HomeTeam_GoalScored_Average_Weighted = home_team.HomeGoalAvgWeighted;
            result.HomeTeam_GoalConceded_Average_Weighted = home_team.HomeGoalsConceededAvgWeighted;
            result.AwayTeam_GoalScored_Average_Weighted = away_team.AwayGoalAvgWeighted;
            result.AwayTeam_GoalConceded_Average_Weighted = away_team.AwayGoalsConceededAvgWeighted;

            //var hg = home_team.HomeGoalAvgWeighted;
            //var hgc = home_team.HomeGoalsConceededAvgWeighted;
            //var ag = away_team.AwayGoalAvgWeighted;
            //var agc = away_team.AwayGoalsConceededAvgWeighted;

            result.Home_Lambda_Strength = result.HomeTeam_GoalScored_Average_Weighted * result.AwayTeam_GoalConceded_Average_Weighted;
            result.Away_Lambda_Strength = result.AwayTeam_GoalScored_Average_Weighted * result.HomeTeam_GoalConceded_Average_Weighted;

            var p1 = new Poisson(result.Home_Lambda_Strength);
            var p2 = new Poisson(result.Away_Lambda_Strength);

            //var matches = new Dictionary<string, Tuple<double, double>>();
            //double phome = 0;
            //double peven = 0;
            //double paway = 0;

            foreach (var h in Range(0, MAX_GOALS))
            {
                foreach (var a in Range(0, MAX_GOALS))
                {
                    var p = p1.Probability(h) * p2.Probability(a);
                    result.MatchResults.Add(new MatchExactResult
                    {
                        HomeScored= h,
                        AwayScored= a,
                        Probability= p,
                        Odd= 1/p
                    });
                    //if (h > a) result.HomeWinsProbability += p;
                    //if (h < a) result.AwayWinsProbability += p;
                    //if (h == a) result.EvenProbability += p;
                }
            }

            return result;
        }
    }
}
