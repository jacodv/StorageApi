namespace StorageAPI.Models
{
  public class StorageBin: Document
  {
    public string ContentType { get; set; }
    public int Quantity { get; set; }
    public double UnitWeight { get; set; }
    public double TotalWeight => Quantity * UnitWeight;
    public Location Location { get; set; }
    public StorageUnit StorageUnit { get; set; }
  }
}