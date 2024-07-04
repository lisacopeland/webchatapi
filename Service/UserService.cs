using Microsoft.Extensions.Options;
using MongoDB.Driver;
using webchat.Models;

namespace webchat.Service
{
    public class UserService
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<UserClass> _usersCollection;
        public UserService(
            IConfiguration config,
            IOptions<WsDatabaseSettingsClass> wsDatabaseSettingsClass)
        {
            _config = config;
            var connectionString = _config["Messages:ConnectionString"];
            var mongoClient = new MongoClient(
                connectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                "wschat");

            _usersCollection = mongoDatabase.GetCollection<UserClass>(
                "users");
        }

        public async Task<List<UserClass>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<List<UserClass>> GetLoggedInUsersAsync() =>
            await _usersCollection.Find(x => x.Online == true).ToListAsync();
        public async Task<UserClass?> GetAsync(string id) =>
            await _usersCollection.Find(x => x._id == id).FirstOrDefaultAsync();

        public async Task<UserClass?> GetByUsernameAsync(string userName) =>
            await _usersCollection.Find(x => x.UserName == userName).FirstOrDefaultAsync();

        public async Task<ApiResponseClass> CreateAsync(UserClass newUserClass)
        {
            UserClass user = await GetByUsernameAsync(newUserClass.UserName);
            ApiResponseClass result;
            if (user != null)
            {
                result = new ApiResponseClass { Success = false };
                result.Message = "User already exists";
                return result;
            }
            await _usersCollection.InsertOneAsync(newUserClass);
            result = new ApiResponseClass { Success = true };
            result.Message = "User created successfully";
            return result;

        }


        public async Task UpdateAsync(string id, UserClass updatedUserClass) =>
            await _usersCollection.ReplaceOneAsync(x => x._id == id, updatedUserClass);

        public async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x._id == id);

    }
}
