using System;
namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers
{
	public class PredictionResponse_PoissonExactResult_BothTeamsToScore
	{
        private readonly ILogger<PredictionResponse_PoissonExactResult_BothTeamsToScore> _logger;
        private PredictionResponse_PoissonExactResult _prediction;

        public PredictionResponse_PoissonExactResult_BothTeamsToScore(
                ILogger<PredictionResponse_PoissonExactResult_BothTeamsToScore> logger)
		{
            _logger = logger;
        }

        public PredictionResponse_PoissonExactResult_BothTeamsToScore CreateReader(PredictionResponse_PoissonExactResult prediction)
        {
            _prediction = prediction;
            return this;
        }

        public Dictionary<string, double> GetBothTeamsToScoreProbabilities()
        {
            var result = new Dictionary<string, double>()
            {
                { "Yes", 0 },
                { "No", 0 },
            };

            foreach (var m in _prediction.MatchResults)
            {
                if (m.HomeScored > 0 && m.AwayScored > 0)
                    result["Yes"] += m.Probability;
                else
                    result["No"] += m.Probability;
            }
            return result;
        }
	}
}

