using System.Collections.Generic;

namespace Лаба3
{
    public class ResultsTable
    {
        private int totalTeams;
        
        public ResultsTable(int totalTeams)
        {
            this.totalTeams = totalTeams;
        }

        public int Place { get; private set; }
        public int Team { get; set; }
        public int GamesPlayed { get; set; }

        private int score;
        public int Wins { get; set; } 
        public int Losses { get; set; }
        public int Draws { get; set; }

        public int GoalsScored { get; set; }
        public int GoalsConceded { get; set; }

        private int totalToChampionsLeagueTeams = 3;
        private int totalToLowerLeagueTeams = 3;
        private bool toChampionsLeague;
        private bool toLowerLeague;

        public bool CheckLowerLeague()
        {
            toLowerLeague = Place >= totalTeams - totalToLowerLeagueTeams + 1 ? true : false; 
            return toLowerLeague;
        }

        public bool CheckChampionsLeague()
        {
            toChampionsLeague = Place <= totalToChampionsLeagueTeams ? true : false;
            return toChampionsLeague;
        }
        
        public int GetTeamScore()
        {
            score = Wins * 3 + Draws;
            return score;
        }

        public static void FindPlace(List<ResultsTable> resultsList, List<Team> teamList)
        {
            for (int i = 0; i < resultsList.Count; i++)
            {
                for (int j = 0; j < resultsList.Count - 1 - i; j++)
                {
                    if (resultsList[j].GetTeamScore() < resultsList[j + 1].GetTeamScore())
                    {
                        ResultsTable temp = resultsList[j];
                        resultsList[j] = resultsList[j + 1];
                        resultsList[j + 1] = temp;

                        Team tempTeam =  teamList[j];
                        teamList[j] = teamList[j + 1];
                        teamList[j + 1] = tempTeam;
                    }
                    else if (resultsList[j].GetTeamScore() == resultsList[j + 1].GetTeamScore())
                    {
                        if (resultsList[j].GoalsScored - resultsList[j].GoalsConceded < resultsList[j + 1].GoalsScored - resultsList[j + 1].GoalsConceded)
                        {
                            ResultsTable temp = resultsList[j];
                            resultsList[j] = resultsList[j + 1];
                            resultsList[j + 1] = temp;

                            Team tempTeam = teamList[j];
                            teamList[j] = teamList[j + 1];
                            teamList[j + 1] = tempTeam;
                        }
                        else if (resultsList[j].GoalsScored - resultsList[j].GoalsConceded == resultsList[j + 1].GoalsScored - resultsList[j + 1].GoalsConceded)
                        {
                            if (resultsList[j].Wins < resultsList[j + 1].Wins)
                            {
                                ResultsTable temp = resultsList[j];
                                resultsList[j] = resultsList[j + 1];
                                resultsList[j + 1] = temp;

                                Team tempTeam = teamList[j];
                                teamList[j] = teamList[j + 1];
                                teamList[j + 1] = tempTeam;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < resultsList.Count; i++)
            {
                resultsList[i].Place = i + 1;
            }
        }
    }
}