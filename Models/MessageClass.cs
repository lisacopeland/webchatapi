using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webchat.Models
{
    public class MessageClass
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        public string Message { get; set; } = null!;

        public string? UserName { get; set; }

        public DateTime? MessageDate { get; set; }

    }
}
