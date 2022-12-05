using MatchPrediction.Models.MatchPrediction;

namespace MatchPrediction.Services.MatchStatsGetterService
{
    public interface IMatchStatsGetterService
    {
        Task<IEnumerable<Match>> GetMatchesStats();
    }
}
