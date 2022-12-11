using System;
namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers
{
	public class PredictionResponse_PoissonExactResult_UnderXGoals : PredictionResponseReader
	{
        private readonly ILogger<PredictionResponse_PoissonExactResult_UnderXGoals> _logger;

        public PredictionResponse_PoissonExactResult_UnderXGoals(
                ILogger<PredictionResponse_PoissonExactResult_UnderXGoals> logger)
		{
            _logger = logger;
        }

        public override Dictionary<string, double> ExecuteReader(PredictionResponse_PoissonExactResult prediction)
        {
            var result = new Dictionary<string, double>();

            var temp = new Dictionary<double, double>()
            {
                { 0.5, 0 },
                { 1.5, 0 },
                { 2.5, 0 },
                { 3.5, 0 },
                { 4.5, 0 },
                { 5.5, 0 },
                { 6.5, 0 },
                { 7.5, 0 },
                { 8.5, 0 },
                { 9.5, 0 },
            };

            foreach (var m in prediction.MatchResults)
            {
                var scored = m.HomeScored + m.AwayScored;
                foreach (var t in temp)
                {
                    if (scored < t.Key)
                        temp[t.Key] += m.Probability;
                }
            }

            foreach (var t in temp)
            {
                result.Add("Under " + t.Key.ToString(), t.Value);
            }
            return result;
        }
	}
}

