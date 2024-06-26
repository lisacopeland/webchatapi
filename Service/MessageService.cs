using Microsoft.Extensions.Options;
using MongoDB.Driver;
using webchat.Models;

namespace webchat.Service
{
    public class MessageService
    {
        private readonly IMongoCollection<MessageClass> _messagesCollection;
        public MessageService(
            IOptions<WsDatabaseSettingsClass> wsDatabaseSettingsClass)
        {
            var mongoClient = new MongoClient(
                "mongodb://localhost:27017");

            var mongoDatabase = mongoClient.GetDatabase(
                "wschat");

            _messagesCollection = mongoDatabase.GetCollection<MessageClass>(
                "wsmessages");
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
