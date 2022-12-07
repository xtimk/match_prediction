using MatchPrediction.Data.Contexts;
using MatchPrediction.Models;

namespace MatchPrediction.Services.QueryService
{
    public class QueryService
    {
        private readonly MatchPredictionContext _db;

        public QueryService(MatchPredictionContext db)
        {
            _db = db;
        }

        public IQueryable<TeamStrengthQueryResult> GetTeamStrength()
        {
            var query = _db.Matches.AsQueryable();
            var HomeGoalsAvg = query.GroupBy(x => x.HomeTeam).Select(x => new { Key = x.Key, HomeTeamGoalsAvg = x.Average(s => s.FTHG) });
            var AwayGoalsAvg = query.GroupBy(x => x.AwayTeam).Select(x => new { Key = x.Key, AwayTeamGoalsAvg = x.Average(s => s.FTAG) });
            var HomeGoalsConceededAvg = query.GroupBy(x => x.HomeTeam).Select(x => new { Key = x.Key, HomeTeamConceededGoalsAvg = x.Average(s => s.FTAG) });
            var AwayGoalsConceededAvg = query.GroupBy(x => x.AwayTeam).Select(x => new { Key = x.Key, AwayTeamConceededGoalsAvg = x.Average(s => s.FTHG) });
            var DivTeam = query.Select(x => new { x.Div, Team = x.HomeTeam }).Distinct();
            var divisionAvgHomeGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueHomeGoalsAvg = x.Average(s => s.FTHG) });
            var divisionAvgAwayGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueAwayGoalsAvg = x.Average(s => s.FTAG) });
            var divisionAvgTotalGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueTotalGoalsAvg = x.Average(s => s.FTHG + s.FTAG) });

            var TeamAvgGoals = HomeGoalsAvg.Join(
                    AwayGoalsAvg,
                    h => h.Key,
                    a => a.Key,
                    (h, a) => new
                    {
                        Team = h.Key,
                        HomeGoalsAvg = h.HomeTeamGoalsAvg,
                        AwayGoalsAvg = a.AwayTeamGoalsAvg
                    }
                )
                .Join(
                    HomeGoalsConceededAvg,
                    h => h.Team,
                    a => a.Key,
                    (h, a) => new
                    {
                        Team = h.Team,
                        HomeGoalsAvg = h.HomeGoalsAvg,
                        AwayGoalsAvg = h.AwayGoalsAvg,
                        HomeGoalsConceededAvg = a.HomeTeamConceededGoalsAvg
                    }
                )
                .Join(
                    AwayGoalsConceededAvg,
                    h => h.Team,
                    a => a.Key,
                    (h, a) => new
                    {
                        Team = h.Team,
                        HomeGoalsAvg = h.HomeGoalsAvg,
                        AwayGoalsAvg = h.AwayGoalsAvg,
                        HomeGoalsConceededAvg = h.HomeGoalsConceededAvg,
                        AwayGoalsConceededAvg = a.AwayTeamConceededGoalsAvg
                    }
                )
                .Join(
                    DivTeam,
                    h => h.Team,
                    a => a.Team,
                    (h, a) => new
                    {
                        Div = a.Div,
                        Team = h.Team,
                        HomeGoalsAvg = h.HomeGoalsAvg,
                        AwayGoalsAvg = h.AwayGoalsAvg,
                        HomeGoalsConceededAvg = h.HomeGoalsConceededAvg,
                        AwayGoalsConceededAvg = h.AwayGoalsConceededAvg,
                    }
                );

            var divisionAvgGoals = divisionAvgHomeGoals.Join(
                divisionAvgAwayGoals,
                dahg => dahg.Key,
                daag => daag.Key,
                (dahg, daag) => new
                {
                    dahg.Key,
                    dahg.LeagueHomeGoalsAvg,
                    daag.LeagueAwayGoalsAvg
                })
                .Join(
                    divisionAvgTotalGoals,
                    l => l.Key,
                    dag => dag.Key,
                    (l, r) => new
                    {
                        Div = l.Key,
                        l.LeagueHomeGoalsAvg,
                        l.LeagueAwayGoalsAvg,
                        r.LeagueTotalGoalsAvg
                    });

            var finalTable = TeamAvgGoals.Join(
                divisionAvgGoals,
                l => l.Div,
                dag => dag.Div,
                (l, dag) => new TeamStrengthQueryResult()
                {
                    Div = l.Div,
                    Team = l.Team,
                    HomeStrenght = (l.HomeGoalsAvg / dag.LeagueHomeGoalsAvg) * (l.HomeGoalsConceededAvg / dag.LeagueAwayGoalsAvg),
                    AwayStrength = (l.AwayGoalsAvg / dag.LeagueAwayGoalsAvg) * (l.AwayGoalsConceededAvg / dag.LeagueHomeGoalsAvg),
                    HomeGoalsAvg = l.HomeGoalsAvg,
                    HomeGoalAvgWeighted = l.HomeGoalsAvg / dag.LeagueHomeGoalsAvg,
                    AwayGoalsAvg = l.AwayGoalsAvg,
                    AwayGoalAvgWeighted = l.HomeGoalsAvg / dag.LeagueAwayGoalsAvg,
                    HomeGoalsConceededAvg = l.HomeGoalsConceededAvg,
                    HomeGoalsConceededAvgWeighted = l.HomeGoalsConceededAvg / dag.LeagueAwayGoalsAvg,
                    AwayGoalsConceededAvg = l.AwayGoalsConceededAvg,
                    AwayGoalsConceededAvgWeighted = l.AwayGoalsConceededAvg / dag.LeagueHomeGoalsAvg,
                });
            return finalTable;
        }

        public IQueryable<LeagueGoalsAverageQueryResult> GetDivisionGoalsAverage()
        {
            var divisionAvgHomeGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueHomeGoalsAvg = x.Average(s => s.FTHG) });
            var divisionAvgAwayGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueAwayGoalsAvg = x.Average(s => s.FTAG) });
            var divisionAvgGoals = _db.Matches.GroupBy(x => x.Div).Select(x => new { x.Key, LeagueTotalGoalsAvg = x.Average(s => s.FTHG + s.FTAG) });
            var queryDatatables = divisionAvgHomeGoals.Join(
                divisionAvgAwayGoals,
                dahg => dahg.Key,
                daag => daag.Key,
                (dahg, daag) => new
                {
                    dahg.Key,
                    dahg.LeagueHomeGoalsAvg,
                    daag.LeagueAwayGoalsAvg
                })
                .Join(
                    divisionAvgGoals,
                    l => l.Key,
                    dag => dag.Key,
                    (l, r) => new LeagueGoalsAverageQueryResult()
                    {
                        Div = l.Key,
                        LeagueHomeGoalsAvg = l.LeagueHomeGoalsAvg,
                        LeagueAwayGoalsAvg = l.LeagueAwayGoalsAvg,
                        LeagueTotalGoalsAvg = r.LeagueTotalGoalsAvg
                    });
            return queryDatatables;
        }

        public IQueryable<string> GetAllTeams()
        {
            var homeTeams = _db.Matches.Select(x => x.HomeTeam);
            var awayTeams = _db.Matches.Select(x => x.AwayTeam);
            var teams = homeTeams.Union(awayTeams).Distinct();
            return teams;
        }
    }
}
