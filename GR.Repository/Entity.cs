using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GR.Repository
{
    public class Entity : IEntity
    {
        public ObjectId Id { get; set; }

        [BsonIgnore]
        public string IdString { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long Version { get; set; }

        public IDictionary<string, object> Metadata { get; set; }

        public Entity()
        {
            Metadata = new Dictionary<string, object>();
        }
    }
}
