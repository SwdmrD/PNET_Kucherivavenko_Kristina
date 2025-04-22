using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.Models;
using MongoDB.Driver;

namespace BlazorApp
{
    public class ActivityLogService
    {
        private readonly IMongoCollection<ActivityLog> _logs;
        private readonly IMongoDatabase _database;
        public ActivityLogService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _logs = _database.GetCollection<ActivityLog>("activity_logs");
        }
        public async Task LogActivity(string action, string entityType, string entityId, string details = "")
        {
            var log = new ActivityLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            await _logs.InsertOneAsync(log);
        }

        public async Task<List<ActivityLog>> GetAllLogs()
        {
            return await _logs.Find(_ => true).ToListAsync();
        }

        public async Task<List<ActivityLog>> GetLogsByEntityType(string entityType)
        {
            return await _logs.Find(log => log.EntityType == entityType).ToListAsync();
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName, string databaseName = null)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                return _database.GetCollection<T>(collectionName);
            }
            else
            {
                var client = (MongoClient)_database.Client;
                var db = client.GetDatabase(databaseName);
                return db.GetCollection<T>(collectionName);
            }
        }
    }
}
