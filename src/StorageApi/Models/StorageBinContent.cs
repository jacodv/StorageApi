using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace StorageApi.Models
{
  public class StorageBinContent
  {
    public StorageBinContent()
    {
      Tags = new List<string>();
    }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public double UnitWeight { get; set; }
    [BsonIgnore]
    public double TotalWeight => Quantity * UnitWeight;

    public List<string> Tags { get; set; }
  }
}