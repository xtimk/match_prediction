using MatchPrediction.Models.MatchPrediction;
using Microsoft.EntityFrameworkCore;

namespace MatchPrediction.Data.Contexts
{
    public class MatchPredictionContext : DbContext
    {
        public MatchPredictionContext(DbContextOptions options) : base(options) { }

        public DbSet<Match> Matches { get; set; }
    }
}
