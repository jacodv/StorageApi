using System.Collections.Generic;
using StorageApi.Data;
using StorageApi.Interfaces;

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
}