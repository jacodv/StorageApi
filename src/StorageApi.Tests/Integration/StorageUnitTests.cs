using StorageApi.Models;
using StorageApi.Tests.Helpers;
using Xunit.Abstractions;

namespace StorageApi.Tests.Integration
{
  public class StorageUnitTests : IntegrationTestBase<StorageUnitModel, StorageUnitInsertUpdateModel>
  {
    public StorageUnitTests(ITestOutputHelper output)
      : base("api/storageunit", output)
    {
      _testUpdate = false;
    }

    #region Overrides

    protected override StorageUnitInsertUpdateModel GetInsertModel()
    {
      return TestDataHelper.NewStorageUnit();
    }

    protected override StorageUnitInsertUpdateModel GetUpdateModel()
    {
      return TestDataHelper.UpdateStorageUnit();
    }

    #endregion
  }
}