using MongoDB.Driver;

namespace GetOption.Infrastructure.Implementations
{
    public  class MongoConnection
    {
        IMongoDatabase mongoDatabase;
        public MongoConnection(MongoConfig mongoConfig)
        {
            string connectionString = $"mongodb://{mongoConfig.Username}:{mongoConfig.Password}@{mongoConfig.Server}:{mongoConfig.Port}/{mongoConfig.DatabaseName}";
            mongoDatabase = new MongoClient(connectionString).GetDatabase(mongoConfig.DatabaseName);
        }


        public  IMongoDatabase Database 
        {
            get { return mongoDatabase; }
        }





    }
}
