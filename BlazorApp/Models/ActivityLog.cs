using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BlazorApp.Models
{
    public class ActivityLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}