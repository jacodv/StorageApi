using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using StorageApi.Helpers;
using StorageApi.Models;

namespace StorageApi.Tests.Helpers
{
  public static class TestDataHelper
  {
    public static Dictionary<ObjectId, Location> Locations;
    public static Dictionary<ObjectId, StorageBin> Bins;
    public static Dictionary<ObjectId, StorageUnit> Units;

    public static LocationInsertUpdateModel NewLocation()
    {
      return Builder<LocationInsertUpdateModel>
        .CreateNew()
        .Build();
    }
    public static LocationInsertUpdateModel UpdatedLocation()
    {
      return Builder<LocationInsertUpdateModel>
        .CreateNew()
        .With(_=>_.Name="UpdatedName")
        .Build();
    }
    public static List<StorageBinContentInsertUpdateModel> NewStorageBinContent(int count)
    {
      return Builder<StorageBinContentInsertUpdateModel>
        .CreateListOfSize(count)
        .Build()
        .ToList();
    }
    public static StorageBinInsertUpdateModel NewStorageBin()
    {
      return Builder<StorageBinInsertUpdateModel>
        .CreateNew()
        .With(_=>_.Contents= NewStorageBinContent(2))
        .Build();
    }
    public static StorageBinInsertUpdateModel UpdatedStorageBin()
    {
      return Builder<StorageBinInsertUpdateModel>
        .CreateNew()
        .With(_=>_.Name="UpdatedName")
        .With(_ => _.Contents = NewStorageBinContent(1))
        .Build();
    }
    public static StorageUnitInsertUpdateModel NewStorageUnit()
    {
      return Builder<StorageUnitInsertUpdateModel>
        .CreateNew()
        .With(_ => _.Rows = 3)
        .With(_ => _.ColumnsPerRow = 3)
        .Build();
    }
    public static StorageUnitInsertUpdateModel UpdateStorageUnit()
    {
      return Builder<StorageUnitInsertUpdateModel>
        .CreateNew()
        .With(_=>_.Name="UpdatedName")
        .With(_ => _.Rows = 2)
        .With(_ => _.ColumnsPerRow = 2)
        .Build();
    }

    public static void EnsureValidData(ILogger logger)
    {
      _logger = logger;
      if(Locations==null || !Locations.Any())
        _ensureLocations();
      if(Bins==null || !Bins.Any())
        _ensureBins();
      if(Units==null || !Units.Any())
        _ensureStorageUnits();
    }

    private static readonly Dictionary<string, StorageBin> _assignedBins = new Dictionary<string, StorageBin>();
    private static ILogger _logger;

    private static void _ensureStorageUnits()
    {
      var units = Builder<StorageUnit>
        .CreateListOfSize(3)
        .All()
        .With((unit, index) => unit.Name = $"Unit_{index}")
        .With((unit,index) => unit.Rows = _create3By4(unit, index))
        .With((unit, index) => unit.Location = Locations.Values.ToList()[index].ToReference())
        .Build();

      foreach (var storageUnit in units)
      {
        var unitIndex = int.Parse(storageUnit.Name.Replace("Unit_",""));
        foreach (var storageUnitRow in storageUnit.Rows)
        {
          foreach (var storageColumn in storageUnitRow.StorageColumns)
          {
            var binKey = $"{unitIndex}_{storageUnitRow.Index}_{storageColumn.Index}";
            if (_assignedBins.ContainsKey(binKey))
              storageUnit.AssignBin(_assignedBins[binKey], storageUnitRow.Index, storageColumn.Index);
          }
        }
      }
      //_logger.LogInformation(JsonConvert.SerializeObject(units));
      Units = units.ToDictionary(grp => grp.Id, item => item);
    }
    private static List<StorageRow> _create3By4(StorageUnit unit, int unitIndex)
    {
      return Builder<StorageRow>
        .CreateListOfSize(3)
        .All()
        .With((_, index) => _.Index = index)
        .With((_, rowIndex) => _.StorageColumns = Builder<StorageColumn>.CreateListOfSize(4)
          .All()
          .With((_, index) => _.Index = index)
          .Do((_, columnIndex) => _assignBin(rowIndex, columnIndex, unitIndex))
          .Build()
          .ToList()
        )
        .Build().ToList();
    }
    private static void _assignBin(in int rowIndex, in int columnIndex, in int unitIndex)
    {
      var maxColIndex = 3 + rowIndex * 4;
      var itemIndex = (maxColIndex - columnIndex) + (unitIndex*12);

      if (Bins.Count < itemIndex + 1)
        return;
      
      _assignedBins.Add($"{unitIndex}_{rowIndex}_{columnIndex}", Bins.Values.Skip(itemIndex).Take(1).First());
    }
    private static void _ensureBins()
    {
      var bins = Builder<StorageBin>
        .CreateListOfSize(30)
        .All()
        .WithFactory(x=>_createBins(x))
        .Build();
      Bins = bins
        .ToDictionary(grp => grp.Id, item => item);

    }
    private static StorageBin _createBins(in int index)
    {
      if (index < 10)
      {
        return StorageBinHelper.ScrewBin(10 + index, 50, 0.1 + ((double)index / 10));
      }

      if (index >= 10 && index < 20)
      {
        if(index % 3 == 0)
        {
          return StorageBinHelper.LedBin("Red", 10 + index, 50, 0.1 + ((double)index / 10));
        }
        if (index % 2 == 0)
        {
          return StorageBinHelper.LedBin("Green", 10 + index, 50, 0.1 + ((double)index / 10));
        }
        return StorageBinHelper.LedBin("Blue", 10 + index, 50, 0.1 + ((double)index / 10));
      }

      return StorageBinHelper.NutBin(index, 100);
    }
    private static void _ensureLocations()
    {
      var locations = Builder<Location>
        .CreateListOfSize(5)
        .Build().ToList();
      Locations = locations
        .ToDictionary(grp => grp.Id, item => item);
    }
  }

  public static class StorageBinHelper
  {
    public static StorageBin ScrewBin(int length, int quantity, double unitWeight)
    {
      return new StorageBin()
      {
        Name = "Screws",
        Contents = new List<StorageBinContent>(){StorageBinContentHelper.ScrewContent(length, quantity, unitWeight)}
      };
    }
    public static StorageBin LedBin(string color, int resistance, int quantity, double unitWeight)
    {
      return new StorageBin()
      {
        Name = $"{color} LED",
        Contents = new List<StorageBinContent>() { StorageBinContentHelper.LedContent(color, resistance, quantity, unitWeight) }
      };
    }
    public static StorageBin NutBin(int index, int quantity)
    {
      return new StorageBin()
      {
        Name = "Nuts",
        Contents = new List<StorageBinContent>() { StorageBinContentHelper.NutContent(index, quantity) }
      };
    }
  }

  public static class StorageBinContentHelper
  {
    public static StorageBinContent ScrewContent(in int length, in int quantity, double unitWeight)
    {
      return new StorageBinContent()
      {
        Name = $"Screws-{length}mm",
        Quantity = quantity,
        UnitWeight = unitWeight,
        Tags = new List<string>(){"Screw", $"{length}mm"}
      };
    }
    public static StorageBinContent LedContent(in string color, in int resistance, in int quantity, double unitWeight)
    {
      return new StorageBinContent()
      {
        Name = $"{color} LED-{resistance}ohm",
        Quantity = quantity,
        UnitWeight = unitWeight,
        Tags = new List<string>() { "LED", color, $"{resistance}ohm" }
      };
    }
    public static StorageBinContent NutContent(in int diameter, in int quantity)
    {
      return new StorageBinContent()
      {
        Name = $"Nuts-{diameter}mm",
        Quantity = quantity,
        UnitWeight = diameter/2,
        Tags = new List<string>() { "Nut", $"{diameter}mm" }
      };
    }
  }
}
