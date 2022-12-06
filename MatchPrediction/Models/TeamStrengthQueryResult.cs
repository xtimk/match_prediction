namespace MatchPrediction.Models
{
    public class TeamStrengthQueryResult
    {
        public string Div { get; set; }
        public string Team { get; set; }
        public double HomeStrenght { get; set; }
        public double AwayStrength { get; set; }
        public double HomeGoalsAvg { get; set; }
        public double HomeGoalAvgWeighted { get; set; }
        public double AwayGoalsAvg { get; set; }
        public double AwayGoalAvgWeighted { get; set; }
        public double HomeGoalsConceededAvg { get; set; }
        public double HomeGoalsConceededAvgWeighted { get; set; }
        public double AwayGoalsConceededAvg { get; set; }
        public double AwayGoalsConceededAvgWeighted { get; set; }
    }
}
