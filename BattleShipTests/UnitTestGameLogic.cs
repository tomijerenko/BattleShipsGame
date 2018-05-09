using BattleShip.GameLogic;
using System;
using Xunit;

namespace BattleShipTests
{
    public class UnitTestGameLogic
    {
        public Battle _battle { get; set; }
        Random random = RandomGenerator.GetRandom;

        private void ResetBattle()
        {
            _battle = new Battle(new BattleField() {
                SocketId = "socketIdFirstPlayerConnected",
                PlayerBattleArray = new string[10, 10]
                {{"x","x","x","x","x","x","x","x","x","x"},
                {"x","x","x","x","x","x","x","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""}},
                Ready = true
            });
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(0, 1000)]
        [InlineData(0, 10000000)]
        [InlineData(0, 100000000000)]
        public void RandomNumberGeneratorTest_Min_Max_InRange(int min, int max)
        {
            Assert.InRange(RandomGenerator.GetRandomNumber(min, max), min, max);
        }

        [Fact]
        public void RandomGenerator_IsSingleton()
        {
            Assert.Same(random, RandomGenerator.GetRandom);
        }

        [Fact]
        public void IsPlayerTurn_DifferentSocketId_false()
        {
            
        }
        [Fact]
        public void IsPlayerTurn_CorrectSocketId_true()
        {
            
        }
        [Fact]
        public void IsGameReady_BattleInit_False()
        {

        }
        [Fact]
        public void IsGameReady_SecondPlayerJoin_true()
        {

        }
        [Fact]
        public void SecondPlayerJoin_FirstTime_true()
        {

        }
        [Fact]
        public void SecondPlayerJoin_SecondOrMoreTime_false()
        {

        }

        //battlearray prevec
        //battlearray premalo
        //battlearray dovolj
    }
}
