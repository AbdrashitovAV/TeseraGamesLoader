using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeseraGamesLoader.Repositories;

namespace TeseraGamesLoader
{
    class Program
    {
        //private static string _resultFileName = "GamesOnCamp.csv";
        private static string _wantedResultFileName = "WantedResult.csv";

        static void Main(string[] args)
        {
            var userRepository = new UserRepository();
            var gameRepository = new GameRepository();

            var users = userRepository.GetAll();

            var gamesWithOwners = users.SelectMany(
                    user => user.OwnGameId.Select(gameId => new { Owner = user.ToString(), GameId = gameId }))
                .GroupBy(t => t.GameId)
                .ToDictionary(t => t.Key,
                    q => q.Select(x => x.Owner)
                        .Aggregate((w, s) => w + ", " + s)
                 );

            var necessaryGameData =
                users.SelectMany(user => user.WantsPlayIDs.Where(gameId => gamesWithOwners.ContainsKey(gameId)))
                    .Distinct()
                    .AsParallel().WithDegreeOfParallelism(16)
                    .Select(gameId=>gameRepository.GetById(gameId))
                    .ToList();

            var resolvedWishlist = new Dictionary<string, Dictionary<string, string>>();
            foreach (var user in users)
            {
                var wantedGamesWithOwners = new Dictionary<string, string>();
                foreach (var wantedGameID in user.WantsPlayIDs)
                {
                    if (!gamesWithOwners.ContainsKey(wantedGameID))
                        continue;
                    wantedGamesWithOwners.Add(gameRepository.GetById(wantedGameID).GameName, gamesWithOwners[wantedGameID]);
                }
                resolvedWishlist.Add(user.ToString(), wantedGamesWithOwners);
            }

            ////Console.WriteLine("_________________");
            ////Console.WriteLine("_________________");

            ////var q = games.Where(x => x.OwnedBy.Count > 4).OrderByDescending(x => x.OwnedBy.Count);
            ////foreach (var game in q)
            ////{
            ////    Console.WriteLine($"{game.GameName}   владельцев {game.OwnedBy.Count}");
            ////}

            ////TODO:добавить ссылку на страницу игры
            //var result = new List<string>() { "Имя игры;teseraId;Владельцы" };

            //foreach (var game in games)
            //{
            //    var ownedByString = String.Empty;
            //    foreach (var ownerId in game.OwnedBy)
            //    {
            //        if (!String.IsNullOrEmpty(ownedByString))
            //            ownedByString += ", ";
            //        ownedByString += users.Single(user => user.UserID == ownerId).ToString();
            //    }
            //    result.Add($"{game.GameName};{game.GameId};{ownedByString}");
            //}
            //File.WriteAllLines(_resultFileName, result, Encoding.UTF8);


            using (var sw = new StreamWriter(_wantedResultFileName))
            {
                foreach (var userList in resolvedWishlist)
                {
                    sw.Write($"Список для {userList.Key}{Environment.NewLine}");
                    foreach (var game in userList.Value)
                    {
                        sw.Write($"\t\t\"{game.Key}\" есть у участников:{game.Value}{Environment.NewLine}");
                    }
                    sw.Write(Environment.NewLine);
                }

            }
        }

    }
}
