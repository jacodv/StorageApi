using System.Collections.Generic;
using FizzWare.NBuilder;
using StorageApi.Controllers;
using StorageApi.Models;
using StorageApi.Tests.Helpers;
using Xunit.Abstractions;

namespace StorageApi.Tests
{
  public class StorageUnitControllerTests : ControllerTestBase<StorageUnit, StorageUnitModel, StorageUnitInsertUpdateModel, StorageUnitController>
  {
    public StorageUnitControllerTests(ITestOutputHelper output)
      : base(output, new StorageUnitProfile())
    {
      _testPut = false;
    }

    #region Overrides
    protected override IList<StorageUnit> AllItems(int count)
    {
      return Builder<StorageUnit>.CreateListOfSize(count).Build();
    }
    protected override StorageUnit NewItem()
    {
      return Builder<StorageUnit>
        .CreateNew()
        .Build();
    }
    protected override StorageUnitInsertUpdateModel ToBeInsertedItem()
    {
      return TestDataHelper.NewStorageUnit();
    }
    protected override StorageUnitInsertUpdateModel ToBeUpdatedItem()
    {
      return TestDataHelper.UpdateStorageUnit();
    }
    #endregion

  }
}