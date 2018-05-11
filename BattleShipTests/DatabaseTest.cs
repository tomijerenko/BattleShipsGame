using BattleShip.Data;
using BattleShip.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

namespace BattleShipTests
{
    public class DatabaseTest
    {
        DataBaseContext _dbContext;

        public DatabaseTest()
        {
            DbContextOptionsBuilder<DataBaseContext> optionsBuilder = new DbContextOptionsBuilder<DataBaseContext>();
            optionsBuilder.UseInMemoryDatabase("DatabaseTest");
            _dbContext = new DataBaseContext(optionsBuilder.Options);
            _dbContext.Statistics.Add(new GameStatistics()
            {
                TotalMissileHits = 5,
                TotalMissileShoots = 20
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public void Create()
        {
            int countBefore = _dbContext.Statistics.Count();
            _dbContext.Statistics.Add(new GameStatistics() { });
            _dbContext.SaveChanges();
            int countAfter = _dbContext.Statistics.Count();

            Assert.NotEqual(countBefore, countAfter);
        }

        [Fact]
        public void Read()
        {
            GameStatistics stats = _dbContext.Statistics.First();

            Assert.NotNull(stats);
        }

        [Fact]
        public void Update()
        {
            int statsBefore = _dbContext.Statistics.First().TotalGamesPlayed;
            GameStatistics statsTemp = _dbContext.Statistics.First();
            statsTemp.TotalGamesPlayed = 500;
            _dbContext.SaveChanges();
            int statsAfter = _dbContext.Statistics.First().TotalGamesPlayed;

            Assert.NotEqual(statsBefore, statsAfter);
        }

        [Fact]
        public void Delete()
        {
            GameStatistics stats = new GameStatistics();
            _dbContext.Statistics.Add(stats);
            _dbContext.SaveChanges();
            int countBefore = _dbContext.Statistics.Count();
            _dbContext.Statistics.Remove(stats);
            _dbContext.SaveChanges();
            int countAfter = _dbContext.Statistics.Count();

            Assert.NotEqual(countBefore, countAfter);
        }
    }
}