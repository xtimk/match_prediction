using System;
namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers
{
	public class PredictionResponse_PoissonExactResult_ExactResult : PredictionResponseReader
	{
        private readonly ILogger<PredictionResponse_PoissonExactResult_ExactResult> _logger;

		public PredictionResponse_PoissonExactResult_ExactResult(
                ILogger<PredictionResponse_PoissonExactResult_ExactResult> logger)
		{
            _logger = logger;
        }

        public override Dictionary<string, double> ExecuteReader(PredictionResponse_PoissonExactResult prediction)
        {
            var result = new Dictionary<string, double>();

            foreach (var m in prediction.MatchResults)
            {
                result.Add(prediction.HomeTeamName + " " + m.HomeScored + " - " + m.AwayScored + " " + prediction.AwayTeamName, m.Probability);
            }

            return result;
        }
    }
}

