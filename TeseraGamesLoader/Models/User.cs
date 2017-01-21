using System;
using System.Collections.Generic;

namespace TeseraGamesLoader.Models
{
    public class User
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime UpdateDate { get; set; }
        public List<string> OwnGameId { get; set; }
        public List<string> WantsPlayIDs { get; set; }

        public override string ToString()
        {
            return $"{LastName} {FirstName} ( {UserID} )";
        }
    }
}
