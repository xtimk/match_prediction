using System;
using MatchPrediction.Models.PredictionResults.ExactResult;

namespace MatchPrediction.Services.MatchPredictionServices.ExactResult
{
	public interface IMatchExactResultTesterService
	{
        Task<PredictionExactResultTesterResult> TeamWinner_AllDivisions(
            DateTime datasetFrom, DateTime datasetTo,
            DateTime dataTestFrom, DateTime dataTestTo);

        Task<PredictionExactResultDivTesterResult> TeamWinner_InDivision(
            DateTime datasetFrom, DateTime datasetTo,
            DateTime dataTestFrom, DateTime dataTestTo,
            string div);

    }
}

