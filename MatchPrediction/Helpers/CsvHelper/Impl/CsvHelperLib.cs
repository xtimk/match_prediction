using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace MatchPrediction.Helpers.CsvHelper.Impl
{
    public class CsvHelperLib<T> : ICsvHelper<T>
    {
        private readonly ILogger<CsvHelperLib<T>> _logger;
        private readonly CsvConfiguration csvConfig;

        public CsvHelperLib(ILogger<CsvHelperLib<T>> logger)
        {
            _logger = logger;
#pragma warning disable CS8604 // Possibile argomento di riferimento Null.
            csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldSkipRecord = record => record.Row.Parser.Record.All(field => string.IsNullOrWhiteSpace(field))
            };
#pragma warning restore CS8604 // Possibile argomento di riferimento Null.
        }

        // Note that this returns IEnumerable.
        // Means that this is like iqueriable, so there is nothing in ram.
        // If i call ToList() I will load all to RAM
        // But its better to paging by using linq things..
        public IEnumerable<T> Read(string filepath)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Read(StreamReader stream)
        {
            _logger.LogInformation("Parsing csv streamreader");
            try
            {
                var csv = new CsvReader(stream, csvConfig);
                var records = csv.GetRecords<T>();
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception encountered while parsing csv file");
                return Enumerable.Empty<T>();
            }
        }

        public IEnumerable<T> Read(Stream stream)
        {
            _logger.LogInformation("Parsing csv stream");
            try
            {
                var streamreader = new StreamReader(stream);
                var csv = new CsvReader(streamreader, csvConfig);
                var records = csv.GetRecords<T>();
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception encountered while parsing csv file");
                return Enumerable.Empty<T>();
            }
        }
    }
}
