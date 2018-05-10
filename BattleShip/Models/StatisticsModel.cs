using System;

namespace BattleShip.Models
{
    public class StatisticsModel
    {
        public TimeSpan TotalTimePlayed { get; set; }
        public int TotalGamesPlayed { get; set; }
        public TimeSpan LongestActiveGame { get; set; }
        public int TotalMissileShoots { get; set; }
        public int TotalMissileHits { get; set; }
        public int TotalMissileMisses { get; set; }
        public int CurrentActiveGames { get; set; }
    }
}
