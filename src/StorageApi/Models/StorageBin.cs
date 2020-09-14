using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using StorageApi.Data;
using StorageApi.Interfaces;

namespace StorageApi.Models
{
  [BsonCollection("Bin")]
  public class StorageBin: Document, IDocumentReference
  {
    public StorageBin()
    {
      Contents = new List<StorageBinContent>();
    }
    public string Name { get; set; }
    public List<StorageBinContent> Contents { get; set; }
    [BsonIgnore]
    public double Weight
    {
      get
      {
        return Contents?.Sum(_ => _.TotalWeight) ?? 0;
      }
    }

  }
}