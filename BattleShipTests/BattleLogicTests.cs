using BattleShip.GameLogic;
using System;
using Xunit;

namespace BattleShipTests
{
    public class BattleLogicTests
    {
        Random random = RandomGenerator.GetRandom;

        [Theory]
        [InlineData(0, 2)]
        [InlineData(0, 1000)]
        [InlineData(0, 10000000)]
        [InlineData(0, 1000000000)]
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
        public void IsPlayerTurn_DifferentSocketId_BattleNotStarted_false()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue
            });
            Assert.False(battle.IsPlayersTurn("NekiRandomSocketKiNeObstaja"));
        }

        [Fact]
        public void IsPlayerTurn_CorrectSocketId_BattleNotStarted_false()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue
            });
            Assert.False(battle.IsPlayersTurn(someSocketValue));
        }

        [Fact]
        public void IsPlayerTurn_DifferentSocketId_BattleStarted_false()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            string secondSocketValue = "dsgsdfhhfdjhgjjkgghjg";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue
            });
            battle.AddSecondBattleField(new BattleField()
            {
                SocketId = secondSocketValue
            });
            Assert.False(battle.IsPlayersTurn("NekiRandomSocketKiNeObstaja"));
        }
        [Fact]
        public void IsPlayerTurn_CorrectSocketId_true()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            string secondSocketValue = "dsgsdfhhfdjhgjjkgghjg";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue
            });
            battle.AddSecondBattleField(new BattleField()
            {
                SocketId = secondSocketValue
            });

            Assert.True(battle.IsPlayersTurn(battle.PlayerSocketTurn));
        }

        [Fact]
        public void IsGameReady_BattleInit_False()
        {
            Battle battle = new Battle(new BattleField()
            {
                Ready = true
            });
            Assert.False(battle.IsGameReady());
        }

        [Fact]
        public void IsGameReady_SecondPlayerJoin_true()
        {
            Battle battle = new Battle(new BattleField()
            {
                Ready = true
            });
            battle.AddSecondBattleField(new BattleField()
            {
                Ready = true
            });

            Assert.True(battle.IsGameReady());
        }

        [Fact]
        public void SecondPlayerJoin_FirstTime_true()
        {
            Battle battle = new Battle(new BattleField());            

            Assert.True(battle.AddSecondBattleField(new BattleField()));
        }
        [Fact]
        public void SecondPlayerJoin_SecondOrMoreTime_false()
        {
            Battle battle = new Battle(new BattleField());
            battle.AddSecondBattleField(new BattleField());

            Assert.False(battle.AddSecondBattleField(new BattleField()));
        }

        [Fact]
        public void ActiveGameStarts_FirstPlayerJoins_ActiveGameTimeEqualsMinValue()
        {
            Battle battle = new Battle(new BattleField());

            Assert.True(battle.ActiveGameTime == DateTime.MinValue);
        }

        [Fact]
        public void ActiveGameStarts_SecondPlayerJoins_TimerStarts()
        {
            DateTime currentTime = DateTime.Now;
            Battle battle = new Battle(new BattleField());
            battle.AddSecondBattleField(new BattleField());

            Assert.True(battle.ActiveGameTime > currentTime);
        }

        [Fact]
        public void Shoot_ShipOnField_true()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
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
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField());

            Assert.True(battle.Shoot("dffdsfsdsdfsd", "0", "0"));
        }

        [Fact]
        public void Shoot_ShipNotOnField_false()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
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
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField());

            Assert.False(battle.Shoot("dffdsfsdsdfsd", "9", "9"));
        }

        [Fact]
        public void Shoot_FieldAlreadyMissed_false()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
                PlayerBattleArray = new string[10, 10]
                {{"m","x","x","x","x","x","x","x","x","x"},
                {"x","x","x","x","x","x","x","x","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""},
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField());

            Assert.False(battle.Shoot("dffdsfsdsdfsd", "0", "0"));
        }

        [Fact]
        public void IsGameOver_BelowMaxHitCount_false()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            string secondSocket = "secondSocket";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
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
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField()
            {
                SocketId = secondSocket
            });

            for (int i = 0; i < 10; i++)
            {
                battle.Shoot(secondSocket, "0", "0");
            }

            Assert.False(battle.IsGameOver());
        }

        [Fact]
        public void Shoot_SecondPlayerNotJoined_null_false()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue
            });

            Assert.False(battle.Shoot(someSocketValue, "0", "0"));
        }

        [Fact]
        public void IsGameOver_SameAsMaxHitCount_true()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            string secondSocket = "secondSocket";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
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
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField()
            {
                SocketId = secondSocket
            });

            for (int i = 0; i < 17; i++)
            {
                battle.Shoot(secondSocket, "0", "0");
            }

            Assert.True(battle.IsGameOver());
        }

        [Fact]
        public void IsGameOver_AboveMaxHitCount_true()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            string secondSocket = "secondSocket";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
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
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField()
            {
                SocketId = secondSocket
            });

            for (int i = 0; i < 22; i++)
            {
                battle.Shoot(secondSocket, "0", "0");
            }

            Assert.True(battle.IsGameOver());
        }


        [Fact]
        public void GetWinnerName_BelowMaxHitsCount_null()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            string secondSocket = "secondSocket";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
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
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField()
            {
                SocketId = secondSocket
            });

            for (int i = 0; i < 10; i++)
            {
                battle.Shoot(secondSocket, "0", "0");
            }

            Assert.Null(battle.GetWinnerName());
        }

        [Fact]
        public void GetWinnerName_SameAsMaxHitsCount_string()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            string secondSocket = "secondSocket";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
                PlayerName = "test",
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
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField()
            {
                SocketId = secondSocket
            });

            for (int i = 0; i < 17; i++)
            {
                battle.Shoot(secondSocket, "0", "0");
            }

            Assert.IsType<string>(battle.GetWinnerName());
        }
        [Fact]
        public void GetWinnerName_AboveAsMaxHitsCount_string()
        {
            string someSocketValue = "socketIdFirstPlayerConnected";
            string secondSocket = "secondSocket";
            Battle battle = new Battle(new BattleField()
            {
                SocketId = someSocketValue,
                PlayerName = "test",
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
                {"","","","","","","","","",""}}
            });
            battle.AddSecondBattleField(new BattleField()
            {
                SocketId = secondSocket
            });

            for (int i = 0; i < 22; i++)
            {
                battle.Shoot(secondSocket, "0", "0");
            }

            Assert.IsType<string>(battle.GetWinnerName());
        }
    }
}
