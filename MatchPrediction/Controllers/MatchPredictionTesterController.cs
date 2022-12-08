using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchPrediction.Services.MatchPredictionServices.ExactResult;
using Microsoft.AspNetCore.Mvc;

namespace MatchPrediction.Controllers
{
    public class MatchPredictionTesterController : Controller
    {
        private readonly IMatchExactResultTesterService _testerService;
        private readonly ILogger<MatchPredictionTesterController> _logger;

        public MatchPredictionTesterController(IMatchExactResultTesterService testerService, ILogger<MatchPredictionTesterController> logger)
        {
            _testerService = testerService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult InputData()
        {
            return View();
        }


        public async Task<IActionResult> AccuracyTest()
        {
            var datasetFrom = new DateTime(2019, 1, 1);
            var datasetTo = new DateTime(2020, 12, 31);

            var dataTestFrom = new DateTime(2021, 1, 1);
            var dataTestTo = new DateTime(2021, 12, 31);

            var div = "I1";

            var result = await _testerService.TeamWinner_AllDivisions(datasetFrom, datasetTo, dataTestFrom, dataTestTo);

            ViewBag.Output = result;

            return View("OutputData");
        }
    }
}