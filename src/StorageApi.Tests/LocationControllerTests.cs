using System.Collections.Generic;
using FizzWare.NBuilder;
using StorageApi.Controllers;
using StorageApi.Models;
using Xunit.Abstractions;

namespace StorageApi.Tests
{
  public class LocationControllerTests : ControllerTestBase<StorageLocation, StorageLocationModel, StorageLocationInsertUpdateModel, LocationController>
  {
    public LocationControllerTests(ITestOutputHelper output)
    : base(output, new StorageLocationProfile())
    {

    }

    #region Overrides
    protected override IList<StorageLocation> AllItems(int count)
    {
      return Builder<StorageLocation>.CreateListOfSize(count).Build();
    }
    protected override StorageLocation NewItem()
    {
      return Builder<StorageLocation>
        .CreateNew()
        .Build();
    }
    protected override StorageLocationInsertUpdateModel ToBeInsertedItem()
    {
      return Builder<StorageLocationInsertUpdateModel>
        .CreateNew()
        .Build();
    }
    protected override StorageLocationInsertUpdateModel ToBeUpdatedItem()
    {
      return new StorageLocationInsertUpdateModel()
      {
        Name = "UpdatedName"
      };
    }
    #endregion
  }
}
