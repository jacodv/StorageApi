using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Repositories
{
  public class CollectionAuditDocument
  {
    public CollectionAuditDocument()
    {
      Id = ObjectId.GenerateNewId();
      CreatedAt = Id.CreationTime;
    }
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public ObjectId Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public BsonDocument DocumentKey { get; set; }
    public string OperationType { get; set; }
    public string CollectionName{ get; set; }
    public string[] RemovedFields { get; set; }
    public BsonDocument UpdatedFields { get; set; }
  }

}
