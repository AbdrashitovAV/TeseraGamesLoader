using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TeseraGamesLoader.Models;

namespace TeseraGamesLoader.Repositories
{
    internal class UserRepository: BaseUserRepository
    {
        public UserRepository()
        {
            var usersToProcess = LoadUsersToProcess("user.csv");

            usersToProcess
                .AsParallel().WithDegreeOfParallelism(8)
                .ForAll(GetUserDataFromTesera);

            _users.AddRange(usersToProcess);
        }

        public List<User> GetAll()
        {
            return _users.ToList();
        }

        private static List<User> LoadUsersToProcess(string userFilename)
        {
            var users = new List<User>();

            if (!File.Exists(userFilename))
                return users;

            var filedata = File.ReadAllLines(userFilename, Encoding.UTF8)
                               .Select(x => x.Trim())
                               .Where(x => !x.StartsWith("#") && !String.IsNullOrEmpty(x))
                               .ToList();

            foreach (var userString in filedata)
            {
                try
                {
                    var splittedString = userString.Split(',');

                    var newUser = new User
                    {
                        UserID = splittedString[0].Trim(),
                        FirstName = splittedString[1].Trim(),
                        LastName = splittedString[2].Trim()
                    };

                    users.Add(newUser);
                }
                catch (Exception)
                {

                    Console.WriteLine($"Строка {Environment.NewLine}{userString}{Environment.NewLine} вызвала ошибку обработки и была проигнорирована.");
                }

            }

            return users;
        }
    }
}
