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
  }
}
