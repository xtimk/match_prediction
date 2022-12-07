namespace MatchPrediction.Models.PredictionResults.ExactResult
{
    public class Prediction_ExactResult_Response
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string HomeTeamName { get; set; } = "";
        public string AwayTeamName { get; set; } = "";
        public double HomeTeam_GoalScored_Average_Weighted { get; set; }
        public double HomeTeam_GoalConceded_Average_Weighted { get; set; }
        public double AwayTeam_GoalScored_Average_Weighted { get; set; }
        public double AwayTeam_GoalConceded_Average_Weighted { get; set; }
        public double  Home_Lambda_Strength { get; set; }
        public double Away_Lambda_Strength { get; set; }
        public List<MatchExactResult> MatchResults { get; set; } = new List<MatchExactResult>();
        public double HomeWinsProbability
        {
            get
            {
                double p = 0;
                foreach (var matchresult in MatchResults)
                {
                    if (matchresult.HomeScored > matchresult.AwayScored)
                    {
                        p += matchresult.Probability;
                    }
                }
                return p;
            }
        }
        public double AwayWinsProbability
        {
            get
            {
                double p = 0;
                foreach (var matchresult in MatchResults)
                {
                    if (matchresult.AwayScored > matchresult.HomeScored)
                    {
                        p += matchresult.Probability;
                    }
                }
                return p;
            }
        }
        public double EvenProbability
        {
            get
            {
                double p = 0;
                foreach (var matchresult in MatchResults)
                {
                    if (matchresult.AwayScored == matchresult.HomeScored)
                    {
                        p += matchresult.Probability;
                    }
                }
                return p;
            }
        }
        public double HomeWinsOdd
        {
            get
            {
                return 1 / HomeWinsProbability;
            }
        }
        public double AwayWinsOdd
        {
            get
            {
                return 1 / AwayWinsProbability;
            }
        }
        public double EvenOdd
        {
            get
            {
                return 1 / EvenProbability;
            }
        }
    }

    public class MatchExactResult
    {
        public int HomeScored { get; set; }
        public int AwayScored { get; set; }
        public double Probability { get; set; }
        public double Odd { get; set; }
    }
}
