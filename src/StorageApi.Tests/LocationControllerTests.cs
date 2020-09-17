using System.Collections.Generic;
using FizzWare.NBuilder;
using MongoDB.Bson;
using StorageApi.Controllers;
using StorageApi.Models;
using Xunit.Abstractions;

namespace StorageApi.Tests
{
  public class LocationControllerTests : ControllerTestBase<Location, LocationModel, LocationInsertUpdateModel, LocationController>
  {
    public LocationControllerTests(ITestOutputHelper output)
    : base(output, new LocationProfile())
    {

    }

    #region Overrides
    protected override IList<Location> AllItems(int count)
    {
      return Builder<Location>.CreateListOfSize(count).Build();
    }
    protected override Location NewItem()
    {
      return Builder<Location>
        .CreateNew()
        .Build();
    }
    protected override LocationInsertUpdateModel ToBeInsertedItem()
    {
      return Builder<LocationInsertUpdateModel>
        .CreateNew()
        .Build();
    }
    protected override LocationInsertUpdateModel ToBeUpdatedItem()
    {
      return new LocationInsertUpdateModel()
      {
        Name = "UpdatedName"
      };
    }
    #endregion
  }
}
