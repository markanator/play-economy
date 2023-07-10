namespace Play.Common.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; init; }
        public int Port { get; init; }
        // expression body definition
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}