using System.Collections.Generic;
using FizzWare.NBuilder;
using StorageApi.Controllers;
using StorageApi.Models;
using StorageApi.Tests.Helpers;
using Xunit.Abstractions;

namespace StorageApi.Tests
{
  public class StorageBinControllerTests : ControllerTestBase<StorageBin, StorageBinModel, StorageBinInsertUpdateModel, StorageBinController>
  {
    public StorageBinControllerTests(ITestOutputHelper output)
      : base(output, new StorageBinProfile(), new StorageBinContentProfile())
    {

    }

    #region Overrides
    protected override IList<StorageBin> AllItems(int count)
    {
      return Builder<StorageBin>.CreateListOfSize(count).Build();
    }
    protected override StorageBin NewItem()
    {
      return Builder<StorageBin>
        .CreateNew()
        .Build();
    }
    protected override StorageBinInsertUpdateModel ToBeInsertedItem()
    {
      return TestDataHelper.NewStorageBin();
    }
    protected override StorageBinInsertUpdateModel ToBeUpdatedItem()
    {
      return TestDataHelper.UpdatedStorageBin();

    }
    #endregion
  }
}