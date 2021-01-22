using System;
using FizzWare.NBuilder;
using FluentAssertions;
using StorageApi.Helpers;
using StorageApi.Models;
using Xunit;

namespace StorageApi.Tests.Models
{
  public class StorageUnitTests
  {
    [Fact]
    public void FromLayout_GivenValidInput_ShouldCreateRowsAndColumns()
    {
      //Setup
      var expectedRows = 3;
      var expectedColumns = 2;

      // Action
      var result = StorageUnit.FromLayout(expectedRows, expectedColumns);
      
      //Assert
      result.Count.Should().Be(expectedRows);
      foreach (var storageRow in result)
      {
        storageRow.StorageColumns.Count.Should().Be(expectedColumns);
      }
    }

    [Fact]
    public void GetAssignedBin_GivenValidAddressWithBin_Should_ReturnBinAsReference()
    {
      //Setup
      var expectedRowIndex = 2;
      var expectedColumnIndex = 2;
      var bin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var unit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(3, 3))
        .Build();
      unit.Rows[2].StorageColumns[2].Bin = bin.ToReference();

      //Action
      var binReference = unit.GetAssignedBin(expectedRowIndex, expectedColumnIndex);

      //Assert
      binReference.Id.Should().Be(bin.Id);
    }
    [Fact]
    public void GetAssignedBin_GivenInvalidRowIndex_Should_Throw()
    {
      //Setup
      var expectedRowIndex = 2;
      var expectedColumnIndex = 2;
      var unit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(1, 3))
        .Build();

      //Action
      Action action = ()=>unit.GetAssignedBin(expectedRowIndex, expectedColumnIndex);

      //Assert
      action.Should().Throw<ArgumentOutOfRangeException>();
    }
    [Fact]
    public void GetAssignedBin_GivenInvalidColumnIndex_Should_Throw()
    {
      //Setup
      var expectedRowIndex = 2;
      var expectedColumnIndex = 2;
      var unit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(3, 1))
        .Build();

      //Action
      Action action = () => unit.GetAssignedBin(expectedRowIndex, expectedColumnIndex);

      //Assert
      action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AssignBin_GivenValidBinAndAddress_ShouldAssignBin()
    {
      //Setup
      var expectedRowIndex = 2;
      var expectedColumnIndex = 2;
      var location = Builder<StorageLocation>
        .CreateNew()
        .Build();
      var bin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var unit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(3, 3))
        .With(_=>_.Location=location.ToReference())
        .Build();

      //Action
      var assignedBin = unit.AssignBin(bin, expectedRowIndex, expectedColumnIndex);

      //Assert
      assignedBin.StorageBinLocation.Should().NotBeNull();
      assignedBin.StorageBinLocation.RowIndex.Should().Be(expectedRowIndex);
      assignedBin.StorageBinLocation.ColumnIndex.Should().Be(expectedColumnIndex);
      assignedBin.StorageBinLocation.Unit.Id.Should().Be(unit.Id);
      assignedBin.StorageBinLocation.Location.Id.Should().Be(location.Id);

      unit.Rows[expectedRowIndex].StorageColumns[expectedColumnIndex].Bin.Id.Should().Be(bin.Id);
    }
    [Fact]
    public void AssignBin_GivenNoBin_ShouldThrow()
    {
      //Setup
      var unit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(3, 3))
        .Build();

      //Action
      Action action =()=> unit.AssignBin(null, 1, 1);

      //Assert
      action.Should().Throw<ArgumentNullException>();
    }
    [Fact]
    public void AssignBin_GivenInvalidRowIndex_ShouldThrow()
    {
      //Setup
      var bin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var unit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(1, 3))
        .Build();

      //Action
      Action action = () => unit.AssignBin(bin, 3, 2);

      //Assert
      action.Should().Throw<ArgumentOutOfRangeException>();
    }
    [Fact]
    public void AssignBin_GivenInvalidColumnIndex_ShouldThrow()
    {
      //Setup
      var bin = Builder<StorageBin>
        .CreateNew()
        .Build();
      var unit = Builder<StorageUnit>
        .CreateNew()
        .With(_ => _.Rows = StorageUnit.FromLayout(3, 1))
        .Build();

      //Action
      Action action = () => unit.AssignBin(bin, 1, 2);

      //Assert
      action.Should().Throw<ArgumentOutOfRangeException>();
    }
  }
}
