using System;
using System.Linq;
using StorageAPI.Models.Interfaces;

namespace StorageAPI.Models
{
  public class Location: Document
  {
    public Address Address { get; set; }
    public Contact Contact { get; set; }

    public virtual IQueryable<StorageUnit> GetStorageUnits(IRepository<Location> repository)
    {
      throw new NotImplementedException(); 
    }
    public virtual IQueryable<StorageBin> GetStorageBins(IRepository<Location> repository)
    {
      throw new NotImplementedException();
    }
  }
}