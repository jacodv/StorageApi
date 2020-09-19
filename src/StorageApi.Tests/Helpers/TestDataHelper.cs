using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using StorageApi.Models;

namespace StorageApi.Tests.Helpers
{
  public static class TestDataHelper
  {
    #region Location
    public static LocationInsertUpdateModel NewLocation()
    {
      return Builder<LocationInsertUpdateModel>
        .CreateNew()
        .Build();
    }
    #endregion

    public static LocationInsertUpdateModel UpdatedLocation()
    {
      return Builder<LocationInsertUpdateModel>
        .CreateNew()
        .With(_=>_.Name="UpdatedName")
        .Build();
    }


    public static List<StorageBinContentInsertUpdateModel> NewStorageBinContent(int count)
    {
      return Builder<StorageBinContentInsertUpdateModel>
        .CreateListOfSize(count)
        .Build()
        .ToList();
    }

    public static StorageBinInsertUpdateModel NewStorageBin()
    {
      return Builder<StorageBinInsertUpdateModel>
        .CreateNew()
        .With(_=>_.Contents= NewStorageBinContent(2))
        .Build();
    }

    public static StorageBinInsertUpdateModel UpdatedStorageBin()
    {
      return Builder<StorageBinInsertUpdateModel>
        .CreateNew()
        .With(_=>_.Name="UpdatedName")
        .With(_ => _.Contents = NewStorageBinContent(1))
        .Build();
    }

    public static StorageUnitInsertUpdateModel NewStorageUnit()
    {
      return Builder<StorageUnitInsertUpdateModel>
        .CreateNew()
        .With(_ => _.Rows = 3)
        .With(_ => _.ColumnsPerRow = 3)
        .Build();
    }

    public static StorageUnitInsertUpdateModel UpdateStorageUnit()
    {
      return Builder<StorageUnitInsertUpdateModel>
        .CreateNew()
        .With(_=>_.Name="UpdatedName")
        .With(_ => _.Rows = 2)
        .With(_ => _.ColumnsPerRow = 2)
        .Build();
    }
  }
}
