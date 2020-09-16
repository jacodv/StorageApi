using MongoDB.Bson;
using MongoDB.Repositories.Interfaces;

namespace MongoDB.Repositories
{
  public class DocumentReference: IDocumentReference
  {
    public ObjectId Id { get; set; }
    public string Name { get; set; }
  }
}