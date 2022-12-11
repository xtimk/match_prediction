using System;
using MatchPrediction.Constants;
using MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers;

namespace MatchPrediction.Managers.PredictionManagers.PoissonExactResult
{
    public class PredictionResponseReaderManager
    {
        private readonly ILogger<PredictionResponseReader> _logger;
        private readonly IServiceProvider _serviceProvider;

        public PredictionResponseReaderManager(ILogger<PredictionResponseReader> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public PredictionResponseReader CreateReader(string readerType)
        {
            PredictionResponseReader? reader = null;
            switch (readerType)
            {
                case var value when value == ReaderConstants.READER_POISSON_EXACT_RESULT_TEAMWINNER:
                    reader = _serviceProvider.GetRequiredService<PredictionResponse_PoissonExactResult_TeamWinner>();
                    break;
                case var value when value == ReaderConstants.READER_POISSON_EXACT_RESULT_BOTHTEAMSTOSCORE:
                    reader = _serviceProvider.GetRequiredService<PredictionResponse_PoissonExactResult_BothTeamsToScore>();
                    break;
                case var value when value == ReaderConstants.READER_POISSON_EXACT_RESULT_EXACTRESULT:
                    reader = _serviceProvider.GetRequiredService<PredictionResponse_PoissonExactResult_ExactResult>();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return reader;
        }
    }
}

