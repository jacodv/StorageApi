using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Repositories.Interfaces;

namespace MongoDB.Repositories
{
  public abstract class Document : IDocument
  {
    private string _id;

    protected Document()
    {
      Id = ObjectId.GenerateNewId().ToString();
    }

    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id
    {
      get => _id;
      set
      {
        if(!ObjectId.TryParse(value, out var objectId))
          throw new ArgumentOutOfRangeException($"Invalid Id:{value}");
        _id = value;
      }
    }

    public string Name { get; set; }
    public DateTime CreatedAt => ObjectId.Parse(Id).CreationTime;
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
  }
}