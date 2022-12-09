using System;
namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers
{
	public class PredictionResponse_PoissonExactResult_ExactResult
	{
        private PredictionResponse_PoissonExactResult _prediction;
        private readonly ILogger<PredictionResponse_PoissonExactResult_ExactResult> _logger;

		public PredictionResponse_PoissonExactResult_ExactResult(
                ILogger<PredictionResponse_PoissonExactResult_ExactResult> logger)
		{
            _logger = logger;
        }

        public PredictionResponse_PoissonExactResult_ExactResult CreateReader(PredictionResponse_PoissonExactResult prediction)
        {
            _prediction = prediction;
            return this;
        }

        public Dictionary<string, double> GetExactResultProbabilities()
        {
            var result = new Dictionary<string, double>();

            foreach (var m in _prediction.MatchResults)
            {
                result.Add(_prediction.HomeTeamName + " " + m.HomeScored + " - " + m.AwayScored + " " + _prediction.AwayTeamName, m.Probability);
            }

            return result;
        }
    }
}

