using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using static System.Linq.Enumerable;

namespace Лаба3
{
    public partial class MainWindow : Window
    {
        int teamsAmount;
        Random rnd = new Random();

        List<Team> teamsList = new List<Team>();
        List<ResultsTable> resultsList = new List<ResultsTable>();
        List<Stadium> stadiumList = new List<Stadium>();
        List<Player> playerList = new List<Player>();
        List<Budget> budgetList = new List<Budget>();

        public MainWindow()
        {
            InitializeComponent();

            generateButton.Click += (s, e) =>
            {
                PostgreExecuter.ClearTables();

                teamsAmount = 20;

                for (int i = 0; i < teamsAmount; i++)
                {
                    //создание команды
                    Team team = GenerateTeam(i + 1);
                    PostgreExecuter.Insert(team);

                    //создание стадиона
                    Stadium stadium = GenerateStadium();
                    PostgreExecuter.Insert(stadium, team);

                    //создание игроков
                    Player[] players = GeneratePlayers();
                    foreach (Player player in players)
                    {
                        PostgreExecuter.Insert(player, team);
                    }

                    //создание таблицы с результатми
                    ResultsTable resultsTable = GenerateResultsTable(team);
                    PostgreExecuter.Insert(resultsTable, team.TeamName);

                    //создание бюджета
                    Budget budget = GenerateBudget(i);
                }

                GenerateGames();

                for (int i = 0; i < teamsAmount; i++)
                {
                    ResultsTable.FindPlace(resultsList, teamsList);
                    PostgreExecuter.Update(resultsList[i], teamsList[i].TeamName);
                }

                //сортировка таблиц

            };
        }

        private Team GenerateTeam(int previousPlace)
        {
            Team team = new Team();
            team.TeamName = GenerateTeamName();

            team.CoachName = GeneratePlayerName() + " " + GeneratePlayerSurname();
            bool busyCoachName = true;
            while (busyCoachName)
            {
                busyCoachName = false;
                foreach (Team _team in teamsList)
                {
                    if (team.CoachName == _team.CoachName)
                    {
                        busyCoachName = true;
                        team.CoachName = GeneratePlayerName() + " " + GeneratePlayerSurname();
                    }
                }
            }

            team.PreviousPlace = previousPlace;

            teamsList.Add(team);
            return team;
        }

        private string GenerateTeamName()
        {
            string[] names = File.ReadAllLines(@"C:\Users\Admin\Desktop\Дз КПИ\ОП\2 семестр\Курсовая\NationalFootbalLeague\teams.txt");

            string name = names[rnd.Next(names.Length)];
            bool flag2 = true;

            while (flag2)
            {
                flag2 = false;
                foreach (Team team in teamsList)
                {
                    if (team.TeamName == name)
                    {
                        name = names[rnd.Next(names.Length)];
                        flag2 = true;
                        break;
                    }
                }
            }

            return name;
        }

        private Stadium GenerateStadium()
        {
            Stadium stadium = new Stadium();

            string[] stadiumNames = File.ReadAllLines(@"C:\Users\Admin\Desktop\Дз КПИ\ОП\2 семестр\Курсовая\NationalFootbalLeague\stadiumNames.txt");
            stadium.StadiumName = stadiumNames[rnd.Next(stadiumNames.Length)];

            bool flag2 = true;
            while (flag2)
            {
                flag2 = false;
                foreach (Stadium _stadium in stadiumList)
                {
                    if (_stadium.StadiumName == stadium.StadiumName && stadium != _stadium)
                    {
                        stadium.StadiumName = stadiumNames[rnd.Next(stadiumNames.Length)];
                        flag2 = true;
                    }
                }
            }

            string[] cities = File.ReadAllLines(@"C:\Users\Admin\Desktop\Дз КПИ\ОП\2 семестр\Курсовая\NationalFootbalLeague\cities.txt");
            stadium.City = cities[rnd.Next(cities.Length)];

            int capacity = rnd.Next(22000, 54000);
            stadium.Capacity = capacity;

            stadiumList.Add(stadium);
            return stadium;
        }

        private Player[] GeneratePlayers()
        {
            Player[] players = new Player[20];
            int counter = 0;
            int leaderIndex = rnd.Next(0, 20);

            for (int i = 0; i < players.Length; i++)
            {
                Player player = new Player();

                player.FirstName = GeneratePlayerName();
                player.Surname = GeneratePlayerSurname();

                bool flag2 = true;
                while (flag2)
                {
                    flag2 = false;
                    foreach (Player _player in playerList)
                    {
                        if (player.FirstName == _player.FirstName && player.Surname == _player.Surname && player != _player)
                        {
                            player.FirstName = GeneratePlayerName();
                            player.Surname = GeneratePlayerSurname();
                            flag2 = true;
                        }
                    }
                }

                player.Age = rnd.Next(18, 36);

                string[] countries = File.ReadAllLines(@"C:\Users\Admin\Desktop\Дз КПИ\ОП\2 семестр\Курсовая\NationalFootbalLeague\countries.txt");
                player.Country = countries[rnd.Next(countries.Length)];
                string position = counter < 3 ? "Goalkeeper" : counter < 9 ? "Defender" : counter < 15 ? "Midfielders" : "Forward";
                player.Position = position;

                int month = rnd.Next(1, 13);
                int day = month != 2 ? rnd.Next(1, 31) : rnd.Next(1, 29);
                int year = rnd.Next(2023, 2030);

                string ContractDueDay = day < 10 ? "0" + day.ToString() : day.ToString();
                string ContractDueMonth = month < 10 ? "0" + month.ToString() : month.ToString();
                string ContractDueYear = year.ToString();
                player.ContractDueDate = ContractDueYear + "-" + ContractDueMonth + "-" + ContractDueDay;

                player.IsLeader = i == leaderIndex ? true : false;

                player.Salary = (int)Math.Round(rnd.NextDouble() * 120000 + 7000);
                player.Cost = (int)Math.Round(Math.Pow(rnd.NextDouble(), 2) * 2000000 + 50000);

                players[i] = player;
                playerList.Add(player);
                counter++;
            }

            return players;
        }


        private Budget GenerateBudget(int teamIndex)
        {
            Budget budget = new Budget();
            budget.Salary += GetSalary(teamIndex);
            budget.Advertising = rnd.Next(500000, 1200000);
            budget.Cash = rnd.Next(1500000, 4000000);
            budgetList.Add(budget);
            PostgreExecuter.Insert(budget, teamsList[teamIndex].TeamName);
            return budget;
        }

        private string GeneratePlayerName()
        {
            string[] names = File.ReadAllLines(@"C:\Users\Admin\Desktop\Дз КПИ\ОП\2 семестр\Курсовая\NationalFootbalLeague\names.txt");
            string name = names[rnd.Next(names.Length)];
            return name;
        }

        private string GeneratePlayerSurname()
        {
            string[] surnames = File.ReadAllLines(@"C:\Users\Admin\Desktop\Дз КПИ\ОП\2 семестр\Курсовая\NationalFootbalLeague\surnames.txt");
            string surname = surnames[rnd.Next(surnames.Length)];
            return surname;
        }

        private ResultsTable GenerateResultsTable(Team team)
        {
            ResultsTable resultsTable = new ResultsTable(20);
            resultsList.Add(resultsTable);
            return resultsTable;
        }

        private void GenerateGames()
        {
            //сезон длится с 1 сентября по 25 декабря (116 дней) и с 1 марта по 24 июня (116 дней)
            //каждая команда играет по 2 игры с каждым соперником у себя дома и в гостях
            //итого каждая команда играет 19*2=38 игр 
            //общее количество игр = 38*20/2=380
            //между играми одной команды должен быть перерыв в минимум 2 дня


            //выбор текущей даты и времени
            int currentYear = rnd.Next(2022, 2024);
            int currentMonth = currentYear == 2022 ? rnd.Next(9, 13) : rnd.Next(1, 7);
            int currentDay = currentMonth == 2 ? rnd.Next(1, 29) : rnd.Next(1, 31);
            int currentHour = rnd.Next(0, 24);
            int currentMinute = rnd.Next(0, 60);
            int currentSecond = rnd.Next(0, 60);

            DateTime currentTime = new DateTime(currentYear, currentMonth, currentDay, currentHour, currentMinute, currentSecond);

            //словарь отыгранных матчей
            List<int[]> gamesPlayed = new List<int[]>();

            //начальная дата
            DateTime date = new DateTime(2022, 9, rnd.Next(2, 5), 0, 0, 0);

            for (int i = 0; i < 38; i++)
            {
                //средняя разница до следующего матча
                int gap = rnd.Next(4, 8);

                if (i == 19)
                {
                    date = new DateTime(2023, 3, rnd.Next(1, 3), 0, 0, 0);
                }

                //добавление этой разницы
                date = date.AddDays(gap);

                //списки команд, у которых уже есть игра в эти дни 
                List<int> selectedTeams = new List<int>();

                for (int j = 0; j < 10; j++)
                {
                    string weather = "-";
                    string referee = "-";

                    //вариации даты и времени матча
                    DateTime matchDate = date.AddDays(rnd.Next(-1, 2));
                    matchDate = matchDate.AddHours(rnd.Next(13, 22));
                    matchDate = matchDate.AddMinutes(rnd.Next(0, 4) * 15);
                    TimeOnly matchEnded = new TimeOnly();

                    //выбор команд-соперниц и их добавление в список/словарь
                    (int homeTeam, int guestTeam) = SelectTeamsForGame(selectedTeams, gamesPlayed);
                    selectedTeams.Add(homeTeam);
                    selectedTeams.Add(guestTeam);
                    int[] currentPair = new int[] {homeTeam, guestTeam};
                    gamesPlayed.Add(currentPair);

                    //нулевые значения для будущей игры
                    int visitors = 0;
                    int ticketPrice = 0;
                    string gameResult = "-";

                    //создание экземляра Game с добавлением параметров
                    Game game = new Game();
                    game.HomeTeam = teamsList[homeTeam].TeamName;
                    game.GuestTeam = teamsList[guestTeam].TeamName;
                    game.Weather = weather;
                    game.Referee = referee;
                    game.Date = matchDate;
                    game.EndTime = matchEnded;
                    game.Visitors = visitors;
                    game.TicketPrice = ticketPrice;
                    game.Result = gameResult;

                    //добавление игры в базу данных
                    PostgreExecuter.Insert(game);

                    //проверка произошла ли уже игра
                    if (matchDate < currentTime)
                    {
                        //длительность матча
                        matchEnded = matchEnded.AddHours(matchDate.Hour);
                        matchEnded = matchEnded.AddMinutes(matchDate.Minute);
                        matchEnded = matchEnded.AddMinutes(105 + rnd.Next(4, 9));

                        //выбор погоды
                        string[] weatherArray = new string[] { "Clear", "Rain", "Snow", "Fog" };
                        weather = rnd.NextDouble() < 0.5 ? weatherArray[0] : date.Month > 10 || date.Month < 3 ?
                            weatherArray[2] : rnd.NextDouble() < 0.6 ? weatherArray[1] : weatherArray[3];

                        //генерация судьи
                        referee = GeneratePlayerName() + " " + GeneratePlayerSurname();

                        //генерация игроков 
                        int[] homeTeamPlayerIDs;
                        int[] guestTeamPlayerIDs;
                        (homeTeamPlayerIDs, guestTeamPlayerIDs) =
                            GeneratePlayersInGame(teamsList[homeTeam].TeamName, teamsList[guestTeam].TeamName);

                        //метод генерации голов/сейвов
                        int homeTeamScore, guestTeamScore;
                        (homeTeamScore, guestTeamScore) =
                            GenerateMoments(homeTeamPlayerIDs, guestTeamPlayerIDs);

                        //заполнение gameResult
                        gameResult = $"{homeTeamScore}:{guestTeamScore}";

                        //Обнолвнеие таблицы результатов
                        if (homeTeamScore > guestTeamScore)
                        {
                            resultsList[homeTeam].Wins++;
                            resultsList[guestTeam].Losses++;
                        }
                        else if (homeTeamScore < guestTeamScore)
                        {
                            resultsList[homeTeam].Losses++;
                            resultsList[guestTeam].Wins++;
                        }
                        else
                        {
                            resultsList[homeTeam].Draws++;
                            resultsList[guestTeam].Draws++;
                        }
                        resultsList[homeTeam].GoalsScored += homeTeamScore;
                        resultsList[homeTeam].GoalsConceded += guestTeamScore;
                        resultsList[guestTeam].GoalsScored += guestTeamScore;
                        resultsList[guestTeam].GoalsConceded += homeTeamScore;

                        resultsList[homeTeam].GamesPlayed++;
                        resultsList[guestTeam].GamesPlayed++;

                        resultsList[homeTeam].GetTeamScore();
                        resultsList[guestTeam].GetTeamScore();


                        //генерация количества зрителей
                        double weatherCoefficient = 1;
                        if (weather == "Rain" || weather == "Snow")
                        {
                            weatherCoefficient = 0.72;
                        }
                        else if (weather == "Fog")
                        {
                            weatherCoefficient = 0.95;
                        }

                        double placeCoefficient =
                            1 - (teamsList[homeTeam].PreviousPlace + teamsList[guestTeam].PreviousPlace) / (teamsAmount * 3.2);

                        visitors = (int)Math.Round(stadiumList[homeTeam].Capacity * weatherCoefficient * placeCoefficient);

                        //генерация цены билета
                        ticketPrice = (int)Math.Round(rnd.Next(50, 75) * placeCoefficient);

                        //добавление прибыли от продажи билетов в бюджет команды
                        budgetList[homeTeam].TicketsSold += visitors;
                        budgetList[homeTeam].TicketsIncome += visitors * ticketPrice;

                        //обновление данных об игре
                        game.HomeTeam = teamsList[homeTeam].TeamName;
                        game.GuestTeam = teamsList[guestTeam].TeamName;
                        game.Weather = weather;
                        game.Referee = referee;
                        game.Date = matchDate;
                        game.EndTime = matchEnded;
                        game.Visitors = visitors;
                        game.TicketPrice = ticketPrice;
                        game.Result = gameResult;

                        PostgreExecuter.Update(game);
                    }
                }
            }

            //рассчет ежемесячных расходов/доходов
            int monthsPased = currentYear == 2023 ? currentMonth + 12 - 9 : currentMonth - 9;
            for (int i = 0; i < budgetList.Count; i++)
            {
                for (int j = 0; j < monthsPased; j++)
                {
                    budgetList[i].Advertising += rnd.Next(200000, 1000000);
                    budgetList[i].Salary += GetSalary(i);
                    budgetList[i].Cash += budgetList[i].CalculateProfit();
                }
                PostgreExecuter.Update(budgetList[i], teamsList[i].TeamName);
            }
        }

        private (int[], int[]) GeneratePlayersInGame(string homeTeam, string guestTeam)
        {
            int[] homeTeamPlayerIDs = new int[11];
            int[] guestTeamPlayerIDs = new int[11];

            int playerID;
            for (int i = 0; i < 11; i++)
            {
                string position;
                if (i == 0)
                {
                    position = "Goalkeeper";
                }
                else if (i < 5)
                {
                    position = "Defender";
                }
                else if (i < 9)
                {
                    position = "Midfielders";
                }
                else
                {
                    position = "Forward";
                }

                playerID = PostgreExecuter.GetPlayerID(position, homeTeam);
                bool busyHomeID = true;
                while (busyHomeID)
                {
                    busyHomeID = false;
                    foreach (int id in homeTeamPlayerIDs)
                    {
                        if (playerID == id)
                        {
                            playerID = PostgreExecuter.GetPlayerID(position, homeTeam);
                            busyHomeID = true;
                        }
                    }
                }
                homeTeamPlayerIDs[i] = playerID;

                playerID = PostgreExecuter.GetPlayerID(position, guestTeam);
                bool busyGuestID = true;
                while (busyGuestID)
                {
                    busyGuestID = false;
                    foreach (int id in guestTeamPlayerIDs)
                    {
                        if (playerID == id)
                        {
                            playerID = PostgreExecuter.GetPlayerID(position, guestTeam);
                            busyGuestID = true;
                        }
                    }
                }
                guestTeamPlayerIDs[i] = playerID;
            }

            foreach (int id in homeTeamPlayerIDs)
            {
                PostgreExecuter.UpdateGamesPlayed(id);
                PostgreExecuter.Insert(id, homeTeam, guestTeam);
            }
            foreach (int id in guestTeamPlayerIDs)
            {
                PostgreExecuter.UpdateGamesPlayed(id);
                PostgreExecuter.Insert(id, homeTeam, guestTeam);
            }

            return (homeTeamPlayerIDs, guestTeamPlayerIDs);
        }


        private (int, int) GenerateMoments(int[] homeTeamPlayerIDs, int[] guestTeamPlayerIDs)
        {
            int momentsAmount = rnd.Next(4, 15);
            int homeTeamScore = 0, guestTeamScore = 0;

            for (int i = 0; i < momentsAmount; i++)
            {
                double eventChecker = rnd.NextDouble();
                int idGoal, idAssist, idSave;

                //момент ведет домашняя команда
                if (rnd.NextDouble() < 0.55)
                {
                    //гол
                    if (eventChecker < 0.3)
                    {
                        homeTeamScore++;
                        double heroChecker = rnd.NextDouble();
                        if (heroChecker < 0.65)
                        {
                            idGoal = homeTeamPlayerIDs[9 + rnd.Next(0, 2)];
                        }
                        else if (heroChecker < 0.92)
                        {
                            idGoal = homeTeamPlayerIDs[5 + rnd.Next(0, 4)];
                        }
                        else
                        {
                            idGoal = homeTeamPlayerIDs[1 + rnd.Next(0, 4)];
                        }

                        idAssist = idGoal;
                        double assistChecker = rnd.NextDouble();
                        while (idAssist == idGoal)
                        {
                            if (assistChecker < 0.30)
                            {
                                idAssist = homeTeamPlayerIDs[9 + rnd.Next(0, 2)];
                            }
                            else if (assistChecker < 0.75)
                            {
                                idAssist = homeTeamPlayerIDs[5 + rnd.Next(0, 4)];
                            }
                            else
                            {
                                idAssist = homeTeamPlayerIDs[1 + rnd.Next(0, 4)];
                            }
                        }

                        PostgreExecuter.UpdateGoal(idGoal);
                        PostgreExecuter.UpdateAssist(idAssist);
                    }

                    //сейв
                    else
                    {
                        double saveChecker = rnd.NextDouble();

                        if (saveChecker < 0.60)
                        {
                            idSave = homeTeamPlayerIDs[0];
                        }
                        else if (saveChecker < 0.70)
                        {
                            idSave = homeTeamPlayerIDs[5 + rnd.Next(0, 4)];
                        }
                        else
                        {
                            idSave = homeTeamPlayerIDs[1 + rnd.Next(0, 4)];
                        }

                        PostgreExecuter.UpdateSave(idSave);
                    }
                }

                //момент ведет гостевая команда
                else
                {
                    //гол
                    if (eventChecker < 0.3)
                    {
                        guestTeamScore++;
                        double heroChecker = rnd.NextDouble();
                        if (heroChecker < 0.65)
                        {
                            idGoal = guestTeamPlayerIDs[9 + rnd.Next(0, 2)];
                        }
                        else if (heroChecker < 0.92)
                        {
                            idGoal = guestTeamPlayerIDs[5 + rnd.Next(0, 4)];
                        }
                        else
                        {
                            idGoal = guestTeamPlayerIDs[1 + rnd.Next(0, 4)];
                        }

                        idAssist = idGoal;
                        double assistChecker = rnd.NextDouble();
                        while (idAssist == idGoal)
                        {
                            if (assistChecker < 0.30)
                            {
                                idAssist = guestTeamPlayerIDs[9 + rnd.Next(0, 2)];
                            }
                            else if (assistChecker < 0.75)
                            {
                                idAssist = guestTeamPlayerIDs[5 + rnd.Next(0, 4)];
                            }
                            else
                            {
                                idAssist = guestTeamPlayerIDs[1 + rnd.Next(0, 4)];
                            }
                        }

                        PostgreExecuter.UpdateGoal(idGoal);
                        PostgreExecuter.UpdateAssist(idAssist);
                    }

                    //сейв
                    else
                    {
                        double saveChecker = rnd.NextDouble();

                        if (saveChecker < 0.60)
                        {
                            idSave = guestTeamPlayerIDs[0];
                        }
                        else if (saveChecker < 0.70)
                        {
                            idSave = guestTeamPlayerIDs[5 + rnd.Next(0, 4)];
                        }
                        else
                        {
                            idSave = guestTeamPlayerIDs[1 + rnd.Next(0, 4)];
                        }

                        PostgreExecuter.UpdateSave(idSave);
                    }
                }

            }

            return (homeTeamScore, guestTeamScore);
        }

        private (int, int) SelectTeamsForGame(List<int> selectedTeams, List<int[]> gamesPlayed)
        {
            int homeTeam = rnd.Next(0, 20);
            int guestTeam = rnd.Next(0, 20);
            while (homeTeam == guestTeam)
            {
                guestTeam = rnd.Next(0, 20);
            }


            bool busyTeams = true;
            while (busyTeams)
            {
                busyTeams = false;
                bool busyHome = true, busyGuest = true;

                while (busyHome)
                {
                    busyHome = false;
                    int selectedCounter = 0;
                    foreach (int selectedTeam in selectedTeams)
                    {
                        if (selectedTeam == homeTeam)
                        {
                            selectedCounter++;
                            if (selectedCounter == 2)
                            {
                                homeTeam = rnd.Next(0, 20);
                                busyHome = true;
                            }         
                        }
                    }
                }
                while (busyGuest)
                {
                    busyGuest = false;
                    int selectedCounter = 0;
                    foreach (int selectedTeam in selectedTeams)
                    {
                        if (selectedTeam == guestTeam)
                        {
                            selectedCounter++;
                            if (selectedCounter == 2)
                            {
                                guestTeam = rnd.Next(0, 20);
                                while (homeTeam == guestTeam)
                                {
                                    guestTeam = rnd.Next(0, 20);
                                }
                                busyGuest = true;
                            }     
                        }
                    }
                }

                for (int i = 0; i < gamesPlayed.Count; i++)
                {
                    try
                    {
                        int[] currentPair = new int[] { homeTeam, guestTeam };
                        if (gamesPlayed[i].SequenceEqual(currentPair))
                        {
                            busyTeams = true;

                            homeTeam = rnd.Next(0, 20);
                            guestTeam = rnd.Next(0, 20);
                            while (homeTeam == guestTeam)
                            {
                                guestTeam = rnd.Next(0, 20);
                            }
                        }
                    }
                    catch { }
                }
            }

            return (homeTeam, guestTeam);
        }

        private int GetSalary(int teamIndex)
        {
            int salary = 0;
            for (int i = 0; i < 20; i++)
            {
                salary += playerList[teamIndex * 20 + i].Salary;
            }
            return salary;
        }
    }
}