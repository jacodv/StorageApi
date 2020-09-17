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
  }
}
