using MatchPrediction.Data.Contexts;
using MatchPrediction.Services.MatchStatsGetterService;
using Microsoft.EntityFrameworkCore;

namespace MatchPrediction.Data
{
    public class DbInitializer
    {
        private readonly ILogger<DbInitializer> _logger;
        private readonly MatchPredictionContext _db;
        private readonly IMatchStatsGetterService _matchStatsGetterService;

        public DbInitializer(
                ILogger<DbInitializer> logger, 
                MatchPredictionContext db,
                IMatchStatsGetterService matchStatsGetterService)
        {
            _logger = logger;
            _db = db;
            _matchStatsGetterService = matchStatsGetterService;
        }
        public async Task Initialize()
        {
            _logger.LogInformation("Checking if db needs to be seeded");
            if(!await _db.Matches.AnyAsync())
            {
                var matches = await _matchStatsGetterService.GetMatchesStats();
                await _db.Matches.AddRangeAsync(matches);
                await _db.SaveChangesAsync();
            }
        }
    }
}
