using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Repositories.Interfaces;
using StorageApi.Interfaces;
using StorageApi.Models;

namespace StorageApi.Helpers
{
  public static class DemoDataHelper
  {
    public const string DemoLocationName = "DemoLocation";
    public const string DemoStorageUnitName = "DemoStorageUnit";
    public const string DemoBinNameFormat = "DemoBin{0}";
    
    public static async Task Populate(IServiceProvider serviceProvider)
    {
      await BuildLocation(serviceProvider.GetService<IRepository<Location>>());
      await BuildDemoBin(serviceProvider.GetService<IRepository<StorageBin>>());
      await BuildDemoStorageUnit(serviceProvider.GetService<IRepository<Location>>(),
        serviceProvider.GetService<IRepository<StorageBin>>(),
        serviceProvider.GetService<IRepository<StorageUnit>>());
    }

    private static async Task BuildDemoStorageUnit(IRepository<Location> locations, IRepository<StorageBin> bins, IRepository<StorageUnit> storageUnits)
    {
      var item = await storageUnits.FindOneAsync(_ => _.Name == DemoStorageUnitName);
      if (item != null)
        return;

      var demoLocation = await locations.FindOneAsync(_ => _.Name == DemoLocationName);
      var bin1 = await bins.FindOneAsync(_ => _.Name == string.Format(DemoBinNameFormat, 1));
      var bin2 = await bins.FindOneAsync(_ => _.Name == string.Format(DemoBinNameFormat, 2));
      var bin3 = await bins.FindOneAsync(_ => _.Name == string.Format(DemoBinNameFormat, 3));

      item = new StorageUnit()
      {
        Name = DemoStorageUnitName,
        Location = demoLocation.ToReference(),
        StorageRows = new List<StorageRow>()
        {
          new StorageRow()
          {
            Index = 0,
            StorageColumns = new List<StorageColumn>()
            {
              new StorageColumn(){ Index = 0, Bin = bin1.ToReference()},
              new StorageColumn(){ Index = 1},
              new StorageColumn(){ Index = 2, Bin = bin2.ToReference()}
            }
          },
          new StorageRow()
          {
            Index = 1,
            StorageColumns = new List<StorageColumn>()
            {
              new StorageColumn(){ Index = 0},
              new StorageColumn(){ Index = 1, Bin = bin3.ToReference()},
              new StorageColumn(){ Index = 2}
            }
          }

        }
      };

      await storageUnits.InsertOneAsync(item);
    }
    private static async Task BuildDemoBin(IRepository<StorageBin> storageUnits)
    {
      var item = await storageUnits.FindOneAsync(_ => _.Name == string.Format(DemoBinNameFormat,1));
      if (item != null)
        return;

      item = new StorageBin()
      {
        Name = string.Format(DemoBinNameFormat, 1),
        Contents = new List<StorageBinContent>()
        {
          new StorageBinContent() {Name = "Red LED Lights", Quantity = 10, UnitWeight = 2, Tags = new List<string>(){"Red", "LED", "Light"}},
          new StorageBinContent() {Name = "Green LED Lights", Quantity = 5, UnitWeight = 2, Tags = new List<string>(){ "Green", "LED", "Light"}},
          new StorageBinContent() {Name = "Blue LED Lights", Quantity = 25, UnitWeight = 2, Tags = new List<string>(){ "Blue", "LED", "Light"}},
        }
      };
      await storageUnits.InsertOneAsync(item);

      item = await storageUnits.FindOneAsync(_ => _.Name == string.Format(DemoBinNameFormat, 2));
      if (item != null)
        return;

      item = new StorageBin()
      {
        Name = string.Format(DemoBinNameFormat, 2),
        Contents = new List<StorageBinContent>()
        {
          new StorageBinContent() {Name = "Uno Boards", Quantity = 1, UnitWeight = 150, Tags = new List<string>(){"Arduino", "Uno", "Board"}},
        }
      };
      await storageUnits.InsertOneAsync(item);

      item = await storageUnits.FindOneAsync(_ => _.Name == string.Format(DemoBinNameFormat, 3));
      if (item != null)
        return;

      item = new StorageBin()
      {
        Name = string.Format(DemoBinNameFormat, 3),
      };
      await storageUnits.InsertOneAsync(item);
    }
    private static async Task BuildLocation(IRepository<Location> locations)
    {
      var item = await locations.FindOneAsync(_ => _.Name == DemoLocationName);
      if (item != null)
        return;

      item = new Location()
      {
        Name = DemoLocationName
      };
      await locations.InsertOneAsync(item);
    }

  }
}
