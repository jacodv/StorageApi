using StorageApi.Models;
using StorageApi.Tests.Helpers;
using Xunit.Abstractions;

namespace StorageApi.Tests.Integration
{
  public class LocationTests : IntegrationTestBase<StorageLocationModel, StorageLocationInsertUpdateModel>
  {
    public LocationTests(ITestOutputHelper output)
    :base("api/location", output)
    {
    }

    #region Overrides

    protected override StorageLocationInsertUpdateModel GetInsertModel()
    {
      return TestDataHelper.NewLocation();
    }

    protected override StorageLocationInsertUpdateModel GetUpdateModel()
    {
      return TestDataHelper.UpdatedLocation();
    }

    #endregion
  }
}
