using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StorageApi.Interfaces
{
  public interface IDocumentReference
  {
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    ObjectId Id { get; set; }
    string Name { get; set; }
  }
}