namespace MatchPrediction.Models.MatchPrediction
{
    public class Match
    {
        public Guid Id { get; set; }
        public string Div { get; set; } = default!;
        public DateTime Date { get; set; }
        public string HomeTeam { get; set; } = default!;
        public string AwayTeam { get; set; } = default!;
        public int FTHG { get; set; }
        public int FTAG { get; set; }
    }
}
