using System;
using MatchPrediction.Managers.PredictionManagers.PoissonExactResult;
using MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers;
using MatchPrediction.Constants;

namespace MatchPrediction.Managers.PredictionManagers
{
	public abstract class PredictionResponseReader
	{
        public PredictionResponseReader() { }

        public abstract Dictionary<string, double> ExecuteReader(PredictionResponse_PoissonExactResult prediction);
	}
}

