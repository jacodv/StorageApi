using System;
using System.Linq;
using StorageAPI.Models.Interfaces;

namespace StorageAPI.Models
{
  public class StorageUnit: Document
  {
    public string StorageUnitType { get; set; }
    public Location Location { get; set; }

    public virtual IQueryable<StorageBin> GetStorageBins(IRepository<StorageUnit> repository)
    {
      throw new NotImplementedException();
    }
  }
}