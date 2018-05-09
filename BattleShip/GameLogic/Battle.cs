using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShip.GameLogic
{
    public class Battle
    {        
        private DateTime _activeGame;
        private string _playerSocketTurn;
        private int myVar;

        public DateTime ActiveGameTime
        {
            get { return _activeGame; }
        }

        public List<BattleField> BattleFields { get; set; }

        public Battle(BattleField firstConnectedBattle)
        {
            BattleFields = new List<BattleField>();
            BattleFields.Add(firstConnectedBattle);
        }

        public bool IsGameReady()
        {
            int counter = 0;

            foreach (BattleField item in BattleFields)
            {
                if (item.Ready)
                    counter++;
                if (counter == 2)
                    return true;
            }
            return false;
        }

        public bool AddSecondBattleField(BattleField secondConnectedBattle)
        {
            if (BattleFields.Count == 2)
                return false;
            else
            {
                _activeGame = DateTime.Now;
                BattleFields.Add(secondConnectedBattle);
                _playerSocketTurn = BattleFields[RandomGenerator.GetRandomNumber(0, 2)].SocketId;

                return true;
            }            
        }

        public bool IsPlayersTurn(string playerSocketId)
        {
            if (_playerSocketTurn == playerSocketId)
                return true;
            else
                return false;
        }

        public bool Shoot(string playerSocketId, string x, string y)
        {
            BattleField battleField = BattleFields.FirstOrDefault(field => field.SocketId != playerSocketId);
            string battleFieldValue = battleField.PlayerBattleArray[Convert.ToInt32(x), Convert.ToInt32(y)];
            _playerSocketTurn = battleField.SocketId;
            if (battleFieldValue != "" && battleFieldValue != "m")
            {
                battleField.NoOfHits++;
                return true;
            }                
            else
            {
                battleFieldValue = "m";
                return false;
            }
        }

        public bool IsGameOver()
        {
            foreach (BattleField item in BattleFields)
            {
                if (item.NoOfHits >= 17)
                    return true;
            }
            return false;
        }

        public string GetWinnerName()
        {
            foreach (BattleField item in BattleFields)
            {
                if (item.NoOfHits >= 17)
                    return item.Player.PlayerName;
            }
            return null;
        }
    }
}