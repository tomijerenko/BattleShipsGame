using System;
using System.Collections.Generic;

namespace BattleShip.GameLogic
{
    public class BattleField
    {
        public string PlayerName { get; set; }
        public string[,] PlayerBattleArray { get; set; }
        public string SocketId { get; set; }
        public bool Ready { get; set; }
        public int NoOfHits { get; set; }
    }
}
