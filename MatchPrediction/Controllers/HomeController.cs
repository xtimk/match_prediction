using MatchPrediction.Models;
using MatchPrediction.Services.MatchStatsGetterService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MatchPrediction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMatchStatsGetterService _matchStatsGetterService;

        public HomeController(
                ILogger<HomeController> logger,
                IMatchStatsGetterService matchStatsGetterService)
        {
            _logger = logger;
            _matchStatsGetterService = matchStatsGetterService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}