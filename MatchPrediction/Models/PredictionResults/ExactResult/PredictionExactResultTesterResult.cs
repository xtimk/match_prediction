using System;
using MatchPrediction.Managers.PredictionManagers.PoissonExactResult;
using MatchPrediction.Models.MatchPrediction;

namespace MatchPrediction.Models.PredictionResults.ExactResult
{
	public class PredictionExactResultTesterResult
	{
        public List<PredictionExactResultDivTesterResult> DivisionPredictions { get; set; }
    }

    public class PredictionExactResultDivTesterResult
	{
		public string Div { get; set; }
		public List<PredictionExactResultMatchResult> PredictedMatchesInDiv { get; set; }
		public double WinnerPredictionAccuracy
		{
			get
			{
				double ok = 0;
				double ko = 0;
				foreach (var m in PredictedMatchesInDiv)
				{
					if (m.WinnerPredictedCorrectly)
						ok++;
					else
						ko++;
				}

				return ok / (ok + ko);
			}
		}
        public double BothTeamsToScorePredictionAccuracy
        {
            get
            {
                double ok = 0;
                double ko = 0;
                foreach (var m in PredictedMatchesInDiv)
                {
                    if (m.BothTeamsToScorePredictedCorrectly)
                        ok++;
                    else
                        ko++;
                }

                return ok / (ok + ko);
            }
        }
    }

	public class PredictionExactResultMatchResult
	{
		public Match Match { get; set; }
		public string Predicted_Winner
		{
			get
			{
				throw new NotImplementedException();
				//if (Prediction.HomeWinsProbability > Prediction.EvenProbability &&
    //                Prediction.HomeWinsProbability > Prediction.AwayWinsProbability)
				//{
				//	return Match.HomeTeam;
				//}
    //            if (Prediction.AwayWinsProbability > Prediction.EvenProbability &&
				//	Prediction.AwayWinsProbability > Prediction.HomeWinsProbability)
    //            {
    //                return Match.AwayTeam;
    //            }
				return "Even";

            }
        }
		public PredictionResponse_PoissonExactResult Prediction { get; set; }
		public bool WinnerPredictedCorrectly {
			get
			{
				if (Predicted_Winner == Match.HomeTeam && Match.FTHG > Match.FTAG)
					return true;
                if (Predicted_Winner == Match.AwayTeam && Match.FTHG < Match.FTAG)
                    return true;
                if (Predicted_Winner == "Even" && Match.FTHG == Match.FTAG)
                    return true;

				return false;
            }
        }

		public bool WinnerPredictedWrong
		{
			get
			{
				return !WinnerPredictedCorrectly;
			}
		}

        public string Predicted_BothTeamsToScore
        {
            get
            {
                throw new NotImplementedException();
                //            if (Prediction.BothTeamsToScoreProbability >= Prediction.BothTeamsNotToScoreProbability)
                //            {
                //                return "YES";
                //            }
                //else
                //{
                //	return "NO";
                //}
            }
        }
        public bool BothTeamsToScorePredictedCorrectly
        {
            get
            {
                if(Predicted_BothTeamsToScore == "YES" && Match.FTHG > 0 && Match.FTAG > 0)
				{
					return true;
				}
                if (Predicted_BothTeamsToScore == "NO" && (Match.FTHG == 0 || Match.FTAG == 0))
                {
                    return true;
                }
				return false;
            }
        }
    }
}

