using MatchPrediction.Data.Contexts;
using MatchPrediction.Helpers.DataTables;
using MatchPrediction.Models;
using MatchPrediction.Services.MatchStatsGetterService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MatchPrediction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMatchStatsGetterService _matchStatsGetterService;
        private readonly MatchPredictionContext _db;

        public HomeController(
                ILogger<HomeController> logger,
                IMatchStatsGetterService matchStatsGetterService,
                MatchPredictionContext db)
        {
            _logger = logger;
            _matchStatsGetterService = matchStatsGetterService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            
            return View();
        }

        public async Task<JsonResult> GetMatches(DataTableAjaxPostModel model)
        {
            var result = await _db.Matches.Where(x => x.Div == "I1").Take(100).ToListAsync();
            return Json(new
            {
                draw= model.draw,
                recordsTotal = result.Count,
                recordsFiltered = 0,
                data = result
            });
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