using BattleShip.Data;
using BattleShip.GameLogic;
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            return View(GameHandler.Statistics);
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
            return Ok(GameHandler.Statistics);
        }
    }
}