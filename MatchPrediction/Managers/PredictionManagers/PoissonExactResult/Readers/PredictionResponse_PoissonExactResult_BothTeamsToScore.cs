using System;
namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers
{
	public class PredictionResponse_PoissonExactResult_BothTeamsToScore : PredictionResponseReader
	{
        private readonly ILogger<PredictionResponse_PoissonExactResult_BothTeamsToScore> _logger;

        public PredictionResponse_PoissonExactResult_BothTeamsToScore(
                ILogger<PredictionResponse_PoissonExactResult_BothTeamsToScore> logger)
		{
            _logger = logger;
        }

        public override Dictionary<string, double> ExecuteReader(PredictionResponse_PoissonExactResult prediction)
        {
            var result = new Dictionary<string, double>()
            {
                { "Yes", 0 },
                { "No", 0 },
            };

            foreach (var m in prediction.MatchResults)
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

