using System.Collections.Generic;
using MongoDB.Repositories;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;

namespace StorageApi.Models
{
  [BsonCollection("StorageUnit")]
  public class StorageUnit : Document, IDocumentReference
  {
    public StorageUnit()
    {
      StorageRows = new List<StorageRow>();
    }
    public string Name { get; set; }
    public List<StorageRow> StorageRows { get; set; }
    public IDocumentReference Location { get; set; }
  }

  public class StorageInsertUpdateModel
  {
    public string Name { get; set; }

  }
}