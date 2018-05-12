using System;

namespace BattleShip.Data.Entities
{
    public class GameStatistics
    {
        public int id { get; set; }
        public TimeSpan TotalTimePlayed { get; set; }
        public int TotalGamesPlayed { get; set; }
        public TimeSpan LongestActiveGame { get; set; }
        public int TotalMissileShoots { get; set; }
        public int TotalMissileHits { get; set; }
        public int TotalMissileMisses { get; set; }
    }
}