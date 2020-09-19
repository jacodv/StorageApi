using StorageApi.Models;
using StorageApi.Tests.Helpers;
using Xunit.Abstractions;

namespace StorageApi.Tests.Integration
{
  public class StorageBinTests : IntegrationTestBase<StorageBinModel, StorageBinInsertUpdateModel>
  {
    public StorageBinTests(ITestOutputHelper output)
      : base("api/storagebin", output)
    {
    }

    #region Overrides

    protected override StorageBinInsertUpdateModel GetInsertModel()
    {
      return TestDataHelper.NewStorageBin();
    }

    protected override StorageBinInsertUpdateModel GetUpdateModel()
    {
      return TestDataHelper.UpdatedStorageBin();
    }

    #endregion
  }
}