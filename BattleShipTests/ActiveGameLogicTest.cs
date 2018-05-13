using BattleShip.Data;
using BattleShip.Data.Entities;
using BattleShip.GameLogic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BattleShipTests
{
    public class ActiveGameLogicTest
    {
        DataBaseContext _dbContext;

        public ActiveGameLogicTest()
        {
            DbContextOptionsBuilder<DataBaseContext> optionsBuilder = new DbContextOptionsBuilder<DataBaseContext>();
            optionsBuilder.UseInMemoryDatabase("ActiveGameLogicTests");
            _dbContext = new DataBaseContext(optionsBuilder.Options);
            _dbContext.Statistics.Add(new GameStatistics());
            _dbContext.SaveChanges();
        }        

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
        //[Fact]
        //public void UpdateLongestActiveGame_TimeIsMinValue_NoUpdate()
        //{
        //    TimeSpan before = _dbContext.Statistics.First().LongestActiveGame;
        //    ActiveGameLogic.UpdateLongestActiveGame(DateTime.MinValue, _dbContext);
        //    TimeSpan after = _dbContext.Statistics.First().LongestActiveGame;

        //    Assert.Equal(before, after);
        //}

        //[Fact]
        //public void UpdateLongestActiveGame_TimeIsSmallerValue_NoUpdate()
        //{
        //    ActiveGameLogic.UpdateLongestActiveGame(DateTime.Now.AddDays(-2), _dbContext);
        //    TimeSpan before = _dbContext.Statistics.First().LongestActiveGame;
        //    ActiveGameLogic.UpdateLongestActiveGame(DateTime.Now.AddDays(-1), _dbContext);
        //    TimeSpan after = _dbContext.Statistics.First().LongestActiveGame;

        //    Assert.Equal(before, after);
        //}

        //[Fact]
        //public void UpdateLongestActiveGame_TimeIsHigherValue_Update()
        //{
        //    ActiveGameLogic.UpdateLongestActiveGame(DateTime.Now.AddDays(-1), _dbContext);
        //    TimeSpan before = _dbContext.Statistics.First().LongestActiveGame;
        //    ActiveGameLogic.UpdateLongestActiveGame(DateTime.Now.AddDays(-2), _dbContext);
        //    TimeSpan after = _dbContext.Statistics.First().LongestActiveGame;

        //    Assert.NotEqual(before, after);
        //}

        //[Fact]
        //public void MissileHitsStatsUpdate_isHit_increments()
        //{
        //    int beforeMissileHitsCount = _dbContext.Statistics.First().TotalMissileHits;

        //    ActiveGameLogic.MissileShootStatsUpdate(true, _dbContext);
        //    int afterMissileHitsCount = _dbContext.Statistics.First().TotalMissileHits;

        //    Assert.NotEqual(beforeMissileHitsCount, afterMissileHitsCount);
        //}

        //[Fact]
        //public void MissileHitsStatsUpdate_isNotHit_same()
        //{
        //    int beforeMissileHitsCount = _dbContext.Statistics.First().TotalMissileHits;
        //    ActiveGameLogic.MissileShootStatsUpdate(false, _dbContext);
        //    int afterMissileHitsCount = _dbContext.Statistics.First().TotalMissileHits;

        //    Assert.Equal(beforeMissileHitsCount, afterMissileHitsCount);
        //}

        //[Fact]
        //public void MissileMissesStatsUpdate_isHit_same()
        //{
        //    int beforeMissileMisses = _dbContext.Statistics.First().TotalMissileMisses;
        //    ActiveGameLogic.MissileShootStatsUpdate(true, _dbContext);
        //    int afterMissileMisses = _dbContext.Statistics.First().TotalMissileMisses;

        //    Assert.Equal(beforeMissileMisses, afterMissileMisses);
        //}

        //[Fact]
        //public void MissileMissesStatsUpdate_isNotHit_increments()
        //{
        //    int beforeMissileMisses = _dbContext.Statistics.First().TotalMissileMisses;
        //    ActiveGameLogic.MissileShootStatsUpdate(false, _dbContext);
        //    int afterMissileMisses = _dbContext.Statistics.First().TotalMissileMisses;

        //    Assert.NotEqual(beforeMissileMisses, afterMissileMisses);
        //}

        //[Fact]
        //public void TotalMissileCountStatsUpdate_isNotHit_increments()
        //{
        //    int before = _dbContext.Statistics.First().TotalMissileShoots;
        //    ActiveGameLogic.MissileShootStatsUpdate(false, _dbContext);
        //    int after = _dbContext.Statistics.First().TotalMissileShoots;

        //    Assert.NotEqual(before, after);
        //}

        //[Fact]
        //public void TotalMissileCountStatsUpdate_isHit_increments()
        //{
        //    int before = _dbContext.Statistics.First().TotalMissileShoots;
        //    ActiveGameLogic.MissileShootStatsUpdate(true, _dbContext);
        //    int after = _dbContext.Statistics.First().TotalMissileShoots;

        //    Assert.NotEqual(before, after);
        //}

        //[Fact]
        //public void IncrementTotalGamesPlayer_Increments()
        //{
        //    int before = _dbContext.Statistics.First().TotalGamesPlayed;
        //    ActiveGameLogic.IncrementTotalGamesPlayed(_dbContext);
        //    int after = _dbContext.Statistics.First().TotalGamesPlayed;

        //    Assert.NotEqual(before, after);
        //}
    }
}