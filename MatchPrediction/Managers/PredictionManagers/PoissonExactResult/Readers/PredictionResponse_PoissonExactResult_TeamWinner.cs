using System;
namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers
{
	public class PredictionResponse_PoissonExactResult_TeamWinner : PredictionResponseReader
    {
        private readonly ILogger<PredictionResponse_PoissonExactResult_TeamWinner> _logger;

        public PredictionResponse_PoissonExactResult_TeamWinner(
                ILogger<PredictionResponse_PoissonExactResult_TeamWinner> logger)
		{
            _logger = logger;
        }

        public override Dictionary<string, double> ExecuteReader(PredictionResponse_PoissonExactResult prediction)
        {
            var home_key = prediction.HomeTeamName + " (Home)";
            var away_key = prediction.AwayTeamName + " (Away)";
            var even_key = "Even";

            var result = new Dictionary<string, double>()
            {
                { home_key, 0 },
                { even_key, 0 },
                { away_key, 0 },
            };

            foreach (var m in prediction.MatchResults)
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

