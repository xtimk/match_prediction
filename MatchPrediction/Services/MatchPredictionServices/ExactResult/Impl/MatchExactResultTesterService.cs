using System;
using MatchPrediction.Data.Contexts;
using MatchPrediction.Models.MatchPrediction;
using MatchPrediction.Models.PredictionResults.ExactResult;
using MatchPrediction.Services.QueryServices;

namespace MatchPrediction.Services.MatchPredictionServices.ExactResult.Impl
{
	public class MatchExactResultTesterService : IMatchExactResultTesterService
    {
        private readonly IMatchExactResultService _matchExactResultService;
        private readonly QueryService _queryServices;
        private readonly MatchPredictionContext _db;
        private readonly ILogger<MatchExactResultTesterService> _logger;

        public MatchExactResultTesterService(
                IMatchExactResultService matchExactResultService,
                QueryService queryServices,
                MatchPredictionContext db,
                ILogger<MatchExactResultTesterService> logger)
		{
            _matchExactResultService = matchExactResultService;
            _queryServices = queryServices;
            _db = db;
            _logger = logger;
        }

        public async Task<PredictionExactResultTesterResult> TeamWinner_AllDivisions(DateTime datasetFrom, DateTime datasetTo, DateTime dataTestFrom, DateTime dataTestTo)
        {
            var divisions = _queryServices.GetAllDivisions().ToList();

            var result = new PredictionExactResultTesterResult
            {
                DivisionPredictions = new List<PredictionExactResultDivTesterResult>()
            };

            foreach (var div in divisions)
            {
                var divResult = await TeamWinner_InDivision(datasetFrom, datasetTo, dataTestFrom, dataTestTo, div);
                result.DivisionPredictions.Add(divResult);
            }
            return result;
        }

        public async Task<PredictionExactResultDivTesterResult> TeamWinner_InDivision(DateTime datasetFrom, DateTime datasetTo, DateTime dataTestFrom, DateTime dataTestTo, string div)
        {
            var matches_to_test = _db.Matches.Where(x => x.Div == div && x.Date >= dataTestFrom && x.Date <= dataTestTo).ToList();

            var result = new PredictionExactResultDivTesterResult
            {
                Div = div,
                PredictedMatchesInDiv = new List<PredictionExactResultMatchResult>()
            };

            matches_to_test = ClearNonExistingTeamInHistoricalData(matches_to_test, datasetFrom, datasetTo);
            foreach (var m in matches_to_test)
            {
                var prediction = await _matchExactResultService.PredictExactResult(m.HomeTeam, m.AwayTeam, datasetFrom, datasetTo);
                result.PredictedMatchesInDiv.Add(
                    new PredictionExactResultMatchResult
                    {
                        Match = m,
                        Prediction = prediction
                    }
                );
            }
            return result;
        }

        private bool TeamExistsInHistoricalData(string team, DateTime from, DateTime to)
        {
            var exists = _db.Matches.Where(x => x.Date >= from && x.Date <= to && x.HomeTeam == team).Any();
            return exists;
        }

        private List<Match> ClearNonExistingTeamInHistoricalData(List<Match> matches, DateTime from, DateTime to)
        {
            var cleared_list = new List<Match>();
            foreach (var m in matches)
            {
                if(TeamExistsInHistoricalData(m.HomeTeam, from, to) && TeamExistsInHistoricalData(m.AwayTeam, from, to))
                {
                    cleared_list.Add(m);
                }
                else
                {
                    _logger.LogWarning("Match " + m.HomeTeam + " vs " + m.AwayTeam + " excluded from tests because one of the two teams dont exists in historical data.");
                }
            }
            return cleared_list;
        }
    }
}

