using System;
using MongoDB.Bson;
using StorageApi.Interfaces;

namespace StorageApi.Models
{
  public abstract class Document : IDocument
  {
    protected Document()
    {
      Id = ObjectId.GenerateNewId();
    }
    public ObjectId Id { get; set; }
    public DateTime CreatedAt => Id.CreationTime;
    public string CreatedBy { get; set; }
  }
}