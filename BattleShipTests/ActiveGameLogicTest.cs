using BattleShip.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BattleShipTests
{
    public class ActiveGameLogicTest
    {     

        [Fact]
        public void AddSocketToEmptyBattle_AddsNewBattle_battlesIncrement()
        {
            int before = ActiveGameLogic.BattlesList.Count;
            ActiveGameLogic.AddSocketToEmptyBattle("trooltest", ActiveGameLogic.BattlesList);
            int after = ActiveGameLogic.BattlesList.Count;

            Assert.NotEqual(before, after);
        }

        [Fact]
        public void AddSocketToBattle_AllBattlefieldsNotPaired_AddsNewBattleField_battlefieldsIncrement()
        {
            ActiveGameLogic.BattlesList = new List<Battle>();
            ActiveGameLogic.AddSocketToEmptyBattle("trooltest", ActiveGameLogic.BattlesList);
            int before = ActiveGameLogic.BattlesList.First().BattleFields.Count;
            ActiveGameLogic.AddSocketToEmptyBattle("dfdsfsffsdfs", ActiveGameLogic.BattlesList);
            int after = ActiveGameLogic.BattlesList.First().BattleFields.Count;

            Assert.NotEqual(before, after);
        }

        [Fact]
        public void AddSocketToBattle_AllBattlefieldsNotPaired_AddsNewBattle_battlesIncrement()
        {
            int before = ActiveGameLogic.BattlesList.Count;
            ActiveGameLogic.AddSocketToEmptyBattle("trooltest", ActiveGameLogic.BattlesList);
            ActiveGameLogic.AddSocketToEmptyBattle("troohsdhsgjgfkjltest", ActiveGameLogic.BattlesList);
            ActiveGameLogic.AddSocketToEmptyBattle("4534tdfsg6y86y8jhg", ActiveGameLogic.BattlesList);
            int after = ActiveGameLogic.BattlesList.Count;

            Assert.NotEqual(before, after);
        }        

        [Fact]
        public void RemoveDisconnectedBattle_Exists_true()
        {
            Battle battle = new Battle(new BattleField());
            ActiveGameLogic.BattlesList.Add(battle);
            bool result = ActiveGameLogic.RemoveDisconnectedBattle(battle, ActiveGameLogic.BattlesList);

            Assert.True(result);
        }

        [Fact]
        public void RemoveDisconnectedBattle_NotExists_false()
        {
            Battle battle = new Battle(new BattleField());
            ActiveGameLogic.BattlesList.Add(battle);
            bool result = ActiveGameLogic.RemoveDisconnectedBattle(new Battle(new BattleField()), ActiveGameLogic.BattlesList);

            Assert.False(result);
        }
        [Fact]
        public void UpdateLongestActiveGame_TimeIsMinValue_NoUpdate()
        {
            TimeSpan before = GameHandler.Statistics.LongestActiveGame;
            ActiveGameLogic.UpdateLongestActiveGame(DateTime.MinValue, GameHandler.Statistics);
            TimeSpan after = GameHandler.Statistics.LongestActiveGame;

            Assert.Equal(before, after);
        }

        [Fact]
        public void UpdateLongestActiveGame_TimeIsSmallerValue_NoUpdate()
        {
            ActiveGameLogic.UpdateLongestActiveGame(DateTime.Now.AddDays(-2), GameHandler.Statistics);
            TimeSpan before = GameHandler.Statistics.LongestActiveGame;
            ActiveGameLogic.UpdateLongestActiveGame(DateTime.Now.AddDays(-1), GameHandler.Statistics);
            TimeSpan after = GameHandler.Statistics.LongestActiveGame;

            Assert.Equal(before, after);
        }

        [Fact]
        public void UpdateLongestActiveGame_TimeIsHigherValue_Update()
        {
            ActiveGameLogic.UpdateLongestActiveGame(DateTime.Now.AddDays(-1), GameHandler.Statistics);
            TimeSpan before = GameHandler.Statistics.LongestActiveGame;
            ActiveGameLogic.UpdateLongestActiveGame(DateTime.Now.AddDays(-2), GameHandler.Statistics);
            TimeSpan after = GameHandler.Statistics.LongestActiveGame;

            Assert.NotEqual(before, after);
        }

        [Fact]
        public void MissileHitsStatsUpdate_isHit_increments()
        {
            int beforeMissileHitsCount = GameHandler.Statistics.TotalMissileHits;

            ActiveGameLogic.MissileShootStatsUpdate(true, GameHandler.Statistics);
            int afterMissileHitsCount = GameHandler.Statistics.TotalMissileHits;

            Assert.NotEqual(beforeMissileHitsCount, afterMissileHitsCount);
        }

        [Fact]
        public void MissileHitsStatsUpdate_isNotHit_same()
        {
            int beforeMissileHitsCount = GameHandler.Statistics.TotalMissileHits;
            ActiveGameLogic.MissileShootStatsUpdate(false, GameHandler.Statistics);
            int afterMissileHitsCount = GameHandler.Statistics.TotalMissileHits;

            Assert.Equal(beforeMissileHitsCount, afterMissileHitsCount);
        }

        [Fact]
        public void MissileMissesStatsUpdate_isHit_same()
        {
            int beforeMissileMisses = GameHandler.Statistics.TotalMissileMisses;
            ActiveGameLogic.MissileShootStatsUpdate(true, GameHandler.Statistics);
            int afterMissileMisses = GameHandler.Statistics.TotalMissileMisses;

            Assert.Equal(beforeMissileMisses, afterMissileMisses);
        }

        [Fact]
        public void MissileMissesStatsUpdate_isNotHit_increments()
        {
            int beforeMissileMisses = GameHandler.Statistics.TotalMissileMisses;
            ActiveGameLogic.MissileShootStatsUpdate(false, GameHandler.Statistics);
            int afterMissileMisses = GameHandler.Statistics.TotalMissileMisses;

            Assert.NotEqual(beforeMissileMisses, afterMissileMisses);
        }

        [Fact]
        public void TotalMissileCountStatsUpdate_isNotHit_increments()
        {
            int before = GameHandler.Statistics.TotalMissileShoots;
            ActiveGameLogic.MissileShootStatsUpdate(false, GameHandler.Statistics);
            int after = GameHandler.Statistics.TotalMissileShoots;

            Assert.NotEqual(before, after);
        }

        [Fact]
        public void TotalMissileCountStatsUpdate_isHit_increments()
        {
            int before = GameHandler.Statistics.TotalMissileShoots;
            ActiveGameLogic.MissileShootStatsUpdate(true, GameHandler.Statistics);
            int after = GameHandler.Statistics.TotalMissileShoots;

            Assert.NotEqual(before, after);
        }

        [Fact]
        public void IncrementTotalGamesPlayer_Increments()
        {
            int before = GameHandler.Statistics.TotalGamesPlayed;
            ActiveGameLogic.IncrementTotalGamesPlayed(GameHandler.Statistics);
            int after = GameHandler.Statistics.TotalGamesPlayed;

            Assert.NotEqual(before, after);
        }
    }
}