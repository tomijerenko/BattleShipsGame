using BattleShip.Controllers;
using Xunit;

namespace BattleShipTests
{
    public class HomeControllerTests
    {
        HomeController controller;

        [Fact]
        public void Index()
        {
            controller = new HomeController();

            Assert.Same(random, RandomGenerator.GetRandom);
        }
    }
}
