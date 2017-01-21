using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using TeseraGamesLoader.Models;

namespace TeseraGamesLoader.Repositories
{
    internal class BaseUserRepository :BaseRepository
    {
        protected List<User> _users = new List<User>();

        protected void GetUserDataFromTesera(User user, ref HtmlParser parser)
        {
            Console.WriteLine($"Загружаем данные пользователя {user}");
            var connectionString = $"http://tesera.ru/user/{user.UserID}/games/owns/all/";
            var userOwnData = GetData(connectionString);

            var ownGameIds = GetGameIdsFromUserData(parser, userOwnData);
            user.OwnGameId = ownGameIds.OrderBy(x => x).ToList();

            var connectionString2 = $"http://tesera.ru/user/{user.UserID}/games/wants/all";
            var userWantsData = GetData(connectionString2);

            var wantsGameIds = GetGameIdsFromUserData(parser, userWantsData);
            user.WantsPlayIDs = wantsGameIds.OrderBy(x => x).ToList();

            user.UpdateDate = DateTime.Today;
            Console.WriteLine($"Данные пользователя {user} загружены");
        }

        protected IEnumerable<string> GetGameIdsFromUserData(HtmlParser parser, string userData)
        {
            var document = parser.Parse(userData);
            var linkedElements = document.All.Where(x => x.LocalName == "div" && x.ClassList.Contains("gameslinked")).ToList();
            var thumb = linkedElements.Select(x => x.Children.Single(t => t.ClassList.Contains("thumb"))).ToList();
            var innerLink = thumb.Select(x => (IHtmlAnchorElement)x.Children.Single(t => t.LocalName == "a")).ToList();
            var gameLink = innerLink.Select(x => x.PathName);
            var gameIds = gameLink.Select(x => x.Split('/')[2]);

            return gameIds;
        }
    }
}
