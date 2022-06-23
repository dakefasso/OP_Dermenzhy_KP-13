using Npgsql;

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Лабораторна4
{
    public partial class MainWindow : Window
    {
        BrushConverter bc = new BrushConverter();
        public MainWindow()
        {
            InitializeComponent();

            closeButton.Click += (s, e) => Close();
            executeButton.Click += (s, e) => Execute();


            teamGamesItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.TeamGame();
            playersInGameItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.PlayersInGame();
            ticketPriceGameItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.TicketPriceGame();
            playerScoredMostItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.PlayerScoredMost();
            teamsScoredConcededItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.TeamsScoredConceded();
            youngestPlayerItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.YoungestPlayer();
            countryPlayersItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.CountryPlayers();
            championsTeamsItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.ChampionsTeams();
            mostProfitTeamItem.Selected += (s, e) => queryTextBox.Text = CommonQueries.MostProfitTeam();
        }

        private void Execute()
        {
            resultDataGrid.Children.Clear();

            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand command = connection.CreateCommand();

            string commandText = queryTextBox.Text;
            command.CommandText = commandText;

            NpgsqlDataReader reader = command.ExecuteReader();

            bool isFirst = true;
            int rowCounter = 1;

            while (reader.Read())
            {
                List<(string, string)> row = new List<(string, string)>();
                row = Read(row, reader, 0);
                int colCounter = 0;

                foreach ((string, string) col in row)
                {
                    if (isFirst)
                    {
                        Border border = new Border();
                        border.Height = 30;
                        border.Width = 200;
                        border.HorizontalAlignment = HorizontalAlignment.Left;
                        border.VerticalAlignment = VerticalAlignment.Top;
                        border.BorderBrush = (Brush)bc.ConvertFrom("#FF673AB7");
                        border.BorderThickness = new Thickness(2);
                        border.Margin = new Thickness(border.Width * colCounter, 0, 0, 0);
                        resultDataGrid.Children.Add(border);

                        Label label = new Label();
                        label.Content = col.Item2;
                        label.HorizontalAlignment = HorizontalAlignment.Center;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.FontSize = 15;
                        label.FontFamily = new FontFamily("Century");
                        border.Child = label;
                    }

                    Border _border = new Border();
                    _border.Height = 30;
                    _border.Width = 200;
                    _border.HorizontalAlignment = HorizontalAlignment.Left;
                    _border.VerticalAlignment = VerticalAlignment.Top;
                    _border.BorderBrush = (Brush)bc.ConvertFrom("#FF673AB7");
                    _border.BorderThickness = new Thickness(2);
                    _border.Margin = new Thickness(_border.Width * colCounter, _border.Height * rowCounter, 0, 0);
                    resultDataGrid.Children.Add(_border);

                    Label _label = new Label();
                    _label.Content = col.Item1;
                    _label.HorizontalAlignment = HorizontalAlignment.Center;
                    _label.VerticalAlignment = VerticalAlignment.Center;
                    _label.FontSize = 15;
                    _label.FontFamily = new FontFamily("Century");
                    _border.Child = _label;

                    colCounter++;
                }

                rowCounter++;
                isFirst = false;
            }

            reader.Close();
            connection.Close();
        }

        static private List<(string, string)> Read(List<(string, string)> row, NpgsqlDataReader reader, int column)
        {
            try
            {
                row.Add((Convert.ToString(reader.GetValue(column)), reader.GetName(column)));
                Read(row, reader, column + 1);
            }
            catch { }
            return row;
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