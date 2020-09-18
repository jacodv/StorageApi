using StorageApi.Models;
using StorageApi.Tests.Helpers;
using Xunit.Abstractions;

namespace StorageApi.Tests.Integration
{
  public class LocationTests : IntegrationTestBase<LocationModel, LocationInsertUpdateModel>
  {
    public LocationTests(ITestOutputHelper output)
    :base("api/location", output)
    {
    }

    #region Overrides

    protected override LocationInsertUpdateModel GetInsertModel()
    {
      return TestDataHelper.NewLocation();
    }

    protected override LocationInsertUpdateModel GetUpdateModel()
    {
      return TestDataHelper.UpdatedLocation();
    }

    #endregion
  }
}
