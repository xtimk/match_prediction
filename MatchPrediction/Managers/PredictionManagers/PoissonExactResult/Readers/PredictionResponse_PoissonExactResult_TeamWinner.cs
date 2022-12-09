using System;
namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers
{
	public class PredictionResponse_PoissonExactResult_TeamWinner
	{
        private readonly ILogger<PredictionResponse_PoissonExactResult_TeamWinner> _logger;
        private PredictionResponse_PoissonExactResult _prediction;

        public PredictionResponse_PoissonExactResult_TeamWinner(
                PredictionResponse_PoissonExactResult prediction,
                ILogger<PredictionResponse_PoissonExactResult_TeamWinner> logger)
		{
            _logger = logger;
        }

        public PredictionResponse_PoissonExactResult_TeamWinner CreateReader(PredictionResponse_PoissonExactResult prediction)
        {
            _prediction = prediction;
            return this;
        }

        public Dictionary<string, double> GetTeamWinnerProbabilities()
        {
            var home_key = _prediction.HomeTeamName + " (Home)";
            var away_key = _prediction.AwayTeamName + " (Away)";
            var even_key = "Even";

            var result = new Dictionary<string, double>()
            {
                { home_key, 0 },
                { even_key, 0 },
                { away_key, 0 },
            };

            foreach (var m in _prediction.MatchResults)
            {
                if (m.HomeScored > m.AwayScored)
                    result[home_key] += m.Probability;
                else if (m.HomeScored < m.AwayScored)
                    result[away_key] += m.Probability;
                else if (m.HomeScored == m.AwayScored)
                    result[even_key] += m.Probability;
                else
                {
                    _logger.LogCritical("Found a match that is not 1, X or 2. Impossible!! Check.");
                    throw new Exception("Found a match that is not 1, X or 2. Impossible!! Check.");
                }

            }
            return result;
        }
	}
}

