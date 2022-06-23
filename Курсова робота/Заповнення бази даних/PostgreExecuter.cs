using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Лаба3
{
    internal class PostgreExecuter
    {
        private static Random rnd = new Random();
        static public void ClearTables()
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
                @"DELETE FROM players_in_games CASCADE;
                  DELETE FROM budget CASCADE;
                  DELETE FROM game CASCADE;
                  DELETE FROM stadium CASCADE;
                  DELETE FROM player CASCADE;
                  DELETE FROM results_table CASCADE;
                  DELETE FROM team CASCADE;";
            command.CommandText = Cut(commandText);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Insert(Team team)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
                @"INSERT INTO team (team_name, coach_name, previous_place) 
                  VALUES(@team_name, @coach_name, @previous_place);";
            command.CommandText = Cut(commandText);

            command.Parameters.AddWithValue("team_name", team.TeamName);
            command.Parameters.AddWithValue("coach_name", team.CoachName);
            command.Parameters.AddWithValue("previous_place", team.PreviousPlace);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Insert(Stadium stadium, Team team)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"INSERT INTO stadium 
               (
                 home_team, city,
                 stadium_name, capacity
               ) 
               VALUES
               (
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @team_name
                ),
                @city, @stadium_name,
                @capacity
               );";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("team_name", team.TeamName);
            command.Parameters.AddWithValue("city", stadium.City);
            command.Parameters.AddWithValue("stadium_name", stadium.StadiumName);
            command.Parameters.AddWithValue("capacity", stadium.Capacity);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Insert(Budget budget, string teamName)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"INSERT INTO budget 
               (
                 team_id, 
                 cash, advertising,
                 tickets_sold, tickets_income,
                 selling_players, income, 
                 salary, buying_players, 
                 expenses, profit
               ) 
               VALUES
               (
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @team_name
                ), 
                 @cash, @advertising,
                 @tickets_sold, @tickets_income, 
                 @selling_players, @income, 
                 @salary, @buying_players, 
                 @expenses, @profit
               );";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("team_name", teamName);
            command.Parameters.AddWithValue("cash", budget.Cash);
            command.Parameters.AddWithValue("advertising", budget.Advertising);
            command.Parameters.AddWithValue("tickets_income", budget.TicketsIncome);
            command.Parameters.AddWithValue("tickets_sold", budget.TicketsSold);
            command.Parameters.AddWithValue("selling_players", budget.SellingPlayers);
            command.Parameters.AddWithValue("income", budget.CalculateIncome());
            command.Parameters.AddWithValue("salary", budget.Salary);
            command.Parameters.AddWithValue("buying_players", budget.BuyingPlayers);
            command.Parameters.AddWithValue("expenses", budget.CalculateExpenses());
            command.Parameters.AddWithValue("profit", budget.CalculateProfit());

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Insert(Game game)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"INSERT INTO game 
               (
                 stadium_id, 
                 home_team,
                 guest_team, weather,
                 game_date, game_started, game_ended, 
                 referee_name, game_result, 
                 visitors, ticket_price
               ) 
               VALUES
               (
                (
                 SELECT stadium_id 
                 FROM stadium
                 WHERE home_team = 
                   (
                    SELECT team_id 
                    FROM team
                    WHERE team_name = @home_team  
                   )
                ), 
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @home_team
                ),
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @guest_team
                ),
                @weather,
                @game_date, @game_started, @game_ended, 
                @referee_name, @game_result, 
                @visitors, @ticket_price
               );";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("home_team", game.HomeTeam);
            command.Parameters.AddWithValue("guest_team", game.GuestTeam);
            command.Parameters.AddWithValue("game_date", game.Date);
            command.Parameters.AddWithValue("game_started", game.Date.TimeOfDay);
            command.Parameters.AddWithValue("game_ended", game.EndTime);
            command.Parameters.AddWithValue("weather", game.Weather);
            command.Parameters.AddWithValue("referee_name", game.Referee);
            command.Parameters.AddWithValue("game_result", game.Result);
            command.Parameters.AddWithValue("visitors", game.Visitors);
            command.Parameters.AddWithValue("ticket_price", game.TicketPrice);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Insert(int playerID, string homeTeam, string guestTeam)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"INSERT INTO players_in_games 
               (
                 game_id, player_id
               ) 
               VALUES
               (
                (
                 SELECT game_id 
                 FROM game
                 WHERE home_team = 
                    (
                     SELECT team_id 
                     FROM team
                     WHERE team_name = @home_team 
                    )
                 AND guest_team =  
                    (
                     SELECT team_id 
                     FROM team
                     WHERE team_name = @guest_team 
                    )
                ),
                @player_id
               );";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("home_team", homeTeam);
            command.Parameters.AddWithValue("guest_team", guestTeam);
            command.Parameters.AddWithValue("player_id", playerID);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Insert(Player player, Team team)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"INSERT INTO player 
               (
                team_id, first_name, surname, age, 
                country, player_position, is_leader,
                games_played, goals_scored, assists, saves,
                salary, player_cost, contract_due
               ) 
               VALUES
               (
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @team_name
                ),
                @first_name, @surname, @age,
                @country, @player_position, @is_leader,
                @games_played, @goals_scored, @assists, @saves,
                @salary, @player_cost, 
                CAST(@contract_due as DATE)
               );";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("team_name", team.TeamName);
            command.Parameters.AddWithValue("first_name", player.FirstName);
            command.Parameters.AddWithValue("surname", player.Surname);
            command.Parameters.AddWithValue("age", player.Age);
            command.Parameters.AddWithValue("country", player.Country);
            command.Parameters.AddWithValue("player_position", player.Position);
            command.Parameters.AddWithValue("is_leader", player.IsLeader);
            command.Parameters.AddWithValue("games_played", player.GamesPlayed);
            command.Parameters.AddWithValue("goals_scored", player.GoalsScored);
            command.Parameters.AddWithValue("assists", player.Assists);
            command.Parameters.AddWithValue("saves", player.Saves);
            command.Parameters.AddWithValue("salary", player.Salary);
            command.Parameters.AddWithValue("player_cost", player.Cost);
            command.Parameters.AddWithValue("contract_due", player.ContractDueDate);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Insert(ResultsTable resultsTable, string teamName)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"INSERT INTO results_table 
               (
                team_id, place, score, games_played,
                wins, draws, losses, 
                goals_scored, goals_conceded,
                to_champions_league, to_lower_league
               ) 
               VALUES
               (
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @team_name
                ),
                @place, @score, @games_played,
                @wins, @draws, @losses,
                @goals_scored, @goals_conceded,
                @to_champions_league, @to_lower_league
               );";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("team_name", teamName);
            command.Parameters.AddWithValue("place", resultsTable.Place);
            command.Parameters.AddWithValue("games_played", resultsTable.GamesPlayed);
            command.Parameters.AddWithValue("score", resultsTable.GetTeamScore());
            command.Parameters.AddWithValue("wins", resultsTable.Wins);
            command.Parameters.AddWithValue("draws", resultsTable.Draws);
            command.Parameters.AddWithValue("losses", resultsTable.Losses);
            command.Parameters.AddWithValue("goals_scored", resultsTable.GoalsScored);
            command.Parameters.AddWithValue("goals_conceded", resultsTable.GoalsConceded);
            command.Parameters.AddWithValue("to_champions_league", resultsTable.CheckChampionsLeague());
            command.Parameters.AddWithValue("to_lower_league", resultsTable.CheckLowerLeague());

            command.ExecuteNonQuery();
            connection.Close();
        }



        static public int GetPlayerID(string position, string teamName)
        {
            List<int> playerIDs = new List<int>();

            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"SELECT player_id 
               FROM player
               WHERE player_position = @player_position 
                AND team_id = 
                 (
                  SELECT team_id 
                  FROM team
                  WHERE team_name = @team_name
                 )";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("player_position", position);
            command.Parameters.AddWithValue("team_name", teamName);

            NpgsqlDataReader reader = command.ExecuteReader();

            int playerID = -1;
            while (reader.Read())
            {
                playerIDs.Add(reader.GetInt32(rnd.Next(0)));
            }

            playerID = playerIDs[rnd.Next(playerIDs.Count)];

            reader.Close();
            connection.Close();
            return playerID;
        }

        static public void Update(Game game)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"UPDATE game 
               SET stadium_id = 
               (
                 SELECT stadium_id 
                 FROM stadium
                 WHERE home_team = 
                   (
                    SELECT team_id 
                    FROM team
                    WHERE team_name = @home_team  
                   )
                ), 
                home_team = 
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @home_team
                ),
                guest_team = 
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @guest_team
                ),
                game_date =  @game_date,
                game_started = @game_started,
                game_ended = @game_ended,
                weather = @weather,
                referee_name = @referee_name,
                game_result = @game_result,
                visitors = @visitors,
                ticket_price = @ticket_price
                WHERE home_team = 
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @home_team  
                )
                AND guest_team = 
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @guest_team
                )";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("home_team", game.HomeTeam);
            command.Parameters.AddWithValue("guest_team", game.GuestTeam);
            command.Parameters.AddWithValue("game_date", game.Date);
            command.Parameters.AddWithValue("game_started", game.Date.TimeOfDay);
            command.Parameters.AddWithValue("game_ended", game.EndTime);
            command.Parameters.AddWithValue("weather", game.Weather);
            command.Parameters.AddWithValue("referee_name", game.Referee);
            command.Parameters.AddWithValue("game_result", game.Result);
            command.Parameters.AddWithValue("visitors", game.Visitors);
            command.Parameters.AddWithValue("ticket_price", game.TicketPrice);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Update(Budget budget, string teamName)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"UPDATE budget 
               SET team_id = 
               (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @team_name 
                ), 
                cash = @cash,
                tickets_sold = @tickets_sold,
                tickets_income = @tickets_income,
                advertising = @advertising,
                selling_players = @selling_players,
                salary = @salary,
                buying_players = @buying_players,
                expenses = @expenses,
                profit = @profit,
                income = @income
                WHERE team_id = 
                    (
                     SELECT team_id
                     FROM team
                     WHERE team_name = @team_name
                    )";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("team_name", teamName);
            command.Parameters.AddWithValue("cash", budget.Cash);
            command.Parameters.AddWithValue("tickets_sold", budget.TicketsSold);
            command.Parameters.AddWithValue("tickets_income", budget.TicketsIncome);
            command.Parameters.AddWithValue("advertising", budget.Advertising);
            command.Parameters.AddWithValue("selling_players", budget.SellingPlayers);
            command.Parameters.AddWithValue("buying_players", budget.BuyingPlayers);
            command.Parameters.AddWithValue("expenses", budget.CalculateExpenses());
            command.Parameters.AddWithValue("profit", budget.CalculateProfit());
            command.Parameters.AddWithValue("income", budget.CalculateIncome());

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void Update(ResultsTable resultsTable, string teamName)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"UPDATE results_table 
               SET team_id = 
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @team_name
                ),
                place = @place,
                score = @score,
                games_played = @games_played,
                wins = @wins,
                draws = @draws,
                losses = @losses,
                goals_scored = @goals_scored,
                goals_conceded = @goals_conceded,
                to_champions_league = @to_champions_league,
                to_lower_league = @to_lower_league
                WHERE team_id = 
                (
                 SELECT team_id 
                 FROM team
                 WHERE team_name = @team_name
                )";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("team_name", teamName);
            command.Parameters.AddWithValue("place", resultsTable.Place);
            command.Parameters.AddWithValue("score", resultsTable.GetTeamScore());
            command.Parameters.AddWithValue("games_played", resultsTable.GamesPlayed);
            command.Parameters.AddWithValue("wins", resultsTable.Wins);
            command.Parameters.AddWithValue("draws", resultsTable.Draws);
            command.Parameters.AddWithValue("losses", resultsTable.Losses);
            command.Parameters.AddWithValue("goals_scored", resultsTable.GoalsScored);
            command.Parameters.AddWithValue("goals_conceded", resultsTable.GoalsConceded);
            command.Parameters.AddWithValue("to_champions_league", resultsTable.CheckChampionsLeague());
            command.Parameters.AddWithValue("to_lower_league", resultsTable.CheckLowerLeague());

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void UpdateGamesPlayed(int playerID)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"UPDATE player 
               SET games_played = 
                 (
                  SELECT games_played
                  FROM player
                  WHERE player_id = @player_id
                 ) + 1
               WHERE player_id = @player_id;";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("player_id", playerID);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void UpdateGoal(int playerID)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"UPDATE player 
               SET goals_scored = 
                 (
                  SELECT goals_scored
                  FROM player
                  WHERE player_id = @player_id
                 ) + 1
                WHERE player_id = @player_id;";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("player_id", playerID);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void UpdateAssist(int playerID)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"UPDATE player 
               SET assists = 
                 (
                  SELECT assists
                  FROM player
                  WHERE player_id = @player_id
                 ) + 1
                WHERE player_id = @player_id;";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("player_id", playerID);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static public void UpdateSave(int playerID)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText =
             @"UPDATE player 
               SET saves = 
                 (
                  SELECT saves
                  FROM player
                  WHERE player_id = @player_id
                 ) + 1
                WHERE player_id = @player_id;";

            command.CommandText = Cut(commandText);
            command.Parameters.AddWithValue("player_id", playerID);

            command.ExecuteNonQuery();
            connection.Close();
        }

        static private NpgsqlConnection GetConnection()
        {
            try
            {
                string server = "localhost",
                database = "FootballLeagueLaba",
                port = "5432",
                userID = "postgres",
                password = GetPassword();

                NpgsqlConnection connection = new NpgsqlConnection($@"Server={server};Port={port};User Id={userID};Password={password};Database={database}");
                connection.Open();

                return connection;
            }
            catch
            {
                MessageBox.Show("Помилка SQL-запиту");
                return new NpgsqlConnection();
            }
        }

        static private string Cut(string text)
        {
            return text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
        }

        static private string GetPassword()
        {
            string path = @"C:\Users\Admin\Desktop\Дз КПИ\ОП\2 семестр\Лабораторные\Лаба3\Лаба3\password.txt";
            return File.ReadAllText(path);
        }
    }
}
