using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MongoDB.Repositories.Interfaces
{
  public interface IDocumentReference
  {
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    [BsonRepresentation(BsonType.ObjectId)] 
    string Id { get; set; }
    string Name { get; set; }
  }
}