using System;
using MongoDB.Bson;
using MongoDB.Repositories.Interfaces;

namespace MongoDB.Repositories
{
  public abstract class Document : IDocument
  {
    protected Document()
    {
      Id = ObjectId.GenerateNewId();
    }
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt => Id.CreationTime;
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
  }
}