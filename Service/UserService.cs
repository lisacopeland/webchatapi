﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using webchat.Models;

namespace webchat.Service
{
    public class UserService
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<UserClass> _usersCollection;

        public UserService(
            IConfiguration config
        )
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
            _usersCollection = mongoDatabase.GetCollection<UserClass>("users");
            }
            catch ( Exception ex )
            {
                Console.WriteLine($"Caught exception establishing db connection {ex.Message}");
            }
        }

        public async Task<List<UserClass>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<List<UserClass>> GetLoggedInUsersAsync() =>
            await _usersCollection.Find(x => x.Online == true).ToListAsync();

        public async Task<UserClass?> GetAsync(string id) =>
            await _usersCollection.Find(x => x._id == id).FirstOrDefaultAsync();

        public async Task<UserClass?> GetByUsernameAsync(string userName) =>
            await _usersCollection.Find(x => x.UserName == userName).FirstOrDefaultAsync();

        public async Task<ApiResponseClass> CreateAsync(UserClass newUser)
        {
            UserClass user = await GetByUsernameAsync(newUser.UserName);
            ApiResponseClass result;
            if (user != null)
            {
                result = new ApiResponseClass { Success = false };
                result.Message = "User already exists";
                return result;
            }
            newUser.Online = false;
            newUser.CreatedDate = DateTime.Now;
            await _usersCollection.InsertOneAsync(newUser);
            result = new ApiResponseClass { Success = true };
            result.Id = newUser._id;
            result.Message = "User created successfully";
            return result;
        }

        public async Task UpdateAsync(string id, UserClass updatedUserClass) =>
            await _usersCollection.ReplaceOneAsync(x => x._id == id, updatedUserClass);

        public async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x._id == id);
    }
}
