using GR.Repository;
using MongoDB.Bson.Serialization.Attributes;

namespace GR.Services
{
    [BsonIgnoreExtraElements]
    public class Ticket : Entity
    {
        public double Price { get; set; }
        public string EventName { get; set; }
        public int Discount { get; set; }
    }
}
