namespace MatchPrediction.Models
{
    public class LeagueGoalsAverageQueryResult
    {
        public string Div { get; set; } = default!;
        public double LeagueHomeGoalsAvg { get; set; }
        public double LeagueAwayGoalsAvg { get; set; }
        public double LeagueTotalGoalsAvg { get; set; }
    }
}
