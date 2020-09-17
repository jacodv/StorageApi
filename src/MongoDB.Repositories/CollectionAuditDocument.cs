using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Repositories.Interfaces;

namespace MongoDB.Repositories
{
  public class CollectionAuditDocument
  {
    public CollectionAuditDocument()
    {
      Id = ObjectId.GenerateNewId();
    }
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public ObjectId Id { get; set; }

    public DateTime CreatedAt => Id.CreationTime;
    public BsonDocument DocumentKey { get; set; }
    public ChangeStreamOperationType OperationType { get; set; }
    public BsonDocument BackingDocument { get; set; }
    public CollectionNamespace CollectionNamespace { get; set; }
    public IDocument FullDocument { get; set; }
    public ChangeStreamUpdateDescription UpdateDescription { get; set; }
  }

}
