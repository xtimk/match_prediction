using MatchPrediction.Models.PredictionResults.ExactResult;

namespace MatchPrediction.Services.MatchPredictionServices.ExactResult
{
    public interface IMatchExactResultService
    {
        Task<Prediction_ExactResult_Response> PredictExactResult(string home_team_name, string away_team_name, DateTime dataSetFrom, DateTime dataSetTo);
    }
}
