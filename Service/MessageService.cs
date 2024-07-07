using Microsoft.Extensions.Options;
using MongoDB.Driver;
using webchat.Models;

namespace webchat.Service
{
    public class MessageService
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<MessageClass> _messagesCollection;
        public MessageService(
            IConfiguration config)
        {
            _config = config;
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };

                var mongoClient = new MongoClient(
                    settings);
                var mongoDatabase = mongoClient.GetDatabase("wschat");
                _messagesCollection = mongoDatabase.GetCollection<MessageClass>("wsmessages");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception establishing db connection {ex.Message}");
            }
        }

        public async Task<List<MessageClass>> GetAsync() =>
            await _messagesCollection.Find(_ => true).ToListAsync();

        public async Task<MessageClass?> GetAsync(string id) =>
            await _messagesCollection.Find(x => x._id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(MessageClass newMessageClass) =>
            await _messagesCollection.InsertOneAsync(newMessageClass);

        public async Task UpdateAsync(string id, MessageClass updatedMessageClass) =>
            await _messagesCollection.ReplaceOneAsync(x => x._id == id, updatedMessageClass);

        public async Task RemoveAsync(string id) =>
            await _messagesCollection.DeleteOneAsync(x => x._id == id);
    }
}
