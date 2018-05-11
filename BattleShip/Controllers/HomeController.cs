using BattleShip.Data;
using BattleShip.Data.Entities;
using BattleShip.GameLogic;
using BattleShip.Helpers;
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace BattleShip.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataBaseContext _context;

        public HomeController(DataBaseContext context)
        {            
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            StatisticsModel stats = _context.Statistics.ToList().First().CreateMapped<GameStatistics, StatisticsModel>();
            stats.CurrentActiveGames = GameHandler.CurrentActiveBattles;

            return View(stats);
        }

        [HttpGet]
        public IActionResult BattleFieldSetup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BattleFieldSetup(GameInitModel model)
        {
            if (ModelState.IsValid && TempData != null)
            {
                TempData["model"] = JsonConvert.SerializeObject(model);
                return RedirectToAction("PlayGame");
            }               
            else
                return View(model);
        }

        [HttpGet]
        public IActionResult PlayGame()
        {
            GameInitModel model;
            if (TempData["model"] != null)
            {
                model = JsonConvert.DeserializeObject<GameInitModel>((string)TempData["model"]);
                
                return View(model);
            }                
            else
                return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult StatisticsData()
        {
            StatisticsModel stats = _context.Statistics.ToList().First().CreateMapped<GameStatistics, StatisticsModel>();
            stats.CurrentActiveGames = GameHandler.CurrentActiveBattles;

            return Ok(stats);
        }
    }
}
