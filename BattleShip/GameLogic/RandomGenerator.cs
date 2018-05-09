using System;

namespace BattleShip.GameLogic
{
    public static class RandomGenerator
    {
        private static readonly Random _getRandom = new Random();

        public static Random GetRandom
        {
            get { return _getRandom; }
        }

        public static int GetRandomNumber(int min, int max)
        {
            lock (_getRandom)
            {
                return _getRandom.Next(min, max);
            }
        }
    }
}
