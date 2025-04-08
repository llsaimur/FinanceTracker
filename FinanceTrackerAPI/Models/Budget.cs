using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FinanceTrackerAPI.Models
{
    public class Budget
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Limit")]
        public decimal Limit { get; set; }

        [BsonElement("StartDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; }
    }
}
