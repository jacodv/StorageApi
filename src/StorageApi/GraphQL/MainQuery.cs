using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using MongoDB.Repositories.Interfaces;
using StorageApi.Models;

namespace StorageApi.GraphQL
{
  [ExtendObjectType(Name = "Query")]
  public class StorageApiQueryType
  {
    public IQueryable<Models.StorageLocation> GetLocations([Service] IRepository<Models.StorageLocation> repository) =>
      repository.AsQueryable();
    public IQueryable<Models.StorageBin> GetStorageBins([Service] IRepository<Models.StorageBin> repository) =>
      repository.AsQueryable();
    public IQueryable<Models.StorageUnit> GetStorageUnits([Service] IRepository<Models.StorageUnit> repository) =>
      repository.AsQueryable();
  }

  public class StorageUnitType : ObjectType<StorageUnit>
  {
    protected override void Configure(IObjectTypeDescriptor<StorageUnit> descriptor)
    {
      base.Configure(descriptor);
      descriptor.Ignore(d => d.AssignBin(default, default, default));
    }
  }
}
