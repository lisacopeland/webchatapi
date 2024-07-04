using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webchat.Models
{
    public class UserClass
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool Online { get; set; }
    }
}
