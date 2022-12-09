using System;
namespace MatchPrediction.Managers.PredictionManagers
{
	public abstract class PredictionResponse
	{
        public bool ExecutedWithoutErrors { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();
    }
}

