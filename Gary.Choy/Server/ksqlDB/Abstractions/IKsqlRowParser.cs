namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlRowParser
    {
        string ParseStreamRowToJson(string input, string[] headers);
    }
}
