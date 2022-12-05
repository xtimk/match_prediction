namespace MatchPrediction.Helpers.CsvHelper
{
    public interface ICsvHelper<T>
    {
        IEnumerable<T> Read(string filepath);
        IEnumerable<T> Read(StreamReader stream);
        IEnumerable<T> Read(Stream stream);
    }
}
