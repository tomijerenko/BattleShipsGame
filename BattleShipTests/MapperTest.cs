using BattleShip.Data.Entities;
using BattleShip.Helpers;
using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BattleShipTests
{
    public class MapperTest
    {
        [Fact]
        public void TestMapperValue_string_string_samevalue()
        {
            GameStatistics valueOne = new GameStatistics() { TotalMissileHits = 3, TotalMissileShoots = 5 };
            StatisticsModel valueTwo = new StatisticsModel() { TotalMissileHits = 3, TotalMissileShoots = 5 };

            StatisticsModel valueThree = valueOne.CreateMapped<GameStatistics, StatisticsModel>();

            Assert.True(valueTwo.TotalMissileHits == valueThree.TotalMissileHits);
            Assert.True(valueTwo.TotalMissileShoots == valueThree.TotalMissileShoots);
        }
    }
}
