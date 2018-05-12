using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void UpdateLongestActiveGame(DateTime gameStartedDateTime, StatisticsModel statisticsModel)
        {
            if (gameStartedDateTime != DateTime.MinValue)
            {
                TimeSpan timePlayed = DateTime.Now - gameStartedDateTime;
                statisticsModel.TotalTimePlayed += timePlayed;
                if (statisticsModel.LongestActiveGame < timePlayed)
                    statisticsModel.LongestActiveGame = timePlayed;
            }
        }

        public static void AddSocketToEmptyBattle(string socketId, List<Battle> battlesList)
        {
            if (battlesList.Count == 0 || battlesList.FirstOrDefault(x => x.BattleFields.Count == 1) == null)
                battlesList.Add(new Battle(new BattleField() { SocketId = socketId }));
            else
                battlesList.FirstOrDefault(x => x.BattleFields.Count == 1).AddSecondBattleField(new BattleField() { SocketId = socketId });
        }

        public static void IncrementTotalGamesPlayed(StatisticsModel statisticsModel)
        {
            statisticsModel.TotalGamesPlayed++;
        }

        public static void MissileShootStatsUpdate(bool isHit, StatisticsModel statisticsModel)
        {
            statisticsModel.TotalMissileShoots++;
            if (isHit)
                statisticsModel.TotalMissileHits++;
            else
                statisticsModel.TotalMissileMisses++;
        }
    }
}
