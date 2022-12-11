using System;
namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult
{
	public class PredictionResponse_PoissonExactResult : PredictionResponse
	{
        public string HomeTeamName { get; set; } = "";
        public string AwayTeamName { get; set; } = "";

        // Poisson related parameters
        public double HomeTeam_GoalScored_Average_Weighted { get; set; }
        public double HomeTeam_GoalConceded_Average_Weighted { get; set; }
        public double AwayTeam_GoalScored_Average_Weighted { get; set; }
        public double AwayTeam_GoalConceded_Average_Weighted { get; set; }
        public double Home_Lambda_Strength { get; set; }
        public double Away_Lambda_Strength { get; set; }

        // List of exact result matches
        public List<MatchExactResult> MatchResults { get; set; } = new List<MatchExactResult>();

        public class MatchExactResult
        {
            public int HomeScored { get; set; }
            public int AwayScored { get; set; }
            public double Probability { get; set; }
            public double Odd { get; set; }
        }
    }
}

