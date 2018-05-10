namespace BattleShip.GameLogic
{
    public static class ShipDataStatic
    {
        public static string[,] Ships { get; set; }

        static ShipDataStatic()
        {
            Ships = new string[,] { 
                { "Carrier", "5" },
                { "BattleShip", "4" },
                { "Submarine", "3" },
                { "Cruiser", "3" },
                { "Destroyer", "2" }
            };
        }
    }
}
