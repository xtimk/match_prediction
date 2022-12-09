using MatchPrediction.Managers.PredictionManagers.PoissonExactResult;
using MatchPrediction.Models.PredictionResults.ExactResult;

namespace MatchPrediction.Services.MatchPredictionServices.ExactResult
{
    public interface IMatchExactResultService
    {
        Task<PredictionResponse_PoissonExactResult> PredictExactResult(string home_team_name, string away_team_name, DateTime dataSetFrom, DateTime dataSetTo);
    }
}
