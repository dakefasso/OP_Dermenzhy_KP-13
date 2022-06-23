namespace Лабораторна4
{
    public class CommonQueries
    {
        static public string TeamGame()
        {
            return
$@"SELECT *
FROM game
WHERE home_team = 
(
SELECT team_id
FROM team
WHERE team_name = 'Уведіть назву команди'
)
OR guest_team = 
(
SELECT team_id
FROM team
WHERE team_name = 'Уведіть назву команди ще раз'
);";
        }

        static public string PlayersInGame()
        {
            return
$@"SELECT *
FROM player
WHERE player_id = 
(
 SELECT player_id 
 FROM players_in_games
 WHERE game_id = 
 (
  SELECT game_id 
  FROM game
  WHERE home_team = 
  (
   SELECT team_id 
   FROM team
   WHERE team_name = 'Уведіть назву команди-хозяйки'
  )
  AND guest_team = 
  (
   SELECT team_id 
   FROM team
   WHERE team_name = 'Уведіть назву команди-гостя'
  )
 )
);";
        }

        static public string TicketPriceGame()
        {
            return
$@"SELECT ticket_price
FROM game
WHERE home_team =
(
 SELECT team_id
 FROM team
 WHERE team_name = 'Уведіть назву команди-хозяйки'
)
AND guest_team =
(
 SELECT team_id
 FROM team
 WHERE team_name = 'Уведіть назву команди-гостя'
);";
        }

        static public string PlayerScoredMost()
        {
            return
$@"SELECT *
FROM player
WHERE goals_scored =
(
 SELECT MAX(goals_scored)
 FROM player
);";
        }

        static public string TeamsScoredConceded()
        {
            return
$@"SELECT *
FROM team
WHERE team_id =
(
 SELECT team_id
 FROM results_table
 WHERE goals_scored-goals_conceded =
 (
  SELECT MAX(goals_scored-goals_conceded)
  FROM results_table
 )
)";
        }

        static public string YoungestPlayer()
        {
            return
$@"SELECT *
FROM player
WHERE age =
(
 SELECT MIN(age)
 FROM player
);";
        }

        static public string CountryPlayers()
        {
            return
$@"SELECT *
FROM player
WHERE country = 'Уведіть назву країни'";
        }

        static public string ChampionsTeams()
        {
            return
$@"SELECT *
FROM team
WHERE team_id = 
(
 SELECT team_id 
 FROM results_table
 WHERE to_champions_league = true
);";
        }

        static public string MostProfitTeam()
        {
            return
$@"SELECT *
FROM team
WHERE team_id =
(
 SELECT team_id
 FROM budget
 WHERE profit =
 (
  SELECT MAX(profit)
  FROM budget
 )
)";
        }
    }
}
