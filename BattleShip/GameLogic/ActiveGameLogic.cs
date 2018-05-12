using BattleShip.Data;
using BattleShip.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleShip.GameLogic
{
    public static class ActiveGameLogic
    {
        public static List<Battle> BattlesList { get; set; }

        static ActiveGameLogic()
        {
            BattlesList = new List<Battle>();
        }

        public static bool RemoveDisconnectedBattle(Battle battle, List<Battle> battlesList)
        {
            return battlesList.Remove(battle);
        }

        public static void UpdateLongestActiveGame(DateTime gameStartedDateTime, DataBaseContext context)
        {
            if (gameStartedDateTime != DateTime.MinValue)
            {
                GameStatistics stats = context.Statistics.ToList().First();
                TimeSpan timePlayed = DateTime.Now - gameStartedDateTime;
                stats.TotalTimePlayed += timePlayed;
                if (stats.LongestActiveGame < timePlayed)
                    stats.LongestActiveGame = timePlayed;
                context.SaveChangesAsync();
            }
        }

        public static void AddSocketToEmptyBattle(string socketId, List<Battle> battlesList)
        {
            if (battlesList.Count == 0 || battlesList.FirstOrDefault(x => x.BattleFields.Count == 1) == null)
                battlesList.Add(new Battle(new BattleField() { SocketId = socketId }));
            else
                battlesList.FirstOrDefault(x => x.BattleFields.Count == 1).AddSecondBattleField(new BattleField() { SocketId = socketId });
        }

        public static void IncrementTotalGamesPlayed(DataBaseContext context)
        {
            context.Statistics.First().TotalGamesPlayed++;
            context.SaveChangesAsync();
        }

        public static void MissileShootStatsUpdate(bool isHit, DataBaseContext context)
        {
            GameStatistics stats = context.Statistics.First();
            stats.TotalMissileShoots++;
            if (isHit)
                stats.TotalMissileHits++;
            else
                stats.TotalMissileMisses++;
            context.SaveChanges();
        }
    }
}
