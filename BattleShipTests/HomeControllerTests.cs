using BattleShip.Controllers;
using BattleShip.Data;
using BattleShip.Data.Entities;
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BattleShipTests
{
    public class HomeControllerTests
    {
        HomeController controller;

        public HomeControllerTests()
        {
            DbContextOptionsBuilder<DataBaseContext> optionsBuilder = new DbContextOptionsBuilder<DataBaseContext>();
            optionsBuilder.UseInMemoryDatabase("FakeDb");
            DataBaseContext _dbContext = new DataBaseContext(optionsBuilder.Options);
            _dbContext.Statistics.Add(new GameStatistics()
            {
                TotalMissileHits = 5,
                TotalMissileShoots = 20
            });

            
            _dbContext.SaveChanges();
            controller = new HomeController(_dbContext);
        }

        [Fact]
        public void Index_returnsView_ModelNotNull()
        {
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<StatisticsModel>(
                viewResult.ViewData.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public void BattleFieldSetup_Get_returnsView()
        {
            var result = controller.BattleFieldSetup();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void BattleFieldSetup_Post_redirectsToPlayGame_ModelValid()
        {
            //mockam TempData
            var mock = new Mock<ITempDataDictionary>();
            controller.TempData = mock.Object;
            var bla = controller.TempData["model"];

            var result = controller.BattleFieldSetup(new GameInitModel()
            {
                PlayerName = "test",
                SocketId = "",
                SerializedBFArray = "[[\"x\",\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]," +
                "[\"x\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]]"
            });
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("PlayGame", redirectToActionResult.ActionName);
        }

        [Fact]
        public void BattleFieldSetup_Post_ReturnsBattleFieldSetupView_ModelNotValid()
        {
            var result = controller.BattleFieldSetup(new GameInitModel() { });
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<GameInitModel>(
                viewResult.ViewData.Model);
            Assert.NotNull(model);
        }
        [Fact]
        public void PlayGame_Get_redirectsToIndex_TempDataNull()
        {
            //mockam TempData
            var mock = new Mock<ITempDataDictionary>();
            controller.TempData = mock.Object;

            var result = controller.PlayGame();
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void PlayGame_Get_ReturnsPlayGameView_TempDataNotNull()
        {
            var mock = new Mock<ITempDataDictionary>();
            mock.SetupGet(x => x["model"]).Returns(
                "{\"PlayerName\":\"sdsdsds\",\"SerializedBFArray\":" +
                "\"[[\\\"\\\",\\\"\\\",\\\"\\\",\\\"x\\\",\\\"x\\\",\\" +
                "\"x\\\",\\\"x\\\",\\\"\\\",\\\"\\\",\\\"\\\"],[\\\"\\\"," +
                "\\\"x\\\",\\\"x\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\"," +
                "\\\"\\\",\\\"\\\",\\\"\\\"],[\\\"\\\",\\\"x\\\",\\\"\\\"," +
                "\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\"]," +
                "[\\\"x\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"x\\\",\\\"x\\\"," +
                "\\\"\\\",\\\"\\\",\\\"\\\"],[\\\"x\\\",\\\"\\\",\\\"\\\",\\\"\\\"," +
                "\\\"\\\",\\\"x\\\",\\\"x\\\",\\\"\\\",\\\"\\\",\\\"\\\"],[\\\"\\\"," +
                "\\\"x\\\",\\\"x\\\",\\\"x\\\",\\\"x\\\",\\\"\\\",\\\"\\\",\\\"\\\"," +
                "\\\"\\\",\\\"\\\"],[\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\"," +
                "\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\"],[\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\"," +
                "\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\"],[\\\"\\\",\\\"\\\"," +
                "\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\"]," +
                "[\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\",\\\"\\\"," +
                "\\\"\\\",\\\"\\\"]]\",\"SocketId\":null}");
            controller.TempData = mock.Object;
            var bla = controller.TempData["model"];


            var result = controller.PlayGame();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<GameInitModel>(
                viewResult.ViewData.Model);
            Assert.NotNull(model);

        }

    }
}
