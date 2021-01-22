using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Repositories.Interfaces;
using Moq;
using Newtonsoft.Json;
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
    private Mock<IRepository<StorageUnit>> _mockUnitRepository;
    
    public StorageBinControllerTests(ITestOutputHelper output)
      : base(output, new StorageBinProfile(), new StorageBinContentProfile(), new DocumentReferenceProfile())
    {
      _controller = _crudController as StorageBinController;
      _mockUnitRepository = new Mock<IRepository<StorageUnit>>(MockBehavior.Strict);
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

    [Fact]
    public async Task AssignBinToUnit_GivenValidModel_ShouldAssignBin()
    {
      //Setup
      var expectedUnit = Builder<StorageUnit>
        .CreateNew()
        .With(_=>_.Rows=StorageUnit.FromLayout(3,3))
        .Build();
      var expectedBin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var model = Builder<AssignStorageBinModel>
        .CreateNew()
        .With(_=>_.UnitId=expectedUnit.Id.ToString())
        .With(_ => _.BinId = expectedBin.Id.ToString())
        .Build();

      _mockUnitRepository
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .ReturnsAsync(expectedUnit);
      _mockRepository
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .ReturnsAsync(expectedBin);
      _mockUnitRepository
        .Setup(mc => mc.ReplaceOneAsync(It.IsAny<StorageUnit>()))
        .Returns<StorageUnit>(Task.FromResult);
      _mockRepository
        .Setup(mc => mc.ReplaceOneAsync(It.IsAny<StorageBin>()))
        .Returns<StorageBin>(Task.FromResult);

      //Action
      var assignedBin = await _controller.AssignBinToUnit(_mockUnitRepository.Object, model);

      //Assert
      assignedBin.StorageBinLocation.Should().NotBeNull();

    }
    [Fact]
    public async Task AssignBinToUnit_GivenValidModelWithExistingBin_ShouldClearExistingBin_And_AssignNewBin()
    {
      //Setup
      var existingBin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var expectedUnit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(3, 3))
        .Build();
      expectedUnit.AssignBin(existingBin, 1, 1);
      var expectedBin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var model = Builder<AssignStorageBinModel>
        .CreateNew()
        .With(_ => _.UnitId = expectedUnit.Id.ToString())
        .With(_ => _.BinId = expectedBin.Id.ToString())
        .Build();

      _mockUnitRepository
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .ReturnsAsync(expectedUnit);
      _mockRepository
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .Returns<string>(binId=> 
          Task.FromResult(existingBin.Id.Equals(binId) ? 
            existingBin : 
            expectedBin));
      _mockUnitRepository
        .Setup(mc => mc.ReplaceOneAsync(It.IsAny<StorageUnit>()))
        .Returns<StorageUnit>(Task.FromResult);
      _mockRepository
        .Setup(mc => mc.ReplaceOneAsync(It.IsAny<StorageBin>()))
        .Returns<StorageBin>(bin=>
        {
          _logger.LogDebug(JsonConvert.SerializeObject(bin, Formatting.Indented));
          return Task.FromResult(bin);
        });

      //Action
      var assignedBin = await _controller.AssignBinToUnit(_mockUnitRepository.Object, model);

      //Assert
      assignedBin.StorageBinLocation.Should().NotBeNull();
      existingBin.StorageBinLocation.Should().BeNull();

    }
    [Fact]
    public void AssignBinToUnit_GivenNoModel_ShouldThrow()
    {
      //Action
      Action call = () => _controller.AssignBinToUnit(_mockUnitRepository.Object, null).Wait();

      //Assert
      call.Should().Throw<ArgumentNullException>();
    }
    [Fact]
    public void AssignBinToUnit_GivenInValidUnitId_ShouldAssignBin()
    {
      //Setup
      var expectedUnit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(3, 3))
        .Build();
      var expectedBin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var model = Builder<AssignStorageBinModel>
        .CreateNew()
        .With(_ => _.UnitId = expectedUnit.Id.ToString())
        .With(_ => _.BinId = expectedBin.Id.ToString())
        .Build();

      _mockUnitRepository
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .Returns(Task.FromResult(default(StorageUnit)));

      //Action
      Action action = () => _controller.AssignBinToUnit(_mockUnitRepository.Object, model).Wait();

      //Assert
      action.Should().Throw<ArgumentOutOfRangeException>();
    }
    [Fact]
    public void AssignBinToUnit_GivenInValidBinId_ShouldAssignBin()
    {
      //Setup
      var expectedUnit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(3, 3))
        .Build();
      var expectedBin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var model = Builder<AssignStorageBinModel>
        .CreateNew()
        .With(_ => _.UnitId = expectedUnit.Id.ToString())
        .With(_ => _.BinId = expectedBin.Id.ToString())
        .Build();

      _mockUnitRepository
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .ReturnsAsync(expectedUnit);
      _mockRepository
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .Returns(Task.FromResult(default(StorageBin)));

      //Action
      Action action = () => _controller.AssignBinToUnit(_mockUnitRepository.Object, model).Wait();

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