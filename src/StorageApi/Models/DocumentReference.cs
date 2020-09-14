using MongoDB.Bson;
using StorageApi.Interfaces;

namespace StorageApi.Models
{
  public class DocumentReference: IDocumentReference
  {
    public ObjectId Id { get; set; }
    public string Name { get; set; }
  }
}