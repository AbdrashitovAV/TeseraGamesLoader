using System;
using System.Collections.Generic;

namespace TeseraGamesLoader.Models

{
    public class Game
    {
        public string GameId { get; set; }
        public string GameName { get; set; }

        public DateTime UpdateDate { get; set; }
        public List<string> OwnedBy{ get; set; }
    }
}
