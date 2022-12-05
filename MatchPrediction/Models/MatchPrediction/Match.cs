namespace MatchPrediction.Models.MatchPrediction
{
    public class Match
    {
        public Guid Id { get; set; }
        public string Div { get; set; }
        public DateTime Date { get; set; }
        //public string Time { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int FTHG { get; set; }
        public int FTAG { get; set; }
    }
}
