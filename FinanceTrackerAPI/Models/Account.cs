using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FinanceTrackerAPI.Models
{
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Balance")]
        public decimal Balance { get; set; }
    }
}
