using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinanceTrackerAPI.Models
{
    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("AccountId")]
        public string AccountId { get; set; }

        [BsonElement("Amount")]
        public decimal Amount { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }

        [BsonElement("Description")]
        public string? Description { get; set; }
    }
}
