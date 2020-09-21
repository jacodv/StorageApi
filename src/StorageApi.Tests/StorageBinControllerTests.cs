using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StorageApi.Controllers;
using StorageApi.Models;
using StorageApi.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace StorageApi.Tests
{
  public class StorageBinControllerTests : ControllerTestBase<StorageBin, StorageBinModel, StorageBinInsertUpdateModel, StorageBinController>
  {
    private readonly StorageBinController _controller;
    public StorageBinControllerTests(ITestOutputHelper output)
      : base(output, new StorageBinProfile(), new StorageBinContentProfile(), new DocumentReferenceProfile())
    {
      _controller = _crudController as StorageBinController;
    }


    [Fact]
    public void GetBinContainingTags_GivenTags_ShouldReturnBins()
    {
      //Setup
      TestDataHelper.EnsureValidData(_logger);
      var expectedTags = new List<string>() { "Red", "LED" };
      _mockRepository.Setup(mc => mc.AsQueryable())
        .Returns(_getExpectedBins(expectedTags));

      //Action
      var itemsResult = _controller.GetContainingTags(expectedTags);

      //Assert
      itemsResult.Count().Should().Be(3);
    }
    [Fact]
    public void GetBinContainingTags_GivenNoTags_ShouldReturnBadRequest()
    {
      //Setup
      TestDataHelper.EnsureValidData(_logger);
      var expectedTags = new List<string>();
      _mockRepository.Setup(mc => mc.AsQueryable())
        .Returns(_getExpectedBins(expectedTags));

      //Action
      Action action=()=> _controller.GetContainingTags(expectedTags);

      //Assert
      action.Should().Throw<ArgumentOutOfRangeException>();
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

    private IQueryable<StorageBin> _getExpectedBins(List<string> expectedTags)
    {
      return TestDataHelper.Bins.Values
        .Where(
          w => w.Contents.Any(
            c => c.Tags.Intersect(expectedTags).Count() == expectedTags.Count))
        .AsQueryable();
    }
  }
}