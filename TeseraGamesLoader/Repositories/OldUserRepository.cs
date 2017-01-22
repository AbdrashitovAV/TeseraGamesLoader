using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TeseraGamesLoader.Models;

namespace TeseraGamesLoader.Repositories
{
    internal class OldUserRepository : BaseUserRepository
    {
        private readonly string _usersDataFileName = "userData.txt";
        private readonly IEnumerable<string> _userIDsToProcess;

        public OldUserRepository()
        {
            _users = LoadUserDataFromFile();

            _userIDsToProcess = LoadUserIDsToProcess();

            if (!_userIDsToProcess.Any())
                return;

            UpdateUserDataForProcessedUsers();
            SaveUserDataToFile();
        }


        public List<User> GetAll()
        {
            return _users.Where(x => _userIDsToProcess.Contains(x.UserID)).ToList();
        }

        private void UpdateUserDataForProcessedUsers()
        {
            foreach (var userId in _userIDsToProcess)
            {
                var user = _users.SingleOrDefault(x => x.UserID == userId);

                if (user == null)
                {
                    user = new User()
                    {
                        UserID = userId,
                        UpdateDate = DateTime.Today
                    };
                }
                else
                {
                    if (user.UpdateDate == DateTime.Today)
                    {
                        Console.WriteLine($"Данные для пользователя {user} свежие и не требуют обновления");
                        continue;
                    }
                }

                GetUserDataFromTesera(user);
            }
        }

        private IEnumerable<string> LoadUserIDsToProcess()
        {
            var filePath = "userid.txt";

            if (!File.Exists(filePath))
            {
                File.Create(filePath);

                Console.WriteLine($"Файл {filePath} не найден и был создан.");
                Console.WriteLine($"Добавьте в него пользователей по одному на строчку и запустите программу заново");

                return new List<string>();
            }

            var userIds = File.ReadAllLines(filePath).ToList();

            return userIds;
        }

        private void SaveUserDataToFile()
        {
            var fileText = JsonConvert.SerializeObject(_users, Formatting.Indented);

            File.WriteAllText(_usersDataFileName, fileText, Encoding.UTF8);
        }

        private List<User> LoadUserDataFromFile()
        {
            if (File.Exists(_usersDataFileName))
            {
                var filedata = File.ReadAllText(_usersDataFileName, Encoding.UTF8);

                return JsonConvert.DeserializeObject<List<User>>(filedata);
            }

            return new List<User>();
        }

        
    }
}