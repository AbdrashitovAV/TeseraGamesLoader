using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using TeseraGamesLoader.Models;

namespace TeseraGamesLoader.Repositories
{
    internal class GameRepository :BaseRepository
    {
        private readonly string _gamesDataFileName = "games.txt";
        private List<Game> _allGames;


        public GameRepository()
        {
            _allGames = LoadGamesDataFromFile();
            foreach (var game in _allGames)
            {
                game.OwnedBy = new List<string>();
            }
            Console.WriteLine($"Данные игр о владельцах очищенны");
            SaveGamesDataInFile();
        }

        public Game GetById(string gameId)
        {
            var game = _allGames.SingleOrDefault(x => x.GameId == gameId);

            if (game == null)
            {
                game = new Game()
                {
                    GameId = gameId,
                    GameName = GetGameNameFromId(gameId, _parser),
                    OwnedBy = new List<string>(),
                    UpdateDate = DateTime.Today
                };

                SaveGamesDataInFile();
            }
            else
            {
                if (game.UpdateDate <= DateTime.Today.AddDays(-4))
                {
                    game.GameName = GetGameNameFromId(gameId, _parser);
                    game.UpdateDate = DateTime.Today;
                }

                SaveGamesDataInFile();
            }

            return game;
        }

        private List<Game> LoadGamesDataFromFile()
        {
            if (File.Exists(_gamesDataFileName))
            {
                var filedata = File.ReadAllText(_gamesDataFileName, Encoding.UTF8);

                return JsonConvert.DeserializeObject<List<Game>>(filedata);
            }

            return new List<Game>();
        }

        private string GetGameNameFromId(string gameId, HtmlParser parser)
        {
            Console.WriteLine($"Загружам данные для игры {gameId}");
            var connectionString = $"http://tesera.ru/game/{gameId}/";

            try
            {
                var gameData = GetData(connectionString);
                var document = parser.Parse(gameData);
                var element =
                    document.All.Single(x => x.LocalName == "h1" && x.Attributes.Any(t => t.Name == "id" && t.Value == "game_title"));
                var spanElement = element.FirstElementChild as IHtmlSpanElement;
                var gameName = spanElement.TextContent;

                Console.WriteLine($"Данные для игры {gameId} загружены");
                return gameName;

            }
            catch (Exception)
            {

                Console.WriteLine($"Возникла проблема при загрузке данных для игры {gameId}.");
                return gameId;
            }
        }

        private void SaveGamesDataInFile()
        {
            var fileText = JsonConvert.SerializeObject(_allGames.ToList().OrderBy(x => x.GameName), Formatting.Indented);

            File.WriteAllText(_gamesDataFileName, fileText, Encoding.UTF8);
        }
    }
}